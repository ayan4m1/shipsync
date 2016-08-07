using System;
using System.Collections.Generic;
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
            return FindByName(name) != null;
        }

        public Ship Find(string name)
        {
            throw new NotImplementedException();
        }

        public Ship Find(Ship ship)
        {
            return Find(ship.Name);
        }

        public bool Remove(Ship ship)
        {
            throw new NotImplementedException();
        }

        public Ship Sync(Ship ship)
        {
            throw new NotImplementedException();
        }

        public Ship FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}