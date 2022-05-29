/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Removes the check for isinst farm for giant crops.
/// </summary>
internal static class RemoveFarmCheck
{
    /// <summary>
    /// Applies the patch that removes the giant crop on farm only check.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(Crop).InstanceMethodNamed(nameof(Crop.newDay)),
            transpiler: new HarmonyMethod(typeof(RemoveFarmCheck).StaticMethodNamed(nameof(Transpiler)), Priority.LowerThanNormal));
    }

    private static bool IsFarmOrSetOtherwise(GameLocation? location)
        => location is Farm || ModEntry.Config.AllowGiantCropsOffFarm;

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).InstanceFieldNamed(nameof(Crop.indexOfHarvest))),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4, 276),
                new(OpCodes.Beq_S),
            })
            .FindPrev(new CodeInstructionWrapper[]
            { // Replace the check for is Farm with our own check.
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Isinst, typeof(Farm)),
                new(OpCodes.Brfalse),
            })
            .Advance(1)
            .ReplaceInstruction(OpCodes.Call, typeof(RemoveFarmCheck).StaticMethodNamed(nameof(IsFarmOrSetOtherwise)))
            .FindNext(new CodeInstructionWrapper[]
            { // Remove the cast to Farm.
                new(OpCodes.Isinst, typeof(Farm)),
            })
            .Remove(1);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}