/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Managers;
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

namespace DynamicReflections.Framework.Patches.SMAPI
{
    internal class DisplayDevicePatch : PatchTemplate
    {
        internal DisplayDevicePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method("StardewModdingAPI.Framework.Rendering.SDisplayDevice:DrawTile", new[] { typeof(Tile), typeof(xTile.Dimensions.Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawTilePrefix)));
            harmony.Patch(AccessTools.Method("StardewModdingAPI.Framework.Rendering.SXnaDisplayDevice:DrawTile", new[] { typeof(Tile), typeof(xTile.Dimensions.Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawTilePrefix)));

            // Perform PyTK related patches
            if (DynamicReflections.modHelper.ModRegistry.IsLoaded("Platonymous.Toolkit"))
            {
                try
                {
                    if (Type.GetType("PyTK.Types.PyDisplayDevice, PyTK") is Type PyTK && PyTK != null)
                    {
                        harmony.Patch(AccessTools.Method(PyTK, "DrawTile", new[] { typeof(Tile), typeof(Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(PyTKDrawTilePrefix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch PyTK in {this.GetType().Name}: DR may not properly display reflections!", LogLevel.Warn);
                    _monitor.Log($"Patch for PyTK failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        private static bool PyTKDrawTilePrefix(IDisplayDevice __instance, SpriteBatch ___m_spriteBatchAlpha, ref Vector2 ___m_tilePosition, Tile? tile, Location location, float layerDepth)
        {
            if (tile is null)
            {
                return true;
            }

            return ActualDrawTilePrefix(tile, location, layerDepth, ___m_spriteBatchAlpha, ref ___m_tilePosition);
        }

        private static bool DrawTilePrefix(IDisplayDevice __instance, SpriteBatch ___m_spriteBatchAlpha, Dictionary<TileSheet, Texture2D> ___m_tileSheetTextures, ref Vector2 ___m_tilePosition, Tile? tile, Location location, float layerDepth)
        {
            if (tile is null || ___m_tileSheetTextures is null || tile.TileSheet is null || ___m_tileSheetTextures.ContainsKey(tile.TileSheet) is false)
            {
                return true;
            }

            return ActualDrawTilePrefix(tile, location, layerDepth, ___m_spriteBatchAlpha, ref ___m_tilePosition);
        }

        private static bool ActualDrawTilePrefix(Tile? tile, Location location, float layerDepth, SpriteBatch ___m_spriteBatchAlpha, ref Vector2 ___m_tilePosition)
        {
            if (DynamicReflections.currentWaterSettings.AreReflectionsEnabled is false)
            {
                return true;
            }

            if (DynamicReflections.isFilteringPuddles is true)
            {
                DrawPuddleTile(tile, ref ___m_tilePosition, ___m_spriteBatchAlpha, location, layerDepth);
                return false;
            }

            if (DynamicReflections.isFilteringSky is true)
            {
                DrawSkyTile(tile, ref ___m_tilePosition, ___m_spriteBatchAlpha, location, layerDepth);
                return false;
            }

            if (DynamicReflections.isFilteringStar is true)
            {
                DrawStarTile(tile, ref ___m_tilePosition, ___m_spriteBatchAlpha, location, layerDepth);
                return false;
            }

            if (DynamicReflections.isDrawingWaterReflection is true && tile.TileIndexProperties.TryGetValue("Water", out _) is true)
            {
                return false;
            }
            else if (DynamicReflections.isFilteringWater is true && tile.TileIndexProperties.TryGetValue("Water", out _) is false)
            {
                return false;
            }

            return true;
        }

        private static void DrawSkyTile(Tile tile, ref Vector2 ___m_tilePosition, SpriteBatch ___m_spriteBatchAlpha, Location location, float layerDepth)
        {
            if (tile.Properties.TryGetValue("SkyIndex", out var skyValue) && Int32.TryParse(skyValue, out int skyIndex))
            {
                ___m_tilePosition.X = location.X;
                ___m_tilePosition.Y = location.Y;
                Vector2 origin = new Vector2(8f, 8f);
                ___m_tilePosition.X += origin.X * (float)Layer.zoom;
                ___m_tilePosition.Y += origin.X * (float)Layer.zoom;
                ___m_spriteBatchAlpha.Draw(DynamicReflections.assetManager.NightSkyTileSheetTexture, ___m_tilePosition, new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), Color.White * DynamicReflections.skyAlpha, 0f, origin, Layer.zoom, SpriteEffects.None, layerDepth);
            }
        }

        private static void DrawStarTile(Tile tile, ref Vector2 ___m_tilePosition, SpriteBatch ___m_spriteBatchAlpha, Location location, float layerDepth)
        {
            if (tile.Properties.TryGetValue("SkyIndex", out var skyValue) && Int32.TryParse(skyValue, out int skyIndex) && skyIndex != SkyManager.DEFAULT_SKY_INDEX)
            {
                var tilePoint = SkyManager.GetTilePoint(skyIndex);
                if (tilePoint.X == 0 && tilePoint.Y == 0)
                {
                    return;
                }

                int effectIndex = 0;
                if (tile.Properties.TryGetValue("SkyEffect", out var skyEffectValue) && Int32.TryParse(skyEffectValue, out int skyEffect))
                {
                    effectIndex = skyEffect;
                }

                float alpha = 1f;
                if (tile.Properties.TryGetValue("SkyAlpha", out var skyAlphaValue) && float.TryParse(skyAlphaValue, out float skyAlpha))
                {
                    alpha = skyAlpha;
                }

                ___m_tilePosition.X = location.X;
                ___m_tilePosition.Y = location.Y;
                Vector2 origin = new Vector2(8f, 8f);
                ___m_tilePosition.X += origin.X * (float)Layer.zoom;
                ___m_tilePosition.Y += origin.X * (float)Layer.zoom;
                ___m_spriteBatchAlpha.Draw(DynamicReflections.assetManager.NightSkyTileSheetTexture, ___m_tilePosition, new Microsoft.Xna.Framework.Rectangle(tilePoint.X * 16, tilePoint.Y * 16, 16, 16), Color.White * alpha, 0f, origin, Layer.zoom, (SpriteEffects)effectIndex, layerDepth);
            }
        }

        private static void DrawPuddleTile(Tile tile, ref Vector2 ___m_tilePosition, SpriteBatch ___m_spriteBatchAlpha, Location location, float layerDepth)
        {
            if (tile.Properties.TryGetValue("PuddleIndex", out var puddleValue) && Int32.TryParse(puddleValue, out int puddleIndex) && puddleIndex != PuddleManager.DEFAULT_PUDDLE_INDEX)
            {
                var tileXOffset = 0;
                var tileYOffset = puddleIndex * 16;
                if (tile.Properties.TryGetValue("BigPuddleIndex", out var bigPuddleValue) && Int32.TryParse(bigPuddleValue, out int bigPuddleIndex) && bigPuddleIndex != PuddleManager.DEFAULT_PUDDLE_INDEX)
                {
                    tileXOffset = bigPuddleIndex * 16;
                }

                int effectIndex = 0;
                if (tile.Properties.TryGetValue("PuddleEffect", out var puddleEffectValue) && Int32.TryParse(puddleEffectValue, out int puddleEffect))
                {
                    effectIndex = puddleEffect;
                }

                float rotation = 0f;
                if (tile.Properties.TryGetValue("PuddleRotation", out var puddleRotationValue) && float.TryParse(puddleRotationValue, out float puddleRotation))
                {
                    rotation = puddleRotation;
                }

                ___m_tilePosition.X = location.X;
                ___m_tilePosition.Y = location.Y;
                Vector2 origin = new Vector2(8f, 8f);
                ___m_tilePosition.X += origin.X * (float)Layer.zoom;
                ___m_tilePosition.Y += origin.X * (float)Layer.zoom;
                ___m_spriteBatchAlpha.Draw(DynamicReflections.assetManager.PuddlesTileSheetTexture, ___m_tilePosition, new Microsoft.Xna.Framework.Rectangle(tileXOffset, tileYOffset, 16, 16), DynamicReflections.currentPuddleSettings.PuddleColor, rotation, origin, Layer.zoom, (SpriteEffects)effectIndex, layerDepth);
            }
        }
    }
}
