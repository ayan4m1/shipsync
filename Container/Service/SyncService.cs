using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using log4net;
using Newtonsoft.Json;
using ShipSync.Container.Configuration;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public sealed class SyncService : ISyncService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SyncService));

        // todo: make this injectable?
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly DropboxClient _client;
        private readonly IPathService _pathService;

        public SyncService(JsonConfig config, IPathService pathService)
        {
            _client = new DropboxClient(config.Dropbox.Client);
            _pathService = pathService;
        }

        public async Task<DateTime?> GetLastSync(GameSave save)
        {
            var savePath = _pathService.CreateRemoteSavePath(save);
            var lookup = await _client.Files.GetMetadataAsync(new GetMetadataArg(path: savePath));
            return lookup?.AsFile?.ServerModified;
        }

        public async Task<bool> Synchronize(GameSave save)
        {
            // where to look for an existing save descriptor
            var remotePath = _pathService.CreateRemoteSavePath(save);

            // see if one already exists
            var serverSave = await _client.Files.DownloadAsync(new DownloadArg(remotePath));
            if (serverSave.Response.IsDeleted || !serverSave.Response.IsFile)
            {
                // if not, we can create the dir and upload ours
                var saveDir = await _client.Files.CreateFolderAsync(new CreateFolderArg(Path.GetDirectoryName(remotePath)));
                if (!saveDir.IsFolder)
                {
                    throw new InvalidOperationException("Object exists at " + saveDir.PathLower + " but it is not a folder!");
                }

                using (var reader = new MemoryStream(_encoding.GetBytes(JsonConvert.SerializeObject(save))))
                {
                    await _client.Files.UploadAsync(remotePath, clientModified: DateTime.UtcNow, body: reader);
                }
            }
            else
            {
                // check to see if ours is more recent
            }

            // create remote paths for each ship in the save directory
            var paths = save.Ships.Select(ship => _pathService.CreateRemoteShipPath(ship));
            foreach (var path in paths)
            {
                var lookup = await _client.Files.GetMetadataAsync(new GetMetadataArg(path));
                if (lookup.IsFile)
                {
                    // if ours is more recent, we should overwrite the old one
                    //var remoteOutdated = lookup.AsFile.ClientModified.CompareTo(DateTime.UtcNow) > 0;
                }

                //await _client.Files.UploadAsync(lookup.PathLower, clientModified: DateTime.UtcNow, body: )
            }

            return false;
        }
    }
}