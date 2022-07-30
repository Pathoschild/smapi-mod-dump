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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
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
    /// Applies patches to draw this fertilizer slightly different.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <remarks>Should avoid this one firing if Multifertilizers is installed.</remarks>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.DrawOptimized), ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new HarmonyMethod(typeof(HoeDirtDrawTranspiler).StaticMethodNamed(nameof(HoeDirtDrawTranspiler.Transpiler))));
    }

    /// <summary>
    /// Gets the correct color for the fertilizer.
    /// </summary>
    /// <param name="fertilizer">Fertilizer ID.</param>
    /// <returns>A color.</returns>
    [MethodImpl(TKConstants.Hot)]
    private static Color GetColor(int fertilizer)
        => ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == fertilizer ? Color.Purple : Color.White;

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(OpCodes.Call, typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.GetFertilizerSourceRect), ReflectionCache.FlagTypes.InstanceFlags)),
                })
            .Advance(1);

            // Grab the relevant local.
            CodeInstruction local = helper.CurrentInstruction.Clone();

            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Call, typeof(Color).GetCachedProperty("White", ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                })
            .GetLabels(out IList<Label> labels, clear: true)
            .ReplaceInstruction(OpCodes.Call, typeof(HoeDirtDrawTranspiler).GetCachedMethod(nameof(GetColor), ReflectionCache.FlagTypes.StaticFlags))
            .Insert(new CodeInstruction[] { local }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}