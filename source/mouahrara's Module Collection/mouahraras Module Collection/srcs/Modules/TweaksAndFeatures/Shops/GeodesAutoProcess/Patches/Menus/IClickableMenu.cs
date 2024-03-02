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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Patches
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
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
				prefix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ReceiveKeyPressPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
				postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ExitThisMenuPostfix))
			);
		}

		private static void PopulateClickableComponentListPostfix(IClickableMenu __instance)
		{
			if (!Context.IsWorldReady || !ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			if (__instance.GetType() == typeof(GeodeMenu))
			{
				__instance.allClickableComponents.Add(GeodeMenuPatch.stopButton);
			}
		}

		private static bool ReceiveKeyPressPrefix(IClickableMenu __instance, Keys key)
		{
			if (!Context.IsWorldReady || !ModEntry.Config.ShopsGeodesAutoProcess)
				return true;
			if (key == 0)
				return true;

			if (__instance.GetType() == typeof(GeodeMenu))
			{
				if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !(__instance as GeodeMenu).readyToClose())
				{
					GeodesAutoProcessUtility.EndGeodeProcessing();
					return false;
				}
			}
			return true;
		}

		private static void ExitThisMenuPostfix(IClickableMenu __instance)
		{
			if (!Context.IsWorldReady || !ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			if (__instance.GetType() == typeof(GeodeMenu))
			{
				if (GeodesAutoProcessUtility.FoundArtifact != null)
				{
					Game1.player.holdUpItemThenMessage(GeodesAutoProcessUtility.FoundArtifact);
				}
				GeodesAutoProcessUtility.CleanBeforeClosingGeodeMenu();
			}
		}
	}
}
