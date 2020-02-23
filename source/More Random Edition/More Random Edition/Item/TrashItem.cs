namespace Randomizer
{
	/// <summary>
	/// Represents a trash item that you can get while fishing, for example
	/// </summary>
	public class TrashItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item id</param>
		public TrashItem(int id) : base(id)
		{
			DifficultyToObtain = ObtainingDifficulties.NoRequirements;
			IsTrash = true;
		}
	}
}
