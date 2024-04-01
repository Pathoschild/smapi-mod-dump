/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Extensions;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SolidFoundations.Framework.Patches.Buildings
{
    internal class BuildingPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Building);

        internal BuildingPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Building.doAction), new[] { typeof(Vector2), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(DoActionPrefix)));
            harmony.Patch(AccessTools.Method(_object, "CheckItemConversionRule", new[] { typeof(BuildingItemConversion), typeof(ItemQueryContext) }), prefix: new HarmonyMethod(GetType(), nameof(CheckItemConversionRulePrefix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Building.performActionOnDemolition), new[] { typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(PerformActionOnDemolitionPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.OnEndMove), null), postfix: new HarmonyMethod(GetType(), nameof(OnEndMovePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.isActionableTile), new[] { typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(IsActionableTilePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.IsValidObjectForChest), new[] { typeof(Item), typeof(Chest) }), postfix: new HarmonyMethod(GetType(), nameof(IsValidObjectForChestPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.updateInteriorWarps), new[] { typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateInteriorWarpsPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.Update), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.performTenMinuteAction), new[] { typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(PerformTenMinuteActionPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.load), null), postfix: new HarmonyMethod(GetType(), nameof(LoadPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.dayUpdate), new[] { typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(DayUpdatePostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Building.PerformBuildingChestAction), new[] { typeof(string), typeof(Farmer) }), transpiler: new HarmonyMethod(GetType(), nameof(PerformBuildingChestActionTranspiler)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.draw), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawTranspiler)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.drawBackground), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawBackgroundTranspiler)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.drawInMenu), new[] { typeof(SpriteBatch), typeof(int), typeof(int) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawMenuTranspiler)));
            harmony.Patch(AccessTools.Method(_object, nameof(Building.drawInConstruction), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawInConstructionTranspiler)));

            harmony.Patch(AccessTools.Constructor(_object, null), postfix: new HarmonyMethod(GetType(), nameof(BuildingPostfix)));
            harmony.Patch(AccessTools.Constructor(_object, new[] { typeof(string), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(BuildingPostfix)));
        }

        private static bool DoActionPrefix(Building __instance, Vector2 tileLocation, Farmer who, ref bool __result)
        {
            // Check if type is one of the extended models
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return true;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            if (who.isRidingHorse())
            {
                __result = false;
                return false;
            }
            if (who.IsLocalPlayer && tileLocation.X >= (float)(int)__instance.tileX.Value && tileLocation.X < (float)((int)__instance.tileX.Value + (int)__instance.tilesWide.Value) && tileLocation.Y >= (float)(int)__instance.tileY.Value && tileLocation.Y < (float)((int)__instance.tileY.Value + (int)__instance.tilesHigh.Value) && (int)__instance.daysOfConstructionLeft.Value > 0)
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:UnderConstruction"));
            }
            else
            {
                GameLocation interior = __instance.GetIndoors();
                if (who.IsLocalPlayer && (__instance.IsAuxiliaryTile(tileLocation) || tileLocation.X == (float)(__instance.humanDoor.X + (int)__instance.tileX.Value) && tileLocation.Y == (float)(__instance.humanDoor.Y + (int)__instance.tileY.Value) && interior != null))
                {
                    if (who.mount != null)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));

                        __result = false;
                        return false;
                    }
                    if (who.team.demolishLock.IsLocked())
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));

                        __result = false;
                        return false;
                    }
                    if (__instance.OnUseHumanDoor(who))
                    {
                        who.currentLocation.playSound("doorClose", tileLocation);
                        bool isStructure = interior != null;

                        Game1.warpFarmer(new LocationRequest(interior.NameOrUniqueName, isStructure, interior), interior.warps[0].X, interior.warps[0].Y - 1, Game1.player.FacingDirection);
                    }

                    __result = true;
                    return false;
                }

                BuildingData data = __instance.GetData();
                if (data != null)
                {
                    Microsoft.Xna.Framework.Rectangle door = __instance.getRectForAnimalDoor(data);
                    door.Width /= 64;
                    door.Height /= 64;
                    door.X /= 64;
                    door.Y /= 64;
                    if ((int)__instance.daysOfConstructionLeft.Value <= 0 && door != Microsoft.Xna.Framework.Rectangle.Empty && door.Contains(Utility.Vector2ToPoint(tileLocation)) && Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
                    {
                        __instance.ToggleAnimalDoor(who);

                        __result = true;
                        return false;
                    }
                    if (who.IsLocalPlayer && __instance.occupiesTile(tileLocation, applyTilePropertyRadius: true) && !__instance.isTilePassable(tileLocation))
                    {
                        Point actualTile = new Point((int)tileLocation.X - __instance.tileX.Value, (int)tileLocation.Y - __instance.tileY.Value);
                        var specialActionAtTile = extendedModel.GetSpecialActionAtTile(actualTile.X, actualTile.Y);
                        if (specialActionAtTile is not null)
                        {
                            specialActionAtTile.Trigger(who, __instance, actualTile);

                            __result = true;
                            return false;
                        }

                        if (who.ActiveObject is not null && extendedModel.LoadChestTiles is not null && extendedModel.GetLoadChestActionAtTile(actualTile.X, actualTile.Y) is var loadChestName && String.IsNullOrEmpty(loadChestName) is false)
                        {
                            __instance.PerformBuildingChestAction(loadChestName, who);

                            __result = true;
                            return false;
                        }
                        if (who.ActiveObject is null && extendedModel.CollectChestTiles is not null && extendedModel.GetCollectChestActionAtTile(actualTile.X, actualTile.Y) is var collectChestName && String.IsNullOrEmpty(collectChestName) is false)
                        {
                            __instance.PerformBuildingChestAction(collectChestName, who);

                            __result = true;
                            return false;
                        }

                        string tileAction = data.GetActionAtTile((int)tileLocation.X - __instance.tileX.Value, (int)tileLocation.Y - __instance.tileY.Value);
                        if (tileAction != null)
                        {
                            tileAction = TokenParser.ParseText(tileAction);
                            if (who.currentLocation.performAction(tileAction, who, new xTile.Dimensions.Location((int)tileLocation.X, (int)tileLocation.Y)))
                            {
                                __result = true;
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (who.IsLocalPlayer && !__instance.isTilePassable(tileLocation) && Building.TryPerformObeliskWarp(__instance.buildingType.Value, who))
                    {
                        __result = true;
                        return false;
                    }
                    if (who.IsLocalPlayer && who.ActiveObject != null && !__instance.isTilePassable(tileLocation))
                    {
                        return __instance.performActiveObjectDropInAction(who, probe: false);
                    }
                }
            }

            return false;
        }

        private static bool CheckItemConversionRulePrefix(Building __instance, ExtendedBuildingItemConversion conversion, ItemQueryContext itemQueryContext)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return true;
            }
            __instance.ProcessItemConversion(conversion, itemQueryContext);

            return false;
        }

        private static void PerformActionOnDemolitionPostfix(Building __instance, GameLocation location)
        {
            __instance.ClearLightSources(location);
        }

        private static void OnEndMovePostfix(Building __instance)
        {
            __instance.ResetLights(__instance.GetParentLocation());
        }

        private static void IsActionableTilePostfix(Building __instance, int xTile, int yTile, Farmer who, ref bool __result)
        {
            if (__instance.IsAuxiliaryTile(new Vector2(xTile, yTile)))
            {
                __result = true;
            }
        }

        private static void IsValidObjectForChestPostfix(Building __instance, Item item, Chest chest, ref bool __result)
        {
            if (__result is true && __instance.IsObjectFilteredForChest(item, chest))
            {
                __result = true;
            }
        }

        private static void UpdateInteriorWarpsPostfix(Building __instance, GameLocation interior = null)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            interior = interior ?? __instance.GetIndoors();
            if (interior == null)
            {
                return;
            }

            GameLocation parentLocation = __instance.GetParentLocation();
            var baseX = __instance.humanDoor.X;
            var baseY = __instance.humanDoor.Y;

            if (extendedModel.TunnelDoors.Count > 0)
            {
                var firstTunnelDoor = extendedModel.TunnelDoors.First();
                baseX = firstTunnelDoor.X;
                baseY = firstTunnelDoor.Y;
            }

            foreach (Warp warp in interior.warps)
            {
                if (parentLocation != null)
                {
                    warp.TargetName = parentLocation.NameOrUniqueName;
                }

                warp.TargetX = baseX + __instance.tileX.Value;
                warp.TargetY = baseY + __instance.tileY.Value + 1;
            }
        }

        private static void UpdatePostfix(Building __instance, GameTime time)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            // Handle lights
            var parentLocation = __instance.GetParentLocation();
            __instance.UpdateLightSources(parentLocation, time);

            // Catch touch actions
            if (parentLocation is not null)
            {
                Vector2 playerStandingPosition = new Vector2(Game1.player.StandingPixel.X / 64, Game1.player.StandingPixel.Y / 64);
                if (parentLocation.lastTouchActionLocation.Equals(Vector2.Zero))
                {
                    Point actualTile = new Point((int)playerStandingPosition.X - __instance.tileX.Value, (int)playerStandingPosition.Y - __instance.tileY.Value);
                    if (extendedModel.TunnelDoors.Any(d => d.X == actualTile.X && d.Y == actualTile.Y))
                    {
                        parentLocation.lastTouchActionLocation = new Vector2(Game1.player.StandingPixel.X / 64, Game1.player.StandingPixel.Y / 64);
                        bool isStructure = false;

                        var indoors = __instance.GetIndoors();
                        if (indoors is not null)
                        {
                            isStructure = true;
                        }

                        Game1.warpFarmer(new LocationRequest(indoors.NameOrUniqueName, isStructure, indoors), indoors.warps[0].X, indoors.warps[0].Y - 1, Game1.player.FacingDirection);
                    }

                    var specialActionAtTile = extendedModel.GetSpecialEventAtTile(actualTile.X, actualTile.Y);
                    if (specialActionAtTile is not null)
                    {
                        parentLocation.lastTouchActionLocation = new Vector2(Game1.player.StandingPixel.X / 64, Game1.player.StandingPixel.Y / 64);
                        specialActionAtTile.Trigger(Game1.player, __instance, actualTile);
                    }
                    else
                    {
                        string eventTile = extendedModel.GetEventAtTile((int)playerStandingPosition.X - __instance.tileX.Value, (int)playerStandingPosition.Y - __instance.tileY.Value);
                        if (eventTile != null)
                        {
                            parentLocation.lastTouchActionLocation = new Vector2(Game1.player.StandingPixel.X / 64, Game1.player.StandingPixel.Y / 64);

                            eventTile = TokenParser.ParseText(eventTile);
                            eventTile = SolidFoundations.modHelper.Reflection.GetMethod(new Dialogue(null, string.Empty, eventTile), "checkForSpecialCharacters").Invoke<string>(eventTile);
                            if (parentLocation.performAction(eventTile, Game1.player, new xTile.Dimensions.Location((int)parentLocation.lastTouchActionLocation.X, (int)parentLocation.lastTouchActionLocation.Y)) is false)
                            {
                                parentLocation.performTouchAction(eventTile, playerStandingPosition);
                            }
                        }
                    }
                }
                else if (!parentLocation.lastTouchActionLocation.Equals(playerStandingPosition))
                {
                    parentLocation.lastTouchActionLocation = Vector2.Zero;
                }
            }
        }

        private static void PerformTenMinuteActionPostfix(Building __instance, int timeElapsed)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }

            BuildingData data = __instance.GetData();
            if (data is null || !(data.ItemConversions?.Count > 0))
            {
                return;
            }

            ItemQueryContext itemQueryContext = new ItemQueryContext(__instance.GetParentLocation(), null, null);
            foreach (ExtendedBuildingItemConversion conversion in data.ItemConversions)
            {
                if (conversion.ShouldTrackTime is false)
                {
                    continue;
                }

                __instance.ProcessItemConversion(conversion, itemQueryContext, minutesElapsed: timeElapsed);
            }
        }

        private static void LoadPostfix(Building __instance)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            GameLocation interior = __instance.GetIndoors();
            if (extendedModel is not null && interior is not null && interior.Map is not null && extendedModel.ForceLocationToBeBuildable is true)
            {
                interior.Map.Properties["CanBuildHere"] = "T";
                interior.isAlwaysActive.Value = true;

                Game1.locations.Add(interior);
            }
        }

        private static void DayUpdatePostfix(Building __instance, int dayOfMonth)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            __instance.RefreshModel(extendedModel);
        }

        private static bool ValidateConditionsForDrawLayer(Building building, ExtendedBuildingDrawLayer layer)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return false;
            }

            return building.ValidateLayer(layer) is false;
        }

        private static Rectangle GetExtendedDrawLayerSourceRectangle(ExtendedBuildingDrawLayer layer, int time, Building building)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false || building.ValidateLayer(layer) is false)
            {
                return layer.GetSourceRect(time);
            }

            return layer.GetSourceRect(time, building);
        }

        private static Rectangle GetBuildingSourceRect(Building building)
        {
            Rectangle rectangle = building.getSourceRect();
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return rectangle;
            }

            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);
            if (extendedModel.DrawLayers is not null && extendedModel.DrawLayers.Any(l => l.HideBaseTexture && building.ValidateLayer(l)))
            {
                rectangle = Rectangle.Empty;
            }

            return rectangle;
        }

        private static int GetMaxItemsToTakeFromStack(BuildingItemConversion conversion, int currentAmount)
        {
            if (conversion is ExtendedBuildingItemConversion extendedConversion && extendedConversion is not null)
            {
                currentAmount = extendedConversion.TakeOnlyRequiredFromStack ? extendedConversion.RequiredCount : currentAmount;
            }

            return currentAmount;
        }

        private static IEnumerable<CodeInstruction> PerformBuildingChestActionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int CONVERSION_INDEX = 3; // Unused: Using OpCodes.Ldloc_3 instead
            int ACCEPT_AMOUNT_INDEX = 5;

            try
            {
                int acceptAmountInsertIndex = -1;

                // Get the indices to insert at
                var lines = instructions.ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].opcode == OpCodes.Stloc_S && lines[i].operand.ToString().Contains(ACCEPT_AMOUNT_INDEX.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        acceptAmountInsertIndex = i;
                        break;
                    }
                }

                lines.Insert(acceptAmountInsertIndex + 1, new CodeInstruction(OpCodes.Ldloc_3));
                lines.Insert(acceptAmountInsertIndex + 2, new CodeInstruction(OpCodes.Ldloc_S, ACCEPT_AMOUNT_INDEX));
                lines.Insert(acceptAmountInsertIndex + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetMaxItemsToTakeFromStack))));
                lines.Insert(acceptAmountInsertIndex + 4, new CodeInstruction(OpCodes.Stloc_S, ACCEPT_AMOUNT_INDEX));

                // Validate the changes
                if (acceptAmountInsertIndex == -1)
                {
                    throw new Exception("Unable to find insert position.");
                }

                return lines;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Buildings.Building.PerformBuildingChestAction: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int MAIN_SOURCE_RECT_INDEX = 5;
            int DRAW_LAYER_SOURCE_RECT_INDEX = 18;

            try
            {
                int conditionInsertIndex = -1;
                object drawLayer = null;
                object continueLabel = null;

                int sourceRectangleInsertIndex = -1;

                int drawMainSourceRectIndex = -1;

                // Get the indices to insert at
                var lines = instructions.ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].opcode == OpCodes.Brtrue && lines[i - 1].opcode == OpCodes.Ldfld && lines[i - 1].operand.ToString().Contains("DrawInBackground", StringComparison.OrdinalIgnoreCase))
                    {
                        drawLayer = lines[i - 2].operand;
                        continueLabel = lines[i].operand;

                        conditionInsertIndex = i;
                        break;
                    }
                }

                lines.Insert(conditionInsertIndex + 1, new CodeInstruction(OpCodes.Ldarg_0));
                lines.Insert(conditionInsertIndex + 2, new CodeInstruction(OpCodes.Ldloc, drawLayer));
                lines.Insert(conditionInsertIndex + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(ValidateConditionsForDrawLayer))));
                lines.Insert(conditionInsertIndex + 4, new CodeInstruction(OpCodes.Brtrue_S, continueLabel));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (sourceRectangleInsertIndex == -1 && lines[i + 1].opcode == OpCodes.Stloc_S && lines[i + 1].operand.ToString().Contains(DRAW_LAYER_SOURCE_RECT_INDEX.ToString(), StringComparison.OrdinalIgnoreCase) && lines[i].operand.ToString().Contains("GetSourceRect", StringComparison.OrdinalIgnoreCase))
                    {
                        sourceRectangleInsertIndex = i;
                        break;
                    }
                }

                lines[sourceRectangleInsertIndex] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetExtendedDrawLayerSourceRectangle)));
                lines.Insert(sourceRectangleInsertIndex, new CodeInstruction(OpCodes.Ldarg_0));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (drawMainSourceRectIndex == -1 && lines[i + 1].opcode == OpCodes.Newobj && lines[i].opcode == OpCodes.Ldloc_S && lines[i].operand.ToString().Contains(MAIN_SOURCE_RECT_INDEX.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        drawMainSourceRectIndex = i;
                    }
                }

                lines[drawMainSourceRectIndex] = new CodeInstruction(OpCodes.Ldarg_0);
                lines.Insert(drawMainSourceRectIndex + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetBuildingSourceRect))));

                // Validate the changes
                if (conditionInsertIndex == -1 || sourceRectangleInsertIndex == -1 || drawMainSourceRectIndex == -1)
                {
                    throw new Exception("Unable to find insert positions.");
                }

                return lines;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Buildings.Building.draw: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static IEnumerable<CodeInstruction> DrawBackgroundTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int DRAW_LAYER_SOURCE_RECT_INDEX = 6;
            try
            {
                int conditionInsertIndex = -1;
                object drawLayer = null;
                object continueLabel = null;

                int sourceRectangleInsertIndex = -1;

                // Get the indices to insert at
                var lines = instructions.ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].opcode == OpCodes.Brfalse && lines[i - 1].opcode == OpCodes.Ldfld && lines[i - 1].operand.ToString().Contains("DrawInBackground", StringComparison.OrdinalIgnoreCase))
                    {
                        drawLayer = lines[i - 2].operand;
                        continueLabel = lines[i].operand;

                        conditionInsertIndex = i;
                        break;
                    }
                }

                lines.Insert(conditionInsertIndex + 1, new CodeInstruction(OpCodes.Ldarg_0));
                lines.Insert(conditionInsertIndex + 2, new CodeInstruction(OpCodes.Ldloc, drawLayer));
                lines.Insert(conditionInsertIndex + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(ValidateConditionsForDrawLayer))));
                lines.Insert(conditionInsertIndex + 4, new CodeInstruction(OpCodes.Brtrue_S, continueLabel));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (sourceRectangleInsertIndex == -1 && lines[i + 1].opcode == OpCodes.Stloc_S && lines[i + 1].operand.ToString().Contains(DRAW_LAYER_SOURCE_RECT_INDEX.ToString(), StringComparison.OrdinalIgnoreCase) && lines[i].operand.ToString().Contains("GetSourceRect", StringComparison.OrdinalIgnoreCase))
                    {
                        sourceRectangleInsertIndex = i;
                        break;
                    }
                }

                lines[sourceRectangleInsertIndex] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetExtendedDrawLayerSourceRectangle)));
                lines.Insert(sourceRectangleInsertIndex, new CodeInstruction(OpCodes.Ldarg_0));

                // Validate the changes
                if (conditionInsertIndex == -1 || sourceRectangleInsertIndex == -1)
                {
                    throw new Exception("Unable to find insert positions.");
                }

                return lines;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Buildings.Building.drawInBackground: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static IEnumerable<CodeInstruction> DrawMenuTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int MAIN_SOURCE_RECT_INDEX = 3; // Unused: Using OpCodes.Ldloc_3 instead
            int DRAW_LAYER_SOURCE_RECT_INDEX = 6;

            try
            {
                int conditionInsertIndex = -1;
                object drawLayer = null;
                object continueLabel = null;

                int sourceRectangleInsertIndex = -1;

                int drawMainSourceRectIndex = -1;

                // Get the indices to insert at
                var lines = instructions.ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].opcode == OpCodes.Brfalse_S && lines[i - 1].opcode == OpCodes.Ldfld && lines[i - 1].operand.ToString().Contains("DrawInBackground", StringComparison.OrdinalIgnoreCase))
                    {
                        drawLayer = lines[i - 2].operand;
                        continueLabel = lines[i].operand;

                        conditionInsertIndex = i;
                        break;
                    }
                }

                lines.Insert(conditionInsertIndex + 1, new CodeInstruction(OpCodes.Ldarg_0));
                lines.Insert(conditionInsertIndex + 2, new CodeInstruction(OpCodes.Ldloc, drawLayer));
                lines.Insert(conditionInsertIndex + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(ValidateConditionsForDrawLayer))));
                lines.Insert(conditionInsertIndex + 4, new CodeInstruction(OpCodes.Brtrue_S, continueLabel));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (sourceRectangleInsertIndex == -1 && lines[i + 1].opcode == OpCodes.Stloc_S && lines[i + 1].operand.ToString().Contains(DRAW_LAYER_SOURCE_RECT_INDEX.ToString(), StringComparison.OrdinalIgnoreCase) && lines[i].operand.ToString().Contains("GetSourceRect", StringComparison.OrdinalIgnoreCase))
                    {
                        sourceRectangleInsertIndex = i;
                        break;
                    }
                }

                lines[sourceRectangleInsertIndex] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetExtendedDrawLayerSourceRectangle)));
                lines.Insert(sourceRectangleInsertIndex, new CodeInstruction(OpCodes.Ldarg_0));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (drawMainSourceRectIndex == -1 && lines[i + 1].opcode == OpCodes.Newobj && lines[i].opcode == OpCodes.Ldloc_3)
                    {
                        drawMainSourceRectIndex = i;
                    }
                }

                lines[drawMainSourceRectIndex] = new CodeInstruction(OpCodes.Ldarg_0);
                lines.Insert(drawMainSourceRectIndex + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(GetBuildingSourceRect))));

                // Validate the changes
                if (conditionInsertIndex == -1 || sourceRectangleInsertIndex == -1 || drawMainSourceRectIndex == -1)
                {
                    throw new Exception("Unable to find insert positions.");
                }

                return lines;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Buildings.Building.drawInBackground: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static IEnumerable<CodeInstruction> DrawInConstructionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                int insertIndex = -1;
                object drawLayer = null;
                object continueLabel = null;

                // Get the indices to insert at
                var lines = instructions.ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].opcode == OpCodes.Brtrue && lines[i - 1].opcode == OpCodes.Ldfld && lines[i - 1].operand.ToString().Contains("OnlyDrawIfChestHasContents", StringComparison.OrdinalIgnoreCase))
                    {
                        drawLayer = lines[i - 2].operand;
                        continueLabel = lines[i].operand;

                        insertIndex = i;
                        continue;
                    }
                }

                if (insertIndex == -1)
                {
                    throw new Exception("Unable to find insert position.");
                }

                // Insert the changes at the specified indices
                lines.Insert(insertIndex + 1, new CodeInstruction(OpCodes.Ldarg_0));
                lines.Insert(insertIndex + 2, new CodeInstruction(OpCodes.Ldloc, drawLayer));
                lines.Insert(insertIndex + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingPatch), nameof(ValidateConditionsForDrawLayer))));
                lines.Insert(insertIndex + 4, new CodeInstruction(OpCodes.Brtrue_S, continueLabel));

                return lines;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Buildings.Building.drawInBackground: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static void BuildingPostfix(Building __instance)
        {
            // Check if type is one of the extended models
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(__instance.buildingType.Value);

            __instance.RefreshModel(extendedModel);
        }
    }
}
