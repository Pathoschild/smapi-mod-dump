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

using GrowableBushes.Framework;

using HarmonyLib;

using StardewValley.TerrainFeatures;

namespace GrowableBushes.HarmonyPatches;

/// <summary>
/// Transpiler on Bush.performToolAction to allow decorative walnut bushes to be axed.
/// </summary>
[HarmonyPatch(typeof(Bush))]
internal static class BushToolActionTranspiler
{
    private static bool IsPlacedBush(Bush bush)
        => (ModEntry.Config.CanAxeAllBushes && bush.tileSheetOffset.Value == 0)
            || (bush.modData?.ContainsKey(InventoryBush.BushModData) == true);

    [HarmonyPatch(nameof(Bush.performToolAction))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Bush).GetCachedField(nameof(Bush.size), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // op_implicit
                OpCodes.Ldc_I4_4,
                OpCodes.Bne_Un_S,
            })
            .Push()
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushToolActionTranspiler).GetCachedMethod(nameof(IsPlacedBush), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPoint),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
