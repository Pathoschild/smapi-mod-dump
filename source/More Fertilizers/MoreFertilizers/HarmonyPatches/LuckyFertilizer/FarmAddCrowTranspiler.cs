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
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.LuckyFertilizer;

/// <summary>
/// Transpiles the Farm.AddCrow function to remove certain fertilized crops from possibly being eaten by crows.
/// </summary>
[HarmonyPatch(typeof(Farm))]
internal static class FarmAddCrowTranspiler
{
    private static bool HasFertilizer(HoeDirt? dirt)
        => ModEntry.LuckyFertilizerID != -1 && dirt is not null && dirt.fertilizer.Value.Equals(ModEntry.LuckyFertilizerID);

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(Farm.addCrows))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldloca_S),
                new (OpCodes.Call, typeof(KeyValuePair<Vector2, TerrainFeature>).InstancePropertyNamed("Value").GetGetMethod()),
                new (OpCodes.Isinst, typeof(HoeDirt)),
                new (OpCodes.Brfalse_S),
            }).Push();

            CodeInstruction? local = helper.CurrentInstruction.Clone();

            helper.Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .Insert(new CodeInstruction[]
            {
                local,
                new (OpCodes.Call, typeof(KeyValuePair<Vector2, TerrainFeature>).InstancePropertyNamed("Value").GetGetMethod()),
                new (OpCodes.Isinst, typeof(HoeDirt)),
                new (OpCodes.Call, typeof(FarmAddCrowTranspiler).StaticMethodNamed(nameof(HasFertilizer))),
                new (OpCodes.Brtrue_S, label),
            }, withLabels: labelsToMove);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Farm.addCrow:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
