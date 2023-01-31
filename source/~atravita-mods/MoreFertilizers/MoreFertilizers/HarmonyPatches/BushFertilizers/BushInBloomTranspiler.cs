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
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using MoreFertilizers.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BushFertilizers;

/// <summary>
/// Holds the transpiler that adjusts Bush.inBloom.
/// </summary>
[HarmonyPatch(typeof(Bush))]
internal static class BushInBloomTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static int ReplaceSeasonForTeaBushes(int prevValue, Bush? bush)
        => bush?.modData?.GetBool(CanPlaceHandler.BountifulBush) == true ? 15 : prevValue;

    [MethodImpl(TKConstants.Hot)]
    private static int ReplaceStartSpring(int prevValue, Bush? bush)
        => bush?.modData?.GetBool(CanPlaceHandler.BountifulBush) == true ? 13 : prevValue;

    [MethodImpl(TKConstants.Hot)]
    private static int ReplaceEndSpring(int prevValue, Bush? bush)
        => bush?.modData?.GetBool(CanPlaceHandler.BountifulBush) == true ? 21 : prevValue;

    [MethodImpl(TKConstants.Hot)]
    private static int ReplaceStartFall(int prevValue, Bush? bush)
        => bush?.modData?.GetBool(CanPlaceHandler.BountifulBush) == true ? 6 : prevValue;

    [MethodImpl(TKConstants.Hot)]
    private static int ReplaceEndFall(int prevValue, Bush? bush)
        => bush?.modData?.GetBool(CanPlaceHandler.BountifulBush) == true ? 14 : prevValue;

    [HarmonyPatch(nameof(Bush.inBloom))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Bush).GetCachedMethod(nameof(Bush.getAge), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_S, 22),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushInBloomTranspiler).GetCachedMethod(nameof(ReplaceSeasonForTeaBushes), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_S, 14),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushInBloomTranspiler).GetCachedMethod(nameof(ReplaceStartSpring), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_S, 19),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushInBloomTranspiler).GetCachedMethod(nameof(ReplaceEndSpring), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_7),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushInBloomTranspiler).GetCachedMethod(nameof(ReplaceStartFall), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_S, 12),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(BushInBloomTranspiler).GetCachedMethod(nameof(ReplaceEndFall), ReflectionCache.FlagTypes.StaticFlags)),
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