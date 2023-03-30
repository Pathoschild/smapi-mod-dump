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

using GrowableGiantCrops.Framework;

using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.ToolPatches;

/// <summary>
/// Patch to prevent shovel from doing damage if that's been disabled.
/// </summary>
[HarmonyPatch(typeof(Tool))]
internal static class ShouldDamageMonsterTranspiler
{
    private static bool ShouldSkipDamagingMonster(Tool tool)
        => tool is ShovelTool && !ModEntry.Config.ShovelDoesDamage;

    [HarmonyPatch(nameof(Tool.DoFunction))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Call, typeof(Tool).GetCachedMethod(nameof(Tool.isHeavyHitter), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse_S,
            })
            .Push()
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(ShouldDamageMonsterTranspiler).GetCachedMethod(nameof(ShouldSkipDamagingMonster), ReflectionCache.FlagTypes.StaticFlags)),
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
