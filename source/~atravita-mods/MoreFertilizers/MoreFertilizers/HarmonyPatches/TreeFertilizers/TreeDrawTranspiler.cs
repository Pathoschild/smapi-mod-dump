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

namespace MoreFertilizers.HarmonyPatches.TreeFertilizers;

/// <summary>
/// Holds patches against the draw code of trees.
/// </summary>
[HarmonyPatch(typeof(Tree))]
internal static class TreeDrawTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color ReplaceColorIfNeeded(Color prevcolor, Tree tree)
    {
        if (!ModEntry.Config.RecolorTrees)
        {
            return prevcolor;
        }
        try
        {
            if (tree.modData?.GetBool(CanPlaceHandler.TreeTapperFertilizer) == true)
            {
                return Color.LightGreen;
            }
            else if (tree.modData?.GetBool(CanPlaceHandler.TreeFertilizer) == true)
            {
                return Color.MonoGameOrange;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.LogOnce($"Crash while drawing trees!\n\n{ex}", LogLevel.Error);
        }
        return prevcolor;
    }

    [HarmonyPatch(nameof(Tree.draw))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Tree).GetCachedField(nameof(Tree.growthStage), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // op_implicit
                OpCodes.Ldc_I4_5,
                OpCodes.Bge,
            })
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Ldfld, typeof(Tree).GetCachedField(nameof(Tree.texture), ReflectionCache.FlagTypes.InstanceFlags)),
            });

            for (int i = 0; i < 2; i++)
            {
                helper.FindNext(new CodeInstructionWrapper[]
                {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                })
                .Advance(1)
                .Insert(new CodeInstruction[]
                {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(TreeDrawTranspiler).GetCachedMethod(nameof(TreeDrawTranspiler.ReplaceColorIfNeeded), ReflectionCache.FlagTypes.StaticFlags)),
                });
            }

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
