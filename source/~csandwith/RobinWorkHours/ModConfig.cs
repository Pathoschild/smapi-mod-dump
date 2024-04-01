/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace RobinWorkHours
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int StartTime { get; set; } = 900;
        public int EndTime { get; set; } = 1700;
        public int FarmTravelTime { get; set; } = 160;
        public int TownTravelTime { get; set; } = 120;
        public int BackwoodsTravelTime { get; set; } = 170;

    }
}
