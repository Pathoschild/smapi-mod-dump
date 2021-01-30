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
	/// A list of categories of crafting items
	/// This helps determine what the randomized recipes can possibly be
	/// </summary>
	public enum CraftableCategories
	{
		/// <summary>
		/// Use this for things like chests, torches, etc. - things you need early game
		/// </summary>
		Easy,

		/// <summary>
		/// Use this for things like paths
		/// </summary>
		EasyAndNeedMany,

		/// <summary>
		/// Use this for more useful things like scarecrows, sprinklers, etc.
		/// </summary>
		Moderate,

		/// <summary>
		/// Use this for things like fertilizer
		/// </summary>
		ModerateAndNeedMany,

		/// <summary>
		/// Use this for not quite endgame items such as quality sprinklers
		/// </summary>
		Difficult,

		/// <summary>
		/// Use this for things like quality fertilizer
		/// </summary>
		DifficultAndNeedMany,

		/// <summary>
		/// Use this for all endgame things, such as Iridium sprinklers
		/// </summary>
		Endgame,

		/// <summary>
		/// A special setting used by wild seeds - this will result in foragables from the current season
		/// being needed to craft the item
		/// </summary>
		Foragables
	}
}
