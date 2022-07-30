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
using StardewValley.Locations;

namespace StopRugRemoval.HarmonyPatches.Niceties;
#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.

/// <summary>
/// Please, for the love of god, stop trying to feed your family stuff that isn't edible, Pierre.
/// </summary>
[HarmonyPatch(typeof(SeedShop))]
internal static class FixStoreDialogue
{
    private static bool IsObjectVaguelyEdible(SObject? obj)
        => obj is not null && obj.Edibility > 0;

    [HarmonyPatch(nameof(SeedShop.getPurchasedItemDialogueForNPC))]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Used for matching only.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, typeof(SObject).GetCachedField(nameof(SObject.quality), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call), // this is an op_implicit
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Beq_S),
            })
            .Advance(1)
            .ReplaceInstruction(OpCodes.Callvirt, typeof(SObject).GetCachedProperty(nameof(SObject.Quality), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod(), keepLabels: true)
            .Advance(1)
            .Remove(1)
            .Advance(1);

            Label? label = (Label)helper.CurrentInstruction.operand;

            helper.ReplaceInstruction(OpCodes.Bge_S, label, keepLabels: true);

            // Now find the part where Pierre talks.
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldstr, "Pierre"),
                new(OpCodes.Call),
                new(OpCodes.Brtrue),
                new(OpCodes.Br),
            })
            .Advance(3)
            .StoreBranchDest()
            .Advance(1);

            Label ret = (Label)helper.CurrentInstruction.operand;

            helper.AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, typeof(FixStoreDialogue).GetCachedMethod(nameof(IsObjectVaguelyEdible), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brfalse, ret),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
