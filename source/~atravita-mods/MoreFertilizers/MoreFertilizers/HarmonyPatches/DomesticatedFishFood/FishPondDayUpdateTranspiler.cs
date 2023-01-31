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
using Netcode;
using StardewValley.Buildings;

namespace MoreFertilizers.HarmonyPatches.DomesticatedFishFood;

/// <summary>
/// Holds transpilers against FishPond.dayUpdate.
/// </summary>
[HarmonyPatch(typeof(FishPond))]
internal static class FishPondDayUpdateTranspiler
{
    private static int GetAdditionalGrowthFactor(Random r, FishPond pond)
    {
        try
        {
            if (pond.modData?.GetBool(CanPlaceHandler.DomesticatedFishFood) == true
                && r.NextDouble() < 0.15)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Speeding up fish growth at pond at {pond.tileX}, {pond.tileY}", LogLevel.Info);
                return 2;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error in speeding up fish growth in fish ponds!\n\n{ex}", LogLevel.Error);
        }
        return 1;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(FishPond.dayUpdate))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(Random).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int) })),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1);

            CodeInstruction? local = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(FishPond).GetCachedField(nameof(FishPond.daysSinceSpawn), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_1),
            })
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(FishPondDayUpdateTranspiler).GetCachedMethod(nameof(GetAdditionalGrowthFactor), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[]
            {
                local,
                new(OpCodes.Ldarg_0),
            }, withLabels: labelsToMove);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
