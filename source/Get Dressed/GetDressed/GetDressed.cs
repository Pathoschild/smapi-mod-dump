using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GetDressed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;
using SFarmer = StardewValley.Farmer;
using Tile = GetDressed.Framework.Tile;

namespace GetDressed
{
    /// <summary>The main entry point.</summary>
    public class GetDressed : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>Encapsulates the underlying mod texture management.</summary>
        private ContentHelper ContentHelper;

        /// <summary>The current per-save config settings.</summary>
        private LocalConfig PlayerConfig;

        /// <summary>The global config settings.</summary>
        private GlobalConfig GlobalConfig;

        /// <summary>Whether the mod is initialised.</summary>
        private bool IsInitialised => this.ContentHelper != null;

        /// <summary>Whether the game world is loaded and ready.</summary>
        private bool IsLoaded => this.IsInitialised && Game1.hasLoadedGame;

        /// <summary>Whether this is the first day since the player loaded their save.</summary>
        private bool IsFirstDay = true;

        /// <summary>The last patched load menu, if the game hasn't loaded yet.</summary>
        private IClickableMenu PreviousLoadMenu;

        /// <summary>The farmer data for all saves, if the game hasn't loaded yet.</summary>
        private SFarmer[] Farmers = new SFarmer[0];

        /// <summary>The per-save configs for valid saves indexed by save name, if the game hasn't loaded yet.</summary>
        private IDictionary<string, LocalConfig> FarmerConfigs = new Dictionary<string, LocalConfig>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // load settings
            this.GlobalConfig = helper.ReadConfig<GlobalConfig>();

            // load content manager
            this.ContentHelper = new ContentHelper(this.Helper, this.Monitor, Game1.content.ServiceProvider);

            // load per-save configs
            this.FarmerConfigs = this
                .ReadLocalConfigs()
                .ToDictionary(p => p.SaveName);

            // hook events
            SaveEvents.AfterReturnToTitle += this.SaveEvents_AfterReturnToTitle;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;

            ControlEvents.MouseChanged += this.Event_MouseChanged;
            ControlEvents.ControllerButtonPressed += this.Event_ControllerButtonPressed;
            GameEvents.UpdateTick += this.Event_UpdateTick;
            ControlEvents.KeyPressed += this.Event_KeyPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The event handler called when the player stops a session and returns to the title screen..</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            // reset state
            this.PreviousLoadMenu = null;
            this.Farmers = new SFarmer[0];
            this.IsFirstDay = true;

            // load per-save configs
            this.FarmerConfigs = this
                .ReadLocalConfigs()
                .ToDictionary(p => p.SaveName);

            // restore load-menu patcher
            GameEvents.UpdateTick += this.Event_UpdateTick;
        }

        /// <summary>The event handler called when the game updates its state.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_UpdateTick(object sender, EventArgs e)
        {
            // patch load menu
            if (this.IsInitialised && !Game1.hasLoadedGame)
            {
                this.PatchLoadMenu();
                return;
            }

            // remove load menu patcher
            this.Farmers = new SFarmer[0];
            this.FarmerConfigs = new Dictionary<string, LocalConfig>();
            if (!string.IsNullOrEmpty(ModConstants.PerSaveConfigPath))
                GameEvents.UpdateTick -= Event_UpdateTick;
        }

        /// <summary>The event handler called when the player loads a save and the world is ready.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            // load config
            this.PlayerConfig = this.Helper.ReadJsonFile<LocalConfig>(ModConstants.PerSaveConfigPath) ?? new LocalConfig();

            // patch player textures
            this.PatchFarmerTexture(Game1.player, this.PlayerConfig);

            // update config on first run
            if (this.PlayerConfig.FirstRun)
            {
                this.PlayerConfig.ChosenAccessory[0] = Game1.player.accessory;
                this.PlayerConfig.FirstRun = false;
                this.Helper.WriteJsonFile(ModConstants.PerSaveConfigPath, this.PlayerConfig);
            }
            else
                Game1.player.accessory = this.PlayerConfig.ChosenAccessory[0];

            // patch farmhouse tilesheet
            FarmHouse farmhouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");
            this.PatchFarmhouseTilesheet(farmhouse);
        }

        /// <summary>The event handler called when the mouse state changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            FarmHouse farmhouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");
            if (this.IsFirstDay || farmhouse.upgradeLevel != this.Helper.Reflection.GetPrivateValue<int>(farmhouse, "currentlyDisplayedUpgradeLevel"))
                this.PatchFarmhouseMap(farmhouse);
            this.IsFirstDay = false;
        }

        /// <summary>The event handler called when the mouse state changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (!this.IsLoaded)
                return;

            if (e.NewState.RightButton == ButtonState.Pressed && e.PriorState.RightButton != ButtonState.Pressed)
                this.CheckForAction();
        }

        /// <summary>The event handler called when the player presses a controller button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (!this.IsLoaded)
                return;

            if (e.ButtonPressed == Buttons.A)
                this.CheckForAction();
        }

        /// <summary>The event handler called when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (!this.IsLoaded || Game1.activeClickableMenu != null)
                return;

            if (e.KeyPressed.ToString() == this.GlobalConfig.MenuAccessKey)
            {
                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.playSound("bigDeSelect");
                Game1.activeClickableMenu = new CharacterCustomizationMenu(this.ContentHelper, this.Helper, this.ModManifest.Version, this.GlobalConfig, this.PlayerConfig, Game1.options.zoomLevel);
            }
        }

        /// <summary>Open the customisation menu if the player activated the dresser.</summary>
        private void CheckForAction()
        {
            if (Game1.player.UsingTool || Game1.pickingTool || Game1.menuUp || (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence) || Game1.nameSelectUp || Game1.numberOfSelectedItems != -1 || Game1.fadeToBlack || Game1.activeClickableMenu != null)
                return;

            // get the activated tile
            Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
            if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                grabTile = Game1.player.GetGrabTile();

            // check tile action
            xTile.Tiles.Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
            xTile.ObjectModel.PropertyValue propertyValue = null;
            tile?.Properties.TryGetValue("Action", out propertyValue);
            if (propertyValue?.ToString() != "GetDressed")
                return;

            // open menu
            Game1.playSound("bigDeSelect");
            Game1.activeClickableMenu = new CharacterCustomizationMenu(this.ContentHelper, this.Helper, this.ModManifest.Version, this.GlobalConfig, this.PlayerConfig, Game1.options.zoomLevel);
        }

        /// <summary>Patch the textures in the load menu if it's active.</summary>
        private void PatchLoadMenu()
        {
            if (!(Game1.activeClickableMenu is TitleMenu))
                return;

            // get load menu
            LoadGameMenu loadMenu = TitleMenu.subMenu as LoadGameMenu;
            if (loadMenu == null || loadMenu == this.PreviousLoadMenu)
                return;

            // wait until menu is initialised
            if (this.Helper.Reflection.GetPrivateValue<Task<List<SFarmer>>>(loadMenu, "_initTask") != null)
                return;
            this.PreviousLoadMenu = loadMenu;

            // override load-game textures
            if (!this.Farmers.Any())
            {
                // get saves
                this.Farmers = this.Helper.Reflection.GetPrivateValue<List<SFarmer>>(loadMenu, "saveGames").ToArray();

                // override textures
                foreach (SFarmer saveEntry in this.Farmers)
                {
                    // get save info (save name stuffed into favorite thing field)
                    string saveName = saveEntry.favoriteThing;
                    LocalConfig config;
                    if (!this.FarmerConfigs.TryGetValue(saveName, out config))
                        //if (!this.FarmerConfigs.TryGetValue(saveName, out LocalConfig config))
                        continue;

                    // initialise for first run
                    if (config.FirstRun)
                    {
                        config.ChosenAccessory[0] = saveEntry.accessory;
                        config.FirstRun = false;
                        this.Helper.WriteJsonFile(Path.Combine("psconfigs", $"{config.SaveName}.json"), config);
                    }

                    // override textures
                    this.PatchFarmerTexture(saveEntry, config);
                    saveEntry.accessory = config.ChosenAccessory[0];
                }
            }

            // inject new farmers
            this.Helper.Reflection
                .GetPrivateField<List<SFarmer>>(loadMenu, "saveGames")
                .SetValue(this.Farmers.ToList());
        }

        /// <summary>Read all per-save configs from disk.</summary>
        private IEnumerable<LocalConfig> ReadLocalConfigs()
        {
            // get saves path
            string savePath = Constants.SavesPath;
            if (!Directory.Exists(savePath))
                yield break;

            // get save names
            string[] directories = Directory.GetDirectories(savePath);
            if (!directories.Any())
                yield break;

            // get per-save configs
            foreach (string saveDir in directories)
            {

                // get config
                string localConfigPath = Path.Combine("psconfigs", $"{new DirectoryInfo(saveDir).Name}.json");
                LocalConfig farmerConfig = this.Helper.ReadJsonFile<LocalConfig>(localConfigPath);
                if (farmerConfig == null)
                {
                    farmerConfig = new LocalConfig();
                    this.Helper.WriteJsonFile(localConfigPath, farmerConfig);
                }
                farmerConfig.SaveName = new DirectoryInfo(saveDir).Name;
                yield return farmerConfig;
            }
        }

        /// <summary>Patch the loaded texture for a player to reflect their custom settings.</summary>
        /// <param name="player">The player whose textures to patch.</param>
        /// <param name="config">The per-save settings for the player.</param>
        private void PatchFarmerTexture(SFarmer player, LocalConfig config)
        {
            Texture2D playerTextures = this.ContentHelper.GetBaseFarmerTexture(player.isMale);
            if (player.isMale)
            {
                this.ContentHelper.PatchTexture(ref playerTextures, $"male_face{config.ChosenFace[0]}_nose{config.ChosenNose[0]}.png", 0, 0);
                for (int i = 0; i < ModConstants.MaleShoeSpriteHeights.Length; i++)
                    this.ContentHelper.PatchTexture(ref playerTextures, $"male_shoes{config.ChosenShoes[0]}.png", 1 * i, (1 * i) * 4, 96, 32, ModConstants.MaleShoeSpriteHeights[i]);
                this.ContentHelper.PatchTexture(ref playerTextures, "male_bottoms.png", (config.ChosenBottoms[0] >= this.GlobalConfig.MaleBottomsTypes) ? 0 : config.ChosenBottoms[0], 3);
            }
            else
            {
                this.ContentHelper.PatchTexture(ref playerTextures, $"female_face{config.ChosenFace[0]}_nose{config.ChosenNose[0]}.png", 0, 0);
                for (int i = 0; i < ModConstants.FemaleShoeSpriteHeights.Length; i++)
                    this.ContentHelper.PatchTexture(ref playerTextures, $"female_shoes{config.ChosenShoes[0]}.png", 1 * i, (1 * i) * 4, 96, 32, ModConstants.MaleShoeSpriteHeights[i]);
                this.ContentHelper.PatchTexture(ref playerTextures, "female_bottoms.png", config.ChosenBottoms[0], 3);
            }
            this.ContentHelper.PatchFarmerRenderer(player, playerTextures);
        }

        /// <summary>Patch the dresser into the farmhouse tilesheet.</summary>
        /// <param name="farmhouse">The farmhouse to patch.</param>
        private void PatchFarmhouseTilesheet(FarmHouse farmhouse)
        {
            IDictionary<TileSheet, Texture2D> tilesheetTextures = this.Helper.Reflection.GetPrivateValue<Dictionary<TileSheet, Texture2D>>(Game1.mapDisplayDevice as XnaDisplayDevice, "m_tileSheetTextures");
            Texture2D texture = tilesheetTextures[farmhouse.map.GetTileSheet("untitled tile sheet")];
            if (texture != null)
            {
                this.ContentHelper.PatchTexture(ref texture, "dresser.png", 0, 231, 16, 16);
                this.ContentHelper.PatchTexture(ref texture, "dresser.png", 1, 232, 16, 16);
            }
        }

        /// <summary>Patch the dresser into the farmhouse map.</summary>
        /// <param name="farmhouse">The farmhouse to patch.</param>
        private void PatchFarmhouseMap(FarmHouse farmhouse)
        {
            if (!this.GlobalConfig.ShowDresser)
                return;

            // get dresser coordinates
            Point position;
            switch (farmhouse.upgradeLevel)
            {
                case 0:
                    position = new Point(this.GlobalConfig.StoveInCorner ? 7 : 10, 2);
                    break;
                case 1:
                    position = new Point(Game1.player.isMarried() ? 25 : 28, 2);
                    break;
                case 2:
                    position = new Point(33, 11);
                    break;
                case 3:
                    position = new Point(33, 11);
                    break;
                default:
                    this.Monitor.Log($"Couldn't patch dresser into farmhouse, unknown upgrade level {farmhouse.upgradeLevel}", LogLevel.Warn);
                    return;
            }

            // inject dresser
            Tile[] tiles = {
                new Tile(TileLayer.Front, position.X, position.Y, 231, "untitled tile sheet"), // dresser top
                new Tile(TileLayer.Buildings, position.X, position.Y + 1, 232, "untitled tile sheet") // dresser bottom
            };
            foreach (Tile tile in tiles)
            {
                Layer layer = farmhouse.map.GetLayer(tile.LayerName);
                TileSheet tilesheet = farmhouse.map.GetTileSheet(tile.Tilesheet);

                if (layer.Tiles[tile.X, tile.Y] == null || layer.Tiles[tile.X, tile.Y].TileSheet.Id != tile.Tilesheet)
                    layer.Tiles[tile.X, tile.Y] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tile.TileID);
                else
                    farmhouse.setMapTileIndex(tile.X, tile.Y, tile.TileID, layer.Id);
            }

            // add action attribute
            farmhouse.setTileProperty(position.X, position.Y + 1, "Buildings", "Action", "GetDressed");
        }
    }
}
