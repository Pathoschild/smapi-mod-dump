
namespace DeluxeGrabber {
    public class ModConfig {
        public int GrabberRange;
        public bool DoGlobalForage;
        public bool DoHarvestTruffles;
        public bool DoHarvestCrops;
        public bool DoHarvestFlowers;
        public bool DoHarvestFruitTrees;
        public bool DoHarvestFarmCave;
        public bool DoGainExperience;
        public int GlobalForageTileX;
        public int GlobalForageTileY;
        public string GlobalForageMap;

        public ModConfig() {
            DoGlobalForage = true;
            DoHarvestTruffles = true;
            DoHarvestCrops = true;
            DoHarvestFlowers = true;
            DoHarvestFruitTrees = true;
            DoHarvestFarmCave = true;
            DoGainExperience = false;
            GrabberRange = 10;
            GlobalForageTileX = 6;
            GlobalForageTileY = 35;
            GlobalForageMap = "Desert";
        }
    }
}
