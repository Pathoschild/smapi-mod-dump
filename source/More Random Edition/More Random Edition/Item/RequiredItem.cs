using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Used to track how many of an item might be required for something
	/// </summary>
	public class RequiredItem
	{
		public Item Item { get; set; }
		public int NumberOfItems { get; set; }
		public ItemQualities MinimumQuality { get; set; } = ItemQualities.Normal;
		private Range _rangeOfItems { get; set; }
		public int MoneyAmount { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public RequiredItem(Item requiredItem, int minValue, int maxValue)
		{
			Item = requiredItem;
			_rangeOfItems = new Range(minValue, maxValue);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="numberOfItems">The number of items required to craft this</param>
		public RequiredItem(Item requiredItem, int numberOfItems = 1)
		{
			Item = requiredItem;
			_rangeOfItems = new Range(numberOfItems, numberOfItems);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemId">The item id of the item that's required</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public RequiredItem(int itemId, int minValue, int maxValue)
		{
			Item = ItemList.Items[itemId];
			_rangeOfItems = new Range(minValue, maxValue);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemId">The item id of the item that's required</param>
		/// <param name="numberOfItems">The number of items required to craft this</param>
		public RequiredItem(int itemId, int numberOfItems = 1)
		{
			Item = ItemList.Items[itemId];
			_rangeOfItems = new Range(numberOfItems, numberOfItems);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public RequiredItem() { }

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemList">The item list</param>
		/// <param name="numberOfItems">The number of items to set each required item to</param>
		public static List<RequiredItem> CreateList(List<Item> itemList, int numberOfItems = 1)
		{
			List<RequiredItem> list = new List<RequiredItem>();
			foreach (Item item in itemList)
			{
				list.Add(new RequiredItem(item.Id, numberOfItems));
			}
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemList">The item list</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public static List<RequiredItem> CreateList(List<Item> itemList, int minValue, int maxValue)
		{
			List<RequiredItem> list = new List<RequiredItem>();
			foreach (Item item in itemList)
			{
				list.Add(new RequiredItem(item.Id, minValue, maxValue));
			}
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of item ids
		/// </summary>
		/// <param name="itemIdList">The item id list</param>
		/// <param name="numberOfItems">The number of items to set each required item to</param>
		public static List<RequiredItem> CreateList(List<int> itemIdList, int numberOfItems = 1)
		{
			List<RequiredItem> list = new List<RequiredItem>();
			foreach (int id in itemIdList)
			{
				list.Add(new RequiredItem(id, numberOfItems));
			}
			return list;
		}

		/// <summary>
		/// Creates a list of required items based on the given list of items
		/// </summary>
		/// <param name="itemIdList">The item id list</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public static List<RequiredItem> CreateList(List<int> itemIdList, int minValue, int maxValue)
		{
			List<RequiredItem> list = new List<RequiredItem>();
			foreach (int id in itemIdList)
			{
				list.Add(new RequiredItem(id, minValue, maxValue));
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
