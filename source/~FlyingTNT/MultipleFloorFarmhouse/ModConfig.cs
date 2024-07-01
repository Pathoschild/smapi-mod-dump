/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/


using System.Collections.Generic;

namespace MultiStoryFarmhouse
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string FloorNames { get; set; } = "ManyRooms, EmptyHall";
        public bool CombineWithCellarStairs { get; set; } = true;
        public int MainFloorStairsX { get; set; } = 21;
        public int MainFloorStairsY { get; set; } = 32;
    }
}
