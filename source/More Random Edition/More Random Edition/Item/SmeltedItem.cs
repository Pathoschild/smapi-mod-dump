namespace Randomizer
{
	/// <summary>
	/// Represents items requiring a furnace to easily obtain
	/// </summary>
	public class SmeltedItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		/// <param name="difficultyToObtain">The difficulty to obtain this item - defaults to medium</param>
		public SmeltedItem(int id, ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.MediumTimeRequirements) : base(id)
		{
			DifficultyToObtain = difficultyToObtain;
			IsSmelted = true;
			ItemsRequiredForRecipe = new Range(1, 5);
		}
	}
}
