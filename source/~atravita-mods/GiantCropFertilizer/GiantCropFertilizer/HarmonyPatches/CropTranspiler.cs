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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
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
    /// Applies the patch against DGA to affect DGA giant crops.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <exception cref="MethodNotFoundException">Could not find DGA's methods.</exception>
    internal static void ApplyDGAPatches(Harmony harmony)
    {
        Type dgaCrop = AccessTools.TypeByName("DynamicGameAssets.Game.CustomCrop, DynamicGameAssets")
            ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA CustomCrop");
        harmony.Patch(
            original: dgaCrop.GetCachedMethod("NewDay", ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new HarmonyMethod(typeof(CropTranspiler).StaticMethodNamed(nameof(TranspileDGA))));
    }

    /// <summary>
    /// Gets the chance for a big crop based on the fertilizer.
    /// </summary>
    /// <param name="fertilizer">Fertilizer index.</param>
    /// <returns>chance.</returns>
    private static double GetChanceForFertilizer(double chance, int fertilizer)
    {
        ModEntry.ModMonitor.DebugOnlyLog($"Testing fertilizer {fertilizer} with {ModEntry.GiantCropFertilizerID}", fertilizer != 0, LogLevel.Info);
        return ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == fertilizer ? ModEntry.Config.GiantCropChance : chance;
    }

    /// <summary>
    /// Removes the big crop fertilizer after a big crop was made.
    /// </summary>
    /// <param name="dirt">HoeDirt instance.</param>
    private static void RemoveFertilizer(HoeDirt? dirt)
    {
        if (dirt is not null && dirt.fertilizer.Value != -1 && dirt.fertilizer.Value == ModEntry.GiantCropFertilizerID)
        {
            ModEntry.ModMonitor.DebugOnlyLog("Successfully created giant crop, now removing fertilizer", LogLevel.Info);
            dirt.fertilizer.Value = HoeDirt.noFertilizer;
        }
    }

    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyPatch(nameof(Crop.newDay))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            Type random = AccessTools.TypeByName("StardewValley.OneTimeRandom")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("StardewValley.OneTimeRandom");
            helper.FindNext(new CodeInstructionWrapper[]
            { // Locate the randomness check for a giant crop.
                new(OpCodes.Call, random.GetCachedMethod("GetDouble", ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ldc_R8, 0.01d),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            { // And replace the hard-coded number if necessary.
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call, typeof(CropTranspiler).GetCachedMethod(nameof(GetChanceForFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Locate the code that deletes the crop after a giant crop is created.
                new(OpCodes.Ldfld, typeof(GameLocation).GetCachedField(nameof(GameLocation.terrainFeatures), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.Wildcard),
                new(OpCodes.Callvirt),
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Ldnull),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldnull),
            })
            .Insert(new CodeInstruction[]
            { // Insert a call that removes the fertilizer as well.
                new(OpCodes.Dup),
                new(OpCodes.Call, typeof(CropTranspiler).GetCachedMethod(nameof(RemoveFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
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

    private static IEnumerable<CodeInstruction>? TranspileDGA(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            Type dgaCrop = AccessTools.TypeByName("DynamicGameAssets.Game.CustomCrop, DynamicGameAssets")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA CustomCrop");
            Type dgaCropPack = AccessTools.TypeByName("DynamicGameAssets.PackData.CropPackData")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA CustomCropPackData, DynamicGameAssets");

            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // Find where DGA gets the giant crop chance.
                new(OpCodes.Call, dgaCrop.GetCachedProperty("Data", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Callvirt, dgaCropPack.GetCachedProperty("GiantChance", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Conv_R8),
            })
            .Advance(3)
            .Insert(new CodeInstruction[]
            { // And replace the number if necessary.
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call, typeof(CropTranspiler).GetCachedMethod(nameof(GetChanceForFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
            }).FindNext(new CodeInstructionWrapper[]
            { // Locate the code that deletes the crop after a giant crop is created.
                new(OpCodes.Ldfld, typeof(GameLocation).GetCachedField(nameof(GameLocation.terrainFeatures), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.Wildcard),
                new(OpCodes.Callvirt),
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Ldnull),
                new(OpCodes.Callvirt, typeof(HoeDirt).GetCachedProperty(nameof(HoeDirt.crop), ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new (OpCodes.Ldnull),
            })
            .Insert(new CodeInstruction[]
            { // Insert a call that removes the fertilizer as well.
                new (OpCodes.Dup),
                new (OpCodes.Call, typeof(CropTranspiler).GetCachedMethod(nameof(RemoveFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to transpile {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}