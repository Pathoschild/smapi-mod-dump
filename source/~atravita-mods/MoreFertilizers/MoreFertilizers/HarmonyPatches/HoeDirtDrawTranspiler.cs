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

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Holds transpiler to draw the fertilizer. This is slightly convoluted to account for Giant Crop Fertilizer.
/// It should slot after that mod.
/// </summary>
internal static class HoeDirtDrawTranspiler
{
    /// <summary>
    /// Gets the correct color for the fertilizer.
    /// </summary>
    /// <param name="prevColor">The previous color.</param>
    /// <param name="fertilizer">Fertilizer ID.</param>
    /// <returns>A color.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static Color GetColor(Color prevColor, int fertilizer)
    {
        if (fertilizer == -1)
        {
            return prevColor;
        }
        if (ModEntry.PaddyCropFertilizerID == fertilizer)
        {
            return Color.LightCyan;
        }
        if (ModEntry.LuckyFertilizerID == fertilizer)
        {
            return Color.Gold;
        }
        if (ModEntry.JojaFertilizerID == fertilizer)
        {
            return Color.CornflowerBlue;
        }
        if (ModEntry.DeluxeJojaFertilizerID == fertilizer)
        {
            return Color.Navy;
        }
        if (ModEntry.BountifulFertilizerID == fertilizer)
        {
            return Color.Aquamarine;
        }
        if (ModEntry.OrganicFertilizerID == fertilizer)
        {
            return Color.PaleGoldenrod;
        }
        return prevColor;
    }

    /// <summary>
    /// Applies patches to draw this fertilizer slightly different.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <remarks>Should avoid this one firing if Multifertilizers is installed.</remarks>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(HoeDirt).GetCachedMethod(nameof(HoeDirt.DrawOptimized), ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new HarmonyMethod(typeof(HoeDirtDrawTranspiler).StaticMethodNamed(nameof(Transpiler))));
    }

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
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.mouseCursors), ReflectionCache.FlagTypes.StaticFlags)),
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(OpCodes.Newobj),
                })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_R4, 0.0f),
            })
            .Insert(new CodeInstruction[]
            {
                local,
                new(OpCodes.Call, typeof(HoeDirtDrawTranspiler).StaticMethodNamed(nameof(HoeDirtDrawTranspiler.GetColor))),
            });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Hoedirt.DrawOptimized:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}