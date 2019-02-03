using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xTile.Layers;
using xTile.Tiles;

namespace TillableGround {
     public class TillableGround : Mod {
        private ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Config = Helper.ReadConfig<ModConfig>();

            if (Config.AllowTillingAnywhere) {
                helper.Events.Input.ButtonReleased += OnButtonReleased;
            }
            else {
                helper.Events.Input.ButtonPressed += OnButtonPressed;
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed( object sender, ButtonPressedEventArgs e ) {
            if (!Context.IsWorldReady) { return; }
                
            Vector2 tile = e.Cursor.Tile;
            int x = (int)tile.X, y = (int)tile.Y;
            if (e.Button == Config.AllowTillingKeybind) {
                SetTileTillable(x, y);
                FancyTillFeedback(x, y, "Tillable");
            }
            else if (e.Button == Config.PreventTillingKeybind) {
                SetTileUntillable(x, y);
                FancyTillFeedback(x, y, "Untillable");
            }
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonReleased( object sender, ButtonReleasedEventArgs e ) {
            if (e.Button.IsUseToolButton() && Game1.player.CurrentTool is StardewValley.Tools.Hoe) {
                foreach (Vector2 tile in GetHoedTiles()) {
                    int x = (int)tile.X, y = (int)tile.Y;
                    SetTileTillable(x, y);
                }
            }
        }

        public void SetTileTillable(int x, int y) {
            Game1.currentLocation.setTileProperty(x, y, "Back", "Diggable", "T");
        }

        public void SetTileUntillable(int x, int y) {
            Layer layer = Game1.currentLocation.map.GetLayer("Back");
            Tile tile = layer.PickTile(new xTile.Dimensions.Location(x, y) * Game1.tileSize, Game1.viewport.Size);
            tile.Properties.Remove("Diggable");
        }

        public List<Vector2> GetHoedTiles() {
            Farmer player = Game1.player;
            Vector2 vector = player.GetToolLocation(false);
            Vector2 offset = new Vector2((float)(vector.X / 64), (float)(vector.Y / 64));

            return this.Helper.Reflection.GetMethod(player.CurrentTool, "tilesAffected").
                Invoke<List<Vector2>>(offset, player.toolPower, player);
        }

        public void FancyTillFeedback(int x, int y, string message) {
            // Add the hoe animation without hoeing
            Game1.currentLocation.temporarySprites.Add(
                 new TemporaryAnimatedSprite(12, new Vector2(x * 64f, y * 64f), Color.White, 8,
                     Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
            );

            Game1.addHUDMessage(new HUDMessage("Made tile " + message, 3) {
                noIcon = true,
                timeLeft = HUDMessage.defaultTime / 4
            });
        }
    }
}
