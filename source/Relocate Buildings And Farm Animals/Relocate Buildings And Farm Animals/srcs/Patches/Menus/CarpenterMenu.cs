/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;
using StardewValley.Locations;
using RelocateBuildingsAndFarmAnimals.Utilities;

namespace RelocateBuildingsAndFarmAnimals.Patches
{
	internal class CarpenterMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveRightClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReceiveRightClickPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReceiveKeyPressPrefix))
			);
		}

		private static void ReceiveRightClickPostfix(IClickableMenu __instance, int x, int y)
		{
			if (__instance is not CarpenterMenu carpenterMenu || carpenterMenu.freeze || !carpenterMenu.onFarm || carpenterMenu.cancelButton.containsPoint(x, y) || Game1.IsFading())
				return;

			if (carpenterMenu.moving)
			{
				if (carpenterMenu.buildingToMove == null)
				{
					Building buildingToMove = carpenterMenu.TargetLocation.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getMouseY(ui_scale: false)) / 64));

					if (buildingToMove != null)
					{
						if (buildingToMove.daysOfConstructionLeft.Value > 0)
							return;
						if ((buildingToMove.HasIndoorsName("Farmhouse") && buildingToMove.GetIndoors() is FarmHouse) || buildingToMove.isCabin || buildingToMove is GreenhouseBuilding)
						{
							Game1.addHUDMessage(new HUDMessage(ModEntry.Helper.Translation.Get("RelocateBuildingMessage.CannotRelocate"), HUDMessage.error_type));
							Game1.playSound("cancel");
							return;
						}
						if (!carpenterMenu.hasPermissionsToMove(buildingToMove))
							return;

						void OnResponse(string response)
						{
							if (carpenterMenu.TargetLocation.NameOrUniqueName.Equals(response))
							{
								carpenterMenu.buildingToMove = buildingToMove;
								carpenterMenu.buildingToMove.isMoving = true;
								Game1.playSound("axchop");
							}
							else
							{
								GameLocation locationFromName = Game1.getLocationFromName(response);

								if (locationFromName != null)
								{
									CarpenterMenuUtility.MainTargetLocation = carpenterMenu.TargetLocation;
									carpenterMenu.TargetLocation = locationFromName;
									carpenterMenu.buildingToMove = buildingToMove;
									carpenterMenu.buildingToMove.isMoving = true;
									Game1.globalFadeToBlack(carpenterMenu.setUpForBuildingPlacement, 0.04f);
									Game1.playSound("axchop");
								}
								else
								{
									ModEntry.Monitor.Log("Can't find location '" + response + "' for building relocate menu.", LogLevel.Error);
								}
							}
						}

						PagedResponsesMenuUtility.Open(ModEntry.Helper.Translation.Get("RelocateBuildingMenu.ChooseLocation"), PagedResponsesMenuUtility.GetRelocateBuildingsResponses(), OnResponse, auto_select_single_choice: true);
					}
				}
			}
		}

		private static bool ReceiveKeyPressPrefix(CarpenterMenu __instance, Keys key)
		{
			if (__instance.freeze || !__instance.onFarm || Game1.IsFading())
				return true;

			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && __instance.readyToClose() && Game1.locationRequest == null)
			{
				if (CarpenterMenuUtility.MainTargetLocation is not null)
				{
					__instance.TargetLocation = CarpenterMenuUtility.MainTargetLocation;
					CarpenterMenuUtility.MainTargetLocation = null;
					__instance.buildingToMove.isMoving = false;
					__instance.buildingToMove = null;
					Game1.globalFadeToBlack(__instance.setUpForBuildingPlacement, 0.04f);
					return false;
				}
			}
			return true;
		}
	}
}
