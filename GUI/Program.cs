using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Eto.Forms;
using log4net;
using log4net.Config;
using ShipSync.Container.Configuration;
using ShipSync.Container.Service;

namespace ShipSync.GUI
{
    public sealed class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static IContainer Container;

        [STAThread]
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            var builder = new ContainerBuilder();

            Log.Debug("Getting current assembly");
            var containerAssy = Assembly.GetAssembly(typeof(JsonConfig));

            Log.Debug("Registering Services and Repositories");
            builder.RegisterAssemblyTypes(containerAssy)
                .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            Log.Debug("Reading configuration");
            var configPath = Path.Combine(Environment.CurrentDirectory, "app.json");
            var configModule = new JsonConfigModule(configPath);
            builder.RegisterModule(configModule);

            Log.Debug("Creating application container");
            Container = builder.Build();
            Log.Debug("Application context initialized");

            using (var appInstance = new Application())
            {
                // see if we need a new token
                //appInstance.Run(new BrowserDialog(Container));

                var pathService = Container.Resolve<IPathService>();
                var steamPath = pathService.FindSteamInstall();
                if (!string.IsNullOrWhiteSpace(steamPath))
                {
                    Log.Info("Detected Steam install at " + steamPath);
                    steamPath = Path.Combine(steamPath, "steamapps", "common", "Kerbal Space Program", "saves");
                    if (!Directory.Exists(steamPath))
                    {
                        Log.Error("No KSP install found at " + steamPath);
                        return;
                    }
                }
                else
                {
                    Log.Error("Cannot handle non-Steam installs");
                    return;
                }

                var craftList = Directory.GetDirectories(steamPath)
                        .SelectMany(dir =>
                        {
                            if (dir.EndsWith("scenarios") || dir.EndsWith("training"))
                            {
                                return new string[] {};
                            }

                            var baseDir = Path.Combine(dir, "Ships");
                            return new[]
                            {
                                Path.Combine(baseDir, "VAB"),
                                Path.Combine(baseDir, "SPH")
                            };
                        })
                        .Where(dir => !string.IsNullOrWhiteSpace(dir))
                        .SelectMany(dir =>
                        {
                            try
                            {
                                return Directory.GetFiles(dir, "*.craft");
                            }
                            catch (Exception ex)
                            {
                                Log.Warn("Failed to enumerate craft in " + dir, ex);
                            }

                            return new string[] { };
                        })
                        .Distinct()
                        .ToList();

                Log.Info("Found " + craftList.Count() + " craft files to sync");

                var baseDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Dropbox", "Apps", "ShipSync");
                foreach (var craft in craftList)
                {
                    var relative = new Uri(steamPath).MakeRelativeUri(new Uri(craft)).ToString();
                    var saveName = relative.Split('/')[1].ToLower();
                    var craftName = Uri.UnescapeDataString(Path.GetFileName(relative).ToLower().Replace('_', '-')).Replace(' ', '-');
                    var destPath = Path.Combine(baseDest, saveName + "_" + craftName);

                    if (File.Exists(destPath))
                    {
                        var craftTime = File.GetLastWriteTime(craft);
                        var dropboxTime = File.GetLastWriteTime(destPath);
                        if (craftTime.CompareTo(dropboxTime) > 0)
                        {
                            Log.Info("Syncing " + craft + " into " + destPath);
                            using (var writer = File.OpenWrite(destPath))
                            {
                                writer.Write(new byte[writer.Length], 0, (int)writer.Length);
                                writer.Position = 0;
                                using (var reader = File.OpenRead(craft))
                                {
                                    reader.CopyTo(writer);
                                }
                                writer.Flush();
                            }
                        }
                    }
                    else
                    {
                        File.Copy(craft, destPath);
                        Log.Info("Adding new craft " + craft + " to " + destPath);
                    }
                }

                /*Log.Info("Creating main application form");
                var mainForm = new MainForm();
                appInstance.Run(mainForm);
                Log.Info("UI thread exited");*/
            }
        }
    }
}