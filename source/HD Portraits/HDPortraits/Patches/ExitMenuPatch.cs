/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace HDPortraits.Patches
{
	[HarmonyPatch]
	internal class ExitMenuPatch
	{
		[HarmonyPatch(typeof(Game1), "exitActiveMenu")]
		[HarmonyPrefix]
		internal static void PrefixExitMenu()
		{
			DialoguePatch.Finish();
		}

		[HarmonyPatch(typeof(Event), "cleanup")]
		[HarmonyPostfix]
		internal static void PostfixEventCleanup()
		{
			PortraitDrawPatch.NpcEventSuffixes.Value.Clear();
			DialoguePatch.Finish();
		}
	}
}
