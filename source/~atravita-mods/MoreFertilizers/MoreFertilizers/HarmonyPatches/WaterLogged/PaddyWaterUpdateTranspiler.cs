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

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.WaterLogged;

/// <summary>
/// Transpiler to update neighbors if a paddy crop is fertilized with the waterlogged fertilizer.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class PaddyWaterUpdateTranspiler
{
    private static void UpdateNeighbors(HoeDirt dirt, int index, int tileX, int tileY, GameLocation location)
    {
        if (index == ModEntry.PaddyCropFertilizerID)
        {
            Vector2 v = new(tileX, tileY);
            dirt.nearWaterForPaddy.Value = -1;
            if (dirt.hasPaddyCrop() && dirt.paddyWaterCheck(location, v))
            {
                dirt.state.Value = -1;
                dirt.updateNeighbors(location, v);
            }
        }
    }

    [HarmonyPatch(nameof(HoeDirt.plant))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // the first use of "applySpeedIncreases" is in the fertilizer section.
                OpCodes.Ldarg_0,
                OpCodes.Ldarg_S,
                new(OpCodes.Call, typeof(HoeDirt).GetCachedMethod("applySpeedIncreases", ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(3)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_S, 6),
                new(OpCodes.Call, typeof(PaddyWaterUpdateTranspiler).GetCachedMethod(nameof(UpdateNeighbors), ReflectionCache.FlagTypes.StaticFlags)),
            });
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