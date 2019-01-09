using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTN2.MapData;

namespace MTN2.Compatibility {
    public class additionalMap {
        public string Location { get; set; }
        public string FileName { get; set; }
        public string mapType { get; set; } = "GameLocation";
        public string displayName { get; set; } = "Untitled";


        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
            foreach (additionalMap am in oldFarm.additionalMaps) {
                farm.AdditionalMaps.Add(new MapFile(am.FileName, am.mapType, am.Location));
            }
        }
    }
}
