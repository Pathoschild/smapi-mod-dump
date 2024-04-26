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
	/// Represents items requiring a furnace to easily obtain
	/// </summary>
	public class SmeltedItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="index">The index of the item</param>
		/// <param name="difficultyToObtain">The difficulty to obtain this item - defaults to medium</param>
		public SmeltedItem(ObjectIndexes index, 
			ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.MediumTimeRequirements) 
			: base(index)
		{
			DifficultyToObtain = difficultyToObtain;
			IsSmelted = true;
			ItemsRequiredForRecipe = new Range(1, 5);
		}
	}
}
