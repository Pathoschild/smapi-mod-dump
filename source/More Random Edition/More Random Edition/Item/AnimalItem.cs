namespace Randomizer
{
	/// <summary>
	/// Represents items that you get from raising animals
	/// </summary>
	public class AnimalItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		public AnimalItem(int id, ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.MediumTimeRequirements) : base(id)
		{
			IsAnimalProduct = true;
			DifficultyToObtain = difficultyToObtain;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		/// <param name="numberOfBuildingUpgrades">The number of upgrades you need to one building to get this animal</param>
		public AnimalItem(int id, int numberOfBuildingUpgrades) : base(id)
		{
			if (numberOfBuildingUpgrades > 0)
			{
				DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			}
			else
			{
				DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements;
			}
		}
	}
}
