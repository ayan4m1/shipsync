using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;

namespace ShipSync.GUI
{
    internal static class EtoExtensions
    {
        public static Size ToSize(this SizeF input)
        {
            return new Size((int)Math.Round(input.Width), (int)Math.Round(input.Height));
        }
    }
}
