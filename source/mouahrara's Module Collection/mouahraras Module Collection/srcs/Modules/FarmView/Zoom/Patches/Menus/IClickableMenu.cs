/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using mouahrarasModuleCollection.FarmView.Zoom.Utilities;

namespace mouahrarasModuleCollection.FarmView.Zoom.Patches
{
	internal class IClickableMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveScrollWheelAction), new Type[] { typeof(int) }),
				postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ReceiveScrollWheelActionPostfix))
			);
		}

		private static void ReceiveScrollWheelActionPostfix(IClickableMenu __instance, int direction)
		{
			if (!Context.IsWorldReady || !ModEntry.Config.FarmViewZoom)
				return;
			if (__instance is not CarpenterMenu && __instance is not PurchaseAnimalsMenu && __instance is not AnimalQueryMenu)
				return;
			if ((__instance is CarpenterMenu || __instance is PurchaseAnimalsMenu) && (bool)__instance.GetType().GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return;
			if (!__instance.overrideSnappyMenuCursorMovementBan())
				return;
			ZoomUtility.AddZoomLevel(direction * 2);
		}
	}
}
