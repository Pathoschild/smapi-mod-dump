/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/iSkLz/DecraftingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using static DecraftingModCS.Utils;

namespace DecraftingModCS
{
    public class CraftableItem
    {
        public CraftingRecipe Recipe;
        public Item Item;
        public List<Item> Ingradients;

        public CraftableItem(Item Item, CraftingRecipe Recipe)
        {
            this.Recipe = Recipe;
            this.Item = Item;
            Ingradients = new List<Item>();
        }

        public bool Equal(Item item)
        {
            return (Item.DisplayName == item.DisplayName && ItemID(item) == ItemID(Item));
        }
    }
}
