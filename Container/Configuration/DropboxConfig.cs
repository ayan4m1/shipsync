using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipSync.Container.Configuration
{
    public class DropboxConfig
    {
        /// <summary>
        /// Dropbox "App Key" field
        /// </summary>
        public string Client;

        /// <summary>
        /// Current Dropbox OAuth2 auth token
        /// </summary>
        public string Token;
    }
}
