/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using MoreFertilizers.Framework;
using Netcode;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.FruitTreePatches;

/// <summary>
/// Transpiles DayUpdate to cause fruit trees to grow up faster.
/// </summary>
[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreeDayUpdateTranspiler
{
    private static int CalculateExtraGrowth(FruitTree tree)
    {
        try
        {
            if (tree.modData?.GetInt(CanPlaceHandler.FruitTreeFertilizer) is int result
                && Game1.random.NextDouble() <= 0.1 * result)
            {
                return 1;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.LogOnce($"Crash while calculating extra growth for fruit trees!\n\n{ex}", LogLevel.Error);
        }
        return 0;
    }

    [HarmonyPatch(nameof(FruitTree.dayUpdate))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new (SpecialCodeInstructionCases.LdArg),
                new (SpecialCodeInstructionCases.LdArg),
                new (OpCodes.Call, typeof(FruitTree).StaticMethodNamed(nameof(FruitTree.IsGrowthBlocked))),
                new (SpecialCodeInstructionCases.StLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(FruitTree).InstanceFieldNamed(nameof(FruitTree.daysUntilMature))),
                new (OpCodes.Dup),
                new (OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).InstancePropertyNamed("Value").GetGetMethod()),
                new (SpecialCodeInstructionCases.StLoc),
                new (SpecialCodeInstructionCases.LdLoc),
                new (OpCodes.Ldc_I4_1),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldc_I4_1),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Call, typeof(FruitTreeDayUpdateTranspiler).StaticMethodNamed(nameof(FruitTreeDayUpdateTranspiler.CalculateExtraGrowth))),
                new (OpCodes.Add),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling FruitTree.DayUpdate:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}