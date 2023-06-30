/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adverserath/StardewValley-CasualLifeMod
**
*************************************************/

namespace CasualLife
{
    class ModConfig
    {
        public bool ControlDayLightLevels { get; set; } = false;
        public bool ControlDayWithKeys { get; set; } = false;
        public bool Is24HourDefault { get; set; } = true;
        public bool DisplaySunTimes { get; set; } = true;
        public int MillisecondsPerSecond { get; set; } = 1000;

    }
}
