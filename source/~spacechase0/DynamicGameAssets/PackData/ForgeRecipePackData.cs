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
using Newtonsoft.Json;

namespace DynamicGameAssets.PackData
{
    public class ForgeRecipePackData : BasePackData
    {
        [JsonConverter(typeof(ItemAbstractionWeightedListConverter))]
        public List<Weighted<ItemAbstraction>> Result { get; set; }
        public ItemAbstraction BaseItem { get; set; }
        public ItemAbstraction IngredientItem { get; set; }
        public int CinderShardCost { get; set; }

        public override object Clone()
        {
            var ret = (ForgeRecipePackData)base.Clone();
            ret.Result = new List<Weighted<ItemAbstraction>>();
            foreach (var choice in this.Result)
                ret.Result.Add((Weighted<ItemAbstraction>)choice.Clone());
            ret.BaseItem = (ItemAbstraction)this.BaseItem.Clone();
            ret.IngredientItem = (ItemAbstraction)this.IngredientItem.Clone();
            return ret;
        }
    }
}
