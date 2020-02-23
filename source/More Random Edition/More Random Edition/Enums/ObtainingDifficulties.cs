namespace Randomizer
{
	/// <summary>
	/// Used to track how difficult it is to get an item - intended to be used when determining
	/// crafting recipes and bundles
	/// </summary>
	public enum ObtainingDifficulties
	{
		/// <summary>
		/// You can reliably get it on your first day
		/// </summary>
		NoRequirements,

		/// <summary>
		/// You only need some time to get it (for example, you need to enter the mine and it's on the first few floors)
		/// </summary>
		SmallTimeRequirements,

		/// <summary>
		/// These take a small amount of time, but once you are there, it's not that bad
		/// Most gemstones will fit into this category
		/// </summary>
		MediumTimeRequirements,

		/// <summary>
		/// Foragables - as we don't really know where/when they will appear
		/// Crops probably fall under this category as well
		/// </summary>
		LargeTimeRequirements,

		/// <summary>
		/// Most artifacts
		/// </summary>
		UncommonItem,

		/// <summary>
		/// Like the prismatic shard, ancient seeds, and rare artifacts like the dinosaur egg
		/// </summary>
		RareItem,

		/// <summary>
		/// Like iridium bars
		/// </summary>
		EndgameItem,

		/// <summary>
		/// Use this for items that you don't want to show up in crafting recipes or bundles
		/// </summary>
		Impossible,

		/// <summary>
		/// Use this for items that you don't want to be used in crafting recipes, but can appear elsewhere
		/// </summary>
		NonCraftingItem
	}
}
