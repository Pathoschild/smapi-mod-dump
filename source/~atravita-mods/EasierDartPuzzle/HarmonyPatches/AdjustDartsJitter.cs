/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Extensions;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Minigames;

namespace EasierDartPuzzle.HarmonyPatches;

/// <summary>
/// Adjusts how much the darts shake.
/// </summary>
[HarmonyPatch(typeof(Darts))]
internal static class AdjustDartsJitter
{
    private static float GetDartsJitterAdjustment()
        => ModEntry.Config.JitterMultiplier;

    private static float GetDartsPrecision()
        => ModEntry.Config.DartPrecision;

    [HarmonyPatch(nameof(Darts.tick))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(OpCodes.Mul),
                    new(OpCodes.Call, typeof(Math).GetCachedMethod(nameof(Math.Sin), ReflectionCache.FlagTypes.StaticFlags)),
                },
                (helper) =>
                {
                    helper.Advance(1)
                    .Insert(new CodeInstruction[]
                    {
                        new(OpCodes.Call, typeof(AdjustDartsJitter).GetCachedMethod(nameof(GetDartsJitterAdjustment), ReflectionCache.FlagTypes.StaticFlags)),
                        new(OpCodes.Div),
                    });
                    return true;
                },
                maxCount: 2)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Darts).GetCachedMethod(nameof(Darts.GetRadiusFromCharge), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(AdjustDartsJitter).GetCachedMethod(nameof(GetDartsPrecision), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Div),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.GetFullName()}!\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}