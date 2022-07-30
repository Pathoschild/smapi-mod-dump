/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace DynamicReflections.Framework.Patches.Tiles
{
    internal class LayerPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Layer);
        private static Color _waterColor;

        internal LayerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Layer.Draw), new[] { typeof(IDisplayDevice), typeof(xTile.Dimensions.Rectangle), typeof(xTile.Dimensions.Location), typeof(bool), typeof(int) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Layer.Draw), new[] { typeof(IDisplayDevice), typeof(xTile.Dimensions.Rectangle), typeof(xTile.Dimensions.Location), typeof(bool), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPostfix)));

            harmony.CreateReversePatcher(AccessTools.Method(_object, nameof(Layer.Draw), new[] { typeof(IDisplayDevice), typeof(xTile.Dimensions.Rectangle), typeof(xTile.Dimensions.Location), typeof(bool), typeof(int) }), new HarmonyMethod(GetType(), nameof(DrawReversePatch))).Patch();

            // Perform PyTK related patches
            if (DynamicReflections.modHelper.ModRegistry.IsLoaded("Platonymous.Toolkit"))
            {
                try
                {
                    if (Type.GetType("PyTK.Extensions.PyMaps, PyTK") is Type PyTK && PyTK != null)
                    {
                        harmony.Patch(AccessTools.Method(PyTK, "drawLayer", new[] { typeof(Layer), typeof(IDisplayDevice), typeof(xTile.Dimensions.Rectangle), typeof(int), typeof(Location), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(PyTKDrawLayerPrefix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch PyTK in {this.GetType().Name}: DR may not properly display reflections!", LogLevel.Warn);
                    _monitor.Log($"Patch for PyTK failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        private static bool DrawPrefix(Layer __instance, IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, Location displayOffset, bool wrapAround, int pixelZoom)
        {
            if (__instance is null || String.IsNullOrEmpty(__instance.Id))
            {
                return true;
            }

            DynamicReflections.isDrawingPuddles = false;
            DynamicReflections.isDrawingWaterReflection = false;
            DynamicReflections.isDrawingMirrorReflection = false;

            if (__instance.Id.Equals("Back", StringComparison.OrdinalIgnoreCase) is true)
            {
                SpriteBatchToolkit.CacheSpriteBatchSettings(Game1.spriteBatch, endSpriteBatch: true);

                // Pre-render the Mirrors layer (this should always be done, regardless of DynamicReflections.shouldDrawMirrorReflection)
                SpriteBatchToolkit.RenderMirrorsLayer();
                if (DynamicReflections.shouldDrawMirrorReflection is true)
                {
                    // Pre-render the mirror reflections
                    DynamicReflections.isFilteringMirror = true;
                    SpriteBatchToolkit.RenderMirrorReflectionPlayerSprite();
                    DynamicReflections.isFilteringMirror = false;
                }

                // Handle preliminary water reflection logic
                if (DynamicReflections.shouldDrawWaterReflection is true)
                {
                    DynamicReflections.isFilteringWater = true;
                    SpriteBatchToolkit.RenderWaterReflectionPlayerSprite();
                }

                // Handle preliminary NPC reflection logic
                if (DynamicReflections.modConfig.AreNPCReflectionsEnabled is true)
                {
                    DynamicReflections.isFilteringWater = true;
                    SpriteBatchToolkit.RenderWaterReflectionNPCs();
                    SpriteBatchToolkit.RenderPuddleReflectionNPCs();
                }

                _waterColor = Game1.currentLocation.waterColor.Value;
                if (DynamicReflections.modConfig.AreSkyReflectionsEnabled is true)
                {
                    if (DynamicReflections.shouldDrawNightSky)
                    {
                        DynamicReflections.isFilteringSky = true;
                        Game1.currentLocation.waterColor.Value = new Color(60, 240, 255) * DynamicReflections.waterAlpha;
                        SpriteBatchToolkit.RenderWaterReflectionNightSky();
                    }
                }

                // Handle preliminary puddles reflection and draw logic
                if (DynamicReflections.currentPuddleSettings.ShouldGeneratePuddles is true)
                {
                    DynamicReflections.isFilteringPuddles = true;
                    SpriteBatchToolkit.RenderPuddles();
                    DynamicReflections.isFilteringPuddles = false;
                    DynamicReflections.isDrawingPuddles = true;
                }
                if (DynamicReflections.shouldDrawPuddlesReflection is true)
                {
                    DynamicReflections.isFilteringPuddles = true;
                    SpriteBatchToolkit.RenderPuddleReflectionPlayerSprite();
                    DynamicReflections.isFilteringPuddles = false;
                }

                // Resume previous SpriteBatch
                SpriteBatchToolkit.ResumeCachedSpriteBatch(Game1.spriteBatch);

                // Draw the filtered layer, if needed
                SpriteBatchToolkit.HandleBackgroundDraw();
                if (DynamicReflections.isFilteringWater is false && DynamicReflections.isFilteringSky is false)
                {
                    return true;
                }

                // Handle Visible Fish Compatability
                LayerPatch.DrawReversePatch(__instance, displayDevice, mapViewport, displayOffset, wrapAround, pixelZoom);
                DynamicReflections.shouldSkipWaterOverlay = true;
                Game1.currentLocation.drawWater(Game1.spriteBatch);
                DynamicReflections.shouldSkipWaterOverlay = false;

                // Draw the sky reflection
                if (DynamicReflections.shouldDrawNightSky is true)
                {
                    SpriteBatchToolkit.CacheSpriteBatchSettings(Game1.spriteBatch, endSpriteBatch: true);

                    SpriteBatchToolkit.DrawNightSky();

                    if (DynamicReflections.isFilteringWater is true)
                    {
                        DynamicReflections.isFilteringWater = false;
                        DynamicReflections.isDrawingWaterReflection = true;
                    }

                    // Resume previous SpriteBatch
                    SpriteBatchToolkit.ResumeCachedSpriteBatch(Game1.spriteBatch);
                }
                else if (DynamicReflections.isFilteringWater is true) // Draw the water reflection, if sky reflections aren't currently active
                {
                    SpriteBatchToolkit.CacheSpriteBatchSettings(Game1.spriteBatch, endSpriteBatch: true);

                    SpriteBatchToolkit.DrawRenderedCharacters(isWavy: DynamicReflections.currentWaterSettings.IsReflectionWavy);

                    DynamicReflections.isFilteringWater = false;
                    DynamicReflections.isDrawingWaterReflection = true;

                    // Resume previous SpriteBatch
                    SpriteBatchToolkit.ResumeCachedSpriteBatch(Game1.spriteBatch);
                }
            }
            else if (__instance.Id.Equals("Buildings", StringComparison.OrdinalIgnoreCase) is true)
            {
                Game1.currentLocation.waterColor.Value = _waterColor;

                // Draw the cached Mirrors layer
                Game1.spriteBatch.Draw(DynamicReflections.mirrorsLayerRenderTarget, Vector2.Zero, Color.White);

                // Skip drawing the player's reflection if not needed
                if (DynamicReflections.shouldDrawMirrorReflection is true)
                {
                    SpriteBatchToolkit.CacheSpriteBatchSettings(Game1.spriteBatch, endSpriteBatch: true);

                    //DynamicReflections.isDrawingMirrorReflection = true;
                    SpriteBatchToolkit.DrawMirrorReflection(DynamicReflections.mirrorsLayerRenderTarget);

                    // Resume previous SpriteBatch
                    SpriteBatchToolkit.ResumeCachedSpriteBatch(Game1.spriteBatch);
                }
            }

            return true;
        }

        private static void DrawPostfix(Layer __instance, IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, Location displayOffset, bool wrapAround, int pixelZoom)
        {
            if (__instance is null || String.IsNullOrEmpty(__instance.Id))
            {
                return;
            }

            if (__instance.Id.Equals("Back", StringComparison.OrdinalIgnoreCase) is true)
            {
                if (DynamicReflections.isDrawingPuddles is true)
                {
                    // Draw the puddles ontop of the "Back" layer
                    DynamicReflections.isFilteringPuddles = true;
                    LayerPatch.DrawReversePatch(__instance, displayDevice, mapViewport, displayOffset, wrapAround, pixelZoom);
                    DynamicReflections.isFilteringPuddles = false;

                    SpriteBatchToolkit.CacheSpriteBatchSettings(Game1.spriteBatch, endSpriteBatch: true);

                    // Draw puddle reflection
                    SpriteBatchToolkit.DrawPuddleReflection(DynamicReflections.puddlesRenderTarget);

                    // Resume previous SpriteBatch
                    SpriteBatchToolkit.ResumeCachedSpriteBatch(Game1.spriteBatch);
                }
            }
        }

        internal static void DrawReversePatch(Layer __instance, IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, Location displayOffset, bool wrapAround, int pixelZoom)
        {
            new NotImplementedException("It's a stub!");
        }

        // PyTK related patches
        private static void PyTKDrawLayerPrefix(Layer __instance, xTile.Display.IDisplayDevice device, xTile.Dimensions.Rectangle viewport, int pixelZoom, Location offset, bool wrap = false)
        {
            DrawPrefix(__instance, device, viewport, offset, wrap, pixelZoom);
        }
    }
}
