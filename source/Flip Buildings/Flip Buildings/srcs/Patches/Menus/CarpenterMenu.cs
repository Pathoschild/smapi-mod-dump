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
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using FlipBuildings.Managers;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class CarpenterMenuPatch
	{
		private const int 							region_flipButton = 110;
		internal static ClickableTextureComponent	flipButton;
		internal static bool						flipping;

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.PropertySetter(typeof(CarpenterMenu), nameof(CarpenterMenu.readOnly)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReadOnlyPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(DrawPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.UpdateAppearanceButtonVisibility)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(UpdateAppearanceButtonVisibilityPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.performHoverAction), new Type[] { typeof(int), typeof(int) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(PerformHoverActionPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReceiveLeftClickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenu)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReturnToCarpentryMenuPostfix))
			);
		}

		private static void ReadOnlyPostfix(ref bool value)
		{
			if (value)
			{
				flipButton.visible = false;
			}
		}

		private static bool DrawPrefix(CarpenterMenu __instance, SpriteBatch b)
		{
			if (Game1.IsFading() || __instance.freeze)
			{
				return true;
			}

			if (__instance.onFarm)
			{
				if (flipping)
				{
					string hoverText = (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
					string s = ModEntry.Helper.Translation.Get("Carpenter_SelectBuilding_Flip");

					SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
					__instance.cancelButton.draw(b);
					if (__instance.GetChildMenu() == null)
					{
						__instance.drawMouse(b);
						if (hoverText.Length > 0)
						{
							IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
						}
					}
					return false;
				}
			}
			return true;
		}

		private static void DrawPostfix(CarpenterMenu __instance, SpriteBatch b)
		{
			if (Game1.IsFading() || __instance.freeze)
			{
				return;
			}

			if (!__instance.onFarm)
			{
				string hoverText = (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

				flipButton.draw(b);
				__instance.drawMouse(b);
				if (__instance.GetChildMenu() == null)
				{
					__instance.drawMouse(b);
					if (hoverText.Length > 0)
					{
						IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
					}
				}
			}
		}

		private static void UpdateAppearanceButtonVisibilityPostfix(CarpenterMenu __instance)
		{
			__instance.backButton.myID = CarpenterMenu.region_backButton;
			__instance.forwardButton.myID = CarpenterMenu.region_forwardButton;
			__instance.appearanceButton.myID = CarpenterMenu.region_appearanceButton;
			flipButton = new ClickableTextureComponent("Flip", new Rectangle(__instance.xPositionOnScreen + __instance.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 384 - 20, __instance.yPositionOnScreen + __instance.maxHeightOfBuildingViewer + 64, 64, 64), null, null, AssetManager.flipButton, new Rectangle(0, 0, 16, 16), 4f)
			{
				myID = region_flipButton,
				rightNeighborID = CarpenterMenu.region_paintButton,
				leftNeighborID = CarpenterMenu.region_appearanceButton,
				upNeighborID = CarpenterMenu.region_appearanceButton,
				visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && __instance.TargetLocation.buildings.Any(building => BuildingHelper.CanBeFlipped(building)))
			};
			__instance.paintButton.myID = CarpenterMenu.region_paintButton;
			__instance.moveButton.myID = CarpenterMenu.region_moveBuitton;
			__instance.upgradeIcon.myID = CarpenterMenu.region_upgradeIcon;
			__instance.okButton.myID = CarpenterMenu.region_okButton;
			__instance.demolishButton.myID = CarpenterMenu.region_demolishButton;
			__instance.cancelButton.myID = CarpenterMenu.region_cancelButton;

			__instance.backButton.leftNeighborID = -1;
			__instance.backButton.rightNeighborID = __instance.forwardButton.myID;
			__instance.forwardButton.leftNeighborID = __instance.backButton.myID;
			__instance.forwardButton.rightNeighborID = __instance.appearanceButton.myID;
			__instance.appearanceButton.leftNeighborID = __instance.forwardButton.myID;
			__instance.appearanceButton.rightNeighborID = flipButton.myID;
			flipButton.leftNeighborID = __instance.appearanceButton.myID;
			flipButton.rightNeighborID = __instance.paintButton.myID;
			__instance.paintButton.leftNeighborID = flipButton.myID;
			__instance.paintButton.rightNeighborID = __instance.moveButton.myID;
			__instance.moveButton.leftNeighborID = __instance.paintButton.myID;
			__instance.moveButton.rightNeighborID = __instance.okButton.myID;
			__instance.upgradeIcon.leftNeighborID = __instance.moveButton.myID;
			__instance.upgradeIcon.rightNeighborID = __instance.okButton.myID;
			__instance.okButton.leftNeighborID = __instance.moveButton.myID;
			__instance.okButton.rightNeighborID = __instance.demolishButton.myID;
			__instance.demolishButton.leftNeighborID = __instance.okButton.myID;
			__instance.demolishButton.rightNeighborID = __instance.cancelButton.myID;
			__instance.cancelButton.leftNeighborID = __instance.demolishButton.myID;
			__instance.cancelButton.rightNeighborID = -1;

			if (!__instance.appearanceButton.visible && !flipButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else if (!flipButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.appearanceButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.appearanceButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = flipButton.myID;
				flipButton.leftNeighborID = __instance.forwardButton.myID;
				flipButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = flipButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !flipButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
				__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
				__instance.paintButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.paintButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !flipButton.visible && !__instance.paintButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.moveButton.myID;
				__instance.moveButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else if (!__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				flipButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = flipButton.myID;
			}
			else if (!flipButton.visible && !__instance.paintButton.visible)
			{
				__instance.appearanceButton.rightNeighborID = __instance.moveButton.myID;
				__instance.moveButton.leftNeighborID = __instance.appearanceButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !flipButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
				__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else
			{
				if (!__instance.moveButton.visible)
				{
					__instance.paintButton.rightNeighborID = __instance.okButton.myID;
					__instance.okButton.leftNeighborID = __instance.paintButton.myID;
				}
				else if (!__instance.paintButton.visible)
				{
					flipButton.rightNeighborID = __instance.moveButton.myID;
					__instance.moveButton.leftNeighborID = flipButton.myID;
				}
				if (!flipButton.visible)
				{
					__instance.appearanceButton.rightNeighborID = __instance.paintButton.myID;
					__instance.paintButton.leftNeighborID = __instance.appearanceButton.myID;
				}
				else if (!__instance.appearanceButton.visible)
				{
					__instance.forwardButton.rightNeighborID = flipButton.myID;
					flipButton.leftNeighborID = __instance.forwardButton.myID;
				}
			}
			if (!__instance.demolishButton.visible)
			{
				__instance.okButton.rightNeighborID = __instance.demolishButton.rightNeighborID;
				__instance.cancelButton.leftNeighborID = __instance.demolishButton.leftNeighborID;
			}
		}

		private static bool PerformHoverActionPrefix(CarpenterMenu __instance, int x, int y)
		{
			if (!__instance.onFarm)
			{
				flipButton.tryHover(x, y);
				if (flipButton.containsPoint(x, y))
				{
					typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, (string)ModEntry.Helper.Translation.Get("Carpenter_Flip"));
					return false;
				}
			}
			else
			{
				if ((!__instance.upgrading && !__instance.demolishing && !__instance.moving && !__instance.painting && !flipping) || __instance.freeze)
				{
					return false;
				}
				foreach (Building building2 in __instance.TargetLocation.buildings)
				{
					building2.color = Color.White;
				}

				Vector2 tile = new((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
				Building building = __instance.TargetLocation.getBuildingAt(tile) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 1f)) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 2f)) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 3f));
				BuildingData buildingData = building?.GetData();

				if (buildingData != null)
				{
					int num = (buildingData.SourceRect.IsEmpty ? building.texture.Value.Height : building.GetData().SourceRect.Height) * 4 / 64 - building.tilesHigh.Value;

					if (building.tileY.Value - num > tile.Y)
					{
						building = null;
					}
				}
				if (flipping)
				{
					if (building != null)
					{
						building.color = BuildingHelper.CanBeFlipped(building) ? Color.Lime : Color.Red * 0.8f;
					}
					return false;
				}
			}
			return true;
		}

		private static bool ReceiveLeftClickPrefix(CarpenterMenu __instance, int x, int y)
		{
			if (__instance.freeze)
			{
				return false;
			}
			if (__instance.cancelButton.containsPoint(x, y))
			{
				return true;
			}
			if (!__instance.onFarm)
			{
				if (flipButton.containsPoint(x, y) && flipButton.visible)
				{
					Game1.globalFadeToBlack(__instance.setUpForBuildingPlacement);
					Game1.playSound("smallSelect");
					__instance.onFarm = true;
					flipping = true;
				}
			}
			if (!__instance.onFarm || __instance.freeze || Game1.IsFading())
			{
				return true;
			}
			if (flipping)
			{
				Vector2 tile = new((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
				Building buildingAt = __instance.TargetLocation.getBuildingAt(tile);

				BuildingHelper.TryToFlip(buildingAt);
				return false;
			}
			return true;
		}

		private static void ReturnToCarpentryMenuPostfix()
		{
			flipping = false;
		}
	}
}
