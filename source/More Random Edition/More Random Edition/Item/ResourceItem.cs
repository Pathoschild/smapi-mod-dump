namespace Randomizer
{
	/// <summary>
	/// Items that are resources - that is, very easy to get in bulk
	/// </summary>
	public class ResourceItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item id</param>
		public ResourceItem(int id) : base(id)
		{
			IsResource = true;
			RequiredItemMultiplier = 5; // 5 will be the minimum number of items for resources by default
			ItemsRequiredForRecipe = new Range(1, 10);
			DifficultyToObtain = ObtainingDifficulties.NoRequirements;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item id</param>
		/// <param name="requiredItemMultiplier">A multiplier for the number of required items for a recipe</param>
		/// <param  name="itemsRequiredForRecipe">A range for the number of items that could be required for a recipe</param>
		public ResourceItem(int id, double requiredItemMultiplier, Range itemsRequiredForRecipe) : base(id)
		{
			IsResource = true;
			RequiredItemMultiplier = requiredItemMultiplier;
			ItemsRequiredForRecipe = itemsRequiredForRecipe;
			DifficultyToObtain = ObtainingDifficulties.NoRequirements;
		}
	}
}
