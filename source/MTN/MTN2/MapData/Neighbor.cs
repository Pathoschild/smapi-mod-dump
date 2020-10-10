/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

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
