using System;
using System.IO;

namespace ShipSync.Container.Service
{
    internal interface ISyncService
    {
        DateTime? GetLastSync(string name);
        bool Sync(string name, Stream data);
    }
}