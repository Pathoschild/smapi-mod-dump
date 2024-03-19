/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace SpousesIsland
{
    internal class ModConfig
    {
        //general
        public int CustomChance { get; set; } = 10;
        public bool ScheduleRandom { get; set; }
        public bool IslandClothes { get; set; }

        public bool Devan { get; set; }

        //spouses blacklisted
        public string Blacklist { get; set; } = "";

        //children-related
        public bool AllowChildren { get; set; }
        public bool UseFurnitureBed { get; set; }
        public string Childbedcolor { get; set; } = "1"; //if not using furniture bed
    }
}