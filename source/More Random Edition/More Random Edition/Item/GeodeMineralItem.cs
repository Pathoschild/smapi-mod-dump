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
	/// Minerals you can only get from geodes
	/// </summary>
	public class GeodeMineralItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="index">The id of the item</param>
		public GeodeMineralItem(ObjectIndexes index) : base(index)
		{
			IsGeodeMineral = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
		}
	}
}
