/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoreFertilizers.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.FruitTreePatches;

[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreeDrawTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static Color ReplaceColorIfNeeded(Color prevcolor, FruitTree tree)
    {
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
                new(OpCodes.Ldfld, typeof(FruitTree).InstanceFieldNamed(nameof(FruitTree.growthStage))),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Bge),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Color).StaticPropertyNamed(nameof(Color.White)).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(FruitTreeDrawTranspiler).StaticMethodNamed(nameof(FruitTreeDrawTranspiler.ReplaceColorIfNeeded))),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling FruitTree.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}
