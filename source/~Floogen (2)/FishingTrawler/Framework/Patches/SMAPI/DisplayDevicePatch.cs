/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.GameLocations;
using FishingTrawler.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;

namespace FishingTrawler.Framework.Patches.SMAPI
{
    internal class DisplayDevicePatch : PatchTemplate
    {
        internal static float? Opacity { get; set; }

        internal DisplayDevicePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method("StardewModdingAPI.Framework.Rendering.SDisplayDevice:DrawTile", new[] { typeof(Tile), typeof(xTile.Dimensions.Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawTilePrefix)));
            harmony.Patch(AccessTools.Method("StardewModdingAPI.Framework.Rendering.SXnaDisplayDevice:DrawTile", new[] { typeof(Tile), typeof(xTile.Dimensions.Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawTilePrefix)));
        }

        private static bool DrawTilePrefix(IDisplayDevice __instance, SpriteBatch ___m_spriteBatchAlpha, Dictionary<TileSheet, Texture2D> ___m_tileSheetTextures, Color ___m_modulationColour, ref Vector2 ___m_tilePosition, ref Microsoft.Xna.Framework.Rectangle ___m_sourceRectangle, Tile? tile, Location location, float layerDepth)
        {
            if (tile is null || ___m_tileSheetTextures is null || tile.TileSheet is null || ___m_tileSheetTextures.ContainsKey(tile.TileSheet) is false)
            {
                return true;
            }

            if (Game1.currentLocation is not TrawlerHull)
            {
                return true;
            }

            // Handle drawing opacity
            if (Opacity is not null)
            {
                xTile.Dimensions.Rectangle tileImageBounds = tile!.TileSheet.GetTileImageBounds(tile!.TileIndex);
                Texture2D tileSheetTexture = ___m_tileSheetTextures[tile!.TileSheet];
                if (!tileSheetTexture.IsDisposed)
                {
                    ___m_tilePosition.X = location.X;
                    ___m_tilePosition.Y = location.Y;
                    ___m_sourceRectangle.X = tileImageBounds.X;
                    ___m_sourceRectangle.Y = tileImageBounds.Y;
                    ___m_sourceRectangle.Width = tileImageBounds.Width;
                    ___m_sourceRectangle.Height = tileImageBounds.Height;
                    float rotation = GetRotation(tile);
                    SpriteEffects effects = GetSpriteEffects(tile);

                    Vector2 origin = new Vector2((float)tileImageBounds.Width / 2f, (float)tileImageBounds.Height / 2f);
                    ___m_tilePosition.X += origin.X * (float)Layer.zoom;
                    ___m_tilePosition.Y += origin.X * (float)Layer.zoom;
                    ___m_spriteBatchAlpha.Draw(tileSheetTexture, ___m_tilePosition, ___m_sourceRectangle, ___m_modulationColour * Opacity.Value, rotation, origin, Layer.zoom, effects, layerDepth);
                }

                return false;
            }

            return true;
        }

        // The following methods are snipped from StardewModdingAPI.Framework.Rendering.SDisplayDevice
        private static float GetRotation(Tile tile)
        {
            if (!tile.Properties.TryGetValue("@Rotation", out var propertyValue) || !int.TryParse((string)propertyValue, out var value))
            {
                return 0f;
            }
            value %= 360;
            if (value == 0)
            {
                return 0f;
            }
            return (float)(Math.PI / (180.0 / (double)value));
        }

        private static SpriteEffects GetSpriteEffects(Tile tile)
        {
            if (!tile.Properties.TryGetValue("@Flip", out var propertyValue) || !int.TryParse((string)propertyValue, out var value))
            {
                return SpriteEffects.None;
            }
            return (SpriteEffects)value;
        }

    }
}