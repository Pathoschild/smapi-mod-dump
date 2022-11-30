/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ColoredCrystalariums;

[HarmonyPatch]
class HarmonyPatches
{
    private static readonly ConditionalWeakTable<Object, ColorHolder> ColorTable = new();

    [HarmonyPatch(typeof(Object), nameof(Object.performObjectDropInAction))]
    [HarmonyPostfix]
    private static void UpdateOverlayColor(Object __instance, bool __result, bool probe)
    {
        if (!__instance.name.Equals("Crystalarium") || __instance.heldObject.Value is null || !__result)// || probe)
            return;

        // generate overlay color based on input item
        Color overlayColor = AssetManager.GetColorFor(__instance.heldObject.Value);

        // store overlay color in the CWT for more performance-friendly access
        ColorTable.AddOrUpdate(__instance, new ColorHolder(overlayColor));
        // store in moddata as a backup if CWT data is lost/cleared
        __instance.modData["sophie.ColorChangingCrystalariums/OverlayColor"] = overlayColor.PackedValue.ToString();
    }

    [HarmonyPatch(declaringType: typeof(Object), methodName: nameof(Object.draw), argumentTypes: new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)})]
    [HarmonyPrefix]
    private static bool DrawOverlay(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        if (!__instance.name.Equals(value: "Crystalarium") || __instance.heldObject.Value is null)
            return true;

        Color overlayColor;

        // try retrieving color from CWT first
        if (ColorTable.TryGetValue(key: __instance, value: out var colorHolder) && colorHolder is not null)
        {
            overlayColor = colorHolder.Color;
        }
        // if that fails, try retrieving color from moddata and reconstructing, then store in CWT
        else if (__instance.modData.TryGetValue(key: "sophie.ColorChangingCrystalariums/OverlayColor", value: out string packedValue) && packedValue is not null or "")
        {
            overlayColor = new Color(packedValue: uint.Parse(s: packedValue));
            ColorTable.AddOrUpdate(key: __instance, value: new ColorHolder(overlayColor: overlayColor));
        }
        // if both of those fail, reconstruct the color manually and store in moddata and CWT - should never happen
        else
        {
            overlayColor = AssetManager.GetColorFor(item: __instance.heldObject.Value);
            __instance.modData.Add(key: "sophie.ColorChangingCrystalariums/OverlayColor", value: overlayColor.PackedValue.ToString());
            ColorTable.AddOrUpdate(key: __instance, value: new ColorHolder(overlayColor: overlayColor));
        }

        float draw_layer =  Math.Max(val1: 0f, val2: ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f;
        Vector2 scaleFactor = __instance.getScale() * 4f;
        Vector2 position = Game1.GlobalToLocal(viewport: Game1.viewport, globalPosition: new Vector2(x: x * 64, y: y * 64 - 64));

        int jitterX = 0;
        int jitterY = 0;

        if (__instance.shakeTimer > 0)
        {
            jitterX = Game1.random.Next(minValue: -1, maxValue: 2);
            jitterY = Game1.random.Next(minValue: -1, maxValue: 2);
        }

        Rectangle destination = new(
            x: (int) (position.X - scaleFactor.X / 2f) + jitterX,
            y: (int) (position.Y - scaleFactor.Y / 2f) + jitterY,
            width: (int) (64f + scaleFactor.X),
            height: (int) (128f + scaleFactor.Y / 2f)
        );

        spriteBatch.Draw(
            texture: Game1.bigCraftableSpriteSheet,
            destinationRectangle: destination,
            sourceRectangle: Object.getSourceRectForBigCraftable(index: __instance.showNextIndex.Value ? (__instance.ParentSheetIndex + 1) : __instance.ParentSheetIndex),
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: draw_layer
        );

        spriteBatch.Draw(
            texture: AssetManager.OverlayTexture,
            destinationRectangle: destination,
            sourceRectangle: new Rectangle(x: 0, y: 0, width: 16, height: 32),
            color: overlayColor,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: draw_layer + 0.0001f
        );

        if (!__instance.readyForHarvest.Value)
        {
            return false;
        }

        float base_sort = (y + 1) * 64 / 10000f + __instance.TileLocation.X / 50000f;
        float yOffset = 4f * (float)Math.Round(value: Math.Sin(a: Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), digits: 2);

        if (!Globals.Config.DrawSimplifiedGemBubbles)
        {
            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                position: Game1.GlobalToLocal(viewport: Game1.viewport,
                    globalPosition: new Vector2(x: x * 64 - 8, y: y * 64 - 96 - 16 + yOffset)),
                sourceRectangle: new Rectangle(x: 141, y: 465, width: 20, height: 24),
                color: Color.White * 0.75f,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: base_sort + 1E-06f
            );
        }

        spriteBatch.Draw(
            texture: Game1.objectSpriteSheet,
            position: Game1.GlobalToLocal(viewport: Game1.viewport, globalPosition: new Vector2(x: x * 64 + 32, y: y * 64 - 64 - 8 + yOffset)),
            sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.objectSpriteSheet, tilePosition: __instance.heldObject.Value.ParentSheetIndex, width: 16, height: 16),
            color: Color.White * 0.75f,
            rotation: 0f,
            origin: new Vector2(x: 8f, y: 8f),
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: base_sort + 1E-05f
        );

        return false;
    }
}

internal class ColorHolder
{
    internal Color Color { get; set; }
    public ColorHolder(Color overlayColor) => Color = overlayColor;
}
