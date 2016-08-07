using System;
using System.IO;
using Dropbox.Api;

namespace ShipSync.Container.Service
{
    internal sealed class SyncService : ISyncService
    {
        private DropboxClient client;

        public SyncService(string appKey, string appSecret)
        {
            //client = new DropboxClient();
        }

        public DateTime? GetLastSync(string name)
        {
            throw new NotImplementedException();
        }

        public bool Sync(string name, Stream data)
        {
            throw new NotImplementedException();
        }
    }
}