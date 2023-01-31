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

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.EverlastingFertilizer;

/// <summary>
/// Removes the season check if the Everlasting Fertilizer is down.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class RemoveSeasonCheck
{
    private const string WinterStar = "spacechase0.TheftOfTheWinterStar";

    private static int TempusGlobeID => ModEntry.JsonAssetsAPI?.GetBigCraftableId("Tempus Globe") ?? -1;

    /// <summary>
    /// Whether or not this hoedirt has the everlasting fertilizer in it.
    /// </summary>
    /// <param name="dirt">Hoedirt.</param>
    /// <returns>True if the fertilizer is the everlasting fertilizer.</returns>
    [MethodImpl(TKConstants.Hot)]
    internal static bool IsInEverlasting(HoeDirt dirt)
    => ModEntry.EverlastingFertilizerID != -1 && dirt.fertilizer?.Value == ModEntry.EverlastingFertilizerID;

    /// <summary>
    /// Applies patches when Theft of the Winter Star is not installed.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.plant), ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new(typeof(RemoveSeasonCheck).GetCachedMethod(nameof(TranspilerSansWinterStar), ReflectionCache.FlagTypes.StaticFlags)));
    }

    /// <summary>
    /// Applies patches when Theft of the Winter Star is installed.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatchesForWinterStar(Harmony harmony)
    {
        try
        {
            harmony.Patch(
                original: typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.plant), ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: new(typeof(RemoveSeasonCheck).GetCachedMethod(nameof(TranspilerWithWinterStar), ReflectionCache.FlagTypes.StaticFlags)));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while patching theft of the winter star. Integration may not work.\n\n{ex}", LogLevel.Error);
        }
    }

    // checks to see if the tile is covered by our fertilizer or is covered by a tempus globe.
    [MethodImpl(TKConstants.Hot)]
    private static bool IsInEverlastingWithTempusGlobe(Crop crop, HoeDirt dirt, int tileX, int tileY, GameLocation location)
    {
        if (IsInEverlasting(dirt))
        {
            crop.seasonsToGrowIn.Set(new[] { "spring", "summer", "fall", "winter" });
            return true;
        }

        if (TempusGlobeID == -1)
        {
            ModEntry.ModMonitor.Log("Tempus globe not found?");
            return false;
        }

        // replicate the Tempus Globe's check.
        for (int x = tileX - 2; x <= tileX + 2; x++)
        {
            for (int y = tileY - 2; y <= tileY + 2; y++)
            {
                if (location.Objects.TryGetValue(new Vector2(x, y), out SObject? obj)
                    && obj.bigCraftable.Value && obj.ParentSheetIndex == TempusGlobeID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // simpler version, if theft of the winter star isn't installed.
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? TranspilerSansWinterStar(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindLast(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdArg,
                (OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.currentLocation), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.IsGreenhouse), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                OpCodes.Brtrue_S,
            })
            .Push()
            .Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(RemoveSeasonCheck).GetCachedMethod(nameof(IsInEverlasting), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S, jumppoint),
            }, withLabels: labels);

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

#warning - todo: the version where Theft of the Winter Star is installed. (Also remove casey's prefix).
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? TranspilerWithWinterStar(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldarg_3,
                (OpCodes.Newobj, typeof(Crop).GetCachedConstructor<int, int, int>(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(4);

            CodeInstruction? crop = helper.CurrentInstruction.ToLdLoc();

            helper.FindLast(instructions: new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdArg,
                (OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.currentLocation), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.IsGreenhouse), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                OpCodes.Brtrue_S,
            })
            .Push()
            .Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                crop,
                new(OpCodes.Ldarg_0), // this
                new(OpCodes.Ldarg_2), // tile_X
                new(OpCodes.Ldarg_3), // tile_Y
                new(OpCodes.Ldarg_S, 6), // game location.
                new(OpCodes.Call, typeof(RemoveSeasonCheck).GetCachedMethod(nameof(IsInEverlastingWithTempusGlobe), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S, jumppoint),
            }, withLabels: labels);

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

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(HoeDirt.canPlantThisSeedHere))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? TranspileCanPlant(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.currentLocation), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                (OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.IsGreenhouse), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                OpCodes.Brtrue_S,
            })
            .Push()
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(RemoveSeasonCheck).GetCachedMethod(nameof(IsInEverlasting), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S, jumppoint),
            }, withLabels: labels);

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