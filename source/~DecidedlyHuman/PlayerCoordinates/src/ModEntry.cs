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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerCoordinates.Helpers;
using PlayerCoordinates.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PlayerCoordinates
{
    public class ModEntry : Mod
    {
        private Texture2D _coordinateBox;
        private ModConfig _config;
        private bool _showHud = true;
        private bool _hudLocked = true;
        private bool _trackingCursor;
        private string _currentMapName;
        private string _modDirectory;
        private Coordinates _currentCoords;
        private Coordinates _currentCursorCoords;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.Display.RenderedHud += this.DrawCoordinates;
            helper.Events.Display.RenderedWorld +=
                this.UpdateCurrentMap; // Wow. This is wildly unnecessarily. I don't need to do this every time the world is RENDERED. TOOD: Stop being dumb, and change this.
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
            helper.Events.Input.ButtonReleased += this.ButtonReleased;
            helper.Events.Display.WindowResized += this.WindowResized;

            // Since I can't think of a way to log co-ordinates on Android, a toggle to make the HUD movable is
            // also out of the question until I figure that out.
#if !ANDROID
			helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
#endif
            this._modDirectory = this.Helper.DirectoryPath;

            // The default source is the current mod folder, so we don't need to specify that here.
            this._coordinateBox = helper.Content.Load<Texture2D>("assets/box.png");
        }

        private void WindowResized(object sender, WindowResizedEventArgs e)
        {
        }

#if !ANDROID // Game1.options does not exist on Android, and SMAPI will kill the mod if we use it.
		private void UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!this._hudLocked)
			{
                this._config.HudXCoord = Game1.getMouseX();
                this._config.HudYCoord = Game1.getMouseY();
			}
		}
#endif

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this._config = this.Helper.ReadConfig<ModConfig>();

            try // This way, we avoid logging an exception if the user doesn't have GMCM installed.
            {
                this.RegisterWithGmcm();
            }
            catch (Exception ex)
            {
                this.Monitor.Log("User doesn't appear to have GMCM installed. Not an error.", LogLevel.Info);
            }
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            configMenuApi.RegisterModConfig(this.ModManifest,
                () => this._config = new ModConfig(),
                () => this.Helper.WriteConfig(this._config));

            configMenuApi.RegisterSimpleOption(this.ModManifest, "Toggle Tracking Target",
                "Whether or not you want the log to specify whether the player or cursor was the source of this co-ordinate.",
                () => this._config.LogTrackingTarget,
                value => this._config.LogTrackingTarget = value);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD Toggle",
                "The key used to toggle the co-ordinate HUD.",
                () => this._config.CoordinateHUDToggle,
                button => this._config.CoordinateHUDToggle = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "Log Co-ordinates",
                "The key used to log the current co-ordinates to file.",
                () => this._config.LogCoordinates,
                button => this._config.LogCoordinates = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "Toggle Tracking Target",
                "The key used to toggle between tracking the cursor and the player's co-ordinates.",
                () => this._config.SwitchToCursorCoords,
                button => this._config.SwitchToCursorCoords = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD X Co-ordinate:",
                "The X co-ordinate of the HUD",
                () => this._config.HudXCoord,
                value => this._config.HudXCoord = value);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD Y Co-ordinate:",
                "The Y co-ordinate of the HUD",
                () => this._config.HudYCoord,
                value => this._config.HudYCoord = value);

            // Since I can't think of a way to log co-ordinates on Android, a toggle to make the HUD movable is
            // also out of the question until I figure that out.
#if ANDROID
#else
			configMenuApi.RegisterSimpleOption(this.ModManifest, "Unlock HUD Position",
				"Press this key to toggle the HUD to be unlocked so you can move it around.",
				() => this._config.HudUnlock,
				(button) => this._config.HudUnlock = button);
#endif
        }

        private void ButtonPressed(object o, ButtonPressedEventArgs button)
        {
            var b = button.Button;

            if (b == this._config.CoordinateHUDToggle) this._showHud = !this._showHud;

            if (b == this._config.SwitchToCursorCoords) this._trackingCursor = !this._trackingCursor;

            if (b == this._config.LogCoordinates) this.LogCurrentCoordinates();

            if (b == this._config.HudUnlock) this._hudLocked = false;
        }

        private void ButtonReleased(object sender, ButtonReleasedEventArgs button)
        {
            var b = button.Button;

            if (b == this._config.HudUnlock)
            {
                this._hudLocked = true;
                this.Helper.WriteConfig(this._config);
            }
        }

        private void UpdateCurrentMap(object o, RenderedWorldEventArgs world)
        {
            string newMap = Game1.player.currentLocation.Name;

            if (newMap == string.Empty)
                newMap = "Unnamed Map";

            this._currentMapName = newMap;
        }

        private void UpdateCurrentCoordinates()
        {
            // Track the cursor instead of the player if appropriate.
            this._currentCoords = this._trackingCursor ? Game1.currentCursorTile : Game1.player.getTileLocation();
        }

        private void LogCurrentCoordinates()
        {
            if (!this._showHud ||
                !Context.IsWorldReady) // Look into why Context.IsWorldReady return true on title screen?
                return;

            string finalPath = Path.Combine(this._modDirectory, "coordinate_output.txt");

            // This is bad, and I need to split things up better. At some point. For now, this is fine.
            var file = new CoordinateLogger(finalPath, this._currentCoords, this._currentMapName, this._trackingCursor,
                this._config.LogTrackingTarget, this.Monitor);

            if (file.LogCoordinates()) // Try to log the co-ordinates, and determine whether or not we were successful.
            {
                Game1.showGlobalMessage("Co-ordinates logged.\r\n" +
                                        "Check the mod folder!");
                Logger.LogMessage(this.Monitor, LogLevel.Info,
                    $"Co-ordinates ({this._currentMapName}: {this._currentCoords}) logged successfully.");
            }
            else
            {
                Game1.showGlobalMessage("Failed to save co-ordinates.\r\n" +
                                        "Check your SMAPI log for details.");
                Logger.LogMessage(this.Monitor, LogLevel.Error,
                    $"Failed to log co-ordinates ({this._currentMapName}: {this._currentCoords}). Exception follows:");
            }
        }

        private void DrawCoordinates(object o, RenderedHudEventArgs world)
        {
            if (!Context.IsWorldReady)
                return;

            if (!this._showHud)
                return;


            // We only need to update our co-ordinates if we're drawing the HUD. Maybe make this an option?
            this.UpdateCurrentCoordinates();

            // Everything below this point is messy and terrible, and I am a bad person for doing so.
            // It works, but do not do what I do.
            // TODO: Make everything below here not terrible.

            DrawHud.Draw(new Vector2(this._config.HudXCoord, this._config.HudYCoord), this._currentCoords,
                this._coordinateBox, world);

#if ANDROID
            // Do absolutely nothing.
#else
			//Draw a rectangle around the cursor position when tracking the cursor.
			if (this._trackingCursor)
			{
				if (this._currentCoords.y < 0 || this._currentCoords.x < 0)
					return; // We don't want to draw our tile rectangle if the cursor coordinates are negative.
							// Also, it's buggy.

				float zoomLevel = Game1.options.zoomLevel;

				world.SpriteBatch.Draw(
					Game1.mouseCursors,
					Game1.GlobalToLocal(
						Game1.viewport,
						new Vector2(this._currentCoords.x, this._currentCoords.y) * Game1.tileSize) * zoomLevel,
					Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
					Color.White,
					0.0f,
					Vector2.Zero,
					zoomLevel,
					SpriteEffects.None,
					0f);
			}
#endif
        }
    }
}
