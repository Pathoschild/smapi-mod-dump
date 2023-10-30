/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using HarmonyLib;
using StardewValley.Menus;

namespace FlipBuildings.Patches
{
	internal class IClickableMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
				postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(PopulateClickableComponentListPostfix))
			);
		}

		private static void PopulateClickableComponentListPostfix(IClickableMenu __instance)
		{
			if (__instance.GetType() == typeof(CarpenterMenu))
				__instance.allClickableComponents.Add(CarpenterMenuPatch.flipButton);
		}
	}
}
