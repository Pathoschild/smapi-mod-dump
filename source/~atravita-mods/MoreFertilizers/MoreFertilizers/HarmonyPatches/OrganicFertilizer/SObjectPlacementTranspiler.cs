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
using MoreFertilizers.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.OrganicFertilizer;

/// <summary>
/// Handles organic seeds for SObject.placementAction.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal static class SObjectPlacementTranspiler
{
    private static void AdjustPlantedObject(HoeDirt? dirt, SObject obj)
    {
        if (dirt?.fertilizer?.Value == HoeDirt.noFertilizer
            && ModEntry.OrganicFertilizerID != -1
            && obj.modData?.GetBool(CanPlaceHandler.Organic) == true)
        {
            dirt.fertilizer.Value = ModEntry.OrganicFertilizerID;
        }
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(SObject.placementAction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // Find the location where Hoedirt.plant is called.
                new(OpCodes.Callvirt, typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.plant), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Brfalse),
            })
            .Advance(2)
            .Push()
            .FindPrev(new CodeInstructionWrapper[]
            { // Back up to the start of that block.
                new(OpCodes.Brfalse),
                new(SpecialCodeInstructionCases.LdLoc),
            }).Advance(1)
            .CopyIncluding(new CodeInstructionWrapper[]
            { // Get the block that gets the reference to the hoedirt. It's kinda long....
                new(OpCodes.Castclass, typeof(HoeDirt)),
            }, out IEnumerable<CodeInstruction> copy);

            List<CodeInstruction> codes = new(copy)
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(SObjectPlacementTranspiler).StaticMethodNamed(nameof(AdjustPlantedObject))),
            };

            helper.Pop()
            .Insert(codes.ToArray());

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling SObject.placementAction:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}