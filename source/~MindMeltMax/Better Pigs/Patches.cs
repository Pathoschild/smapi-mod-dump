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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WinterPigs
{
    internal static class Patches
    {
        private static IMonitor IMonitor;
        private static IModHelper IHelper;
        private static string ModDataHappinessKey => $"{IHelper.ModRegistry.ModID}.Happiness";

        public static void Patch(IMonitor monitor, IModHelper helper)
        {
            IMonitor = monitor;
            IHelper = helper;

            Harmony harmony = new(helper.ModRegistry.ModID);


            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
                transpiler: new(typeof(Patches), nameof(FarmAnimal_Behaviors_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                transpiler: new(typeof(Patches), nameof(FarmAnimal_Pet_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                transpiler: new(typeof(Patches), nameof(FarmAnimal_UpdateWhenNotCurrentLocation_Transpiler)) //I counted, it's 50 characters
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.DigUpProduce)),
                transpiler: new(typeof(Patches), nameof(FarmAnimal_DigUpProduce_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                postfix: new(typeof(Patches), nameof(FarmAnimal_DayUpdate_Postfix))
            );
        }

        internal static IEnumerable<CodeInstruction> FarmAnimal_Behaviors_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);

            try
            {
                matcher.Start().MatchStartForward([
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Callvirt, AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere))),
                    new(OpCodes.Brtrue)
                ]).RemoveInstructions(4);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to find rain flag, rain behavior was not changed", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_Behaviors_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            try
            {
                matcher.Start().MatchStartForward([
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Callvirt, AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsWinterHere))),
                    new(OpCodes.Brtrue),
                ]).RemoveInstructions(4);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to find winter flag, winter behavior was not changed", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_Behaviors_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            try
            {
                matcher.Start().MatchEndForward([
                    new(OpCodes.Ldsfld),
                    new(OpCodes.Callvirt, AccessTools.Method(typeof(Random), nameof(Random.NextDouble))),
                    new(OpCodes.Ldc_R8),
                    new(OpCodes.Bge_Un),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldarg_0),
                ]).Advance(-2).InsertAndAdvance([
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(getAnimalCrackerMultiplier))),
                    new(OpCodes.Mul)
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to find pig dig up random, dig up behavior was not changed", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_Behaviors_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            return matcher.Instructions();
        }

        internal static IEnumerable<CodeInstruction> FarmAnimal_Pet_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            try
            {
                matcher.Start().MatchStartForward([
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Ldstr, "give_gift")
                ]).CreateLabel(out var l1).MatchStartBackwards([
                    // if (eatGoldenCrackers.HasValue && !eatGoldenCrackers.GetValueOrDefault())
                    // ...
                    new(OpCodes.Ldloca_S),
                    new(OpCodes.Call),
                    new(OpCodes.Brfalse_S),
                    new(OpCodes.Ldloca_S),
                    new(OpCodes.Call),
                    new(OpCodes.Brtrue_S),
                    new(OpCodes.Ldstr, "cancel")
                ]).InsertAndAdvance([
                    // Change above to:
                    // if (this.type.Value != "Pig" && eatGoldenCrackers.HasValue && !eatGoldenCrackers.GetValueOrDefault())
                    // ...
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.type))),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetString), nameof(NetString.Value))),
                    new(OpCodes.Ldstr, "Pig"),
                    new(OpCodes.Call, AccessTools.Method(typeof(string), "op_Equality")),
                    new(OpCodes.Brtrue_S, l1),
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to find golden cracker gift instruction, golden cracker behavior was not changed", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_Pet_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                CodeInstruction startInsert = new(OpCodes.Ldarg_0);
                matcher.Start().MatchStartForward([
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, AccessTools.Method(typeof(Character), nameof(Character.Halt)))
                ]).Instruction.MoveLabelsTo(startInsert);
                matcher.CreateLabel(out var l2).InsertAndAdvance([
                    startInsert,
                    new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(checkSkipPet))),
                    new(OpCodes.Brfalse_S, l2),
                    new(OpCodes.Ret)
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to insert skip pet check", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_Pet_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            return matcher.Instructions();
        }

        internal static IEnumerable<CodeInstruction> FarmAnimal_UpdateWhenNotCurrentLocation_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            try
            {
                CodeInstruction startInsert = new(OpCodes.Ldarg_0);

                matcher.End().MatchStartBackwards([
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Pop)
                ]).CreateLabel(out var l3).MatchStartBackwards([
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Callvirt, AccessTools.Method(typeof(Building), nameof(Building.GetParentLocation))),
                    new(OpCodes.Stloc_0),
                ]).CreateLabel(out var l4).Start().MatchStartForward([
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Brfalse),
                    new(OpCodes.Ldsfld),
                    new(OpCodes.Ldc_R8),
                    new(OpCodes.Call)
                ]).Instruction.MoveLabelsTo(startInsert);
                matcher.InsertAndAdvance([
                    startInsert,
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(canGoOutside))),
                    new(OpCodes.Brfalse, l3),
                    new(OpCodes.Br, l4),
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to find 'Can go outside' check, Whether or not an animal can go outside was not changed", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_UpdateWhenNotCurrentLocation_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                matcher.Start().MatchStartForward([
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Callvirt),
                ]).InsertAndAdvance([
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(updateHappiness)))
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to insert happiness update", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_UpdateWhenNotCurrentLocation_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            return matcher.Instructions();
        }

        internal static IEnumerable<CodeInstruction> FarmAnimal_DigUpProduce_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            try
            {
                CodeInstruction startInsert = new(OpCodes.Ldsfld, AccessTools.Field(typeof(ModEntry), nameof(ModEntry.IConfig)));

                matcher.End().CreateLabel(out var l1).Start().MatchStartForward([
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.friendshipTowardFarmer))),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Conv_R8),
                    new(OpCodes.Ldc_R8),
                ]).Instruction.MoveLabelsTo(startInsert);
                matcher.InsertAndAdvance([
                    startInsert,
                    new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Config), nameof(Config.NoTruffleLimit))),
                    new(OpCodes.Brtrue_S, l1)
                ]);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed to insert truffle limit patch", LogLevel.Error);
                IMonitor.Log($"[{nameof(FarmAnimal_DigUpProduce_Transpiler)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }

            return matcher.Instructions();
        }

        internal static void FarmAnimal_DayUpdate_Postfix(FarmAnimal __instance) => __instance.modData.Remove(ModDataHappinessKey);

        private static double getAnimalCrackerMultiplier(FarmAnimal animal)
        {
            if (!animal.hasEatenAnimalCracker.Value)
                return 1.0;
            return ModEntry.IConfig.PigAnimalCrackerMultiplier;
        }

        private static bool canGoOutside(FarmAnimal animal, Building currentBuilding, GameLocation environment)
        {
            if (ModEntry.IConfig.AnimalsStayInside.Contains(animal.type.Value) || currentBuilding is null || !Game1.random.NextBool(0.002) || !currentBuilding.animalDoorOpen.Value || Game1.timeOfDay >= 1630 || environment.farmers.Any())
                return false;
            bool isRaining = environment.IsRainingHere();
            bool isWinter = environment.IsWinterHere();
            bool isLightning = environment.IsLightningHere();
            bool isGreenRain = environment.IsGreenRainingHere();
            bool isSnowing = environment.IsSnowingHere();
            if (ModEntry.IConfig.AnimalsGoOutsideHorribleWeather.Contains(animal.type.Value))
                return true;
            if (ModEntry.IConfig.AnimalsGoOutsideBadWeather.Contains(animal.type.Value) && !isLightning && !isGreenRain && !isSnowing)
                return true;
            if (!isRaining && !isWinter)
                return true;
            return false;
        }

        private static bool checkSkipPet(FarmAnimal animal)
        {
            if (!ModEntry.IConfig.DelayedPet || animal.Sprite.CurrentAnimation is null)
                return false;
            return true;
        }

        private static void updateHappiness(FarmAnimal animal, GameLocation environment)
        {
            if (animal.modData.ContainsKey(ModDataHappinessKey))
                return;
            byte happiness = getHappinessDecreaseFromWeather(environment);
            int friendship = getFriendshipDecreaseFromWeather(environment);
            /*if (Game1.currentSeason.Equals("winter"))
            {
                happiness = (byte)(Game1.isSnowing ? 75 : 50);
                friendship = Game1.isSnowing ? 15 : 10;
            }
            else if (Game1.isRaining)
            {
                happiness = 25;
                friendship = 5;
            }*/
            animal.happiness.Value = (byte)Math.Max(0, animal.happiness.Value - happiness);
            animal.friendshipTowardFarmer.Value = Math.Max(0, animal.friendshipTowardFarmer.Value - friendship);
            animal.modData.Add(ModDataHappinessKey, "1");
        }

        private static byte getHappinessDecreaseFromWeather(GameLocation environment)
        {
            bool isRaining = environment.IsRainingHere();
            bool isWinter = environment.IsWinterHere();
            bool isLightning = environment.IsLightningHere();
            bool isGreenRain = environment.IsGreenRainingHere();
            bool isSnowing = environment.IsSnowingHere();

            if (isLightning)
                return 100;
            if (isSnowing || isGreenRain)
                return 75;
            return (byte)(isWinter ? 50 : (isRaining ? 25 : 0));
        }

        private static int getFriendshipDecreaseFromWeather(GameLocation environment)
        {
            bool isRaining = environment.IsRainingHere();
            bool isWinter = environment.IsWinterHere();
            bool isLightning = environment.IsLightningHere();
            bool isGreenRain = environment.IsGreenRainingHere();
            bool isSnowing = environment.IsSnowingHere();

            if (isLightning)
                return 25;
            if (isSnowing || isGreenRain)
                return 15;
            return isWinter ? 10 : (isRaining ? 5 : 0);
        }
    }
}
