/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using TheLion.Stardew.Professions.Framework.Patches;

namespace TheLion.Stardew.Professions.Framework
{
	/// <summary>Unified entry point for applying Harmony patches.</summary>
	internal class HarmonyPatcher
	{
		/// <summary>Iterate through and apply any number of patches.</summary>
		/// <param name="patches">A sequence of <see cref="BasePatch"/> instances.</param>
		internal void ApplyAll(params BasePatch[] patches)
		{
			ModEntry.Log("Applying Harmony patches...", LogLevel.Trace);
			var harmony = new Harmony(ModEntry.UniqueID);
			foreach (var patch in patches) patch?.Apply(harmony);
		}
	}
}