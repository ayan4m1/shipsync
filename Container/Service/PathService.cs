using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Microsoft.Win32;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public sealed class PathService : IPathService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PathService));

        /// <summary>
        /// All characters matching this pattern will be replaced with CleanReplacement
        /// </summary>
        private static readonly Regex CleanPattern = new Regex(@"\W");

        /// <summary>
        /// Will take the place of any odd characters
        /// </summary>
        private const string CleanReplacement = "-";

        /// <summary>
        /// Internal identifier used to locate our files in the Dropbox filesystem
        /// </summary>
        private readonly string _appName;

        public PathService()
        {
            try
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName().FullName;
                _appName = assemblyName.Substring(assemblyName.IndexOf('.'));
            }
            catch (Exception ex)
            {
                Log.Error("PathService failed to find a root assembly name", ex);
                throw;
            }
        }

        public string RemotePath => _appName;

        public string CreateRemotePath(Ship ship)
        {
            return _appName + "/Ships/" + ship.FileName + ".craft";
        }

        public string CreateRemotePath(GameSave save)
        {
            return _appName + "/Saves/" + save?.SaveSource + ".json";
        }

        public string CleanName(string input)
        {
            return CleanPattern.Replace(input, CleanReplacement).ToLower();
        }

        public bool TestGameDirectory(string baseDir)
        {
            if (!Directory.Exists(baseDir))
            {
                return false;
            }

            return File.Exists(Path.Combine(baseDir, "buildID64.txt"));
        }

        public SaveSource FindSaveSource()
        {
            var save = new SaveSource();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                    try
                    {
                        var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                        var steam = hive.OpenSubKey(@"SOFTWARE\Valve\Steam");
                        if (steam != null && steam.SubKeyCount > 0)
                        {
                            save.InstallPath = Path.Combine(steam.GetValue("InstallPath").ToString(), "steamapps/common/Kerbal Space Program");
                        }
                        else
                        {
                            Log.Warn("Did not find a Steam install in the Windows registry");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Exception trying to find steam install.", e);
                    }
                    break;
                case PlatformID.MacOSX:
                    if (TestGameDirectory("~/Library/Application Support/Steam/steamapps/common/Kerbal Space Program"))
                    {
                        save.InstallPath = "~/Library/Application Support/Steam/steamapps/common/Kerbal Space Program";
                    }
                    break;
                case PlatformID.Unix:
                    if (TestGameDirectory("~/Steam/Kerbal Space Program"))
                    {
                        save.InstallPath = "~/Steam/Kerbal Space Program";
                    }
                    else if (TestGameDirectory("~/.local/share/Steam/SteamApps/common/Kerbal Space Program"))
                    {
                        save.InstallPath = "~/.local/share/Steam/SteamApps/common/Kerbal Space Program";
                    }
                    else if (TestGameDirectory("~/.steam/steam/steamapps/common/Kerbal Space Program"))
                    {
                        save.InstallPath = "~/.steam/steam/steamapps/common/Kerbal Space Program";
                    }
                    break;
                default:
                    Log.Warn("You are running some exotic platform that we don't care about...");
                    break;
            }

            return save;
        }
    }
}
