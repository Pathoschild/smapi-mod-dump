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
	/// Items that are resources - that is, very easy to get in bulk
	/// </summary>
	public class ResourceItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="index">The item index</param>
		public ResourceItem(ObjectIndexes index) : base(index)
		{
			IsResource = true;
			RequiredItemMultiplier = 5; // 5 will be the minimum number of items for resources by default
			ItemsRequiredForRecipe = new Range(1, 10);
			DifficultyToObtain = ObtainingDifficulties.NoRequirements;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The item index</param>
        /// <param name="requiredItemMultiplier">A multiplier for the number of required items for a recipe</param>
        /// <param  name="itemsRequiredForRecipe">A range for the number of items that could be required for a recipe</param>
        public ResourceItem(ObjectIndexes index, double requiredItemMultiplier, Range itemsRequiredForRecipe) : base(index)
		{
			IsResource = true;
			RequiredItemMultiplier = requiredItemMultiplier;
			ItemsRequiredForRecipe = itemsRequiredForRecipe;
			DifficultyToObtain = ObtainingDifficulties.NoRequirements;
		}
	}
}
