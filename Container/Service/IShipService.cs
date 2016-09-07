using System.Collections;
using System.Collections.Generic;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface IShipService
    {
        IEnumerable<string> FindShips();
        IEnumerable<string> FindShipsInSave(GameSave save);
    }
}