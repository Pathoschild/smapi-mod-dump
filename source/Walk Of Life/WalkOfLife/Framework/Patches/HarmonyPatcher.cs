/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using Harmony;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Unified entry point for applying Harmony patches.</summary>
	internal class HarmonyPatcher
	{
		/// <summary>Iterate through and apply any number of patches.</summary>
		/// <param name="patches">A sequence of <see cref="IPatch"/> implementations.</param>
		internal void ApplyAll(params IPatch[] patches)
		{
			var harmony = HarmonyInstance.Create(AwesomeProfessions.UniqueID);
			foreach (var patch in patches) patch?.Apply(harmony);
		}
	}
}