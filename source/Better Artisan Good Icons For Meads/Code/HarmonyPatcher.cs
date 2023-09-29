/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/bagi-meads
**
*************************************************/

#nullable enable
using System;
using System.Collections;
using System.Linq;
using DaLion.Meads.Content;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace DaLion.Meads;

internal static class HarmonyPatcher
{
    private delegate bool GetDrawInfoDelegate(SObject output, out Texture2D textureSheet, out Rectangle mainPosition,
        out Rectangle iconPosition);

    private static readonly GetDrawInfoDelegate GetDrawInfo = "BetterArtisanGoodIcons.ArtisanGoodsManager".ToType()
        .RequireMethod("GetDrawInfo").CreateDelegate<GetDrawInfoDelegate>();

    internal static void Apply(Harmony harmony)
    {
        // vanilla patches for mead rendering
        harmony.Patch(
            original: typeof(Furniture).RequireMethod(nameof(Furniture.draw),
                new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
            prefix: new(typeof(HarmonyPatcher).RequireMethod(nameof(FurnitureDrawPrefix)),
                before: new[] { "cat.betterartisangoodicons" })
        );

        harmony.Patch(
            original: typeof(SObject).RequireMethod(nameof(SObject.draw),
                new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
            prefix: new(typeof(HarmonyPatcher).RequireMethod(nameof(ObjectDrawPrefix)),
                before: new[] { "cat.betterartisangoodicons" })
        );

        harmony.Patch(
            original: typeof(SObject).RequireMethod(nameof(SObject.draw),
                new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(float) }),
            prefix: new(typeof(HarmonyPatcher).RequireMethod(nameof(ObjectDrawOverloadPrefix)),
                before: new[] { "cat.betterartisangoodicons" })
        );

        harmony.Patch(
            original: typeof(SObject).RequireMethod(nameof(SObject.drawInMenu),
                new[]
                {
                    typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                    typeof(StackDrawType), typeof(Color), typeof(bool)
                }),
            prefix: new(typeof(HarmonyPatcher).RequireMethod(nameof(ObjectDrawInMenuPrefix)),
                before: new[] { "cat.betterartisangoodicons" })
        );

        harmony.Patch(
            original: typeof(SObject).RequireMethod(nameof(SObject.drawWhenHeld),
                new[] { typeof(SpriteBatch), typeof(Vector2), typeof(Farmer) }),
            prefix: new(typeof(HarmonyPatcher).RequireMethod(nameof(ObjectDrawWhenHeldPrefix)),
                before: new[] { "cat.betterartisangoodicons" })
        );

        // BAGI patches
        harmony.Patch(
            original: "BetterArtisanGoodIcons.ArtisanGoodTextureProvider".ToType().RequireMethod("GetSourceName"),
            prefix: new(typeof(HarmonyPatcher), nameof(ArtisanGoodTextureProviderGetSourceNamePrefix))
        );

        harmony.Patch(
            original: "BetterArtisanGoodIcons.Content.ContentSourceManager".ToType().RequireMethod("GetTextureProviders"),
            postfix: new(typeof(HarmonyPatcher), nameof(ContentSourceManagerGetTextureProvidersPostfix))
        );
    }

    #region harmony patches

    /// <summary>Patch mead good source name.</summary>
    private static bool ArtisanGoodTextureProviderGetSourceNamePrefix(ref bool __result, SObject item, ref string sourceName)
    {
        if (item.name != "Mead") return true; // run original logic

        sourceName = "_Base";
        __result = true;
        return false; // don't run original logic
    }

    /// <summary>Patch to supplement load mead texture providers.</summary>
    private static void ContentSourceManagerGetTextureProvidersPostfix(IModHelper helper, IMonitor monitor)
    {
        var providers = helper.ContentPacks.GetOwned()
            .Select(pack => ContentSourceManager.TryLoadContentSource(new ContentPackSource(pack), monitor));

        var textureProviders = (IList)"BetterArtisanGoodIcons.ArtisanGoodsManager".ToType().RequireField("TextureProviders")
            .GetValue(null)!;
        foreach (var provider in providers)
            if (provider is not null)
                textureProviders.Add(provider);
    }

    /// <summary>Patch to draw BAGI-like meads on furniture.</summary>
    private static bool FurnitureDrawPrefix(Furniture __instance, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y,
        float alpha = 1f)
    {
        if (__instance.heldObject.Value is not
            { ParentSheetIndex: Globals.MEAD_INDEX_I, preservedParentSheetIndex.Value: > 0 } mead) return true; // run original logic

        var got = GetDrawInfo(__instance, out var spritesheet, out var sourceRectangle, out _);
        if (!got) return true; // run original logic

        // draw the furniture
        if (x == -1)
        {
            spriteBatch.Draw(
                texture: Furniture.furnitureTexture,
                position: Game1.GlobalToLocal(Game1.viewport, ___drawPosition),
                sourceRectangle: __instance.sourceRect.Value,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }
        else
        {
            spriteBatch.Draw(
                texture: Furniture.furnitureTexture,
                position: Game1.GlobalToLocal(Game1.viewport,
                    globalPosition: new Vector2(
                        x: x * 64,
                        y: y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height))),
                sourceRectangle: __instance.sourceRect.Value,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }

        // draw shadow
        spriteBatch.Draw(
            texture: Game1.shadowTexture,
            position: Game1.GlobalToLocal(Game1.viewport,
                globalPosition: new Vector2(
                    x: __instance.boundingBox.Value.Center.X - 32,
                    y: __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))) +
                new Vector2(32f, 53.3333321f),
            sourceRectangle: Game1.shadowTexture.Bounds,
            color: Color.White * alpha,
            rotation: 0f,
            origin: new(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
            scale: 4f,
            effects: SpriteEffects.None, __instance.boundingBox.Value.Bottom / 10000f
        );

        // draw the held item
        spriteBatch.Draw(
            texture: spritesheet,
            position: Game1.GlobalToLocal(Game1.viewport,
                globalPosition: new Vector2(
                    x: __instance.boundingBox.Value.Center.X - 32,
                    y: __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))),
            sourceRectangle: sourceRectangle,
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (__instance.boundingBox.Value.Bottom + 1) / 10000f
        );

        return false; // run original logic
    }

    /// <summary>Patch to draw BAGI-like meads when held by machines.</summary>
    private static bool ObjectDrawPrefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
        if (__instance.isTemporarilyInvisible) return false; // don't run original logic

        if (!__instance.bigCraftable.Value || !__instance.readyForHarvest.Value || __instance.heldObject.Value is not
            { ParentSheetIndex: Globals.MEAD_INDEX_I, preservedParentSheetIndex.Value: > 0 } mead) return true; // run original logic

        var got = GetDrawInfo(__instance, out var spritesheet, out var sourceRectangle, out _);
        if (!got) return true; // run original logic

        var (sx, sy) = __instance.getScale() * Game1.pixelZoom;

        var (px, py) = Game1.GlobalToLocal(
            viewport: Game1.viewport,
            globalPosition: new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize)
        );

        var destinationRect = new Rectangle(
            (int)(px - sx / 2f) + (__instance.shakeTimer > 0
                ? Game1.random.Next(-1, 2) : 0),
            (int)(py - sy / 2f) + (__instance.shakeTimer > 0
                ? Game1.random.Next(-1, 2) : 0),
            (int)(64f + sx),
            (int)(128f + sy / 2f)
        );

        spriteBatch.Draw(
            texture: Game1.bigCraftableSpriteSheet,
            destinationRectangle: destinationRect,
            sourceRectangle: SObject.getSourceRectForBigCraftable(__instance.showNextIndex.Value
                ? __instance.ParentSheetIndex + 1
                : __instance.ParentSheetIndex),
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f
        );

        var num = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
        spriteBatch.Draw(
            texture: Game1.mouseCursors,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    x * 64 - 8,
                    y * 64 - 96 - 16 + num
                )
            ),
            sourceRectangle: new Rectangle(141, 465, 20, 24),
            color: Color.White * 0.75f,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (float)((y + 1) * 64 / 10000f + 1E-06f + __instance.TileLocation.X / 10000f + 9.99999997475243E-07 + __instance.TileLocation.X / 10000.0)
        );

        spriteBatch.Draw(
            texture: spritesheet,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    x * 64 + 32,
                    y * 64 - 64 - 8 + num
                )
            ),
            sourceRectangle: sourceRectangle,
            color: Color.White * 0.75f,
            rotation: 0f,
            origin: new Vector2(8f, 8f),
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: (float)((y + 1) * 64 / 10000f + __instance.TileLocation.X / 10000f + 9.99999974737875E-06 + __instance.TileLocation.X / 10000.0)
        );

        return false; // don't run original logic
    }

    /// <summary>Patch to draw BAGI-like meads.</summary>
    private static bool ObjectDrawOverloadPrefix(SObject __instance, SpriteBatch spriteBatch, int xNonTile, int yNonTile,
        float layerDepth, float alpha = 1f)
    {
        if (__instance.isTemporarilyInvisible || Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
            return false; // don't run original logic

        if (__instance is not { ParentSheetIndex: Globals.MEAD_INDEX_I, preservedParentSheetIndex.Value: > 0 } mead) return true; // run original logic

        var got = GetDrawInfo(__instance, out var spritesheet, out var sourceRectangle, out _);
        if (!got) return true; // run original logic

        if (__instance.Fragility != 2)
        {
            var shadowTexture = Game1.shadowTexture;
            spriteBatch.Draw(
                texture: shadowTexture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(xNonTile + 32, yNonTile + 51 + 4)
                ),
                sourceRectangle: shadowTexture.Bounds,
                color: Color.White * alpha,
                rotation: 0f,
                origin: new(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: layerDepth - 1E-06f
            );
        }

        spriteBatch.Draw(
            texture: spritesheet,
            position: Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(
                    xNonTile + 32 + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0),
                    yNonTile + 32 + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)
                )
            ),
            sourceRectangle: sourceRectangle,
            color: Color.White * alpha,
            rotation: 0f,
            origin: new Vector2(8f, 8f),
            scale: __instance.Scale.Y > 1f ? __instance.getScale().Y : 4f,
            effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            layerDepth: layerDepth
        );

        return false; // don't run original logic
    }

    /// <summary>Patch to draw BAGI-like meads in menu.</summary>
    private static bool ObjectDrawInMenuPrefix(SObject __instance, SpriteBatch spriteBatch, Vector2 location,
        float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (__instance is not { ParentSheetIndex: Globals.MEAD_INDEX_I, preservedParentSheetIndex.Value: > 0 } mead) return true; // run original logic

        var got = GetDrawInfo(__instance, out var spritesheet, out var sourceRectangle, out _);
        if (!got) return true; // run original logic

        if (drawShadow)
        {
            var shadowTexture = Game1.shadowTexture;
            spriteBatch.Draw(
                texture: shadowTexture,
                position: location + new Vector2(32f, 48f),
                sourceRectangle: shadowTexture.Bounds,
                color: color * 0.5f,
                rotation: 0f,
                origin: new(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                scale: 3f,
                effects: SpriteEffects.None,
                layerDepth: layerDepth - 0.0001f
            );
        }

        spriteBatch.Draw(
            texture: spritesheet,
            position: location + new Vector2(
                (int)(32f * scaleSize),
                (int)(32f * scaleSize)
            ),
            sourceRectangle: sourceRectangle,
            color: color * transparency,
            rotation: 0f,
            origin: new Vector2(8f, 8f) * scaleSize,
            scale: 4f * scaleSize,
            effects: SpriteEffects.None,
            layerDepth: layerDepth
        );

        var shouldDrawStackNumber =
            (drawStackNumber == StackDrawType.Draw && __instance.maximumStackSize() > 1 && __instance.Stack > 1 ||
             drawStackNumber == StackDrawType.Draw_OneInclusive) && scaleSize > 0.3 &&
            __instance.Stack != int.MaxValue;

        if (shouldDrawStackNumber)
        {
            Utility.drawTinyDigits(
                toDraw: __instance.Stack,
                spriteBatch,
                position: location + new Vector2(
                    64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize) + 3f * scaleSize,
                    64f - 18f * scaleSize + 1f
                ),
                scale: 3f * scaleSize,
                layerDepth: 1f,
                color
            );
        }

        if (drawStackNumber != StackDrawType.Hide && __instance.Quality > 0)
        {

            var num = __instance.Quality < 4
                ? 0f
                : ((float)Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f;

            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                position: location + new Vector2(12f, 52f + num),
                sourceRectangle: __instance.Quality < 4
                    ? new(338 + (__instance.Quality - 1) * 8, 400, 8, 8)
                    : new(346, 392, 8, 8),
                color: color * transparency,
                rotation: 0f,
                origin: new(4f, 4f),
                scale: 3f * scaleSize * (1f + num),
                effects: SpriteEffects.None,
                layerDepth: layerDepth
            );
        }

        return false; // don't run original logic
    }

    /// <summary>Patch to draw BAGI-like meads when held.</summary>
    private static bool ObjectDrawWhenHeldPrefix(SObject __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
        if (__instance is not { ParentSheetIndex: Globals.MEAD_INDEX_I, preservedParentSheetIndex.Value: > 0 } mead) return true; // run original logic

        var got = GetDrawInfo(__instance, out var spritesheet, out var sourceRectangle, out _);
        if (!got) return true; // run original logic

        spriteBatch.Draw(
            texture: spritesheet,
            position: objectPosition,
            sourceRectangle: sourceRectangle,
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f)
        );

        if (f.ActiveObject == null || !f.ActiveObject.Name.Contains("=")) return false; // don't run original logic

        spriteBatch.Draw(
            texture: spritesheet,
            position: objectPosition + new Vector2(32f, 32f),
            sourceRectangle: sourceRectangle,
            color: Color.White,
            rotation: 0f,
            origin: new Vector2(32f, 32f),
            scale: 4f + Math.Abs(Game1.starCropShimmerPause) / 8f,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f)
        );

        if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97) return false; // don't run original logic

        Game1.starCropShimmerPause += 0.04f;
        if (Game1.starCropShimmerPause >= 0.8f) Game1.starCropShimmerPause = -0.8f;

        return false; // don't run original logic
    }

    #endregion harmony patches
}