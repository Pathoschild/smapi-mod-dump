using System.Collections.Generic;
using SilentOak.QualityProducts;
using SilentOak.QualityProducts.Utils;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Cooking
{
    /// <summary>
    /// Cooking item to be consumed from a list of items.
    /// </summary>
    internal class Ingredient
    {
        /***
         * Public properties
         ***/

        public IList<Item> ItemList { get; }
        public int Index { get; }
        public int Amount { get; }

        public Item Item
        {
            get
            {
                return ItemList[Index];
            }
        }


        /***
         * Public methods
         ***/

        /// <summary>
        /// Initializes a new instance of the <see cref="T:QualityProducts.Cooking.Ingredient"/> class.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        /// <param name="index">Index of item to be used.</param>
        /// <param name="amount">Amount to be used.</param>
        public Ingredient(IList<Item> itemList, int index, int amount)
        {
            ItemList = itemList;
            Index = index;
            Amount = amount;
        }

        /// <summary>
        /// Consume the items from the list.
        /// </summary>
        public void Consume()
        {
            if (Item is SObject @object)
            {
                Util.Monitor.VerboseLog($"Consuming {@object.Stack} {@object.DisplayName} (quality {@object.Quality})...");
            }
            else
            {
                Util.Monitor.VerboseLog($"Consuming {Item.Stack} {Item.DisplayName}...");
            }

            ItemList[Index].Stack -= Amount;
            if (ItemList[Index].Stack <= 0)
            {
                ItemList[Index] = null;
            }
        }
    }
}
