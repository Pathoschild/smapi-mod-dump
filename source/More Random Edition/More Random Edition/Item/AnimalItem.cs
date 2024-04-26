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
    /// Represents items that you get from raising animals
    /// </summary>
    public class AnimalItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="index">The id of the item</param>
		public AnimalItem(ObjectIndexes index, 
			ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.MediumTimeRequirements) 
			: base(index)
		{
			IsAnimalProduct = true;
			DifficultyToObtain = difficultyToObtain;
		}
	}
}
