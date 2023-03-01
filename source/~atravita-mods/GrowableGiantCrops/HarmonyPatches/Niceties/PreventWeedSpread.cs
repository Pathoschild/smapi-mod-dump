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

using Microsoft.Xna.Framework;

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

/// <summary>
/// Prevents weeds from spreading from placed weeds.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class PreventWeedSpread
{
    private static bool ShouldSkipThisWeed(KeyValuePair<Vector2, SObject> weedCandidate)
        => weedCandidate.Value.modData?.GetBool(PatchesForSObject.ModDataMiscObject) == true;

    [HarmonyPatch(nameof(GameLocation.spawnWeedsAndStones))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find num =/2
                SpecialCodeInstructionCases.LdLoc,
                OpCodes.Ldc_I4_2,
                OpCodes.Div,
                SpecialCodeInstructionCases.StLoc,
            })
            .FindNext(new CodeInstructionWrapper[]
            { // find the start of the loop, this is our jump point.
                OpCodes.Ldc_I4_0,
                SpecialCodeInstructionCases.StLoc,
                OpCodes.Br,
            })
            .Advance(2)
            .Push()
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPast)
            .Pop()
            .FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(GameLocation).GetCachedField(nameof(GameLocation.objects), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Callvirt,
                (OpCodes.Callvirt, typeof(Random).GetCachedMethod<int>(nameof(Random.Next), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call,
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(5);

            CodeInstruction ldloc = helper.CurrentInstruction.ToLdLoc();
            helper.Advance(1)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(PreventWeedSpread).GetCachedMethod(nameof(ShouldSkipThisWeed), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPast),
            });

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
