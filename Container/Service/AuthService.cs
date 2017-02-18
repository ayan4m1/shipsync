using System;
using Autofac.Core;
using System.Threading.Tasks;
using Autofac;
using Dropbox.Api;
using Dropbox.Api.Files;
using log4net;
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
            Client = new DropboxClient(token);
            Log.Debug("Created Dropbox Client with token " + token);
        }

        public async Task<bool> TestAccessToken()
        {
            try
            {
                var result = await Client.Files.GetMetadataAsync(new GetMetadataArg("./"));
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
