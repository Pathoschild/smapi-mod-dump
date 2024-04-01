/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace DungeonMerchants
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;

        public List<int> DwarfFloors { get; set; } = new List<int>();
        public List<int> MerchantFloors { get; set; } = new List<int>();
        public int DwarfFloorMin = -1;
        public int DwarfFloorMax = 120;
        public int DwarfFloorMult = -1;
        public float DwarfFloorChancePercent = 5;
        public int MerchantFloorMin = 121;
        public int MerchantFloorMax = -1;
        public int MerchantFloorMult = -1;
        public float MerchantFloorChancePercent = 5;
    }
}
