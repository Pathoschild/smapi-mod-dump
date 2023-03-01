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

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.HarmonyPatches.GrassPatches;

/// <summary>
/// Holds patches against farm animals.
/// </summary>
[HarmonyPatch(typeof(FarmAnimal))]
internal static class FarmAnimalPatches
{
    private static bool ShouldEatThisGrass(TerrainFeature feature)
    {
        if (feature is not Grass grass || grass.grassType.Value == Grass.cobweb)
        {
            return false;
        }
        if (!ModEntry.Config.ShouldAnimalsEatPlacedGrass && grass.modData?.ContainsKey(SObjectPatches.ModDataKey) == true)
        {
            return false;
        }
        return true;
    }

    [HarmonyPatch(nameof(FarmAnimal.grassEndPointFunction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find and replace t is Grass with our own check.
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Isinst, typeof(Grass)),
                OpCodes.Brfalse_S,
            })
            .Advance(1)
            .ReplaceInstruction(
                opcode: OpCodes.Call,
                operand: typeof(FarmAnimalPatches).GetCachedMethod(nameof(ShouldEatThisGrass), ReflectionCache.FlagTypes.StaticFlags));

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
