using System.Collections.Generic;
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

        /// <summary>
        /// Creates a GameSave object containing the provided
        /// enumeration of ships. They will be set as owned by
        /// this save.
        /// </summary>
        /// <param name="ships">List of ships to associate with this save</param>
        public GameSave(IEnumerable<Ship> ships)
        {
            _ships.AddRange(ships.Select(ship =>
            {
                ship.Save = this;
                return ship;
            }));
        }
    }
}
