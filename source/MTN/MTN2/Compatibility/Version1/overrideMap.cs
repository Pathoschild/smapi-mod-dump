/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using MTN2.MapData;

namespace MTN2.Compatibility {
    public class overrideMap {
        public FileType type { get; set; } = FileType.xnb;
        public string Location { get; set; }
        public string FileName { get; set; }

        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
            if (oldFarm.overrideMaps == null) return;
            foreach(overrideMap om in oldFarm.overrideMaps) {
                farm.Overrides.Add(new MapFile(om.FileName, om.Location, om.type));
            }
        }
    }
}
