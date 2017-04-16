using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.GUI
{
    public class SaveRow
    {
        public bool Checked { get; set; }
        public string Name { get; set; }
        public GameSave Tag { get; set; }
    }
}
