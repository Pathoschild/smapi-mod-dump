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
using StardewValley.Events;
using StardewValley.TerrainFeatures;

namespace ReviveDeadCrops.HarmonyPatches;

/// <summary>
/// Transpiles the fairy event so she revives crops. Was always kinda weird when she
/// would go to a dead crop....
/// </summary>
[HarmonyPatch(typeof(FairyEvent))]
internal static class TranspileFairy
{
    private static void AnimateReviveCrop(GameLocation location, Vector2 tile)
    {
        if (location.terrainFeatures.TryGetValue(tile, out var terrain) && terrain is HoeDirt dirt
            && dirt.crop?.dead.Value == true)
        {
            dirt.crop.dead.Value = false;
        }
    }

    private static void ActuallyReviveCrop(Crop crop)
        => ModEntry.Api.RevivePlant(crop);

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FairyEvent.tickUpdate))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? UpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Callvirt, typeof(HoeDirt).GetCachedProperty(nameof(HoeDirt.crop), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.currentPhase), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindPrev(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(FairyEvent).GetCachedField("f", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldfld),
            })
            .GetLabels(out var labels)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(FairyEvent).GetCachedField("f", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(FairyEvent).GetCachedField("targetCrop", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call, typeof(TranspileFairy).GetCachedMethod(nameof(AnimateReviveCrop), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FairyEvent.makeChangesToLocation))]
    private static IEnumerable<CodeInstruction>? MakeChangesTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Callvirt, typeof(HoeDirt).GetCachedProperty(nameof(HoeDirt.crop), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Callvirt, typeof(Crop).GetCachedMethod(nameof(Crop.growCompletely), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Call, typeof(TranspileFairy).GetCachedMethod(nameof(ActuallyReviveCrop), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}