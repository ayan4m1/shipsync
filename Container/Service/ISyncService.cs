using System;
using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface ISyncService
    {
        Task<DateTime?> GetLastSync(GameSave save);
        Task<bool> Synchronize(GameSave save);
    }
}