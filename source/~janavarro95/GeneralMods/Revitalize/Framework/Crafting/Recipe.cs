using System.Collections.Generic;
using System.Linq;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Crafting
{
    public class Recipe
    {
        public Dictionary<Item, int> ingredients;
        public Dictionary<Item, int> outputs;

        private Item displayItem;

        public Item DisplayItem
        {
            get => this.displayItem ?? this.outputs.ElementAt(0).Key;
            set => this.displayItem = value;
        }

        public string outputDescription;
        public string outputName;

        public StatCost statCost;

        public Recipe() { }

        /// <summary>Constructor for single item output.</summary>
        /// <param name="inputs">All the ingredients required to make the output.</param>
        /// <param name="output">The item given as output with how many</param>
        public Recipe(Dictionary<Item, int> inputs, KeyValuePair<Item, int> output, StatCost StatCost=null)
        {
            this.ingredients = inputs;
            this.DisplayItem = output.Key;
            this.outputDescription = output.Key.getDescription();
            this.outputName = output.Key.DisplayName;
            this.outputs = new Dictionary<Item, int>
            {
                [output.Key] = output.Value
            };
            this.statCost = StatCost ?? new StatCost();
        }

        public Recipe(Dictionary<Item, int> inputs, Dictionary<Item, int> outputs, string OutputName, string OutputDescription, Item DisplayItem = null,StatCost StatCost=null)
        {
            this.ingredients = inputs;
            this.outputs = outputs;
            this.outputName = OutputName;
            this.outputDescription = OutputDescription;
            this.DisplayItem = DisplayItem;
            this.statCost = StatCost ?? new StatCost();
        }

        /// <summary>Checks if a player contains all recipe ingredients.</summary>
        public bool PlayerContainsAllIngredients()
        {
            return this.InventoryContainsAllIngredient(Game1.player.Items.ToList());
        }

        /// <summary>Checks if a player contains a recipe ingredient.</summary>
        public bool PlayerContainsIngredient(KeyValuePair<Item, int> pair)
        {
            return this.InventoryContainsIngredient(Game1.player.Items.ToList(), pair);
        }

        /// <summary>Checks if an inventory contains all items.</summary>
        public bool InventoryContainsAllIngredient(List<Item> items)
        {
            foreach (KeyValuePair<Item, int> pair in this.ingredients)
                if (!this.InventoryContainsIngredient(items, pair)) return false;
            return true;
        }

        /// <summary>Checks if an inventory contains an ingredient.</summary>
        public bool InventoryContainsIngredient(List<Item> items, KeyValuePair<Item, int> pair)
        {
            foreach (Item i in items)
            {
                if (i != null && this.ItemEqualsOther(i, pair.Key) && pair.Value == i.Stack)
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

        public void consume(ref List<Item> from)
        {
            if (this.InventoryContainsAllIngredient(from)==false)
                return;

            InventoryManager manager = new InventoryManager(from);
            List<Item> removalList = new List<Item>();
            foreach (KeyValuePair<Item, int> pair in this.ingredients)
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
                    }
                }
            }

            foreach (var v in removalList)
                manager.items.Remove(v);
            removalList.Clear();
            from = manager.items;
        }

        public void produce(ref List<Item> to, bool dropToGround = false, bool isPlayerInventory = false)
        {
            var manager = isPlayerInventory
                ? new InventoryManager(new List<Item>())
                : new InventoryManager(to);
            foreach (KeyValuePair<Item, int> pair in this.outputs)
            {
                Item item = pair.Key.getOne();
                item.addToStack(pair.Value - 1);
                bool added = manager.addItem(item);
                if (!added && dropToGround)
                    Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.getDirection());
            }
            to = manager.items;
        }

        public void craft(ref List<Item> from, ref List<Item> to, bool dropToGround = false, bool isPlayerInventory = false)
        {
            InventoryManager manager = new InventoryManager(to);
            if (manager.ItemCount + this.outputs.Count >= manager.capacity)
            {
                if (isPlayerInventory)
                    Game1.showRedMessage("Inventory Full");
                else return;
            }
            this.consume(ref from);
            this.produce(ref to, dropToGround, isPlayerInventory);
        }

        public void craft()
        {
            List<Item> playerItems = Game1.player.Items.ToList();
            List<Item> outPutItems = new List<Item>();
            this.craft(ref playerItems, ref outPutItems, true, true);

            Game1.player.Items = playerItems; //Set the items to be post consumption.
            foreach (Item I in outPutItems)
                Game1.player.addItemToInventory(I); //Add all items produced.
            this.statCost.payCost();
        }

        public bool PlayerCanCraft()
        {
            return this.PlayerContainsAllIngredients() && this.statCost.canSafelyAffordCost();
        }

        public bool CanCraft(List<Item> items)
        {
            return this.InventoryContainsAllIngredient(items);
        }
    }
}
