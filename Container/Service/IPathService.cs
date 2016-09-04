using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface IPathService
    {
        string RemotePath { get; }
        string CreateRemoteSavePath(GameSave save);
        string CreateRemoteShipPath(Ship ship);
        string CleanName(string name);
    }
}
