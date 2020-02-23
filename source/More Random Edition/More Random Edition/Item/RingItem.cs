namespace Randomizer
{
	/// <summary>
	/// Represents a fish
	/// </summary>
	public class RingItem : Item
	{
		public RingItem(int id) : base(id)
		{
			DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
			IsRing = true;
			CanStack = false;
		}
	}
}
