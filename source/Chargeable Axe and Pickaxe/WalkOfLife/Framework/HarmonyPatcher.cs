/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Unified entry point for applying multiple patches.</summary>
	internal class HarmonyPatcher
	{
		private readonly string _uniqueId;

		/// <summary>Construct an instance.</summary>
		/// <param name="uniqueId">The unique id for this mod.</param>
		internal HarmonyPatcher(string uniqueId)
		{
			_uniqueId = uniqueId;
		}

		/// <summary>Iterate through and apply any number of patches.</summary>
		/// <param name="patches">A sequence of base patch instances.</param>
		internal void ApplyAll(params BasePatch[] patches)
		{
			var harmony = HarmonyInstance.Create(_uniqueId);
			foreach (var patch in patches) patch.Apply(harmony);
		}
	}
}
