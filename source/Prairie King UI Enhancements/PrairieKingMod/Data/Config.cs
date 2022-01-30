/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Incognito357/PrairieKingUIEnhancements
**
*************************************************/


namespace PrairieKingUIEnhancements
{
    public class Config
    {
        public bool FixLevelOverflow { get; set; } = true;
        public bool ColoredIndicators { get; set; } = true;
        public bool FixNumberOverflow { get; set; } = true;
        public bool ShowDeathCounter { get; set; } = true;
        public bool ShowPowerupTimers { get; set; } = true;
        public bool FixBossHPOverflow { get; set; } = true;
        public bool FixTransitionColor { get; set; } = true;
        public bool HarderFinalBoss { get; set; } = true;
    }
}
