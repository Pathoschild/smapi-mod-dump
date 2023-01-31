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
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using MoreFertilizers.Framework;

namespace MoreFertilizers.HarmonyPatches.FishFood;

/// <summary>
/// Transpiles GetFish. Also has the relevant helper function.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class GetFishTranspiler
{
    /// <summary>
    /// Given a location and a chance, alters the chance to adjust for fish food.
    /// </summary>
    /// <param name="prevChance">Previous chance.</param>
    /// <param name="loc">Gamelocation.</param>
    /// <returns>chance.</returns>
    internal static double AlterFishChance(double prevChance, GameLocation loc)
    {
        try
        {
            if (prevChance < 0.3 && loc.modData?.GetBool(CanPlaceHandler.FishFood) == true)
            {
                double newChance = Math.Sqrt(Math.Clamp(prevChance, 0, 1));
                ModEntry.ModMonitor.DebugOnlyLog($"Adjusting fish chance at {loc.NameOrUniqueName}: {prevChance} => {newChance}", LogLevel.Debug);
                return newChance;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error in adjusting fish chances at {loc.NameOrUniqueName}:\n\n{ex}", LogLevel.Error);
        }
        return prevChance;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(GameLocation.getFish))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper
            .FindNext(new CodeInstructionWrapper[]
            { // find the loading of the fish data.
                new (OpCodes.Ldsfld),
                new (OpCodes.Ldstr, @"Data\Fish"),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Finding chance *= 1.1 (from the beginner's rod). We'll get our locals here.
                new (SpecialCodeInstructionCases.LdLoc, typeof(double)),
                new (OpCodes.Ldc_R8, 1.1),
                new (OpCodes.Mul),
                new (SpecialCodeInstructionCases.StLoc, typeof(double)),
            });

            CodeInstruction? ldloc = helper.CurrentInstruction.Clone();
            CodeInstruction? stloc = ldloc.ToStLoc();

            helper.Advance(2)
            .FindNext(new CodeInstructionWrapper[]
            { // we'll insert our adjustment just before the chance = Math.Min(chance, 0.9); statement.
                new (SpecialCodeInstructionCases.LdLoc, typeof(double)),
                new (OpCodes.Ldc_R8),
                new (OpCodes.Call, typeof(Math).GetCachedMethod(nameof(Math.Min), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(double), typeof(double) } )),
                new (SpecialCodeInstructionCases.StLoc, typeof(double)),
            })
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new (OpCodes.Ldarg_0),
                new (OpCodes.Call, typeof(GetFishTranspiler).GetCachedMethod(nameof(GetFishTranspiler.AlterFishChance), ReflectionCache.FlagTypes.StaticFlags)),
                stloc,
            }, withLabels: labelsToMove);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling GameLocation.GetFish:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}