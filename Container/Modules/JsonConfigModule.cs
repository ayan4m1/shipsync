using System.IO;
using Autofac;
using log4net;
using Newtonsoft.Json;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Modules
{
    public class JsonConfigModule : Module
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonConfigModule));

        private readonly string _configPath;

        public JsonConfigModule(string configPath)
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
                var configObject = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                builder.Register(c => configObject);
            }
        }
    }
}