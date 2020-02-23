namespace Randomizer
{
	/// <summary>
	/// Represents an item you make in your kitchen
	/// </summary>
	public class CookedItem : Item
	{
		/// <summary>
		/// The speicial ingredient used to cook this item
		/// </summary>
		public string IngredientName { get; set; }

		public CookedItem(int id) : base(id)
		{
			IsCooked = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
		}
	}
}
