/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using GenericModConfigMenu;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley.Locations;
using xTile.Tiles;
using HarmonyLib;
using xTile.ObjectModel;

namespace StrongerTools {
    internal class ModEntry : Mod {

        Item item;
        ModConfig config = new();
        
        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            GameLocation location = Game1.getLocationFromName("Backwoods");
            
            AddTerrainFeature(location, new Vector2(34, 10), Tree.pineTree);
            AddTerrainFeature(location, new Vector2(41, 16), Tree.bushyTree);
            AddTerrainFeature(location, new Vector2(12, 19), Tree.bushyTree);
            AddTerrainFeature(location, new Vector2(15, 17), Tree.bushyTree);
            AddTerrainFeature(location, new Vector2(20, 20), Tree.bushyTree);
            AddTerrainFeature(location, new Vector2(25, 16), Tree.bushyTree);
            AddTerrainFeature(location, new Vector2(17, 14), Tree.leafyTree);
            AddTerrainFeature(location, new Vector2(19, 12), Tree.leafyTree);
            AddTerrainFeature(location, new Vector2(21, 11), Tree.leafyTree);
            AddTerrainFeature(location, new Vector2(28, 9), Tree.leafyTree);
            AddTerrainFeature(location, new Vector2(46, 12), Tree.leafyTree);
            Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
        }

        void AddTerrainFeature(GameLocation location, Vector2 tilePosition, string treeType) {
            if (!location.IsTileOccupiedBy(tilePosition)) {
                location.terrainFeatures.Add(tilePosition, new Tree(treeType, Tree.treeStage));
                if (config.ShowLogs) Monitor.Log($"Placing stage 5 pine tree at {location.Name} (X:{tilePosition.X}, Y:{tilePosition.Y})", LogLevel.Info);
            }
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Debug"
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.ShowLogs,
                setValue: value => config.ShowLogs = value,
                name: () => "Enabled"
            );
            
            //EditAudio("thunder", "mySound.wav", "Sound");
            //EditAudio("thunder_small", "mySound.wav", "Sound");
        }

        void EditAudio(string cue, string audioFile, string category) {
            var existingCueDef = Game1.soundBank.GetCueDefinition(cue);
            SoundEffect audio;
            string filePathCombined = Path.Combine(Helper.DirectoryPath, audioFile);

            using (var stream = new FileStream(filePathCombined, FileMode.Open)) {
                audio = SoundEffect.FromStream(stream);
            }

            existingCueDef.SetSound(audio, Game1.audioEngine.GetCategoryIndex(category), false);
        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e) {
            if (!Context.IsPlayerFree) return;
            if (Game1.activeClickableMenu != null) return;

            item = Game1.player.CurrentTool;
            
            if (e.Button.IsUseToolButton()) {
                if (item is Pickaxe pickaxe) {
                    switch (pickaxe.ItemId) {
                        case "GoldPickaxe":
                            if (pickaxe.additionalPower.Value < 3) pickaxe.additionalPower.Value = 3;
                            break;
                        case "IridiumPickaxe":
                            if (pickaxe.additionalPower.Value < 10) pickaxe.additionalPower.Value = 10;
                            break;
                    }

                    if (config.ShowLogs) Monitor.Log($"{pickaxe.Name} current additional power: +{pickaxe.additionalPower.Value}", LogLevel.Info);
                }
            }
        }

    }
}
