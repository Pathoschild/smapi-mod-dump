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
	/// Represents an item that is normally foragable
	/// </summary>
	public class ForagableItem : Item
	{
		public ForagableItem(int id) : base(id)
		{
			ShouldBeForagable = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			ItemsRequiredForRecipe = new Range(1, 3);
		}
	}
}
