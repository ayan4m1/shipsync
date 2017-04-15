﻿using System.Collections.Generic;
using System.Linq;

namespace ShipSync.Container.Entity
{
    public class GameSave
    {
        // name of the instance that this save belongs to
        public SaveSource SaveSource;

        // directory name under the source "saves/" dir
        public string Name;

        // private list exposed as a public ReadOnlyCollection
        public IEnumerable<Ship> Ships => _ships.AsReadOnly();
        private readonly List<Ship> _ships = new List<Ship>();

        public void AddShips(IEnumerable<Ship> ships)
        {
            _ships.AddRange(ships);
        }
    }
}
