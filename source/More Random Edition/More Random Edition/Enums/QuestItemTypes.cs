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
	/// Represents what type of item a quest is asking the player to get
	/// </summary>
	public enum QuestItemTypes
	{
		/// <summary>
		/// Means that the quest won't change the item type - it's always the same ID
		/// </summary>
		Static,

		/// <summary>
		/// A crop
		/// </summary>
		Crop,

		/// <summary>
		/// A cooked dish
		/// </summary>
		Dish,

		/// <summary>
		/// A fish
		/// </summary>
		Fish,

		/// <summary>
		/// A random item
		/// </summary>
		Item
	}
}
