using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationCleaner.Framework.Config
{
    internal class Objects
    {
        public bool WeedRemoval { get; set; } = true;
        public bool StoneRemoval { get; set; } = true;
        public bool OreRemoval { get; set; } = true;
        public bool GeodeRemoval { get; set; } = true;
        public bool TwigRemoval { get; set; } = true;
    }
}
