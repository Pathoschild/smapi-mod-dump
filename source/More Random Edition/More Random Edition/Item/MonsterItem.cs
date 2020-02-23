namespace Randomizer
{
	/// <summary>
	/// Represents items that you get from killing monsters
	/// </summary>
	public class MonsterItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		public MonsterItem(int id, ObtainingDifficulties difficulty) : base(id, difficulty)
		{
			IsMonsterItem = true;
		}
	}
}
