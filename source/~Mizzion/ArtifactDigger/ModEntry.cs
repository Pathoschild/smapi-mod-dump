/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using MyStardewMods.Common;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using StardewValley.Tools;
using GenericModConfigMenu;
using StardewModdingAPI.Utilities;



namespace ArtifactDigger
{
    public class ModEntry : Mod
    {
        private int _magneticRadius, _defaultMagneticRadius, _playerOriginalMagneticRadius, _radiusResetStatus;
        private bool _magneticRadiusResetActive;
        private ModConfig _config;
        private IGenericModConfigMenuApi _cfgMenu;
        string[] keys;
        private SButton _activateKey;
        private static Texture2D _buildingPlacementTiles;
        private List<Vector2> _location, _digLocation;

        private readonly bool _isDebugging = false;

        /// <summary>
        /// The void that is ran before any other.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _defaultMagneticRadius = 128;

            _location = new List<Vector2>();
            _digLocation = new List<Vector2>();

            //Events

            //GameLoop Events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoad;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

            //Input Events
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            //Display Events
            //helper.Events.Display.RenderedHud += OnHudRendered;
            helper.Events.Display.RenderedWorld += OnHudRendered;
            
        }

        /// <summary>
        /// Event that runs when the game is launched
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">GameLaunched event args</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            #region "Generic Mod Config Menu"

            _cfgMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_cfgMenu is null) return;

            _cfgMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
                );

            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Artifact Digger Settings",
                tooltip: null
                );

            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Dig Radius",
                tooltip: () => "How many tile out from the player should we dig. ",
                getValue: () => _config.DigRadius,
                setValue: value => _config.DigRadius = value
            );

            _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.HighlightArtifactSpots,
               setValue: value => _config.HighlightArtifactSpots = value,
               name: () => "Highlight Artifact Spots",
               tooltip: () => "Whether artifact spots should be highlighted."
           );

            _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.AutoArtifactScan,
               setValue: value => _config.AutoArtifactScan = value,
               name: () => "Auto Scan for Artifact Spots.",
               tooltip: () => "Whether or not we should scan for artifact spots automaticall. Can cause major lag."
           );

            _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.ShakeTrees,
               setValue: value => _config.ShakeTrees = value,
               name: () => "Enable Tree Shaker",
               tooltip: () => "Whether or not we should shake trees."
           );

            _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.ShakeBushes,
               setValue: value => _config.ShakeBushes = value,
               name: () => "Enable Bush Shaker",
               tooltip: () => "Whether or not we should shake bushes"
           );

            _cfgMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Activation Key",
                tooltip: () => "The keybind to activate the digger.",
                getValue: () => _config.ArtifactScanKey,
                setValue: value => _config.ArtifactScanKey = value
            );

            /*
             _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.HighlightArtifactSpots,
               setValue: value => _config.HighlightArtifactSpots = value,
               name: () => "",
               tooltip: () => ""
           ); 
             
             configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: I18n.Options_OpenMenuKey_Name,
                tooltip: I18n.Options_OpenMenuKey_Desc,
                getValue: () => this.Config.OpenMenuKey,
                setValue: value => this.Config.OpenMenuKey = value
            );
            */
            #endregion
        }

        /// <summary>
        /// Event that is raised before the game is saved. This will be used to reset the magnetic radius to fix that bug.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Saving Event Args</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            Game1.player.MagneticRadius = _playerOriginalMagneticRadius;
            if (_isDebugging)
                Monitor.Log($"Resetting the players Magnetic Radius from {Game1.player.MagneticRadius} to {_playerOriginalMagneticRadius}. Their Original was : {_playerOriginalMagneticRadius}");
        }


        /// <summary>
        /// Event that is raised after a save is loaded. Will be used to set the players magnetic radius to the modyfied value.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Saved Event Args</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            //Modify the magnetic radius
            _playerOriginalMagneticRadius = Game1.player.MagneticRadius;

            if (_isDebugging)
                Monitor.Log($"Magnetic Player Radius: {Game1.player.MagneticRadius}.");
        }

        /// <summary>
        /// Void that is ran when a Save is loaded.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event args for SaveLoaded</param>
        private void OnSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            //Check and make sure the keybind is valid
            
            if(!Enum.TryParse(_config.ArtifactScanKey.ToString(), out _activateKey))
            {
                _config.ArtifactScanKey = SButton.Z;
                Monitor.LogOnce("Activation Keybind wasnt valid. Reset it to Z");
            }


            _playerOriginalMagneticRadius = Game1.player.MagneticRadius;

            if (_isDebugging)
                Monitor.Log($"Magnetic Player Radius: {Game1.player.MagneticRadius}.");

        }

        /// <summary>
        /// Method that gets ran when a button is pressed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">ButtonPressed Event Args</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var str = SafeToRun();

            if (!Context.IsWorldReady)
                return;

            if(e.IsDown(_config.ArtifactScanKey) && !_config.AutoArtifactScan && str)
            {
                DoScan();

                if (_config.ShakeBushes)
                    ShakeBushes();
                if (_config.ShakeTrees)
                    ShakeTrees();
            }

            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<ModConfig>();
                
                Monitor.Log($"Mod Config was reloaded: {_config.DigRadius}");
            }
            if (e.IsDown(SButton.F2))
            {
                Game1.player.MagneticRadius = _defaultMagneticRadius;
                Monitor.Log($"Players Magnetic Radius was reset from Current: {_playerOriginalMagneticRadius} to New: {Game1.player.MagneticRadius}");
                _playerOriginalMagneticRadius = Game1.player.MagneticRadius;
                
            }
        }

        /// <summary>
        /// Event that happens after the Hud os Rendered
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">RenderedHud event args</param>
        public void OnHudRendered(object sender, RenderedWorldEventArgs e)
        {
            _buildingPlacementTiles ??= Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            var str = SafeToRun();

            if (!str)
                return;

            foreach (var artLoc in _location)
                DrawRadius(Game1.spriteBatch, 1, artLoc);
        }


        /// <summary>
        /// Event that runs once a second
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">One Second Update Ticked event args.</param>
        public void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            var str = SafeToRun();

            if (!str)
                return;

            if (_config.HighlightArtifactSpots || _isDebugging)
                ShowSpots();

            if (_config.AutoArtifactScan)
            {
                DoScan();

                if (_config.ShakeBushes)
                    ShakeBushes();

                if (_config.ShakeTrees)
                    ShakeTrees();
            }

            if (_magneticRadiusResetActive && _radiusResetStatus < 5)
            {
                _radiusResetStatus++;
                if (_isDebugging)
                    Monitor.Log($"Magnetic Radius Active: {_magneticRadiusResetActive}, Reset Status: {_radiusResetStatus}");
            }               
            else
            {
                _radiusResetStatus = 0;
                _magneticRadiusResetActive = false;
                
            }
                
        }


        #region "Custom Methods"

        /// <summary>
        /// Void that will do the scanning for Artifacts.
        /// </summary>
        public void DoScan()
        {
            GameLocation currentLocation = Game1.currentLocation;
            Game1.player.MagneticRadius = _config.DigRadius * _playerOriginalMagneticRadius;
            _magneticRadiusResetActive = _magneticRadiusResetActive ? _magneticRadiusResetActive : false;
            _radiusResetStatus = _radiusResetStatus < 5 ? _radiusResetStatus : 0;

            if (_isDebugging)
                Monitor.Log($"Cur Radius: {Game1.player.MagneticRadius}, Old Radius: {_playerOriginalMagneticRadius}");

           
            getDigSpots();
            foreach (var i in _digLocation)
            {
                currentLocation.Objects.TryGetValue(i, out SObject @object);

                Hoe h = new Hoe();
                if (_isDebugging)
                    Monitor.Log($"Found something {@object.DisplayName} {@object.TileLocation.X}, {@object.TileLocation.Y}");
                h.DoFunction(currentLocation, Convert.ToInt32(i.X * 64f), Convert.ToInt32(i.Y * 64f), 0, Game1.player);
            }

            //Activate Resetter
            _magneticRadiusResetActive = true;
            

        }

        /// <summary>
        /// Sets up the Hud to show the artifact spots.
        /// </summary>
        public void ShowSpots()
        {
            var gridRadius = GetTileGrid(Game1.player.getTileLocation(), _config.DigRadius).ToArray();

            GameLocation currentLocation = Game1.currentLocation;

            //Clear the location list when we start the scan
            _location.Clear();

            foreach (var i in gridRadius)
            {
                var g = i;
                currentLocation.Objects.TryGetValue(g, out SObject @object);

                if (@object is { ParentSheetIndex: 590 })
                    _location.Add(g);
            }
        }

        private void getDigSpots()
        {
            var gridRadius = GetTileGrid(Game1.player.getTileLocation(), _config.DigRadius).ToArray();

            GameLocation currentLocation = Game1.currentLocation;

            //Clear the location list when we start the scan
            _digLocation.Clear();

            foreach (var i in gridRadius)
            {
                var g = i;
                currentLocation.Objects.TryGetValue(g, out SObject @object);

                if (@object is { ParentSheetIndex: 590 })
                    _digLocation.Add(g);
            }
        }
        /// <summary>
        /// Method to find and shake bushes
        /// </summary>
        public void ShakeBushes()
        {
            var gridRadius = GetTileGrid(Game1.player.getTileLocation(), _config.DigRadius).ToArray();
            GameLocation currentLocation = Game1.currentLocation;

            foreach (var i in gridRadius)
            {
                var rec = AbsoluteTile(i);

                if (currentLocation.largeTerrainFeatures.FirstOrDefault(b => b.getBoundingBox(b.tilePosition.Value).Intersects(rec)) is Bush bush)
                    if (!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
                        bush.performUseAction(bush.tilePosition.Value, currentLocation);


            }
        }

        /// <summary>
        /// Method to find and shake any Tree that has a seed waiting.
        /// </summary>
        public void ShakeTrees()
        {
            var gridRadius = GetTileGrid(Game1.player.getTileLocation(), _config.DigRadius).ToArray();
            GameLocation currentLocation = Game1.currentLocation;

            foreach (var i in gridRadius)
            {
                currentLocation.terrainFeatures.TryGetValue(i, out TerrainFeature @terrain);

                if (@terrain is Tree tree)
                {
                    if (tree.hasSeed.Value)
                        tree.performUseAction(i, currentLocation);
                }

                if (@terrain is FruitTree ft)
                {
                    if (ft.fruitsOnTree.Value > 0)
                    {
                        ft.performUseAction(i, currentLocation);
                    }
                }
            }
        }


        /// <summary>
        /// Method that checks to see if our code should run
        /// </summary>
        /// <returns>Whether the game is loaded and we're outside</returns>
        public bool SafeToRun()
        {
            return Game1.currentLocation != null && Game1.player != null && Game1.hasLoadedGame && Game1.player.CanMove && Game1.activeClickableMenu == null && Game1.CurrentEvent == null && Game1.gameMode == 3 && Game1.currentLocation.IsOutdoors;
        }


        /// <summary>Get a grid of tiles.</summary>
        /// <param name="origin">The center of the grid.</param>
        /// <param name="distance">The number of tiles in each direction to include.</param>
        private IEnumerable<Vector2> GetTileGrid(Vector2 origin, int distance)
        {
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                    yield return new Vector2(origin.X + x, origin.Y + y);
            }
        }


        /// <summary>Draw a radius around the player.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="radius">The radius to draw</param>
        /// <param name="tile">The tile location.</param>
        public void DrawRadius(SpriteBatch spriteBatch, int radius, Vector2 tile)//loc)
        {
            // get tile area in screen pixels
            var area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);

            // choose tile color
            Color color = Color.DarkRed;//enabled ? Color.Green : Color.Red;

            // draw background
            spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, area.Height), color * 0.5f);

            // draw border
            int borderSize = 1;
            Color borderColor = color * 0.8f;
            spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, borderSize), borderColor); // top
            spriteBatch.DrawLine(area.X, area.Y, new Vector2(borderSize, area.Height), borderColor); // left
            spriteBatch.DrawLine(area.X + area.Width, area.Y, new Vector2(borderSize, area.Height), borderColor); // right
            spriteBatch.DrawLine(area.X, area.Y + area.Height, new Vector2(area.Width, borderSize), borderColor); // bottom
        }

        /// <summary>
        /// Get a rectangle.
        /// </summary>
        /// <param name="tile">The tile location</param>
        /// <returns></returns>
        public Rectangle AbsoluteTile(Vector2 tile)
        {
            Vector2 loc = tile * Game1.tileSize;
            return new Rectangle(Convert.ToInt32(loc.X), Convert.ToInt32(loc.Y), Game1.tileSize, Game1.tileSize);
        }


        #endregion

    }
}
