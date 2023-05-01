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

using MoreFertilizers.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BushFertilizers;

/// <summary>
/// A transpiler to increase the chances a bountiful bush will produce.
/// </summary>
[HarmonyPatch(typeof(Bush))]
internal static class BountifulBushTranspiler
{
    private static bool ShouldSkipRandomCheck(Bush bush)
        => bush.modData?.ContainsKey(CanPlaceHandler.BountifulBush) == true;

    [HarmonyPatch(nameof(Bush.dayUpdate))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // this.size == 1
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Bush).GetCachedField(nameof(Bush.size), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // op_implicit
                OpCodes.Ldc_I4_1,
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.random), ReflectionCache.FlagTypes.StaticFlags)),
                (OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.NextDouble), ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.Wildcard,
                new (SpecialCodeInstructionCases.Wildcard, (instr) => instr.Branches(out _)),
            })
            .Push()
            .Advance(4)
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BountifulBushTranspiler).GetCachedMethod(nameof(ShouldSkipRandomCheck), ReflectionCache.FlagTypes.StaticFlags)),
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
