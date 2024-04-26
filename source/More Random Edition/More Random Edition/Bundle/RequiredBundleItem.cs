/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// Used to track how many of an item might be required for bundles
	/// THIS IS ONLY FOR BUNDLES, AS IT USES THE BUNDLE RNG!
    /// </summary>
    public class RequiredBundleItem
	{
		public Item Item { get; set; }
		public int NumberOfItems { get; set; }
		public ItemQualities MinimumQuality { get; set; } = ItemQualities.Normal;
		private Range RangeOfItems { get; set; }
		public int MoneyAmount { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public RequiredBundleItem(Item requiredItem, int minValue, int maxValue)
		{
			Item = requiredItem;
			RangeOfItems = new Range(minValue, maxValue);
			NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="numberOfItems">The number of items required to craft this</param>
		public RequiredBundleItem(Item requiredItem, int numberOfItems = 1)
		{
            Item = requiredItem;
			RangeOfItems = new Range(numberOfItems, numberOfItems);
			NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemId">The item id of the item that's required</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public RequiredBundleItem(ObjectIndexes itemId, int minValue, int maxValue)
		{
            Item = itemId.GetItem();
			RangeOfItems = new Range(minValue, maxValue);
			NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemId">The item id of the item that's required</param>
        /// <param name="minValue">The max number of items required to craft this</param>
        /// <param name="maxValue">The minimum number of items required to craft this</param>
        public RequiredBundleItem(BigCraftableIndexes itemId, int minValue, int maxValue)
        {
            Item = itemId.GetItem();
            RangeOfItems = new Range(minValue, maxValue);
            NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemId">The item id of the item that's required</param>
        /// <param name="numberOfItems">The number of items required to craft this</param>
        public RequiredBundleItem(ObjectIndexes itemId, int numberOfItems = 1)
		{
            Item = itemId.GetItem();
			RangeOfItems = new Range(numberOfItems, numberOfItems);
			NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemId">The item id of the item that's required</param>
        /// <param name="numberOfItems">The number of items required to craft this</param>
        public RequiredBundleItem(BigCraftableIndexes itemId, int numberOfItems = 1)
        {
            Item = itemId.GetItem();
            RangeOfItems = new Range(numberOfItems, numberOfItems);
            NumberOfItems = RangeOfItems.GetRandomValue(BundleRandomizer.Rng);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RequiredBundleItem() { }

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemList">The item list</param>
		/// <param name="numberOfItems">The number of items to set each required item to</param>
		public static List<RequiredBundleItem> CreateList(List<Item> itemList, int numberOfItems = 1)
		{
			List<RequiredBundleItem> list = new();
			foreach (Item item in itemList)
			{
				if (item.IsBigCraftable)
				{
                    list.Add(new RequiredBundleItem(item.BigCraftableIndex, numberOfItems));
                }
				else
				{
                    list.Add(new RequiredBundleItem(item.ObjectIndex, numberOfItems));
                }
			}
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemList">The item list</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public static List<RequiredBundleItem> CreateList(List<Item> itemList, int minValue, int maxValue)
		{
			List<RequiredBundleItem> list = new List<RequiredBundleItem>();
			foreach (Item item in itemList)
			{
                if (item.IsBigCraftable)
                {
                    list.Add(new RequiredBundleItem(item.BigCraftableIndex, minValue, maxValue));
                }
                else
                {
                    list.Add(new RequiredBundleItem(item.ObjectIndex, minValue, maxValue));
                }
            }
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of item ids
		/// </summary>
		/// <param name="itemIdList">The item id list</param>
		/// <param name="numberOfItems">The number of items to set each required item to</param>
		public static List<RequiredBundleItem> CreateList(List<ObjectIndexes> itemIdList, int numberOfItems = 1)
		{
			List<RequiredBundleItem> list = new();
			foreach (ObjectIndexes id in itemIdList)
			{
				list.Add(new RequiredBundleItem(id, numberOfItems));
			}
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemIdList">The item id list</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public static List<RequiredBundleItem> CreateList(List<ObjectIndexes> itemIdList, int minValue, int maxValue)
		{
			List<RequiredBundleItem> list = new();
			foreach (ObjectIndexes id in itemIdList)
			{
				list.Add(new RequiredBundleItem(id, minValue, maxValue));
			}
			return list;
		}

		/// <summary>
		/// Gets the string used for bundles
		/// </summary>
		/// <param name="useMoneyAmount">Whether to use the money amount for the string</param>
		/// <returns />
		public string GetStringForBundles(bool useMoneyAmount)
		{
			if (useMoneyAmount)
			{
				return $"-1 {MoneyAmount} {MoneyAmount}";
			}

			int numberOfItems = !Item.CanStack ? 1 : NumberOfItems;
			return $"{Item.Id} {numberOfItems} {(int)MinimumQuality}";
		}

		/// <summary>
		/// Gets the string used for buildings
		/// </summary>
		/// <returns />
		public string GetStringForBuildings()
		{
			int numberOfItems = !Item.CanStack ? 1 : NumberOfItems;
			return $"{Item.Id} {numberOfItems}";
		}
	}
}
