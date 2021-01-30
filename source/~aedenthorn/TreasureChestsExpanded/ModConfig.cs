/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace TreasureChestsExpanded
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MaxItems { get; set; } = 5;
        public int ItemsBaseMaxValue { get; set; } = 100;
        public int MinItemValue { get; set; } = 20;
        public int MaxItemValue { get; set; } = -1;
        public int CoinBaseMin { get; set; } = 20;
        public int CoinBaseMax { get; set; } = 100;
        public float IncreaseRate { get; set; } = 0.2f;
        public List<string> ItemListTypes { get; set; } = new List<string>
        {
            "Weapon", "Shirt", "Pants", "Hat", "Boots", "BigCraftable", "Ring", "Seed", "Mineral", "Relic"
        };
        public List<string> ItemListAllTypesDoNotEditJustCopyFromHere { get; set; } = new List<string>
        {
            "Weapon", "Shirt", "Pants", "Hat", "Boots", "BigCraftable", "Ring", "Cooking", "Seed", "Mineral", "Fish", "Relic", "BasicObject"
        };
    }
}
