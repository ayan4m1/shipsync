using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public class SourceService : ISourceService
    {
        public SaveSource Create(string installPath)
        {
            var result = new SaveSource();
            
            return result;
        }

        public bool Validate(SaveSource source)
        {
            throw new NotImplementedException();
        }
    }
}
