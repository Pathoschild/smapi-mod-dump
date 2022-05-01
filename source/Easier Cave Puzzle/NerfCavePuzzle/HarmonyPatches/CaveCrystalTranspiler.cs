/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/NerfCavePuzzle
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;

namespace NerfCavePuzzle.HarmonyPatches;

/// <summary>
/// Transpilers against the CaveCrystal inner class.
/// </summary>
internal static class CaveCrystalTranspiler
{
    /// <summary>
    /// Applies the transpilers against CaveCrystal.
    /// </summary>
    /// <param name="harmony">armony instance.</param>
    /// <exception cref="MethodNotFoundException">Either the inner type or the method was not found.</exception>
    internal static void ApplyPatch(Harmony harmony)
    {
        try
        {
            Type cavecrystal = typeof(IslandWestCave1).GetNestedType("CaveCrystal") ?? throw new MethodNotFoundException("IslandWestCave1+CaveCrystal not found");
            harmony.Patch(
                original: cavecrystal.InstanceMethodNamed("activate"),
                transpiler: new HarmonyMethod(typeof(CaveCrystalTranspiler).StaticMethodNamed(nameof(CaveCrystalTranspiler.ActivateTranspiler))));
            harmony.Patch(
                original: cavecrystal.InstanceMethodNamed("update"),
                transpiler: new HarmonyMethod(typeof(CaveCrystalTranspiler).StaticMethodNamed(nameof(CaveCrystalTranspiler.UpdateTranspiler))));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while transpiling CaveCrystal to make the flashes go slower.\n\n{ex}", LogLevel.Error);
        }
    }

    private static float GetFlashScale()
        => ModEntry.Config.FlashScale;

    private static IEnumerable<CodeInstruction>? ActivateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindFirst(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, 1000f),
                new(OpCodes.Stfld, typeof(IslandWestCave1).GetNestedType("CaveCrystal")?.InstanceFieldNamed("glowTimer") ?? throw new MethodNotFoundException("IslandWestCave1+CaveCrystal not found")),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(CaveCrystalTranspiler).StaticMethodNamed(nameof(CaveCrystalTranspiler.GetFlashScale))),
                new(OpCodes.Mul),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling CaveCrystal.activate.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    private static IEnumerable<CodeInstruction>? UpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldc_R4, 1000f),
                    new(OpCodes.Div),
                    new(OpCodes.Call, typeof(Utility).StaticMethodNamed(nameof(Utility.Lerp))),
                },
                transformer: (helper) =>
                {
                    helper.Advance(1)
                        .Insert(new CodeInstruction[]
                        {
                            new(OpCodes.Call, typeof(CaveCrystalTranspiler).StaticMethodNamed(nameof(CaveCrystalTranspiler.GetFlashScale))),
                            new(OpCodes.Mul),
                        });
                    return true;
                });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling CaveCrystal.update.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}