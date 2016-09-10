using System;

namespace ShipSync.Container.Entity
{
    public class SaveSource
    {
        public readonly Guid Id;
        public string Name;
        public string InstallPath;

        public SaveSource()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return Id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
