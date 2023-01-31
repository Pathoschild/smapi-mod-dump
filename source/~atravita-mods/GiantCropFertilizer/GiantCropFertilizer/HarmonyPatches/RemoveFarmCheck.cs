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

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Removes the check for isinst farm for giant crops.
/// </summary>
internal static class RemoveFarmCheck
{
    /// <summary>
    /// Applies the patch that removes the giant crop on farm only check.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        // use a lower priority to slot after other mods that might want to transpile this method as well.
        harmony.Patch(
            original: typeof(Crop).GetCachedMethod(nameof(Crop.newDay), ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new HarmonyMethod(typeof(RemoveFarmCheck).GetCachedMethod(nameof(Transpiler), ReflectionCache.FlagTypes.StaticFlags), Priority.HigherThanNormal));
    }

    private static bool IsFarmOrSetOtherwise(GameLocation? location)
        => location is Farm || ModEntry.Config.AllowGiantCropsOffFarm || location?.Map?.Properties?.ContainsKey("AllowGiantCrops") == true;

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.indexOfHarvest), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4, 276),
                new(OpCodes.Beq_S),
            })
            .FindPrev(new CodeInstructionWrapper[]
            { // Replace the check for is Farm with our own check.
                new(SpecialCodeInstructionCases.LdArg),
                new(OpCodes.Isinst, typeof(Farm)),
                new(OpCodes.Brfalse),
            })
            .Advance(1)
            .ReplaceInstruction(OpCodes.Call, typeof(RemoveFarmCheck).GetCachedMethod(nameof(IsFarmOrSetOtherwise), ReflectionCache.FlagTypes.StaticFlags))
            .FindNext(new CodeInstructionWrapper[]
            { // Remove the cast to Farm. Both have to be removed to avoid nre.
                new(OpCodes.Isinst, typeof(Farm)),
            })
            .Remove(1);

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