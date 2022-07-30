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
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.Compat;

/// <summary>
/// Paches MultiFertilzer's Draw to draw my fertilizer, goddammit.
/// </summary>
internal static class MultiFertilizerDrawTranspiler
{
    /// <summary>
    /// Applies the patches against Multifertilizer's fertilizer drawing to draw this fertilizer.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <exception cref="MethodNotFoundException">A method was not found.</exception>
    internal static void ApplyPatches(Harmony harmony)
    {
        Type multidraw = AccessTools.TypeByName("MultiFertilizer.Patches.HoeDirtPatcher")
            ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("MultiFert's draw");
        harmony.Patch(
            original: multidraw.GetCachedMethod("DrawMultiFertilizer", ReflectionCache.FlagTypes.StaticFlags),
            transpiler: new HarmonyMethod(typeof(MultiFertilizerDrawTranspiler), nameof(MultiFertilizerDrawTranspiler.Transpiler)));
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.JumpTo(0)
            .GetLabels(out IList<Label>? labels, clear: true)
            .Insert(
                new CodeInstruction[]
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldarg_2),
                new (OpCodes.Ldarg_S, 10),
                new (OpCodes.Call, typeof(MultiFertilizerDrawTranspiler).StaticMethodNamed(nameof(DrawThisFertilzer))),
            }, withLabels: labels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling MultiFertilizer's Hoedirt.Draw:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    private static void DrawThisFertilzer(
        SpriteBatch spriteBatch,
        Vector2 pos,
        HoeDirt dirt)
    {
        try
        {
            if (ModEntry.PlantableFertilizerIDs.Contains(dirt.fertilizer.Value))
            {
                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    position: pos,
                    sourceRectangle: dirt.GetFertilizerSourceRect(dirt.fertilizer.Value),
                    color: HoeDirtDrawTranspiler.GetColor(Color.White, dirt.fertilizer.Value),
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Game1.pixelZoom,
                    effects: SpriteEffects.None,
                    layerDepth: 1.92e-08f);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while trying to draw fertlizer in MultiFertilizer compat patch!\n\n{ex}", LogLevel.Error);
        }
    }
}