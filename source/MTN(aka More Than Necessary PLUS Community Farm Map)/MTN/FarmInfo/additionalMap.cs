using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo
{
    /// <summary>
    /// A simple class that retains information for additional maps in custom farms.
    /// 
    /// Populated by JsonSerializer
    /// </summary>
    public class additionalMap<T>
    {
        public fileType type { get; set; } = fileType.xnb;

        public string Location { get; set; }
        public string FileName { get; set; }
        public string mapType { get; set; } = "GameLocation";
        public string displayName { get; set; } = "Untitled";

        public T map;

        public T Map {
            get {
                return map;
            }
        }

        public additionalMap() { }


        public additionalMap(string name, string location, string displayName)
        {
            FileName = name;
            Location = location;
            this.displayName = displayName;
        }

        public additionalMap(additionalMap<GameLocation> m, T map) {
            FileName = m.FileName;
            Location = m.Location;
            type = m.type;
            mapType = m.mapType;
            displayName = m.displayName;
            this.map = map;
        }

        public additionalMap(string FileName, string Location, fileType type, string mapType, string displayName, T map) {
            this.FileName = FileName;
            this.Location = Location;
            this.type = type;
            this.mapType = mapType;
            this.displayName = displayName;
            this.map = map;
        }
    }
}
