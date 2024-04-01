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
        private Texture2D coordinateBox;
        private ModConfig config;
        private bool showHud = true;
        private bool hudLocked = true;
        private bool trackingCursor;
        private string currentMapName;
        private string modDirectory;
        private Coordinates currentCoords;
        private Coordinates currentCursorCoords;

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
            this.modDirectory = this.Helper.DirectoryPath;

            // The default source is the current mod folder, so we don't need to specify that here.
            this.coordinateBox = helper.ModContent.Load<Texture2D>("assets/box.png");
        }

        private void WindowResized(object sender, WindowResizedEventArgs e)
        {
        }

#if !ANDROID // Game1.options does not exist on Android, and SMAPI will kill the mod if we use it.
		private void UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!this.hudLocked)
			{
                this.config.HudXCoord = Game1.getMouseX();
                this.config.HudYCoord = Game1.getMouseY();
			}
		}
#endif

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();

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
                () => this.config = new ModConfig(),
                () => this.Helper.WriteConfig(this.config));

            configMenuApi.RegisterSimpleOption(this.ModManifest, "Toggle Tracking Target",
                "Whether or not you want the log to specify whether the player or cursor was the source of this co-ordinate.",
                () => this.config.LogTrackingTarget,
                value => this.config.LogTrackingTarget = value);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD Toggle",
                "The key used to toggle the co-ordinate HUD.",
                () => this.config.CoordinateHUDToggle,
                button => this.config.CoordinateHUDToggle = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "Log Co-ordinates",
                "The key used to log the current co-ordinates to file.",
                () => this.config.LogCoordinates,
                button => this.config.LogCoordinates = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "Toggle Tracking Target",
                "The key used to toggle between tracking the cursor and the player's co-ordinates.",
                () => this.config.SwitchToCursorCoords,
                button => this.config.SwitchToCursorCoords = button);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD X Co-ordinate:",
                "The X co-ordinate of the HUD",
                () => this.config.HudXCoord,
                value => this.config.HudXCoord = value);
            configMenuApi.RegisterSimpleOption(this.ModManifest, "HUD Y Co-ordinate:",
                "The Y co-ordinate of the HUD",
                () => this.config.HudYCoord,
                value => this.config.HudYCoord = value);

            // Since I can't think of a way to log co-ordinates on Android, a toggle to make the HUD movable is
            // also out of the question until I figure that out.
#if ANDROID
#else
			configMenuApi.RegisterSimpleOption(this.ModManifest, "Unlock HUD Position",
				"Press this key to toggle the HUD to be unlocked so you can move it around.",
				() => this.config.HudUnlock,
				(button) => this.config.HudUnlock = button);
#endif
        }

        private void ButtonPressed(object o, ButtonPressedEventArgs button)
        {
            var b = button.Button;

            if (b == this.config.CoordinateHUDToggle) this.showHud = !this.showHud;

            if (b == this.config.SwitchToCursorCoords) this.trackingCursor = !this.trackingCursor;

            if (b == this.config.LogCoordinates) this.LogCurrentCoordinates();

            if (b == this.config.HudUnlock) this.hudLocked = false;
        }

        private void ButtonReleased(object sender, ButtonReleasedEventArgs button)
        {
            var b = button.Button;

            if (b == this.config.HudUnlock)
            {
                this.hudLocked = true;
                this.Helper.WriteConfig(this.config);
            }
        }

        private void UpdateCurrentMap(object o, RenderedWorldEventArgs world)
        {
            string newMap = Game1.player.currentLocation.Name;

            if (newMap == string.Empty)
                newMap = "Unnamed Map";

            this.currentMapName = newMap;
        }

        private void UpdateCurrentCoordinates()
        {
            // Track the cursor instead of the player if appropriate.
            this.currentCoords = this.trackingCursor ? Game1.currentCursorTile : Game1.player.Tile;
        }

        private void LogCurrentCoordinates()
        {
            if (!this.showHud ||
                !Context.IsWorldReady) // Look into why Context.IsWorldReady return true on title screen?
                return;

            string finalPath = Path.Combine(this.modDirectory, "coordinate_output.txt");

            // This is bad, and I need to split things up better. At some point. For now, this is fine.
            var file = new CoordinateLogger(finalPath, this.currentCoords, this.currentMapName, this.trackingCursor,
                this.config.LogTrackingTarget, this.Monitor);

            if (file.LogCoordinates()) // Try to log the co-ordinates, and determine whether or not we were successful.
            {
                Game1.showGlobalMessage("Co-ordinates logged.\r\n" +
                                        "Check the mod folder!");
                Logger.LogMessage(this.Monitor, LogLevel.Info,
                    $"Co-ordinates ({this.currentMapName}: {this.currentCoords}) logged successfully.");
            }
            else
            {
                Game1.showGlobalMessage("Failed to save co-ordinates.\r\n" +
                                        "Check your SMAPI log for details.");
                Logger.LogMessage(this.Monitor, LogLevel.Error,
                    $"Failed to log co-ordinates ({this.currentMapName}: {this.currentCoords}). Exception follows:");
            }
        }

        private void DrawCoordinates(object o, RenderedHudEventArgs world)
        {
            if (!Context.IsWorldReady)
                return;

            if (!this.showHud)
                return;

            if (Game1.game1.takingMapScreenshot)
                return;


            // We only need to update our co-ordinates if we're drawing the HUD. Maybe make this an option?
            this.UpdateCurrentCoordinates();

            // Everything below this point is messy and terrible, and I am a bad person for doing so.
            // It works, but do not do what I do.
            // TODO: Make everything below here not terrible.

            DrawHud.Draw(new Vector2(this.config.HudXCoord, this.config.HudYCoord), this.currentCoords,
                this.coordinateBox, world);

#if ANDROID
            // Do absolutely nothing.
#else
			//Draw a rectangle around the cursor position when tracking the cursor.
			if (this.trackingCursor)
			{
				if (this.currentCoords.y < 0 || this.currentCoords.x < 0)
					return; // We don't want to draw our tile rectangle if the cursor coordinates are negative.
							// Also, it's buggy.

				float zoomLevel = Game1.options.zoomLevel;

				world.SpriteBatch.Draw(
					Game1.mouseCursors,
					Game1.GlobalToLocal(
						Game1.viewport,
						new Vector2(this.currentCoords.x, this.currentCoords.y) * Game1.tileSize) * zoomLevel,
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
