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
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Patches for HoeDirt.applySpeedIncreases.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class HoeDirtSpeedIncreases
{
    /// <summary>
    /// Gets whether or not to consider this particular fertilizer for increased speed.
    /// </summary>
    /// <param name="fertilizer">Int fertilizer ID.</param>
    /// <returns>True to consider, false to skip.</returns>
    private static bool ShouldSpeedUpForThisFertilizer(int fertilizer)
        => fertilizer != -1 && (fertilizer == ModEntry.PaddyCropFertilizerID || fertilizer == ModEntry.SecretJojaFertilizerID);

    private static float AdjustIncreaseForFertilizer(float prev, HoeDirt dirt)
    {
        if (dirt.fertilizer.Value == ModEntry.SecretJojaFertilizerID)
        {
            prev += 0.2f;
        }
        else if (dirt.fertilizer.Value == ModEntry.PaddyCropFertilizerID && dirt.hasPaddyCrop())
        {
            prev += 0.1f;
        }
        return prev;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch("applySpeedIncreases")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find the guard clause and exclude our fertilizers.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(HoeDirt).GetCachedField(nameof(HoeDirt.fertilizer), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call), // op_implicit
                new(OpCodes.Ldc_I4, 465),
                new(OpCodes.Beq_S),
            }).Copy(5, out IEnumerable<CodeInstruction> copy)
            .Advance(4)
            .Push()
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label considerFertLabel)
            .Pop()
            .Advance(1)
            .GetLabels(out IList<Label>? labelsToMove, clear: true);

            CodeInstruction[] copyarray = copy.ToArray();
            copyarray[3] = new(OpCodes.Call, typeof(HoeDirtSpeedIncreases).GetCachedMethod(nameof(ShouldSpeedUpForThisFertilizer), ReflectionCache.FlagTypes.StaticFlags));
            copyarray[4] = new(OpCodes.Brtrue_S, considerFertLabel);

            helper.Insert(copyarray, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Callvirt, typeof(Crop).GetCachedMethod(nameof(Crop.ResetPhaseDays), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_R4, 0.0f),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1);

            CodeInstruction? ldloc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction? stloc = helper.CurrentInstruction.Clone();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(HoeDirt).GetCachedField(nameof(HoeDirt.fertilizer), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call), // op implicit,
                new(OpCodes.Ldc_I4, 465),
            })
            .GetLabels(out IList<Label>? secondLabelsToMove, clear: true)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(HoeDirtSpeedIncreases).GetCachedMethod(nameof(AdjustIncreaseForFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
                stloc,
            }, withLabels: secondLabelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling HoeDirt.ApplySpeedIncreases:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}