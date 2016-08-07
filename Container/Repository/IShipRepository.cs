using ShipSync.Container.Entity;

namespace ShipSync.Container.Repository
{
    internal interface IShipRepository
    {
        bool Exists(string name);

        Ship Find(Ship ship);
        Ship Find(string name);

        Ship Sync(Ship ship);
        bool Remove(Ship ship);
    }
}