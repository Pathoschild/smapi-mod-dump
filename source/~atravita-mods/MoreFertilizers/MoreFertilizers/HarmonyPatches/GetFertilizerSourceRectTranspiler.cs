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
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Transpiler that inserts a call to let me adjust the rectangle drawn per fertilizer.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class GetFertilizerSourceRectTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static int GetIndexForFertilizer(int fertilizer)
    {
        if (fertilizer == -1)
        {
            return 0;
        }
        else if (fertilizer == ModEntry.PaddyCropFertilizerID)
        {
            return 3;
        }
        else if (fertilizer == ModEntry.LuckyFertilizerID)
        {
            return 5;
        }
        else if (fertilizer == ModEntry.BountifulFertilizerID)
        {
            return 7;
        }
        else if (fertilizer == ModEntry.JojaFertilizerID || fertilizer == ModEntry.DeluxeJojaFertilizerID)
        {
            return 4;
        }
        else if (fertilizer == ModEntry.OrganicFertilizerID)
        {
            return 2;
        }
        return 0;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(HoeDirt.GetFertilizerSourceRect))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            Label branchpast = helper.Generator.DefineLabel();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4, 173),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldc_I4_3),
                new(OpCodes.Div),
            }).Advance(1);

            CodeInstruction ldloc = helper.CurrentInstruction.Clone();
            CodeInstruction stloc = helper.CurrentInstruction.ToStLoc();

            helper.Advance(-1)
            .GetLabels(out IList<Label>? labels, clear: true)
            .AttachLabel(branchpast)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Brtrue_S, branchpast), // The  previous code has already returned a value, I don't need to.
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, typeof(GetFertilizerSourceRectTranspiler).StaticMethodNamed(nameof(GetIndexForFertilizer))),
                stloc,
            }, withLabels: labels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
