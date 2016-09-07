using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atrox;

namespace ShipSync.Container.Entity
{
    public class SaveSource
    {
        public readonly Guid Id;
        public readonly string Name;

        public SaveSource()
        {
            Id = Guid.NewGuid();
            Name = new Haikunator().Haikunate(tokenLength: 0);
        }

        public SaveSource(Guid id, string name)
        {
            Id = id;
            Name = name;
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
