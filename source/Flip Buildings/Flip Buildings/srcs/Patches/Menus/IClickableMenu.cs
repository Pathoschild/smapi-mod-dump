/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

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
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveRightClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ReceiveRightClickPrefix))
			);
		}

		private static void PopulateClickableComponentListPostfix(IClickableMenu __instance)
		{
			if (__instance.GetType() == typeof(CarpenterMenu))
			{
				__instance.allClickableComponents.Add(CarpenterMenuPatch.flipButton);
			}
		}

		private static bool ReceiveRightClickPrefix(IClickableMenu __instance, int x, int y)
		{
			if (__instance.GetType() != typeof(CarpenterMenu))
				return true;

			CarpenterMenu carpenterMenu = __instance as CarpenterMenu;

			if (carpenterMenu.freeze)
			{
				return true;
			}
			if (carpenterMenu.cancelButton.containsPoint(x, y))
			{
				return true;
			}
			if (!carpenterMenu.onFarm || carpenterMenu.freeze || Game1.IsFading())
			{
				return true;
			}
			if (CarpenterMenuPatch.flipping)
			{
				Vector2 tile = new((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
				Building buildingAt = carpenterMenu.TargetLocation.getBuildingAt(tile);

				BuildingUtility.TryToFlip(buildingAt, true);
				return false;
			}
			return true;
		}
	}
}
