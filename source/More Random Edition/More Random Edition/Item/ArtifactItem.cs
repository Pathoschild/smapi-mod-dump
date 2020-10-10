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
	/// Represents all artifacts
	/// </summary>
	public class ArtifactItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		/// <param name="difficultyToObtain">The difficulty to obtain this artifact - defaults to UncommonItem</param>
		public ArtifactItem(int id, ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.UncommonItem) : base(id)
		{
			DifficultyToObtain = difficultyToObtain;
			IsArtifact = true;
		}
	}
}
