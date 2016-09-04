using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using log4net;
using ShipSync.Container.Configuration;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    internal sealed class SyncService : ISyncService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SyncService));

        private readonly DropboxClient _client;
        private readonly IPathService _pathService;

        public SyncService(JsonConfig config, IPathService pathService)
        {
            _client = new DropboxClient(config.DropboxToken);
            _pathService = pathService;
        }

        public async Task<DateTime?> GetLastSync(GameSave save)
        {
            var savePath = _pathService.CreateRemoteSavePath(save);
            var lookup = await _client.Files.GetMetadataAsync(new GetMetadataArg(path: savePath));
            return lookup?.AsFile?.ServerModified;
        }

        public async Task<bool> TrySync(GameSave save)
        {
            var paths = save.Ships.Select(ship => _pathService.CreateRemoteShipPath(ship));
            foreach (var path in paths)
            {
                var lookup = _client.Files.DownloadAsync(new DownloadArg(path));
                // todo: deserialize result as a JSON object converted to GameSave
            }
            throw new NotImplementedException();
        }
    }
}