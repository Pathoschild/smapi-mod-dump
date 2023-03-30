/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace Fireworks;

[HarmonyPatch]
class HarmonyPatches
{
	[HarmonyPatch(typeof(Event), nameof(Event.command_cutscene))]
	[HarmonyPrefix]
	public static bool command_cutscene_Prefix(Event __instance, string[] split)
	{
		// if custom event script is active, skip prefix and run original code
		if (__instance.currentCustomEventScript != null)
		{
			return true;
		}
		if (split[1] == "ModJam")
		{
			__instance.currentCustomEventScript = new EventScript_ModJam();
		}

		return true;
	}
}
