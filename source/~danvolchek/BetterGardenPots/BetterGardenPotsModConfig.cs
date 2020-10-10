/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterGardenPots
{
    internal class BetterGardenPotsModConfig
    {
        public bool MakeBeeHousesNoticeFlowersInGardenPots { get; set; } = true;
        public bool MakeSprinklersWaterGardenPots { get; set; } = true;
        public bool HarvestMatureCropsWhenGardenPotBreaks { get; set; } = true;

        public bool AllowPlantingAncientSeedsInGardenPots { get; set; } = false;
        public bool AllowCropsToGrowInAnySeasonOutsideWhenInGardenPot { get; set; } = false;
    }
}
