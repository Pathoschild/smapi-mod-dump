namespace Randomizer
{
	/// <summary>
	/// Minerals you can only get from geodes
	/// </summary>
	public class GeodeMineralItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		public GeodeMineralItem(int id) : base(id)
		{
			IsGeodeMineral = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
		}
	}
}
