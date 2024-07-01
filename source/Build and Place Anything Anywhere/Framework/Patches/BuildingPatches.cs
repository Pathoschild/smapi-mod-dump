/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.UI;
using Common.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using xTile.Dimensions;

namespace AnythingAnywhere.Framework.Patches;
internal sealed class BuildingPatches : PatchHelper
{
    public void Apply()
    {
        Patch<GameLocation>(PatchType.Postfix, nameof(GameLocation.isBuildable), nameof(IsBuildablePostfix), [typeof(Vector2), typeof(bool)]);
        Patch<CarpenterMenu>(PatchType.Prefix, nameof(CarpenterMenu.receiveLeftClick), nameof(ReceiveLeftClickPrefix), [typeof(int), typeof(int), typeof(bool)]);
        Patch<CarpenterMenu>(PatchType.Prefix, nameof(CarpenterMenu.GetInitialBuildingPlacementViewport), nameof(GetInitialBuildingPlacementViewportPrefix), [typeof(GameLocation)]);
        Patch<CarpenterMenu>(PatchType.Transpiler, nameof(CarpenterMenu.draw), nameof(CarpenterMenuDrawTranspiler), [typeof(SpriteBatch)]);
    }

    // Sets tiles buildable for construction (just visual)
    private static void IsBuildablePostfix(GameLocation __instance, Vector2 tileLocation, ref bool __result, bool onlyNeedsToBePassable = false)
    {
        if (!ModEntry.Config.EnableBuilding) return;
        if (ModEntry.Config.EnableBuildAnywhere)
        {
            __result = true;
        }
        else if (!__instance.IsOutdoors && !ModEntry.Config.EnableBuildingIndoors)
        {
            __result = false;
        }
        else if (__instance.isTilePassable(tileLocation) && !__instance.isWaterTile((int)tileLocation.X, (int)tileLocation.Y))
        {
            __result = !__instance.IsTileOccupiedBy(tileLocation, CollisionMask.All, CollisionMask.All);
        }
        else
        {
            __result = false; // Set to false if the tile is not passable
        }
    }
    private static bool ReceiveLeftClickPrefix(CarpenterMenu __instance, int x, int y, bool playSound = true)
    {
        if (!ModEntry.Config.EnableBuilding)
            return true;

        if (__instance.freeze)
            return true;

        if (!__instance.onFarm)
            return true;

        if (__instance.cancelButton.containsPoint(x, y))
            return true;

        if (!__instance.onFarm && __instance.backButton.containsPoint(x, y))
            return true;

        if (!__instance.onFarm && __instance.forwardButton.containsPoint(x, y))
            return true;

        if (!__instance.onFarm)
            return true;

        if (!__instance.onFarm || __instance.freeze || Game1.IsFading())
            return true;

        GameLocation farm;
        Building destroyed;
        GameLocation interior;
        Cabin? cabin;
        if (__instance.demolishing)
        {
            farm = __instance.TargetLocation;
            destroyed = farm.getBuildingAt(new Vector2((float)(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (float)(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64));
            if (destroyed == null)
            {
                return false;
            }
            interior = destroyed.GetIndoors();
            cabin = interior as Cabin;
            if (cabin != null && !Game1.IsMasterGame)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
                return false;
            }
            if (!__instance.CanDemolishThis(destroyed))
            {
                return false;
            }
            if (!Game1.IsMasterGame && !__instance.hasPermissionsToDemolish(destroyed))
            {
                return false;
            }
            Cabin? cabin2 = cabin;
            if (cabin != null && cabin2?.HasOwner == true && cabin.owner.isCustomized.Value)
            {
                Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.owner.Name), Game1.currentLocation.createYesNoResponses(), (f, answer) =>
                {
                    if (answer == "Yes")
                    {
                        Game1.activeClickableMenu = __instance;
                        Game1.player.team.demolishLock.RequestLock(ContinueDemolish, BuildingLockFailed);
                    }
                    else
                    {
                        DelayedAction.functionAfterDelay(__instance.returnToCarpentryMenu, 500);
                    }
                });
            }
            else
            {
                Game1.player.team.demolishLock.RequestLock(ContinueDemolish, BuildingLockFailed);
            }

            return false;
        }

        if (__instance.upgrading)
        {
            Building toUpgrade = __instance.TargetLocation.getBuildingAt(new Vector2((float)(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (float)(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64));
            if (toUpgrade != null && toUpgrade.buildingType.Value == __instance.Blueprint.UpgradeFrom)
            {
                __instance.ConsumeResources();
                toUpgrade.upgradeName.Value = __instance.Blueprint.Id;
                toUpgrade.daysUntilUpgrade.Value = Math.Max(__instance.Blueprint.BuildDays, 1);
                toUpgrade.showUpgradeAnimation(__instance.TargetLocation);
                Game1.playSound("axe");
                if (!ModEntry.Config.BuildModifier!.IsDown() || !__instance.CanBuildCurrentBlueprint())
                {
                    DelayedAction.functionAfterDelay(__instance.returnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    __instance.freeze = true;
                }
                ModEntry.Multiplayer?.globalChatInfoMessage("BuildingBuild", Game1.player.Name, "aOrAn:" + __instance.Blueprint.TokenizedDisplayName, __instance.Blueprint.TokenizedDisplayName, Game1.player.farmName.Value);
                if (__instance.Blueprint.BuildDays < 1)
                {
                    toUpgrade.FinishConstruction();
                }
                else
                {
                    Game1.netWorldState.Value.MarkUnderConstruction(__instance.Builder, toUpgrade);
                }
            }
            else if (toUpgrade != null)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), 3));
            }
            return false;
        }

        if (__instance.painting)
            return true;

        if (__instance.moving)
            return true;

        Game1.player.team.buildLock.RequestLock(delegate
        {
            if (__instance.onFarm && Game1.locationRequest == null)
            {
                if (__instance.tryToBuild())
                {
                    __instance.ConsumeResources();
                    if (!ModEntry.Config.BuildModifier!.IsDown() || !__instance.CanBuildCurrentBlueprint())
                    {
                        DelayedAction.functionAfterDelay(__instance.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
                        __instance.freeze = true;
                    }
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), 3));
                }
            }
            Game1.player.team.buildLock.ReleaseLock();
        });

        return false;

        void BuildingLockFailed()
        {
            if (__instance.demolishing)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
            }
        }

        void ContinueDemolish()
        {
            if (!__instance.demolishing || !farm.buildings.Contains(destroyed)) return;

            if (destroyed.daysOfConstructionLeft.Value > 0 || destroyed.daysUntilUpgrade.Value > 0)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), 3));
            }
            else if (interior is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), 3));
            }
            else if (interior?.farmers.Any() == true)
            {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
            }
            else
            {
                if (cabin != null)
                {
                    if (Game1.getAllFarmers().Any(farmer => farmer.currentLocation != null && farmer.currentLocation.Name == cabin.GetCellarName()))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
                        return;
                    }

                    if (cabin.IsOwnerActivated)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), 3));
                        return;
                    }
                }
                destroyed.BeforeDemolish();
                Chest? chest = null;
                if (cabin != null)
                {
                    List<Item> items = cabin.demolish();
                    if (items.Count > 0)
                    {
                        chest = new Chest(playerChest: true);
                        chest.fixLidFrame();
                        chest.Items.OverwriteWith(items);
                    }
                }

                if (!farm.destroyStructure(destroyed)) return;
                Game1.flashAlpha = 1f;
                destroyed.showDestroyedAnimation(__instance.TargetLocation);
                Game1.playSound("explosion");
                Utility.spreadAnimalsAround(destroyed, farm);
                if (!ModEntry.Config.BuildModifier!.IsDown())
                {
                    DelayedAction.functionAfterDelay(__instance.returnToCarpentryMenu, 1500);
                    __instance.freeze = true;
                }
                if (chest != null)
                {
                    farm.objects[new Vector2(destroyed.tileX.Value + (destroyed.tilesWide.Value / 2), destroyed.tileY.Value + (destroyed.tilesHigh.Value / 2))] = chest;
                }
            }
        }
    }

    private static bool GetInitialBuildingPlacementViewportPrefix(CarpenterMenu __instance, GameLocation location, ref Location __result)
    {
        if (!ModEntry.Config.EnableBuilding) return true;
        if (Game1.activeClickableMenu is not BuildAnywhereMenu) return true;

        __result = CenterOnTile((int)Game1.player.Tile.X, (int)Game1.player.Tile.Y);

        return false;

        static Location CenterOnTile(int x, int y)
        {
            x = (int)((x * 64) - (Game1.viewport.Width / 2f));
            y = (int)((y * 64) - (Game1.viewport.Height / 2f));
            return new Location(x, y);
        }
    }

    // Don't display gold if build cost is less than 1 instead of 0
    private static IEnumerable<CodeInstruction> CarpenterMenuDrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var codeInstructions = instructions.ToList();
        try
        {
            var matcher = new CodeMatcher(codeInstructions, generator);

            matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Property(typeof(CarpenterMenu.BlueprintEntry), nameof(CarpenterMenu.BlueprintEntry.BuildCost)).GetGetMethod()),
                    new CodeMatch(OpCodes.Ldc_I4_0))
                .Set(OpCodes.Ldc_I4_1, null)
                .ThrowIfNotMatch("Could not find blueprint.BuildCost");

            return matcher.InstructionEnumeration();
        }
        catch (Exception e)
        {
            ModEntry.ModMonitor.Log($"There was an issue modifying the instructions for {typeof(CarpenterMenu)}.{original.Name}: {e}", LogLevel.Error);
            return codeInstructions;
        }
    }
}