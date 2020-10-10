/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

namespace FishingAdjustMod
{
    internal class Config
    {
        public float AdjustRatio { get; set; } = 0.8f;

        public bool OverrideGetSpringFishKing { get; set; } = false;
        public double SpringFishKingThreshold { get; set; } = 0.3;

        public bool OverrideGetSummerFishKing { get; set; } = false;
        public double SummerFishKingThreshold { get; set; } = 0.3;

        public bool OverrideGetFallFishKing { get; set; } = false;
        public double FallFishKingThreshold { get; set; } = 0.3;

        public bool OverrideGetWinterFishKing { get; set; } = false;
        public double WinterFishKingThreshold { get; set; } = 0.3;

        public bool OverrideGetSewerFishKing { get; set; } = false;
        public double SewerFishKingThreshold { get; set; } = 0.3;
    }
}
