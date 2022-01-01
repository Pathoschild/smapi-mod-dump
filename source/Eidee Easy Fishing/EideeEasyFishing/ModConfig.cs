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
    class ModConfig
    {
        public bool BiteFaster { get; set; } = false;
        public bool HitAutomatically { get; set; } = false;
        public bool SkipMinigame { get; set; } = false;
        public bool FishEasyCaught { get; set; } = true;
        public bool TreasureAlwaysBeFound { get; set; } = false;
        public bool TreasureEasyCaught { get; set; } = false;
        public bool AlwaysCaughtDoubleFish { get; set; } = false;
        public bool CaughtDoubleFishOnAnyBait { get; set; } = false;
        public bool AlwaysMaxCastPower { get; set; } = false;
        public ModConfigRawKeys Controls { get; set; } = new ModConfigRawKeys();
    }
}
