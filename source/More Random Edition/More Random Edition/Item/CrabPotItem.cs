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
	/// Represents a trash item that you can get while fishing, for example
	/// </summary>
	public class CrabPotItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item id</param>
		public CrabPotItem(int id) : base(id)
		{
			IsCrabPotItem = true;
			DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements;
		}
	}
}
