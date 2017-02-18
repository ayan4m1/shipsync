using System;
using System.Collections.Generic;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public sealed class ShipService : IShipService
    {
        public IEnumerable<GameSave> FindSavesInSource(SaveSource source)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ship> FindShipsInSave(GameSave save)
        {
            throw new NotImplementedException();
        }
    }
}