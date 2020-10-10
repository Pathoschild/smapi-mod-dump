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
using MTN2.MapData;

namespace MTN2.Compatibility {
    public class additionalMap {
        public string Location { get; set; }
        public string FileName { get; set; }
        public string mapType { get; set; } = "GameLocation";
        public string displayName { get; set; } = "Untitled";


        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
            if (oldFarm.additionalMaps == null) return;
            foreach (additionalMap am in oldFarm.additionalMaps) {
                farm.AdditionalMaps.Add(new MapFile(am.FileName, am.mapType, am.Location));
            }
        }
    }
}
