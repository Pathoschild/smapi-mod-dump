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

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

/// <summary>
/// A patch to prevent placed weeds from dying in winter.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal static class PreservePlacedWeedsTranspiler
{
    private static bool ShouldPreserveThisWeed(SObject weedCandidate)
    => ModEntry.Config.PreservePlacedWeeds && weedCandidate.modData?.GetBool(PatchesForSObject.ModDataMiscObject) == true;

    [HarmonyPatch(nameof(Game1.setGraphicsForSeason))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // season.Equals("winter")
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Ldstr, "winter"),
                (OpCodes.Callvirt, typeof(string).GetCachedMethod<string>("Equals", ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse,
            })
            .FindNext(new CodeInstructionWrapper[]
            { // o.Name.Contains("Weed");
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Callvirt, typeof(Item).GetCachedProperty(nameof(Item.Name), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Ldstr, "Weed"),
                (OpCodes.Callvirt, typeof(string).GetCachedMethod<string>("Contains", ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse_S,
            })
            .Push()
            .GetLabels(out IList<Label>? labelsToMove);

            CodeInstruction ldloc = helper.CurrentInstruction.Clone();
            helper.Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPast)
            .Pop()
            .Insert(new CodeInstruction[]
            { // also jump past if it's a weed we should preserve.
                ldloc,
                new (OpCodes.Call, typeof(PreservePlacedWeedsTranspiler).GetCachedMethod(nameof(ShouldPreserveThisWeed), ReflectionCache.FlagTypes.StaticFlags)),
                new (OpCodes.Brtrue, jumpPast),
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
