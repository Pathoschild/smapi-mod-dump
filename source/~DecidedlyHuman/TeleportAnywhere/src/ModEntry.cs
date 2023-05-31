/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Input;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TeleportAnywhere.UI;
using TeleportAnywhere.Utilities;
using xTile.Layers;
using xTile.Tiles;

namespace TeleportAnywhere
{
    public class ModEntry : Mod
    {
        private LocationUtils locationUtils;
        private GameLocation startingLocation;
        private Vector2 startingTile;
        // private TeleportUi teleportUi;
        private Logger logger;

        // TODO: Add tile property

        public override void Entry(IModHelper helper)
        {
            this.locationUtils = new LocationUtils(this.Monitor, helper);
            helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
            helper.Events.Player.Warped += this.PlayerOnWarped;
            helper.Events.Display.MenuChanged += this.DisplayOnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += this.GameLoopOnUpdateTicked;
            this.logger = new Logger(this.Monitor, helper.Translation);
            InputEvents.InitInput(helper, (s, level) => { this.Monitor.Log(s, level); });
            // helper.Events.Display.Rendered += DisplayOnRenderedHud;
            Maps.PopulateMapList();
        }

        private void LogCallback(string str)
        {
            this.logger.Log(str, LogLevel.Error);
        }

        private void GameLoopOnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // if (Game1.activeClickableMenu is TeleportUi ui)
            // {
            //     // If the left mouse button is held down, we want to process our hover events.
            //     if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed &&
            //         Game1.oldMouseState.LeftButton == ButtonState.Pressed)
            //     {
            //         ui.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
            //         ui.lef
            //     }
            //     else if (Game1.input.GetMouseState().LeftButton == ButtonState.Released)
            //         ui.(Game1.getMouseX(), Game1.getMouseY());
            // }
        }

        // private void DisplayOnRenderedHud(object? sender, RenderedEventArgs e)
        // {
        //     e.SpriteBatch.Draw(Game1.mouseCursors,
        //         new Vector2(Game1.player.Position.X - Game1.viewport.X, Game1.player.Position.Y - Game1.viewport.Y),
        //         new Rectangle(218, 1322, Game1.player.GetBoundingBox().Width, Game1.player.GetBoundingBox().Height),
        //         Color.White,
        //         0f,
        //         Vector2.Zero,
        //         1f,
        //         SpriteEffects.None,
        //         1f);
        // }

        private void DisplayOnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // if (e.OldMenu is TeleportUi ui && ui.TeleportDone == false)
            //     this.locationUtils.ResetToStartingLocation(this.startingLocation, this.startingTile);
        }

        private void PlayerOnWarped(object? sender, WarpedEventArgs e)
        {
            // InputEvents.RegisterEvent(KeyPressType.Hold, (Keys)SButton.LeftShift, () => { this.logger.Log("Left shift held.");}, this.LogCallback);
            // InputEvents.RegisterEvent(KeyPressType.Released, (Keys)SButton.LeftShift, () => { this.logger.Log("Left shift released.");}, this.LogCallback);
            // InputEvents.RegisterEvent(KeyPressType.Hold, (Keys)SButton.OemPeriod, () => { this.logger.Log("Period held.");}, this.LogCallback);
            // InputEvents.RegisterEvent(KeyPressType.Released, (Keys)SButton.OemPeriod, () => { this.logger.Log("Period released.");}, this.LogCallback);
        }

        private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu != null)
                return;

            if (!Context.IsPlayerFree)
                return;

#if DEBUG

            if (e.IsDown(SButton.OemPipe))
            {
                foreach (Layer layer in Game1.currentLocation.Map.Layers)
                {
                    foreach (Tile? tile in layer.Tiles.Array)
                    {
                        foreach (string? tileProperties in tile.Properties.Keys)
                        {
                            foreach (char property in tileProperties)
                            {
                            }
                        }
                    }
                }
            }

#endif

            if (e.IsDown(SButton.F5))
            {
                // if (Game1.activeClickableMenu != null)
                // {
                //     if (Game1.activeClickableMenu is TeleportUi)
                //         Game1.activeClickableMenu = null;
                // }
                // else
                // {
                //     var ui = new TeleportUi(this.locationUtils, this.Monitor, this.Helper);
                //     ui.Enabled = true;
                //     this.startingLocation = Game1.player.currentLocation;
                //     this.startingTile = Game1.player.getTileLocation();
                //
                //     Game1.activeClickableMenu = ui;
                // }
            }

            if (e.IsDown(SButton.F6))
            {
                // foreach (var render in mapRenders)
                // {
                //     Stream outputFile = File.Create($"C:\\tmp\\maps\\{render.Key}.png");
                //     (render.Value as Texture2D).SaveAsPng(outputFile, render.Value.Width, render.Value.Height);
                // }
            }

            if (e.IsDown(SButton.OemSemicolon))
            {
                // GameLocation location = Maps.GetLocation("ScienceHouse");

                var locations = Maps.GetLocations("o");
            }

            if (e.IsDown(SButton.OemOpenBrackets))
            {
            }

            if (e.IsDown(SButton.OemComma))
            {
            }
        }
    }
}
