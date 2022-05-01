/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Handles the transpiler for Crop.newDay.
/// </summary>
[HarmonyPatch(typeof(Crop))]
internal static class CropTranspiler
{
    /// <summary>
    /// Gets the chance for a big crop based on the fertilizer.
    /// </summary>
    /// <param name="fertilizer">Fertilizer index.</param>
    /// <returns>chance.</returns>
    public static double GetChanceForFertilizer(int fertilizer)
    {
        ModEntry.ModMonitor.DebugOnlyLog($"Testing fertilizer {fertilizer} with {ModEntry.GiantCropFertilizerID}", LogLevel.Info);
        return ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == fertilizer ? ModEntry.Config.GiantCropChance : 0.01;
    }

    /// <summary>
    /// Removes the big crop fertilizer after a big crop was made.
    /// </summary>
    /// <param name="dirt">Hoedirt instance.</param>
    public static void RemoveFertilizer(HoeDirt? dirt)
    {
        if (dirt is not null && dirt.fertilizer.Value != -1 && dirt.fertilizer.Value == ModEntry.GiantCropFertilizerID)
        {
            ModEntry.ModMonitor.DebugOnlyLog("Successfully created giant crop, now removing fertilizer", LogLevel.Info);
            dirt.fertilizer.Value = HoeDirt.noFertilizer;
        }
    }

    [HarmonyPatch(nameof(Crop.newDay))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            Type random = AccessTools.TypeByName("StardewValley.OneTimeRandom") ?? throw new MethodNotFoundException("StardewValley.OneTimeRandom not found");
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Call, random.StaticMethodNamed("GetDouble")),
                new (OpCodes.Ldc_R8, 0.01d),
            })
            .Advance(1)
            .GetLabels(out IList<Label>? labels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(CropTranspiler).StaticMethodNamed(nameof(CropTranspiler.GetChanceForFertilizer)))
            .Insert(new CodeInstruction[] { new (OpCodes.Ldarg_2) }, withLabels: labels)
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldfld, typeof(GameLocation).InstanceFieldNamed(nameof(GameLocation.terrainFeatures))),
                new (SpecialCodeInstructionCases.Wildcard),
                new (OpCodes.Callvirt),
                new (OpCodes.Isinst, typeof(HoeDirt)),
                new (OpCodes.Ldnull),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldnull),
            })
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Dup),
                new (OpCodes.Call, typeof(CropTranspiler).StaticMethodNamed(nameof(CropTranspiler.RemoveFertilizer))),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}