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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.ClintsShop.GeodesAutoProcess.Utilities;

namespace mouahrarasModuleCollection.ClintsShop.GeodesAutoProcess.Patches
{
	internal class GeodeMenuPatch
	{
		private const int 							region_stopButton = 4321;
		internal static ClickableTextureComponent	stopButton;
		private static int							i;

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(GeodeMenu)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(GeodeMenuPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.performHoverAction), new Type[] { typeof(int), typeof(int) }),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(PerformHoverActionPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReceiveLeftClickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReceiveLeftClickPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.update), new Type[] { typeof(GameTime) }),
				prefix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(Updateprefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.readyToClose)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReadyToClosePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.emergencyShutDown)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(EmergencyShutDownPostfix))
			);
		}

		private static void GeodeMenuPostfix(GeodeMenu __instance)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;

			GeodesAutoProcessUtility.InitializeAfterOpeningGeodeMenu(__instance);

			int x = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 648;
			int y = __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (ModEntry.Helper.Translation.Locale.Equals("ko-kr") ? 284 : 240);
			const int width = 96;
			const int height = 52;

			__instance.geodeSpot.myID = GeodeMenu.region_geodeSpot;
			stopButton = new ClickableTextureComponent(null, new Rectangle(x, y, width, height), null, null, Game1.mouseCursors, new Rectangle(441, 411, 24, 13), 4f)
			{
				myID = region_stopButton,
				rightNeighborID = -1,
				leftNeighborID = -1,
				visible = true
			};
			__instance.trashCan.myID = GeodeMenu.region_trashCan;
			__instance.okButton.myID = GeodeMenu.region_okButton;

			__instance.geodeSpot.leftNeighborID = -1;
			__instance.geodeSpot.rightNeighborID = stopButton.myID;
			stopButton.leftNeighborID = __instance.geodeSpot.myID;
			stopButton.rightNeighborID = __instance.trashCan.myID;
			__instance.trashCan.leftNeighborID = stopButton.myID;
			__instance.trashCan.rightNeighborID = -1;

			__instance.trashCan.upNeighborID = -1;
			__instance.trashCan.downNeighborID = __instance.okButton.myID;
			__instance.okButton.upNeighborID = __instance.trashCan.myID;
			__instance.okButton.downNeighborID = -1;

			if (__instance.inventory.inventory != null && __instance.inventory.inventory.Count >= 12)
			{
				stopButton.downNeighborID = 0;
				for (int i = 9; i < 12; i++)
				{
					if (__instance.inventory.inventory[i] != null)
						__instance.inventory.inventory[i].upNeighborID = stopButton.myID;
				}
			}
		}

		private static void DrawPostfix(GeodeMenu __instance, SpriteBatch b)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;

			b.Draw(stopButton.texture, stopButton.getVector2(), stopButton.sourceRect, Color.White * (GeodesAutoProcessUtility.IsProcessing() ? 1f : 0.5f), 0.0f, Vector2.Zero, stopButton.scale * 4f, SpriteEffects.None, 0.99f);
			__instance.drawMouse(b);
		}

		private static void PerformHoverActionPostfix(int x, int y)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;

			stopButton.scale = (GeodesAutoProcessUtility.IsProcessing() && stopButton.bounds.Contains(x, y)) ? 1.05f : 1f;
		}

		private static bool ReceiveLeftClickPrefix(GeodeMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return true;
			if (__instance.waitingForServerResponse)
				return true;
			if (!__instance.geodeSpot.containsPoint(x, y))
				return true;

			if (__instance.heldItem != null && Utility.IsGeode(__instance.heldItem) && Game1.player.Money >= 25 && __instance.geodeAnimationTimer <= 0)
			{
				if (Game1.player.freeSpotsInInventory() > 1 || (Game1.player.freeSpotsInInventory() == 1 && __instance.heldItem.Stack == 1))
				{
					GeodesAutoProcessUtility.StartGeodeProcessing();
					return false;
				}
			}
			return true;
		}

		private static void ReceiveLeftClickPostfix(GeodeMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;
			if (__instance.waitingForServerResponse)
				return;
			if (!GeodesAutoProcessUtility.IsProcessing())
				return;

			if (stopButton.containsPoint(x, y))
				GeodesAutoProcessUtility.EndGeodeProcessing();
		}

		private static bool Updateprefix(GeodeMenu __instance, GameTime time)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return true;
			if (__instance.geodeAnimationTimer <= 0)
				return true;

			if (i < ModEntry.Config.ClintsShopGeodesAutoProcessSpeedMultiplier)
			{
				i++;
				__instance.update(time);
				return true;
			}
			i = 0;
			return false;
		}

		private static void ReadyToClosePostfix(ref bool __result)
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;

			if (GeodesAutoProcessUtility.IsProcessing())
				__result = false;
		}

		private static void EmergencyShutDownPostfix()
		{
			if (!ModEntry.Config.ClintsShopGeodesAutoProcess)
				return;

			GeodesAutoProcessUtility.CleanBeforeClosingGeodeMenu();
		}
	}
}
