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

namespace MoreFertilizers.HarmonyPatches.LuckyFertilizer;

/// <summary>
/// Transpiles the Farm.AddCrow function to remove certain fertilized crops from possibly being eaten by crows.
/// </summary>
[HarmonyPatch(typeof(Farm))]
internal static class FarmAddCrowTranspiler
{
    /// <summary>
    /// Gets a value indiciating whether or not this tile has the lucky fertilizer on it.
    /// </summary>
    /// <param name="dirt">The hoedirt instance.</param>
    /// <returns>true if has fertilizer, false otherwise.</returns>
    internal static bool HasLuckyFertilizer(HoeDirt? dirt)
        => ModEntry.LuckyFertilizerID != -1 && dirt is not null && dirt.fertilizer.Value.Equals(ModEntry.LuckyFertilizerID);

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(Farm.addCrows))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // exclude from the initial count.
                new (OpCodes.Ldloca_S),
                new (OpCodes.Call, typeof(KeyValuePair<Vector2, TerrainFeature>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new (OpCodes.Isinst, typeof(HoeDirt)),
                new (OpCodes.Brfalse_S),
            }).Push();

            CodeInstruction? local = helper.CurrentInstruction.Clone();

            helper.Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .Insert(new CodeInstruction[]
            {
                local,
                new(OpCodes.Call, typeof(KeyValuePair<Vector2, TerrainFeature>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Call, typeof(FarmAddCrowTranspiler).GetCachedMethod(nameof(HasLuckyFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S, label),
            }, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // skip past if picked anyways.
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.Next), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int) } )),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(GameLocation).GetCachedField(nameof(GameLocation.terrainFeatures), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt),
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(OpCodes.Brfalse),
            })
            .Copy(5, out IEnumerable<CodeInstruction>? codes)
            .Advance(5)
            .Push()
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label secondLabel)
            .Pop()
            .Advance(1)
            .GetLabels(out IList<Label>? labelsToMove2)
            .Insert(codes.ToArray(), labelsToMove2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(FarmAddCrowTranspiler).GetCachedMethod(nameof(HasLuckyFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue_S, secondLabel),
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
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
