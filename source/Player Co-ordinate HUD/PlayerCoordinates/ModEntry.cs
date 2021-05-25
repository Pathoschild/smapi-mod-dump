/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewMods
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerCoordinates.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PlayerCoordinates
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private Texture2D _coordinateBox;
        private ModConfig _config;
        private bool _showHud = true;
        private bool _trackingCursor = false;
        private string _currentMapName;
        private string _modDirectory;
        private Coordinates _currentCoords;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Display.RenderedHud += DrawCoordinates;
            helper.Events.Display.RenderedWorld += UpdateCurrentMap;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            _modDirectory = Helper.DirectoryPath;

            // The default source is the current mod folder, so we don't need to specify that here.
            _coordinateBox = helper.Content.Load<Texture2D>("assets/box.png");
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this._config = this.Helper.ReadConfig<ModConfig>();

            try // This way, we avoid logging an exception if the user doesn't have GMCM installed.
            {
                RegisterWithGmcm();
            }
            catch (Exception ex)
            {
            }
        }

        private void RegisterWithGmcm()
        {
            GenericModConfigMenuAPI configMenuApi =
                Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            configMenuApi.RegisterModConfig(ModManifest,
                () => _config = new ModConfig(),
                () => Helper.WriteConfig(_config));

            configMenuApi.RegisterSimpleOption(ModManifest, "HUD Toggle",
                "The key used to toggle the co-ordinate HUD.",
                () => _config.CoordinateHUDToggle,
                (SButton button) => _config.CoordinateHUDToggle = button);
            configMenuApi.RegisterSimpleOption(ModManifest, "Log Co-ordinates",
                "The key used to log the current co-ordinates to file.",
                () => _config.LogCoordinates,
                (SButton button) => _config.LogCoordinates = button);
            configMenuApi.RegisterSimpleOption(ModManifest, "Toggle Tracking Target",
                "The key used to toggle between tracking the cursor and the player's co-ordinates.",
                () => _config.SwitchToCursorCoords,
                (SButton button) => _config.SwitchToCursorCoords = button);
        }

        private void ButtonPressed(object o, ButtonPressedEventArgs button)
        {
            SButton b = button.Button;

            if (b == _config.CoordinateHUDToggle)
                _showHud = !_showHud;

            if (b == _config.SwitchToCursorCoords)
                _trackingCursor = !_trackingCursor;

            if (b == _config.LogCoordinates)
                LogCurrentCoordinates();
        }

        private void UpdateCurrentMap(object o, RenderedWorldEventArgs world)
        {
            string newMap = Game1.player.currentLocation.Name;

            if (newMap == String.Empty)
                newMap = "Unnamed Map";
            else
                _currentMapName = newMap;
        }

        private void UpdateCurrentCoordinates()
        {
            // Track the cursor instead of the player if appropriate.
            _currentCoords = (_trackingCursor) ? Game1.currentCursorTile : Game1.player.getTileLocation();
        }

        private void LogCurrentCoordinates()
        {
            if (!_showHud)
                return;

            string finalPath = Path.Combine(_modDirectory, "coordinate_output.txt");

            // This is bad, and I need to split things up better. At some point. For now, this is fine.
            FileHandler file = new FileHandler(finalPath, _currentCoords, _currentMapName, Monitor);

            if (file.LogCoordinates()) // Try to log the co-ordinates, and determine whether or not we were successful.
                Game1.showGlobalMessage($"Co-ordinates logged.\r\n" +
                                        $"Check the mod folder!");
            else
                Game1.showGlobalMessage($"Failed to save co-ordinates.\r\n" +
                                        $"Check your SMAPI log for details.");
        }

        private void DrawCoordinates(object o, RenderedHudEventArgs world)
        {
            if (!Context.IsWorldReady)
                return;

            if (!_showHud)
                return;

            // We only need to update our co-ordinates if we're drawing the HUD. Maybe make this an option?
            UpdateCurrentCoordinates();

            // Everything below this point is messy and terrible, and I am a bad person for doing so.
            // It works, but do not do what I do.
            // TODO: Make everything below here not terrible.
            Vector2 topTextPosition = new Vector2(23, 17);
            Vector2 bottomTextPosition = new Vector2(topTextPosition.X, topTextPosition.Y + 36);

            world.SpriteBatch.Draw(_coordinateBox, new Vector2(9, 9), Color.White);
            Utility.drawTextWithShadow(world.SpriteBatch,
                $"X: {_currentCoords.x}",
                Game1.dialogueFont,
                topTextPosition,
                Color.Black);
            Utility.drawTextWithShadow(world.SpriteBatch,
                $"Y: {_currentCoords.y}",
                Game1.dialogueFont,
                bottomTextPosition,
                Color.Black);

            //Draw a rectangle around the cursor position when tracking the cursor.
            if (_trackingCursor)
            {
                if (_currentCoords.y < 0 || _currentCoords.x < 0)
                    return; // We don't want to draw our tile rectangle if the cursor coordinates are negative.

                world.SpriteBatch.Draw(
                    Game1.mouseCursors,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        new Vector2(_currentCoords.x, _currentCoords.y) * Game1.tileSize) * Game1.options.zoomLevel,
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Game1.options.zoomLevel,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}