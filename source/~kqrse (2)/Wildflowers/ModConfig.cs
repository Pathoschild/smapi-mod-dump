/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace Wildflowers
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool WildFlowersMakeFlowerHoney { get; set; } = true;
        public bool FixFlowerFind { get; set; } = true;
        public int BeeRange { get; set; } = 5;
        public bool WeaponsHarvestFlowers { get; set; } = false;
        public float wildflowerGrowChance { get; set; } = 0.005f;
        public List<string> DisallowNames { get; set; } = new List<string>();
    }
}
