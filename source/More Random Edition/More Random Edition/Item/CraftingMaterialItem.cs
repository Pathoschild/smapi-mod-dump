namespace Randomizer
{
	/// <summary>
	/// Used to track one item and how many of them are required to craft some other item
	/// </summary>
	public class CraftingMaterialItem
	{
		public Item RequiredItem { get; set; }
		public int NumberOfItems { get; set; }
		private Range _rangeOfItems { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="minValue">The max number of items required to craft this</param>
		/// <param name="maxValue">The minimum number of items required to craft this</param>
		public CraftingMaterialItem(Item requiredItem, int minValue, int maxValue)
		{
			RequiredItem = requiredItem;
			_rangeOfItems = new Range(minValue, maxValue);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requiredItem">The item that's required</param>
		/// <param name="numberOfItems">The number of items required to craft this</param>
		public CraftingMaterialItem(Item requiredItem, int numberOfItems)
		{
			RequiredItem = requiredItem;
			_rangeOfItems = new Range(numberOfItems, numberOfItems);
			NumberOfItems = _rangeOfItems.GetRandomValue();
		}
	}
}
