/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Display;
using xTile.Tiles;

namespace AlternativeTextures.Framework.Patches.SMAPI
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

            if (AlternativeTextures.modHelper.ModRegistry.IsLoaded("Platonymous.Toolkit"))
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
                    _monitor.Log($"Failed to patch PyTK in {this.GetType().Name}: AT may not display mailbox textures correctly!", LogLevel.Warn);
                    _monitor.Log($"Patch for PyTK failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
            if (AlternativeTextures.modHelper.ModRegistry.IsLoaded("Platonymous.TMXLoader"))
            {
                try
                {
                    if (Type.GetType("TMXLoader.PyDisplayDevice, TMXLoader") is Type PyTK && PyTK != null)
                    {
                        harmony.Patch(AccessTools.Method(PyTK, "DrawTile", new[] { typeof(Tile), typeof(Location), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(PyTKDrawTilePrefix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch TMXLoader in {this.GetType().Name}: AT may not display mailbox textures correctly!", LogLevel.Warn);
                    _monitor.Log($"Patch for TMXLoader failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
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
            if (AlternativeTextures.locationsToMailboxTileSheets.ContainsKey(Game1.currentLocation.Name) is false || AlternativeTextures.locationsToMailboxTileIds.ContainsKey(Game1.currentLocation.Name) is false)
            {
                return true;
            }

            int textureVariation = -1;
            AlternativeTextureModel textureModel = null;
            if (Game1.currentLocation.modData.ContainsKey("AlternativeTextureName.Mailbox"))
            {
                textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(Game1.currentLocation.modData["AlternativeTextureName.Mailbox"]);
                if (textureModel is null || Game1.currentLocation.modData.TryGetValue("AlternativeTextureVariation.Mailbox", out string rawVariationIndex) is false)
                {
                    return true;
                }

                textureVariation = Int32.Parse(rawVariationIndex);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
            }

            if (textureModel is null)
            {
                return true;
            }

            // Handle drawing custom mailbox
            if (AlternativeTextures.locationsToMailboxTileIds[Game1.currentLocation.Name].Contains(tile.TileIndex) is false)
            {
                return true;
            }

            bool hasTileSheet = false;
            foreach (string tileSheet in AlternativeTextures.locationsToMailboxTileSheets[Game1.currentLocation.Name])
            {
                if (tile.TileSheet.ImageSource.Contains(tileSheet))
                {
                    hasTileSheet = true;
                    break;
                }
            }

            if (hasTileSheet)
            {
                if (tile.Layer.Id != "Front")
                {
                    return false;
                }
                ___m_spriteBatchAlpha.Draw(textureModel.GetTexture(textureVariation), new Vector2(location.X, location.Y), new Microsoft.Xna.Framework.Rectangle(0, textureModel.GetTextureOffset(textureVariation), 16, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);

                return false;
            }

            return true;
        }
    }
}