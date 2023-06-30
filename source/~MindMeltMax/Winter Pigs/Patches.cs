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
                original: AccessTools.Method(typeof(FarmAnimal), "behaviors", new[] { typeof(GameTime), typeof(GameLocation) }),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(BehaviorsTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(UpdateWhenNotCurrentLocationPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(DayUpdatePrefix))
            );
        }

        private static IEnumerable<CodeInstruction> BehaviorsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int removeAbleIndices = 0;

            bool foundRainFlag = false;
            bool foundWinterFlag = false;

            var isRainingField = AccessTools.Field(typeof(Game1), nameof(Game1.isRaining));
            var currentSeasonField = AccessTools.Field(typeof(Game1), nameof(Game1.currentSeason));

            foreach (var instruction in instructions)
            {
                if (removeAbleIndices > 0)
                {
                    yield return new(OpCodes.Nop);
                    removeAbleIndices--;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldsfld)
                {
                    if (instruction.operand == (object)isRainingField)
                    {
                        foundRainFlag = true;
                        removeAbleIndices = 1;
                        yield return new(OpCodes.Nop);
                        continue;
                    }
                    if (instruction.operand == (object)currentSeasonField)
                    {
                        foundWinterFlag = true;
                        removeAbleIndices = 3;
                        yield return new(OpCodes.Nop);
                        continue;
                    }
                }

                yield return instruction;
            }

            if (!foundRainFlag)
                IMonitor.LogOnce($"Failed to find rain flag, rain behavior was not changed", LogLevel.Error);
            if (!foundWinterFlag)
                IMonitor.LogOnce($"Failed to find winter flag, winter behavior was not changed", LogLevel.Error);
        }

        //I tried to transpile this aswell, but I need access to some instance variables 
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
                        if (locationFromName.isCollidingPosition(new Rectangle((currentBuilding.tileX.Value + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y) * 64 + 2, (__instance.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, __instance, false, false, false) || locationFromName.isCollidingPosition(new Rectangle((currentBuilding.tileX.Value + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y + 1) * 64 + 2, (__instance.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, __instance, false, false, false))
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
                        if (!__instance.modData.ContainsKey(ModDataKey))
                        {
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
                            __instance.happiness.Value = (byte)Math.Max(0, __instance.happiness.Value - happiness);
                            __instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer.Value - friendship);
                            __instance.modData.Add(ModDataKey, "");
                        }
                        __instance.faceDirection(2);
                        __instance.SetMovingDown(true);
                        __instance.Position = new Vector2(currentBuilding.getRectForAnimalDoor().X, (currentBuilding.tileY.Value + currentBuilding.animalDoor.Y) * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 32);
                        if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
                        {
                            ++FarmAnimal.NumPathfindingThisTick;
                            __instance.controller = new PathFindController(__instance, locationFromName, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), Game1.random.Next(4), false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
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
                            if (!__instance.isCoopDweller())
                                __instance.position.X -= 32f;
                        }
                        __instance.noWarpTimer = 3000;
                        --currentBuilding.currentOccupants.Value;
                        if (Utility.isOnScreen(__instance.getTileLocationPoint(), 192, locationFromName))
                            locationFromName.localSound("sandyStep");
                        environment.isTileOccupiedByFarmer(__instance.getTileLocation())?.TemporaryPassableTiles.Add(__instance.GetBoundingBox());
                    }
                    __instance.UpdateRandomMovements();
                    IHelper.Reflection.GetMethod(__instance, "behaviors").Invoke(time, environment);
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

        private static bool DayUpdatePrefix(FarmAnimal __instance, GameLocation environtment)
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

        private static bool canGoOutside(FarmAnimal animal, Building currentBuilding, GameLocation environment) => currentBuilding != null && Game1.random.NextDouble() < 0.002 && currentBuilding.animalDoorOpen.Value && Game1.timeOfDay < 1630 && ((!Game1.isRaining && !Game1.currentSeason.Equals("winter")) || animal.type.Value.Contains("Pig")) && !Game1.isLightning && !environment.farmers.Any();
    }
}
