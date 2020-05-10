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
