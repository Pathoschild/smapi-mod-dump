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
using StardewValley.Tools;

namespace MoreFertilizers.HarmonyPatches.FishFood;

/// <summary>
/// Reduces the time needed to bite for the fish food.
/// </summary>
[HarmonyPatch(typeof(FishingRod))]
internal static class FishTimePatch
{
    private static float AdjustTimeToBite(float prevTime)
    {
        if (Game1.currentLocation?.modData?.GetBool(CanPlaceHandler.FishFood) == true)
        {
            return prevTime * 0.9f;
        }
        return prevTime;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch("calculateTimeUntilFishingBite")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.Next), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int), typeof(int) } )),
                new(OpCodes.Conv_R4),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(2);

            CodeInstruction? ldloc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction? stloc = helper.CurrentInstruction.Clone();

            helper.Advance(1)
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(FishTimePatch).GetCachedMethod(nameof(AdjustTimeToBite), ReflectionCache.FlagTypes.StaticFlags)),
                stloc,
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling time to bite for fish:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}