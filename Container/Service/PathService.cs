using System;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
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

        public string CreateRemoteShipPath(Ship ship)
        {
            return _appName + "/Ships/" + ship.FileName + ".craft";
        }

        public string CreateRemoteSavePath(GameSave save)
        {
            return _appName + "/Saves/" + save?.SaveSource + ".json";
        }

        public string CleanName(string input)
        {
            return CleanPattern.Replace(input, CleanReplacement).ToLower();
        }
    }
}
