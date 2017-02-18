using System.Collections.Generic;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface IShipService
    {
        IEnumerable<GameSave> FindSavesInSource(SaveSource source);
        IEnumerable<Ship> FindShipsInSave(GameSave save);
    }
}