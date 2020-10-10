/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

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
