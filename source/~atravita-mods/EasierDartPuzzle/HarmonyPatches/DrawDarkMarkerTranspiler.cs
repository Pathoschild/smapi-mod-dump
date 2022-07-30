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
using AtraBase.Toolkit.Extensions;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Minigames;

namespace EasierDartPuzzle.HarmonyPatches;

/// <summary>
/// Draws a second, more precise marker.
/// </summary>
[HarmonyPatch(typeof(Darts))]
internal static class DrawDarkMarkerTranspiler
{
    [MethodImpl(TKConstants.Hot)]
    private static void DrawImpl(SpriteBatch b, Texture2D tex, Darts instance)
    {
        if (ModEntry.Config.ShowDartMarker)
        {
            b.Draw(
                texture: tex,
                position: instance.TransformDraw(instance.aimPosition),
                sourceRectangle: new Rectangle(0, 320, 64, 64),
                color: Color.Crimson,
                rotation: 0f,
                origin: new Vector2(32f, 32),
                scale: 0.05f,
                effects: SpriteEffects.None,
                layerDepth: 0.0001f);
        }
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyPatch(nameof(Darts.draw))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, typeof(Darts).GetCachedMethod(nameof(Darts.IsAiming), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Brtrue_S),
            })
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .GetLabels(out var labels, clear: true)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1), // spritebatch
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Darts).GetCachedField("texture", ReflectionCache.FlagTypes.InstanceFlags)), // texture
                new(OpCodes.Ldarg_0), // instance
                new(OpCodes.Call, typeof(DrawDarkMarkerTranspiler).GetCachedMethod(nameof(DrawImpl), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.GetFullName()}!\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
