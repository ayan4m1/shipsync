using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Repository
{
    /// <summary>
    ///     Stores ship sync metadata using JSON
    /// </summary>
    internal sealed class ShipRepository : IShipRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipRepository));
        private static readonly IList<Ship> Ships = new List<Ship>();

        public bool Exists(string name)
        {
            return Find(name) != null;
        }

        public Ship Find(string name)
        {
            return Ships.First(ship => ship.Name.ToLower().Equals(name.ToLower()));
        }

        public Ship Find(Ship ship)
        {
            return Find(ship.Name);
        }

        public bool Remove(Ship ship)
        {
            if (!Exists(ship.Name))
            {
                return false;
            }

            Ships.Remove(Find(ship));
            return true;
        }

        public Ship Sync(Ship ship)
        {
            throw new NotImplementedException();
        }
    }
}