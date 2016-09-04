using System;
using System.IO;
using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    internal interface ISyncService
    {
        Task<DateTime?> GetLastSync(GameSave save);
        Task<bool> TrySync(GameSave save);
    }
}