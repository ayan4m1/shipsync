using System;
using System.IO;
using Autofac.Core;
using System.Threading.Tasks;
using Autofac;
using Dropbox.Api;
using Dropbox.Api.Files;
using log4net;
using Newtonsoft.Json;
using ShipSync.Container.Configuration;

namespace ShipSync.Container.Service
{
    public class AuthService : IAuthService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PathService));
        private readonly JsonConfig _config;

        public AuthService(JsonConfig config)
        {
            _config = config;
            UpdateClient();
        }

        public DropboxClient Client { get; private set; }

        public string ClientIdentifier
        {
            get
            {
                Log.Debug("Looking up OAuth client identifier");
                return _config.Dropbox.Client;
            }
        }

        public void UpdateToken(string token)
        {
            _config.Dropbox.Token = token;
            UpdateClient();
            Save();
        }

        private void UpdateClient()
        {
            Client = new DropboxClient(_config.Dropbox.Token);
            Log.Debug("Created Dropbox Client with token " + _config.Dropbox.Token);
        }

        public void Save()
        {
            Log.Debug("Attempting to serialize JSON config");
            var configPath = Path.Combine(Environment.CurrentDirectory, "app.json");
            using (var configFile = File.CreateText(configPath))
            {
                Log.Info("Writing JSON config");
                new JsonSerializer().Serialize(configFile, _config);
            }
        }

        public async Task<bool> TestAccessToken()
        {
            try
            {
                if (Client == null)
                {
                    Log.Error("Client not initialized at test-time!");
                    return false;
                }

                var result = await Client.Files.GetMetadataAsync("/");
                return result.IsFolder;
            }
            catch (Exception e)
            {
                Log.Error("Error trying to check access token", e);
            }

            return false;
        }
    }
}
