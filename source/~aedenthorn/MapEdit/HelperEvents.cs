/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using xTile.Layers;
using xTile.Tiles;

namespace MapEdit
{
    public class HelperEvents
    {
        public static ModConfig Config;
        public static IModHelper Helper;
        public static IMonitor Monitor;

        public static void Initialize(ModConfig config, IMonitor monitor, IModHelper helper)
        {
            Config = config;
            Helper = helper;
            Monitor = monitor;
        }
        public static void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ModActions.DeactivateMod();
            ModEntry.cleanMaps.Clear();
        }

        public static void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            MapActions.GetMapCollectionData();
        }


        public static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.modActive && (Helper.Input.IsDown(Config.PasteButton) || Helper.Input.IsSuppressed(Config.PasteButton)) && ModEntry.pastedTileLoc.X > -1 && ModEntry.pastedTileLoc != Game1.currentCursorTile)
            {
                TileActions.PasteCurrentTile();
            }
            else if (ModEntry.modActive && (Helper.Input.IsDown(Config.RevertButton) || Helper.Input.IsSuppressed(Config.RevertButton)) && MapActions.MapHasTile(Game1.currentCursorTile))
            {
                TileActions.RevertCurrentTile();
            }
        }


        public static void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (!Config.EnableMod)
                return;
            ModActions.DeactivateMod();
            MapActions.UpdateCurrentMap(false);
        }

        public static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Config.EnableMod || !Context.IsPlayerFree)
            {
                ModActions.DeactivateMod();
                return;
            }

            if (e.Button == Config.ToggleButton)
            {
                Helper.Input.Suppress(e.Button);
                ModEntry.modActive = !ModEntry.modActive;
                ModEntry.copiedTileLoc = new Vector2(-1, -1);
                ModEntry.currentTileDict.Clear();
                Monitor.Log($"Toggled mod: {ModEntry.modActive}");
                if (ModEntry.modActive)
                    ModActions.ShowMessage(string.Format(Helper.Translation.Get("mod-active"), Config.ToggleButton));
                else
                    ModActions.ShowMessage(string.Format(Helper.Translation.Get("mod-inactive"), Config.ToggleButton));
            }
            else if (ModEntry.modActive && e.Button == Config.CopyButton)
            {
                Helper.Input.Suppress(e.Button);

                TileActions.CopyCurrentTile();

            }
            else if (ModEntry.modActive && ModEntry.copiedTileLoc.X > -1 && e.Button == Config.PasteButton && ModEntry.pastedTileLoc != Game1.currentCursorTile)
            {
                Helper.Input.Suppress(e.Button);
                TileActions.PasteCurrentTile();

            }
            else if (ModEntry.modActive && e.Button == Config.RevertButton && MapActions.MapHasTile(Game1.currentCursorTile))
            {
                Helper.Input.Suppress(e.Button);
                TileActions.RevertCurrentTile();
            }
            else if (ModEntry.modActive && e.Button == SButton.Escape)
            {
                Helper.Input.Suppress(e.Button);
                if (ModEntry.copiedTileLoc.X > -1)
                {
                    ModEntry.copiedTileLoc = new Vector2(-1, -1);
                    ModEntry.pastedTileLoc = new Vector2(-1, -1);
                    ModEntry.currentLayer = 0;
                    ModEntry.currentTileDict.Clear();
                }
                else
                    ModActions.DeactivateMod();
            }
            else if (ModEntry.modActive && e.Button == Config.RefreshButton)
            {
                Helper.Input.Suppress(e.Button);
                ModEntry.cleanMaps.Clear();
                MapActions.GetMapCollectionData();
                MapActions.UpdateCurrentMap(true);
            }
            else if (ModEntry.modActive && e.Button == Config.ScrollUpButton)
            {
                Helper.Input.Suppress(e.Button);
                ModActions.SwitchTile(true);
            }
            else if (ModEntry.modActive && e.Button == Config.ScrollDownButton)
            {
                Helper.Input.Suppress(e.Button);
                ModActions.SwitchTile(false);
            }
        }

        public static void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Config.EnableMod || !ModEntry.modActive)
                return;

            if (Game1.activeClickableMenu != null)
            {
                ModEntry.modActive = false;
                return;
            }

            Vector2 mouseTile = Game1.currentCursorTile;
            Vector2 mouseTilePos = mouseTile * Game1.tileSize - new Vector2(Game1.viewport.X, Game1.viewport.Y);
            if (ModEntry.copiedTileLoc.X > -1)
            {
                foreach (var kvp in ModEntry.currentTileDict)
                {
                    int offset = kvp.Key.Equals("Front") ? (16 * Game1.pixelZoom) : 0;
                    float layerDepth = (ModEntry.copiedTileLoc.Y * (16 * Game1.pixelZoom) + 16 * Game1.pixelZoom + offset) / 10000f;
                    Tile tile = kvp.Value;
                    if (tile == null)
                        continue;

                    var xRect = tile.TileSheet.GetTileImageBounds(tile.TileIndex);
                    Rectangle sourceRectangle = new Rectangle(xRect.X, xRect.Y, xRect.Width, xRect.Height);

                    Texture2D texture2D = null;
                    try
                    {
                        Helper.Reflection.GetField<Dictionary<TileSheet, Texture2D>>(Game1.mapDisplayDevice, "m_tileSheetTextures", true)?.GetValue()?.TryGetValue(tile.TileSheet, out texture2D);
                    }
                    catch
                    {
                        Helper.Reflection.GetField<Dictionary<TileSheet, Texture2D>>(Game1.mapDisplayDevice, "m_tileSheetTextures2", false)?.GetValue()?.TryGetValue(tile.TileSheet, out texture2D);
                    }
                    if (texture2D != null)
                        e.SpriteBatch.Draw(texture2D, mouseTilePos, sourceRectangle, Color.White, 0f, Vector2.Zero, Layer.zoom, SpriteEffects.None, layerDepth);
                }
                e.SpriteBatch.Draw(ModEntry.copiedTexture, mouseTilePos, Color.White);

            }
            else if (MapActions.MapHasTile(mouseTile))
                e.SpriteBatch.Draw(ModEntry.existsTexture, mouseTilePos, Color.White);
            else
                e.SpriteBatch.Draw(ModEntry.activeTexture, mouseTilePos, Color.White);

        }
    }
}