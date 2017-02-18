using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;

namespace ShipSync.Container.Service
{
    public interface IAuthService
    {
        DropboxClient Client { get; }

        string ClientIdentifier { get; }

        void UpdateToken(string token);

        Task<bool> TestAccessToken();
    }
}
