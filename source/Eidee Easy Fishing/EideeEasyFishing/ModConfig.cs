/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

namespace EideeEasyFishing
{
    internal class ModConfig
    {
        public bool BiteFaster { get; set; } = false;
        public bool HitAutomatically { get; set; } = false;
        public bool SkipMinigame { get; set; } = false;
        public bool FishEasyCaught { get; set; } = false;
        public bool TreasureAlwaysBeFound { get; set; } = false;
        public bool TreasureEasyCaught { get; set; } = false;
        public bool AlwaysCaughtDoubleFish { get; set; } = false;
        public bool CaughtDoubleFishOnAnyBait { get; set; } = false;
        public bool AlwaysMaxCastPower { get; set; } = false;
        public float FishMovementSpeedMultiplier { get; set; } = 0.5f;
        public float ProgressBarDecreaseMultiplier { get; set; } = 0.5f;
        public float ProgressBarIncreaseMultiplier { get; set; } = 1.25f;
        public float TreasureCatchSpeedMultiplier { get; set; } = 1.5f;
        public ModConfigRawKeys Controls { get; set; } = new();
    }
}