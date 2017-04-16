using System.IO;
using Autofac;
using log4net;
using Newtonsoft.Json;

namespace ShipSync.Container.Configuration
{
    public class DropboxConfigModule : Module
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DropboxConfigModule));

        private readonly string _configPath;

        public DropboxConfigModule(string configPath)
        {
            _configPath = configPath;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            if (!File.Exists(_configPath))
            {
                var ex = new FileNotFoundException(_configPath);
                Log.Error("Exception trying to load config", ex);
                throw ex;
            }

            using (var reader = File.OpenText(_configPath))
            {
                Log.Debug("Attempting to deserialize JSON config");
                var configObject = JsonConvert.DeserializeObject<DropboxConfig>(reader.ReadToEnd());
                builder.Register(c => configObject);
            }
        }
    }
}