/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using xTile.Layers;
using xTile.Tiles;
using xTile.Dimensions;

namespace PassableDescents {
    internal class ModEntry : Mod {

        int delay = 0;
        int delayMax = 10;
        Layer currentLayer;
        ModConfig Config;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnWarped(object? sender, StardewModdingAPI.Events.WarpedEventArgs e) {
            if (e.NewLocation is MineShaft mineShaft) {
                currentLayer = mineShaft.Map.RequireLayer("Buildings");
            }
        }

        private void OnUpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e) {
            if (Context.IsWorldReady) {
                delay++;
                if (delay > delayMax) {
                    if (Game1.currentLocation is MineShaft) {
                        if (currentLayer == null) return;
                        List<Vector2> adjacentTiles = Utility.getAdjacentTileLocations(Game1.player.Tile);

                        foreach (Vector2 tile in adjacentTiles) {
                            Tile currentTile = currentLayer.PickTile(new Location((int)tile.X * 64, (int)tile.Y * 64), Game1.viewport.Size);
                            
                            if (currentTile != null && (currentTile.TileIndex == 173 || currentTile.TileIndex == 174) 
                                && !currentTile.Properties.ContainsKey("Passable")) {
                                currentTile.Properties.Add("Passable", "T");
                                if (Config.Logging) Monitor.Log($"Descent found at {tile.X}, {tile.Y}. Making passable", LogLevel.Info);
                            }
                        }
                    }

                    delay = 0;
                }
            }
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("logging.label"),
                tooltip: () => Helper.Translation.Get("logging.tooltip"),
                getValue: () => this.Config.Logging,
                setValue: value => this.Config.Logging = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("delay.label"),
                tooltip: () => Helper.Translation.Get("delay.tooltip"),
                getValue: () => this.Config.Delay,
                setValue: value => this.Config.Delay = value,
                min: 0
            );
        }

    }
}
