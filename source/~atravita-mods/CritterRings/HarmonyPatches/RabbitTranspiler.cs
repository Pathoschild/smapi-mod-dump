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

using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;

namespace CritterRings.HarmonyPatches;

/// <summary>
/// A transpiler to prevent rabbits from harvesting walnuts.
/// </summary>
[HarmonyPatch(typeof(Rabbit))]
internal static class RabbitTranspiler
{
    private static bool IsWalnutInBloom(LargeTerrainFeature? terrainFeature)
        => terrainFeature is Bush bush && bush.size.Value == Bush.walnutBush && bush.tileSheetOffset.Value == 1;

    [HarmonyPatch("update")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Isinst, typeof(Bush)),
                OpCodes.Brfalse_S,
            });

            CodeInstruction loc = helper.CurrentInstruction.Clone();

            helper.Push()
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                loc,
                new(OpCodes.Call, typeof(RabbitTranspiler).GetCachedMethod(nameof(IsWalnutInBloom), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPoint),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.Name}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
