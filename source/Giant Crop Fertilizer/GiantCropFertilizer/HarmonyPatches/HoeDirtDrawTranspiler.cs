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
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Holds transpiler to draw the fertilizer.
/// </summary>
internal static class HoeDirtDrawTranspiler
{

    /// <summary>
    /// Gets the correct color for the fertilizer.
    /// </summary>
    /// <param name="fertilizer">Fertilizer ID.</param>
    /// <returns>A color.</returns>
    public static Color GetColor(int fertilizer)
        => ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == fertilizer ? Color.Purple : Color.White;

    /// <summary>
    /// Applies patches to draw this fertilizer slightly different.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <remarks>Should avoid this one firing if Multifertilizers is installed.</remarks>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(HoeDirt).InstanceMethodNamed(nameof(HoeDirt.DrawOptimized)),
            transpiler: new HarmonyMethod(typeof(HoeDirtDrawTranspiler).StaticMethodNamed(nameof(HoeDirtDrawTranspiler.Transpiler))));
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(OpCodes.Call, typeof(HoeDirt).InstanceMethodNamed(nameof(HoeDirt.GetFertilizerSourceRect))),
                })
            .Advance(1);

            // Grab the relevant local.
            CodeInstruction local = helper.CurrentInstruction.Clone();

            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Call, typeof(Color).StaticPropertyNamed("White").GetGetMethod()),
                })
            .GetLabels(out IList<Label> labels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(HoeDirtDrawTranspiler).StaticMethodNamed(nameof(HoeDirtDrawTranspiler.GetColor)))
            .Insert(new CodeInstruction[]
            {
                local,
            }, withLabels: labels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}