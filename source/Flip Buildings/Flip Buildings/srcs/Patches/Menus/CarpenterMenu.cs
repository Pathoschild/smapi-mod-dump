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
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using FlipBuildings.Managers;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class CarpenterMenuPatch
	{
		private const int 							region_flipButton = 109;
		internal static ClickableTextureComponent	flipButton;
		private static bool							flipping;

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
				original: AccessTools.Method(typeof(CarpenterMenu), "resetBounds"),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ResetBoundsPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.performHoverAction)),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(PerformHoverActionPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick)),
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
			if (Game1.IsFading() || (bool)typeof(CarpenterMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return true;
			if ((bool)typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
			{
				if (flipping)
				{
					string s = ModEntry.Helper.Translation.Get("Carpenter_SelectBuilding_Flip");
					SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
					__instance.cancelButton.draw(b);
					__instance.drawMouse(b);
					if (((string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Length > 0)
						IClickableMenu.drawHoverText(b, (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance), Game1.dialogueFont);
					return false;
				}
			}
			return true;
		}

		private static void DrawPostfix(CarpenterMenu __instance, SpriteBatch b)
		{
			if (Game1.IsFading() || (bool)typeof(CarpenterMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return;
			if (!(bool)typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
			{
				flipButton.draw(b);
				__instance.drawMouse(b);
				if (((string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)).Length > 0)
					IClickableMenu.drawHoverText(b, (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance), Game1.dialogueFont);
			}
		}

		private static void ResetBoundsPostfix(CarpenterMenu __instance)
		{
			__instance.backButton.myID = CarpenterMenu.region_backButton;
			__instance.forwardButton.myID = CarpenterMenu.region_forwardButton;
			flipButton = new ClickableTextureComponent("Flip", new Microsoft.Xna.Framework.Rectangle(__instance.xPositionOnScreen + __instance.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 384 + (__instance.paintButton.visible ? 0 : 64) - 20, __instance.yPositionOnScreen + __instance.maxHeightOfBuildingViewer + 64, 64, 64), null, null, AssetManager.flipButton, new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 4f)
			{
				myID = region_flipButton,
				rightNeighborID = -1,
				leftNeighborID = -1,
				visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings
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
			__instance.forwardButton.rightNeighborID = flipButton.myID;
			flipButton.leftNeighborID = __instance.forwardButton.myID;
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

			if (!flipButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else
			{
				if (!flipButton.visible && !__instance.paintButton.visible)
				{
					__instance.forwardButton.rightNeighborID = __instance.moveButton.myID;
					__instance.moveButton.leftNeighborID = __instance.forwardButton.myID;
				}
				else if (!flipButton.visible && !__instance.moveButton.visible)
				{
					__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
					__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
					__instance.paintButton.rightNeighborID = __instance.okButton.myID;
					__instance.okButton.leftNeighborID = __instance.paintButton.myID;
				}
				else if (!__instance.paintButton.visible && !__instance.moveButton.visible)
				{
					flipButton.rightNeighborID = __instance.okButton.myID;
					__instance.okButton.leftNeighborID = flipButton.myID;
				}
				else
				{
					if (!flipButton.visible)
					{
						__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
						__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
					}
					else if (!__instance.paintButton.visible)
					{
						flipButton.rightNeighborID = __instance.moveButton.myID;
						__instance.moveButton.leftNeighborID = flipButton.myID;
					}
					else if (!__instance.moveButton.visible)
					{
						__instance.paintButton.rightNeighborID = __instance.okButton.myID;
						__instance.okButton.leftNeighborID = __instance.paintButton.myID;
					}
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
			if (!(bool)typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
			{
				flipButton.tryHover(x, y);
				if (flipButton.containsPoint(x, y))
					typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, (string)ModEntry.Helper.Translation.Get("Carpenter_Flip"));
				else
					return true;
			}
			else
			{
				if ((bool)typeof(CarpenterMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
					return false;
				Farm farm = Game1.getFarm();
				Vector2 vector2 = new Vector2((Game1.viewport.X + Game1.getOldMouseX(false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(false)) / 64);
				Building building = farm.getBuildingAt(vector2);
				foreach (Building build in ((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildings)
					build.color.Value = Color.White;
				if (building != null)
				{
					building.color.Value = CanBeFlipped(building) ? Color.Lime : Color.Red * 0.8f;
				}
				else
				{
					if (farm.GetHouseRect().Contains(Utility.Vector2ToPoint(vector2)))
					{
						farm.frameHouseColor = CanBeFlipped(building) ? Color.Lime : Color.Red * 0.8f;
					}
				}
			}
			return false;
		}

		private static bool ReceiveLeftClickPrefix(CarpenterMenu __instance, int x, int y, bool playSound = true)
		{
			if ((bool)typeof(CarpenterMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return false;
			if (__instance.cancelButton.containsPoint(x, y))
				return true;
			if (!(bool)typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) && flipButton.containsPoint(x, y) && flipButton.visible)
			{
				Game1.globalFadeToBlack(new Game1.afterFadeFunction(__instance.setUpForBuildingPlacement));
				Game1.playSound("smallSelect");
				typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, true);
				flipping = true;
			}
			if (!(bool)typeof(CarpenterMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) || Game1.IsFading() || (bool)typeof(CarpenterMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return true;
			if (flipping)
			{
				Farm farm = Game1.getFarm();
				Vector2 vector2 = new Vector2((Game1.viewport.X + Game1.getMouseX(false)) / 64, (Game1.viewport.Y + Game1.getMouseY(false)) / 64);
				Building building = farm.getBuildingAt(vector2);
				if (building != null)
				{
					if (!CanBeFlipped(building, out int reason))
					{
						Game1.addHUDMessage(new HUDMessage(reason == 1 ? ModEntry.Helper.Translation.Get("Carpenter_CannotFlip") : reason == 2 ? ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_Permission") : ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_PlayerHere"), Color.Red, 3500f) { whatType = 3 } );
						Game1.playSound("cancel");
						return false;
					}
					if (!building.modData.ContainsKey(ModDataKeys.FLIPPED))
						building.modData.Add(ModDataKeys.FLIPPED, "T");
					else
						building.modData.Remove(ModDataKeys.FLIPPED);
					if (!CompatibilityHelper.IsSolidFoundationsLoaded || !CompatibilityHelper.GenericBuildingType.IsAssignableFrom(building.GetType()))
						BuildingHelper.Update(building);
					else
						GenericBuildingHelper.Update(building);
					Game1.playSound("axchop");
				}
				else
				{
					if (farm.GetHouseRect().Contains(Utility.Vector2ToPoint(vector2)))
					{
						if (!CanBeFlipped(building, out int reason))
						{
							Game1.addHUDMessage(new HUDMessage(reason == 1 ? ModEntry.Helper.Translation.Get("Carpenter_CannotFlip") : reason == 2 ? ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_Permission") : ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_PlayerHere"), Color.Red, 3500f) { whatType = 3 } );
							Game1.playSound("cancel");
							return false;
						}
						if (farm.modData.ContainsKey(ModDataKeys.FLIPPED))
							farm.modData.Remove(ModDataKeys.FLIPPED);
						else
							farm.modData.Add(ModDataKeys.FLIPPED, "T");
						FarmHouseHelper.Flip();
						Game1.playSound("axchop");
						ModEntry.Helper.Multiplayer.SendMessage("FarmHouseHelper.Flip()", "InvokeMethod", modIDs: new[] { ModEntry.ModManifest.UniqueID });
					}
				}
				return false;
			}
			return true;
		}

		private static bool CanBeFlipped(Building building)
		{
			return CanBeFlipped(building, out int unused);
		}

		private static bool CanBeFlipped(Building building, out int reason)
		{
			static bool IsBuildingFlippable(Building building)
			{
				if (building != null && building is GreenhouseBuilding && !Game1.getFarm().greenhouseUnlocked.Value)
					return false;
				return true;
			}
			static bool HasPermissionToFlip(Building building)
			{
				if (Game1.IsMasterGame)
					return true;
				if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On)
					return true;
				if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings)
				{
					if (building != null)
					{
						if (building.hasCarpenterPermissions())
							return true;
						if (building.isCabin && building.indoors.Value is Cabin)
						{
							Farmer owner = (building.indoors.Value as Cabin).owner;
							if (Game1.player.UniqueMultiplayerID == owner.UniqueMultiplayerID)
								return true;
							if (Game1.player.spouse == owner.UniqueMultiplayerID.ToString())
								return true;
						}
					}
					else
					{
						if (Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID)
							return true;
						if (Game1.player.spouse == Game1.MasterPlayer.UniqueMultiplayerID.ToString())
							return true;
					}
				}
				return false;
			}
			static bool IsPlayerHere(Building building)
			{
				if (building != null)
				{
					Rectangle buildingRect = new Rectangle(building.tileX.Value * 64, building.tileY.Value * 64, building.tilesWide.Value * 64, building.tilesHigh.Value * 64);
					foreach (Farmer farmer in Game1.getOnlineFarmers())
					{
						if (farmer.GetBoundingBox().Intersects(buildingRect))
							return true;
					}
				}
				else
				{
					Rectangle houseRect = Game1.getFarm().GetHouseRect();
					houseRect.X *= 64;
					houseRect.Y *= 64;
					houseRect.Width *= 64;
					houseRect.Height *= 64;
					foreach (Farmer farmer in Game1.getOnlineFarmers())
					{
						if (farmer.GetBoundingBox().Intersects(houseRect))
							return true;
					}
				}
				return false;
			}

			reason = 0;
			if (!IsBuildingFlippable(building))
			{
				reason = 1;
				return false;
			}
			if (!HasPermissionToFlip(building))
			{
				reason = 2;
				return false;
			}
			if (IsPlayerHere(building))
			{
				reason = 4;
				return false;
			}
			return true;
		}

		private static void ReturnToCarpentryMenuPostfix(CarpenterMenu __instance)
		{
			flipping = false;
		}
	}
}
