
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
