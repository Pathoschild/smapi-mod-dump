/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using HarmonyLib;
using StardewValley.Menus;
using RelocateBuildingsAndFarmAnimals.Utilities;

namespace RelocateBuildingsAndFarmAnimals.Patches
{
	internal class DialogueBoxPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.closeDialogue)),
				postfix: new HarmonyMethod(typeof(PagedResponsesMenuUtility), nameof(PagedResponsesMenuUtility.AfterClose))
			);
		}
	}
}
