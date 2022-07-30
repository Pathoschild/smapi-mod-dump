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
using Microsoft.Xna.Framework;
using MoreFertilizers.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.FruitTreePatches;

/// <summary>
/// Transpilers to color fertilized fruit trees.
/// </summary>
[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreeDrawTranspiler
{
    /// <summary>
    /// Applies this patch to DGA.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyDGAPatch(Harmony harmony)
    {
        try
        {
            Type dgaFruitTree = AccessTools.TypeByName("DynamicGameAssets.Game.CustomFruitTree")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA Fruit Trees");
            harmony.Patch(
                original: dgaFruitTree.InstanceMethodNamed("draw"),
                transpiler: new HarmonyMethod(typeof(FruitTreeDrawTranspiler), nameof(Transpiler)));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling DGA. Integration may not work correctly.\n\n{ex}", LogLevel.Error);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private static Color ReplaceColorIfNeeded(Color prevcolor, FruitTree tree)
    {
        if (!ModEntry.Config.RecolorFruitTrees)
        {
            return prevcolor;
        }
        try
        {
            if (tree.modData?.GetInt(CanPlaceHandler.FruitTreeFertilizer) is int result)
            {
                return result > 1 ? Color.Red : Color.Orange;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.LogOnce($"Crash while drawing fruit trees!\n\n{ex}", LogLevel.Error);
        }
        return prevcolor;
    }

    [HarmonyPatch(nameof(FruitTree.draw))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(FruitTree).GetCachedField(nameof(FruitTree.growthStage), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Bge),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(FruitTreeDrawTranspiler).GetCachedMethod(nameof(FruitTreeDrawTranspiler.ReplaceColorIfNeeded), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling FruitTree.Draw:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
