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
using GrowableGiantCrops.Framework.InventoryModels;
using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

/// <summary>
/// A transpiler to judge giant crops.
/// </summary>
[HarmonyPatch(typeof(Event))]
internal static class GrangeJudgeTranspiler
{
    private static int JudgeGiantCrop(Item item, Dictionary<int, bool> categories)
    {
        if (item is InventoryGiantCrop)
        {
            categories[InventoryGiantCrop.GiantCropCategory] = true;
            return 8;
        }
        return 0;
    }

    [HarmonyPatch("judgeGrange")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper
            .FindNext(new CodeInstructionWrapper[]
            { // points = 14;
                (OpCodes.Ldc_I4_S, 14),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction stPoints = helper.CurrentInstruction.Clone();
            CodeInstruction ldPoints = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            { // new Dictionary<int, bool>
                (OpCodes.Newobj, typeof(Dictionary<int, bool>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction ldCategories = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            { // item is SObject
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Isinst, typeof(SObject)),
                OpCodes.Brfalse,
            });

            CodeInstruction ldObject = helper.CurrentInstruction.Clone();

            helper.Advance(2)
            .Push()
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label success)
            .Pop()
            .Advance(1)
            .GetLabels(out IList<Label>? labelsToMove);

            Label failure = helper.Generator.DefineLabel();

            // insert a block that judges our giant crop
            // basically:
            //
            // if (item is InventoryGiantCrop)
            // {
            //    categories[InventoryGiantCrop.GiantCropCategory] == true;
            //    points += 5;
            //    continue;
            // }
            helper.Insert(new CodeInstruction[]
            {
                ldObject,
                ldCategories,
                new (OpCodes.Call, typeof(GrangeJudgeTranspiler).GetCachedMethod(nameof(JudgeGiantCrop), ReflectionCache.FlagTypes.StaticFlags)),
                new (OpCodes.Dup),
                new (OpCodes.Brfalse_S, failure),
                ldPoints,
                new (OpCodes.Add),
                stPoints,
                new (OpCodes.Br, success),
                new CodeInstruction(OpCodes.Pop).WithLabels(failure),
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
