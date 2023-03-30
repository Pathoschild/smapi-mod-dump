/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;
using AtraShared.Utils.Extensions;
using StardewValley.Tools;
using GrowableGiantCrops.Framework;

namespace GrowableGiantCrops.HarmonyPatches.ToolPatches;

[HarmonyPatch(typeof(GameLocation))]
internal static class ShovelUpgradablePatch
{
    [HarmonyPatch(nameof(GameLocation.blacksmith))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Isinst, typeof(GenericTool)),
                OpCodes.Brfalse_S,
            })
            .Push()
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop();

            CodeInstruction ldloc = helper.CurrentInstruction.Clone();
            helper.GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Isinst, typeof(ShovelTool)),
                new(OpCodes.Brtrue, jumpPoint),
            }, labelsToMove);

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
