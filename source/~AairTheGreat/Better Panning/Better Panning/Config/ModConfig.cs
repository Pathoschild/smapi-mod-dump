using System.Collections.Generic;

namespace BetterPanning
{
    public class ModConfig
    {
        public bool showDistance { get; set; }
        public bool sp_alwaysCreatePanningSpots { get; set; }
        public bool mp_alwaysCreatePanningSpots { get; set; }
        public int maxNumberOfOrePointsGathered { get; set; }
        public bool showHudData { get; set; }
        public int hudXPostion { get; set; }
        public int hudYPostion { get; set; }
        public double additionalLootChance { get; set; }
        public bool useCustomPanningTreasure { get; set; }
        public bool enableGeodeMineralsTreasure { get; set; }
        public bool enablePanningTrash { get; set; }
        public bool  enableArtifactTreasures { get; set; }
        public bool  enableAllArtifactsAfterFoundThemAll { get; set; }
        public bool enableSeedPanning { get; set; }
        public bool enableAllSeedsEverySeason { get; set; }
        public bool enableAllSecondYearSeedsOnFirstYear { get; set; }
        public bool useCustomFarmMaps { get; set; }
        public Dictionary<int, string> customMaps { get; set; }
    }
}
