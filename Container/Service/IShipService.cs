using System.Collections;
using System.Collections.Generic;

namespace ShipSync.Container.Service
{
    internal interface IShipService
    {
        IEnumerable<string> FindAllShips();
        IEnumerable<string> FindShipsInSave(string saveName);
    }
}