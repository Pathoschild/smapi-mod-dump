/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Harmony;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Harmony patch interface.</summary>
	internal interface IPatch
	{
		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		void Apply(HarmonyInstance harmony);
	}
}