using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterPanning
{
    public class MapOreConfig
    {
        public string AreaName { get; set; }
        public int numberOfOreSpotsPerDay { get; set; }
        public List<Point> OreSpots { get; set; }
    }
}
