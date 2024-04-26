/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class Patches()
    {
        private static Harmony s_harmony;
        private static IMonitor s_monitor;
        private static Func<ModConfig> s_config;
        private static IReflectionHelper s_reflectionHelper;

        internal static void Initialise(Harmony harmony, IMonitor monitor, Func<ModConfig> config, IReflectionHelper reflectionHelper)
        {
            s_harmony = harmony;
            s_monitor = monitor;
            s_config = config;
            s_reflectionHelper = reflectionHelper;

            ApplyPatches();
        }

        private static void ApplyPatches()
        {
            s_harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.tickUpdate)),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(FishingRodTickUpdatePatch))
            );
            s_harmony.Patch(
                original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.update)),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(BobberBar_Update_Transpiler))
            );
            s_harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(FishingRod_DoDoneFishing_Transpiler))
            );
            s_harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "GetFishFromLocationData", [typeof(string), typeof(Vector2), typeof(int), typeof(Farmer), typeof(bool), typeof(bool), typeof(GameLocation), typeof(ItemQueryContext)]),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(GameLocation_GetFishFromLocationData_Transpiler))
            );
            s_harmony.Patch(AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(PullFishFromWaterPatch))
            );
        }

        private static IEnumerable<CodeInstruction> FishingRodTickUpdatePatch(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = [];

            Label label = generator.DefineLabel();

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "coin")
                {
                    output.Insert(output.Count - 17, new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => DoAutoLootFish())));
                    output.Insert(output.Count - 17, new CodeInstruction(OpCodes.Brtrue, label));
                    output[^2].labels.Add(label);
                }
                output.Add(instruction);
            }

            return output;
        }

        private static IEnumerable<CodeInstruction> BobberBar_Update_Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new(instructions, generator);

            Label skipVibrationsLabel = generator.DefineLabel();

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldfld, typeof(BobberBar).GetField(nameof(BobberBar.distanceFromCatching))),
                new(OpCodes.Ldc_R4, 0.002f),
                new(OpCodes.Add),
                new(OpCodes.Stfld, typeof(BobberBar).GetField(nameof(BobberBar.distanceFromCatching)))
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(BobberBar_Update_Transpiler)}. Match for \"distanceFromCatching\" was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => FishInBarMultiplier()))
                );

            codeMatcher.Start();

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldfld, typeof(BobberBar).GetField(nameof(BobberBar.treasureCatchLevel))),
                new(OpCodes.Ldc_R4, 0.0135f),
                new(OpCodes.Add),
                new(OpCodes.Stfld, typeof(BobberBar).GetField(nameof(BobberBar.treasureCatchLevel)))
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(BobberBar_Update_Transpiler)}. Match for \"treasureCatchLevel\" was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => TreasureInBarMultiplier()))
                );

            codeMatcher.Start();

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldc_R4),
                new(OpCodes.Ldc_R4),
                new(OpCodes.Call, AccessTools.Method(typeof(Rumble), nameof(Rumble.rumble), [typeof(float), typeof(float)]))
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(BobberBar_Update_Transpiler)}. Match for \"Rumble.rumble\" was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher.InsertAndAdvance(
                new(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => DoSkipVibration())),
                new(OpCodes.Brtrue, skipVibrationsLabel)
            );

            codeMatcher.Advance(3);

            codeMatcher.AddLabels(new[] { skipVibrationsLabel });

            return codeMatcher.InstructionEnumeration();
        }

        private static IEnumerable<CodeInstruction> FishingRod_DoDoneFishing_Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new(instructions, generator);

            Label infiniteBaitLabel = generator.DefineLabel();
            Label infiniteTackleLabel = generator.DefineLabel();

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldloc_2),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, typeof(Item).GetProperty(nameof(Item.Stack)).GetMethod),
                new(OpCodes.Stloc_S),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Sub)
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(FishingRod_DoDoneFishing_Transpiler)}. Match for bait entry point was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher.Insert(
                new(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => HasInfiniteBait())),
                new(OpCodes.Brtrue, infiniteBaitLabel)
            );

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Stloc_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(FishingRod).GetMethod(nameof(FishingRod.GetTackle))),
                new(OpCodes.Callvirt, typeof(List<SObject>).GetMethod(nameof(List<SObject>.GetEnumerator))),
                new(OpCodes.Stloc_S)
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(FishingRod_DoDoneFishing_Transpiler)}. Match for tackle entry point was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher.Insert(
                new(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => HasInfiniteTackle())),
                new(OpCodes.Brtrue, infiniteTackleLabel)
            );

            codeMatcher.AddLabels(new[] { infiniteBaitLabel });

            codeMatcher.MatchStartForward(
                new(OpCodes.Ldloc_0),
                new(OpCodes.Brfalse_S),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, typeof(Farmer).GetProperty(nameof(Farmer.IsLocalPlayer)).GetMethod),
                new(OpCodes.Brfalse_S)
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(FishingRod_DoDoneFishing_Transpiler)}. Match for tackle jump point was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher.AddLabels(new[] { infiniteTackleLabel });

            return codeMatcher.InstructionEnumeration();
        }

        private static IEnumerable<CodeInstruction> GameLocation_GetFishFromLocationData_Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new(instructions, generator);

            codeMatcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_S),
                new(OpCodes.Callvirt, typeof(IEnumerable<SpawnFishData>).GetMethod(nameof(IEnumerable<SpawnFishData>.GetEnumerator))),
                new(OpCodes.Stloc_S)
            );

            if (!codeMatcher.IsValid)
            {
                s_monitor.Log($"Failed to patch {nameof(GameLocation_GetFishFromLocationData_Transpiler)}. Match for possibleFish entry point was invalid.", LogLevel.Error);
                return null;
            }

            codeMatcher.Advance(-1);

            codeMatcher.RemoveInstruction();

            codeMatcher.Insert(
                new(OpCodes.Call, typeof(Patches).GetMethod(nameof(FilterPossibleFish))),
                new(OpCodes.Callvirt, typeof(IEnumerable<SpawnFishData>).GetMethod(nameof(IEnumerable<SpawnFishData>.GetEnumerator)))
            );

            return codeMatcher.InstructionEnumeration();
        }

        public static IEnumerable<SpawnFishData> FilterPossibleFish(IEnumerable<SpawnFishData> possibleFish)
        {
            List<SpawnFishData> filteredPossibleFish = [];

            foreach (SpawnFishData fish in possibleFish)
            {
                if (IsFishInPreferredCategory(ItemRegistry.GetData(fish.ItemId)))
                    filteredPossibleFish.Add(fish);
            }

            return filteredPossibleFish.AsEnumerable();
        }

        private static bool IsFishInPreferredCategory(ParsedItemData fish)
        {
            ModConfig config = s_config();

            if (fish == null && config.AllowCatchingRubbish)
                return true;
            else if (!config.AllowCatchingFish && !config.AllowCatchingRubbish && !config.AllowCatchingOther)
                return true;
            else if (fish == null)
                return false;

            if ((fish.Category == SObject.FishCategory && config.AllowCatchingFish) ||
                ((fish.Category == SObject.junkCategory) && config.AllowCatchingRubbish) ||
                (fish.Category == 0 && config.AllowCatchingOther))
                return true;

            return false;
        }

        private static bool DoSkipVibration()
        {
            return s_config().DisableVibrations;
        }

        private static bool HasInfiniteTackle()
        {
            return s_config().InfiniteTackle;
        }

        private static bool HasInfiniteBait()
        {
            return s_config().InfiniteBait;
        }

        private static float TreasureInBarMultiplier()
        {
            return 0.0135f * s_config().TreasureInBarMultiplier;
        }

        private static float FishInBarMultiplier()
        {
            return 0.002f * s_config().FishInBarMultiplier;
        }

        private static bool DoAutoLootFish()
        {
            return s_config().AutoLootFish;
        }

        private static bool PullFishFromWaterPatch(ref int fishDifficulty)
        {
            try
            {
                ModConfig config = s_config();

                if (!config.AdjustXpGainDifficulty && config.DifficultyMultiplier > 0)
                    fishDifficulty = (int)(fishDifficulty / config.DifficultyMultiplier);

                return true;
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(PullFishFromWaterPatch)}:\n{e}", LogLevel.Error);
                return true;
            }
        }
    }
}
