/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterPanning
{
    public class ModConfig
    {
        public bool useCustomPanningTreasure { get; set; }
        public bool enableSplashSounds { get; set; }
        public bool enableGeodeMineralsTreasure { get; set; }
        public bool enablePanningTrash { get; set; }
        public bool enableArtifactTreasures { get; set; }
        public bool enableAllArtifactsAfterFoundThemAll { get; set; }
        public bool enableSeedPanning { get; set; }
        public bool enableAllSeedsEverySeason { get; set; }
        public bool enableAllSecondYearSeedsOnFirstYear { get; set; }
        public bool sp_alwaysCreatePanningSpots { get; set; }
        public bool mp_alwaysCreatePanningSpots { get; set; }
        public double chanceOfCreatingPanningSpot { get; set; }
        public int maxNumberOfOrePointsGathered { get; set; }
        public bool showHudData { get; set; }
        public int hudXPostion { get; set; }
        public int hudYPostion { get; set; }
        public bool showDistance { get; set; }
        public double additionalLootChance { get; set; }
        public bool useCustomFarmMaps { get; set; }
        public Dictionary<int, string> customMaps { get; set; }
        public int configVersion { get; set; }
    }
}
