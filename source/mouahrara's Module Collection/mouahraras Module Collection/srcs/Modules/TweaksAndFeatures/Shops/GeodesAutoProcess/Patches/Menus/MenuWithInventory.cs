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
using HarmonyLib;
using StardewValley.Menus;
using mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Patches
{
	internal class MenuWithInventoryPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(MenuWithInventory), nameof(MenuWithInventory.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(MenuWithInventoryPatch), nameof(ReceiveKeyPressPrefix))
			);
		}

		private static bool ReceiveKeyPressPrefix(MenuWithInventory __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return true;

			if (__instance.GetType() == typeof(GeodeMenu))
			{
				if (__instance.okButton != null && __instance.okButton.containsPoint(x, y) && !(__instance as GeodeMenu).readyToClose())
				{
					GeodesAutoProcessUtility.EndGeodeProcessing();
					return false;
				}
			}
			return true;
		}
	}
}
