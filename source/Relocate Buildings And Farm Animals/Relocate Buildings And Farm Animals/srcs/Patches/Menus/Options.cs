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
using StardewValley;
using StardewValley.Menus;
using RelocateBuildingsAndFarmAnimals.Utilities;

namespace RelocateBuildingsAndFarmAnimals.Patches
{
	internal class OptionsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.PropertyGetter(typeof(Options), nameof(Options.zoomLevel)),
				postfix: new HarmonyMethod(typeof(OptionsPatch), nameof(ZoomLevelPostfix))
			);
		}

		private static void ZoomLevelPostfix(ref float __result)
		{
			if (Game1.activeClickableMenu is not DialogueBox || !PagedResponsesMenuUtility.IsPagedResponsesMenu)
				return;

			__result = PagedResponsesMenuUtility.ZoomLevel;
		}
	}
}
