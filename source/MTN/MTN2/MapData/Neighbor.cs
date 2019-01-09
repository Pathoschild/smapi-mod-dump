using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData {
    public class Neighbor {
        public string MapName { get; set; }
        public List<Warp> WarpPoints { get; set; }

        public Neighbor() { }

        public Neighbor(string MapName) {
            this.MapName = MapName;
            WarpPoints = new List<Warp>();
        }
        public Neighbor(string MapName, List<Warp> WarpPoints) {
            this.MapName = MapName;
            this.WarpPoints = WarpPoints;
        }
    }
}
