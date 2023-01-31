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

namespace SpecialOrdersExtended.Niceties;

/// <summary>
/// Holds patches against DeliverObjective to allow partial deliveries.
/// </summary>
[HarmonyPatch(typeof(DeliverObjective))]
internal static class PartialDelieverObjectives
{
    [HarmonyPatch(nameof(DeliverObjective.ShouldShowProgress))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void Postfix(ref bool __result)
        => __result = true;

    private static void PlayChime()
        => Game1.currentLocation.playSound("discoverMineral");

    [HarmonyPatch(nameof(DeliverObjective.OnItemDelivered))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // this.GetMaxCount() - this.GetCount(); Copy these codes for later.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(OrderObjective).GetCachedMethod(nameof(OrderObjective.GetMaxCount), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(OrderObjective).GetCachedMethod(nameof(OrderObjective.GetCount), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Sub),
            })
            .Copy(5, out IEnumerable<CodeInstruction>? copy)
            .FindNext(new CodeInstructionWrapper[]
            { // Math.Min(item.Stack, this.GetMaxCount() - this.GetCount()
                new(OpCodes.Call, typeof(Math).GetCachedMethod<int, int>(nameof(Math.Min), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // if (required_count > stack) return 0;
                new(SpecialCodeInstructionCases.LdLoc),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Bge_S),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
            })
            .GetLabels(out IList<Label>? labels, clear: true)
            .Remove(5)
            .AttachLabels(labels)
            .FindNext(new CodeInstructionWrapper[]
            { // if (!string.IsNullOrEmpty(this.message.Value) . We need to prevent the "completed!" message if it isn't complete yet.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(DeliverObjective).GetCachedField(nameof(DeliverObjective.message), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Callvirt),
                new(OpCodes.Call, typeof(string).GetCachedMethod(nameof(string.IsNullOrEmpty), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S),
            })
            .Push()
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? secondLabels, clear: true)
            .DefineAndAttachLabel(out Label complete)
            .Insert(copy.ToArray(), withLabels: secondLabels) // can use the copy here, the counts have been updated by this point.
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Brfalse, complete),
                new(OpCodes.Call, typeof(PartialDelieverObjectives).GetCachedMethod(nameof(PlayChime), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Br, jumppoint),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}