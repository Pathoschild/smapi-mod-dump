/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Crafting
{
    /// <summary>
    /// A vanilla recipe to be used with standard stardew valley machines such as the furnace.
    /// </summary>
    public class VanillaRecipe
    {
        /// <summary>
        /// The time it takes to make the recipe.
        /// </summary>
        public int timeToMake;
        /// <summary>
        /// The items required to craft the recipe.
        /// </summary>
        public Dictionary<Item, int> requiredItems;
        /// <summary>
        /// The item that becomes the held object for the vanilla object.
        /// </summary>
        public KeyValuePair<Item, int> outputItem;
        /// <summary>
        /// The stats it takes to craft this recipe.
        /// </summary>
        public StatCost stats;
        /// <summary>
        /// If true the produced item is automatically added it the player's inventory instead.
        /// </summary>
        public bool automaticallyPutIntoPlayerInventory;

        public VanillaRecipe()
        {

        }

        public VanillaRecipe(Dictionary<Item, int> RequiredItems, KeyValuePair<Item, int> OutputItem, int TimeToMake, StatCost Stats, bool AutoIntoPlayerItems)
        {
            this.requiredItems = RequiredItems;
            this.outputItem = OutputItem;
            this.timeToMake = TimeToMake;
            this.stats = Stats;
            this.automaticallyPutIntoPlayerInventory = AutoIntoPlayerItems;
        }

        public bool PlayerCanCraft()
        {
            return this.PlayerContainsAllIngredients() && this.stats.canSafelyAffordCost();
        }

        public bool CanCraft(List<Item> items)
        {
            return this.InventoryContainsAllIngredient(items);
        }

        /// <summary>Checks if a player contains all recipe ingredients.</summary>
        public bool PlayerContainsAllIngredients()
        {
            return this.InventoryContainsAllIngredient(Game1.player.Items);
        }

        /// <summary>Checks if a player contains a recipe ingredient.</summary>
        public bool PlayerContainsIngredient(KeyValuePair<Item, int> pair)
        {
            return this.InventoryContainsIngredient(Game1.player.Items.ToList(), pair);
        }

        /// <summary>Checks if an inventory contains all items.</summary>
        public bool InventoryContainsAllIngredient(IList<Item> items)
        {
            foreach (KeyValuePair<Item, int> pair in this.requiredItems)
                if (this.InventoryContainsIngredient(items, pair)==false) return false;
            return true;
        }

        /// <summary>Checks if an inventory contains an ingredient.</summary>
        public bool InventoryContainsIngredient(IList<Item> items, KeyValuePair<Item, int> pair)
        {
            foreach (Item i in items)
            {
                if (i != null && this.ItemEqualsOther(i, pair.Key) && pair.Value <= i.Stack)
                    return true;
            }
            return false;
        }

        /// <summary>Checks roughly if two items equal each other.</summary>
        public bool ItemEqualsOther(Item self, Item other)
        {
            return
                self.Name == other.Name
                && self.getCategoryName() == other.getCategoryName()
                && self.GetType() == other.GetType();
        }

        /// <summary>
        /// Consumes all of the ingredients for the recipe.
        /// </summary>
        /// <param name="from"></param>
        public void consume(ref IList<Item> from)
        {
            if (this.InventoryContainsAllIngredient(from) == false)
                return;

            InventoryManager manager = new InventoryManager(from);
            List<Item> removalList = new List<Item>();
            foreach (KeyValuePair<Item, int> pair in this.requiredItems)
            {
                foreach (Item item in manager.items)
                {
                    if (item == null) continue;
                    if (this.ItemEqualsOther(item, pair.Key))
                    {
                        if (item.Stack == pair.Value)
                            removalList.Add(item); //remove the item
                        else
                            item.Stack -= pair.Value; //or reduce the stack size.

                        ModCore.log("Remove: " + pair.Key.Name);
                    }
                }
            }

            foreach (var v in removalList)
                manager.items.Remove(v);
            removalList.Clear();
            from = manager.items;
        }

        /// <summary>
        /// Produces outputs for the crafting recipe.
        /// </summary>
        public void produce(ref StardewValley.Object obj)
        {

            Item I = this.outputItem.Key.getOne();
            I.Stack = this.outputItem.Value;
            if (this.automaticallyPutIntoPlayerInventory == false)
            {
                obj.heldObject.Value = (StardewValley.Object)I;
                obj.MinutesUntilReady = this.timeToMake;
            }
            else
            {
                if (Game1.player.isInventoryFull() == true)
                {
                    Game1.createItemDebris(I, Game1.player.getTileLocation(), Game1.random.Next(0, 4), Game1.player.currentLocation);
                }
                else
                {
                    Game1.player.addItemToInventory(I);
                }
            }

            return;
        }


        /// <summary>
        /// Consumes all ingredients in given inventory and adds in outputs to the other given inventory.
        /// </summary>
        private void craft(ref IList<Item> from, ref StardewValley.Object obj)
        {
            this.consume(ref from);
            this.stats.payCost();
            this.produce(ref obj);
        }

        /// <summary>
        /// Checks to see if the player can craft this object and if so go ahead and craft it.
        /// </summary>
        /// <param name="o"></param>
        public bool craft(StardewValley.Object o)
        {
            if (this.PlayerCanCraft())
            {
                IList<Item> playerItems = Game1.player.Items;
                List<Item> outPutItems = new List<Item>();
                ModCore.log("Can craft recipe.");
                this.craft(ref playerItems, ref o);
                return true;
            }
            return false;
        }
    }
}
