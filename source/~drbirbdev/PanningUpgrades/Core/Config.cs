/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared.Config;

namespace PanningUpgrades
{
    [ConfigClass]
    internal class Config
    {

        [ConfigOption]
        public bool BuyablePan { get; set; } = false;

        [ConfigOption(Min = 0, Max = 100000, Interval = 500)]
        public int BuyCost { get; set; } = 1000;

        [ConfigOption(Min = 0, Max = 3, Interval = 0.1f)]
        public float UpgradeCostMultiplier { get; set; } = 1.0f;

        [ConfigOption(Min = 1, Max = 20, Interval = 1)]
        public int UpgradeCostBars { get; set; } = 5;

        [ConfigOption(Min = 1, Max = 5, Interval = 1)]
        public int UpgradeDays { get; set; } = 2;

        [ConfigOption(Min = 0, Max = 1, Interval = 0.01f)]
        public float ExtraDrawBaseChance { get; set; } = 0.7f;

        [ConfigOption(Min = 0, Max = 1, Interval = 0.01f)]
        public float DailyLuckMultiplier { get; set; } = 1.0f;

        [ConfigOption(Min = 0, Max = 1, Interval = 0.01f)]
        public float LuckLevelMultiplier { get; set; } = 0.1f;

        [ConfigSectionTitle("AnimationSection")]

        [ConfigParagraph("AnimationSectionText")]

        [ConfigOption]
        public int AnimationFrameDuration { get; set; } = 140;

        [ConfigOption]
        public int AnimationYOffset { get; set; } = -8;

    }
}
