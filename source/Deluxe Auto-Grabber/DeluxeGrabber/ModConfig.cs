
namespace DeluxeGrabber {
    public class ModConfig {
        public int GrabberRange;
        public bool DoGlobalForage;
        public bool DoHarvestCrops;
        public bool DoHarvestFlowers;
        public bool DoGainExperience;
        public int GlobalForageTileX;
        public int GlobalForageTileY;
        public string GlobalForageMap;

        public ModConfig() {
            DoGlobalForage = true;
            DoHarvestCrops = true;
            DoHarvestFlowers = true;
            DoGainExperience = false;
            GrabberRange = 10;
            GlobalForageTileX = 6;
            GlobalForageTileY = 35;
            GlobalForageMap = "Desert";
        }
    }
}
