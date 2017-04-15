using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Eto;
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
                appInstance.Run(new BrowserDialog()
                {
                    AuthService = Container.Resolve<IAuthService>()
                });
                Log.Info("Auth resolved");

                Log.Info("Creating main application form");
                var mainForm = new MainForm()
                {
                    PathService = Container.Resolve<IPathService>(),
                    ShipService = Container.Resolve<IShipService>()
                };

                try
                {
                    appInstance.Run(mainForm);
                    Log.Info("UI thread exited");
                }
                catch (Exception e)
                {
                    Log.Error("Runtime exception", e);
                }
            }
        }
    }
}