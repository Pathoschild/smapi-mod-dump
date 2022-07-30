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
using StardewValley.Menus;

namespace PrismaticClothing.HarmonyPatches;

/// <summary>
/// Holds a transpiler against TailoringMenu.CraftItem.
/// </summary>
[HarmonyPatch(typeof(TailoringMenu))]
internal static class TranspileCraftItem
{
#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed;
    [HarmonyPatch(nameof(TailoringMenu.CraftItem))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find right_item.HasContextTag("color_prismatic");
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Ldstr, "color_prismatic"),
                new(OpCodes.Callvirt, typeof(Item).GetCachedMethod(nameof(Item.HasContextTag), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.Wildcard, (instr) => instr.opcode == OpCodes.Brfalse || instr.opcode == OpCodes.Brfalse_S),
            })
            .Push()
            .Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop();

            CodeInstruction? arg = helper.CurrentInstruction.Clone();

            helper.GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            { // and insert if (right_item is prismatic shard, skip past that previous block).
                arg,
                new(OpCodes.Ldc_I4, 74), // prismatic shard
                new(OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.IsNormalObjectAtParentSheetIndex), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, label),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
