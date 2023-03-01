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
/// Patches on grass.
/// </summary>
[HarmonyPatch(typeof(Grass))]
internal static class GrassPatches
{
    /// <summary>
    /// Prevents grass from being scythed if it was placed.
    /// </summary>
    /// <param name="__instance">Grass instance.</param>
    /// <param name="__result">Set to false to preserve the grass.</param>
    /// <returns>False to skip original, true to continue to original</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Grass.performToolAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool PrefixToolAction(Grass __instance, ref bool __result)
    {
        if (ModEntry.Config.ShouldPlacedGrassIgnoreScythe && __instance.modData?.ContainsKey(SObjectPatches.ModDataKey) == true)
        {
            __result = false;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Prevents placed grass from dying on season change.
    /// </summary>
    /// <param name="__instance">Grass.</param>
    /// <param name="__result">True to remove, false otherwise.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Grass.seasonUpdate))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void PostfixSeasonUpdate(Grass __instance, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        if (__instance.modData?.ContainsKey(SObjectPatches.ModDataKey) == true)
        {
            __result = false;
            __instance.loadSprite();
        }
    }

    [HarmonyPatch(nameof(Grass.dayUpdate))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find and remove this.grassType == 1. We want all grass to grow!
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Grass).GetCachedField(nameof(Grass.grassType), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call,
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un_S,
            })
            .GetLabels(out IList<Label>? labelsToMove)
            .Remove(5)
            .AttachLabels(labelsToMove);

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
