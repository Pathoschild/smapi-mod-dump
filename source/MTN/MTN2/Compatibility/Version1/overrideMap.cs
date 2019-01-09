using MTN2.MapData;

namespace MTN2.Compatibility {
    public class overrideMap {
        public FileType type { get; set; } = FileType.xnb;
        public string Location { get; set; }
        public string FileName { get; set; }

        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
            foreach(overrideMap om in oldFarm.overrideMaps) {
                farm.Overrides.Add(new MapFile(om.FileName, om.Location, om.type));
            }
        }
    }
}
