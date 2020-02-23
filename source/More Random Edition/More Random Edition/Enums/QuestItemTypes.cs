namespace Randomizer
{
	/// <summary>
	/// Represents what type of item a quest is asking the player to get
	/// </summary>
	public enum QuestItemTypes
	{
		/// <summary>
		/// Means that the quest won't change the item type - it's always the same ID
		/// </summary>
		Static,

		/// <summary>
		/// A crop
		/// </summary>
		Crop,

		/// <summary>
		/// A cooked dish
		/// </summary>
		Dish,

		/// <summary>
		/// A fish
		/// </summary>
		Fish,

		/// <summary>
		/// A random item
		/// </summary>
		Item
	}
}
