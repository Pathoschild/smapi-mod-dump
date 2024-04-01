/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WinterPigs
{
    internal static class Patches
    {
        private static IMonitor IMonitor;
        private static IModHelper IHelper;
        private static string ModDataKey => $"{IHelper.ModRegistry.ModID}.Happiness";

        public static void Patch(IMonitor monitor, IModHelper helper)
        {
            IMonitor = monitor;
            IHelper = helper;

            Harmony harmony = new(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(BehaviorsTranspiler))
            );

            /*harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                transpiler: new(typeof(Patches), nameof(UpdateWhenNotCurrentLocationTranspiler))
            );*/

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(UpdateWhenNotCurrentLocationPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(DayUpdatePrefix))
            );
        }

        // Tested working
        private static IEnumerable<CodeInstruction> BehaviorsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int removeAbleIndices = 0;
            int index = -1;

            bool foundRainFlag = false;
            bool foundWinterFlag = false;

            var isRainingMethod = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
            var isWinterMethod = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsWinterHere));

            foreach (var instruction in instructions)
            {
                ++index;
                if (removeAbleIndices > 0)
                {
                    yield return new(OpCodes.Nop);
                    removeAbleIndices--;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldloc_0)
                {
                    var el = instructions.ElementAt(index + 2);
                    if (el.opcode == OpCodes.Callvirt)
                    {
                        if (el.operand == (object)isRainingMethod)
                        {
                            foundRainFlag = true;
                            removeAbleIndices = 3;
                            yield return new(OpCodes.Nop);
                            continue;
                        }
                        if (el.operand == (object)isWinterMethod)
                        {
                            foundWinterFlag = true;
                            removeAbleIndices = 3;
                            yield return new(OpCodes.Nop);
                            continue;
                        }
                    }
                }

                yield return instruction;
            }

            if (!foundRainFlag)
                IMonitor.LogOnce($"Failed to find rain flag, rain behavior was not changed", LogLevel.Error);
            if (!foundWinterFlag)
                IMonitor.LogOnce($"Failed to find winter flag, winter behavior was not changed", LogLevel.Error);
        }

        //Causes the game to freeze, if you think you can fix it, you're welcome to give it a shot (either way, I'll revisit it some day)
        /*private static IEnumerable<CodeInstruction> UpdateWhenNotCurrentLocationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            int removeAbleIndices = 0;
            int index = -1;
            bool isHandled = false;
            var canGoOutsideMethod = AccessTools.Method(typeof(Patches), nameof(canGoOutside));
            var updateRandomMovementsMethod = AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.UpdateRandomMovements));
            var jumpLabel = generator.DefineLabel();

            foreach (var instruction in instructions)
            {
                ++index;
                if (removeAbleIndices > 0)
                {
                    removeAbleIndices--;
                    continue;
                }

                if (isHandled)
                {
                    if (instruction.opcode == OpCodes.Ldarg_0)
                    {
                        var el = instructions.ElementAt(index + 1);
                        if (el.opcode == OpCodes.Callvirt && el.operand == (object)updateRandomMovementsMethod)
                        {
                            instruction.labels = [instruction.labels[^1], jumpLabel];
                            generator.MarkLabel(jumpLabel);
                        }
                    }
                    yield return instruction;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldarg_1)
                {
                    var el = instructions.ElementAt(index + 1);
                    if (el.opcode == OpCodes.Brfalse)
                    {
                        yield return new(OpCodes.Ldarg_0) { labels = instruction.labels };
                        yield return new(OpCodes.Ldarg_1);
                        yield return new(OpCodes.Ldarg_3);
                        yield return new(OpCodes.Call, canGoOutsideMethod);
                        yield return new(OpCodes.Brfalse_S, jumpLabel);

                        removeAbleIndices = 22;
                        isHandled = true;
                        continue;
                    }
                }
                yield return instruction;
            }
        }*/

        //I really tried...
        private static bool UpdateWhenNotCurrentLocationPrefix(FarmAnimal __instance, Building currentBuilding, GameTime time, GameLocation environment)
        {
            try
            {
                if (!Game1.shouldTimePass() || !Game1.IsMasterGame || !__instance.type.Value.Contains("Pig"))
                    return true;
                var pushEvent = IHelper.Reflection.GetField<NetEvent1Field<int, NetInt>>(__instance, "doFarmerPushEvent").GetValue();
                var pokeEvent = IHelper.Reflection.GetField<NetEvent0>(__instance, "doBuildingPokeEvent").GetValue();
                var diveEvent = IHelper.Reflection.GetField<NetEvent0>(__instance, "doDiveEvent").GetValue();
                pushEvent.Poll();
                pokeEvent.Poll();
                diveEvent.Poll();
                __instance.update(time, environment, __instance.myID.Value, false);
                if (__instance.hopOffset != Vector2.Zero)
                    __instance.HandleHop();
                else
                {
                    if (canGoOutside(__instance, currentBuilding, environment))
                    {
                        Farm locationFromName = (Farm)Game1.getLocationFromName("Farm");
                        if (locationFromName.isCollidingPosition(new Rectangle((currentBuilding.tileX.Value + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y) * 64 + 2, (__instance.buildingTypeILiveIn.Value == "Coop" ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, __instance, false, false, false) || locationFromName.isCollidingPosition(new Rectangle((currentBuilding.tileX.Value + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y + 1) * 64 + 2, (__instance.buildingTypeILiveIn.Value == "Coop" ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, __instance, false, false, false))
                            return false;
                        if (locationFromName.animals.ContainsKey(__instance.myID.Value))
                        {
                            for (int index = locationFromName.animals.Count() - 1; index >= 0; --index)
                            {
                                if (locationFromName.animals.Pairs.ElementAt(index).Key.Equals(__instance.myID.Value))
                                {
                                    locationFromName.animals.Remove(__instance.myID.Value);
                                    break;
                                }
                            }
                        }
                        (currentBuilding.indoors.Value as AnimalHouse)!.animals.Remove(__instance.myID.Value);
                        locationFromName.animals.Add(__instance.myID.Value, __instance);
                        updateHappiness(__instance);
                        __instance.faceDirection(2);
                        __instance.SetMovingDown(true);
                        __instance.Position = new Vector2(currentBuilding.getRectForAnimalDoor().X, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y) * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 32);
                        if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
                        {
                            ++FarmAnimal.NumPathfindingThisTick;
                            __instance.controller = new PathFindController(__instance, locationFromName, FarmAnimal.grassEndPointFunction, Game1.random.Next(4), FarmAnimal.behaviorAfterFindingGrassPatch, 200, Point.Zero);
                        }
                        if (__instance.controller is null || __instance.controller.pathToEndPoint is null || __instance.controller.pathToEndPoint.Count < 3)
                        {
                            __instance.SetMovingDown(true);
                            __instance.controller = null;
                        }
                        else
                        {
                            __instance.faceDirection(2);
                            __instance.Position = new Vector2(__instance.controller.pathToEndPoint.Peek().X * 64, __instance.controller.pathToEndPoint.Peek().Y * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 16);
                            if (__instance.displayHouse != "Coop")
                                __instance.position.X -= 32f;
                        }
                        __instance.noWarpTimer = 3000;
                        --currentBuilding.currentOccupants.Value;
                        if (Utility.isOnScreen(__instance.TilePoint, 192, locationFromName))
                            locationFromName.localSound("sandyStep");
                        environment.isTileOccupiedByFarmer(__instance.Tile)?.TemporaryPassableTiles.Add(__instance.GetBoundingBox());
                    }
                    __instance.UpdateRandomMovements();
                    __instance.behaviors(time, environment);
                }
                return false;
            }
            catch(Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(FarmAnimal.updateWhenNotCurrentLocation)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool DayUpdatePrefix(FarmAnimal __instance, GameLocation environment)
        {
            try
            {
                if (__instance.modData.ContainsKey(ModDataKey))
                    __instance.modData.Remove(ModDataKey);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(FarmAnimal.dayUpdate)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            return true;
        }

        private static bool canGoOutside(FarmAnimal animal, Building currentBuilding, GameLocation environment)
        {
            return currentBuilding != null &&
                   Game1.random.NextDouble() < 0.002 &&
                   currentBuilding.animalDoorOpen.Value &&
                   Game1.timeOfDay < 1630 &&
                   ((!Game1.isRaining && !Game1.currentSeason.Equals("winter")) ||
                   animal.type.Value.Contains("Pig")) &&
                   !Game1.isLightning &&
                   !environment.farmers.Any();
        }

        private static void updateHappiness(FarmAnimal animal)
        {
            if (animal.modData.ContainsKey(ModDataKey))
                return;
            byte happiness = 0;
            int friendship = 0;
            if (Game1.currentSeason.Equals("winter"))
            {
                happiness = (byte)(Game1.isSnowing ? 75 : 50);
                friendship = Game1.isSnowing ? 15 : 10;
            }
            else if (Game1.isRaining)
            {
                happiness = 25;
                friendship = 5;
            }
            animal.happiness.Value = (byte)Math.Max(0, animal.happiness.Value - happiness);
            animal.friendshipTowardFarmer.Value = Math.Max(0, animal.friendshipTowardFarmer.Value - friendship);
            animal.modData.Add(ModDataKey, "");
        }
    }
}
