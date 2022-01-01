/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace DynamicGameAssets.PackData
{
    public class TailoringRecipePackData : BasePackData
    {
        public List<string> FirstItemTags { get; set; } = new(new[] { "item_cloth" });
        public List<string> SecondItemTags { get; set; }

        [DefaultValue(true)]
        public bool ConsumeSecondItem { get; set; } = true;

        [JsonConverter(typeof(ItemAbstractionWeightedListConverter))]
        public List<Weighted<ItemAbstraction>> CraftedItem { get; set; }
    }
}
