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
using MyStardewMods.Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace ArtifactDigger
{
    public class ArtifactDigger : Mod
    {
        private int _radius, _magneticRadius;
        private bool _treeShaker, _bushShaker, _autoScan,_highlightArtifactSpots;
        private ModConfig _config;
        private SButton _activateKey;
        private static Texture2D _buildingPlacementTiles;
        private List<Vector2> _location;

        private bool _isDebugging = false;


        /// <summary>
        /// The void that is ran before any other.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            //Set Variables
            _radius = _config.DigRadius;
            _treeShaker = _config.ShakeTrees;
            _bushShaker = _config.ShakeBushes;
            _autoScan = false;//_config.AutoArtifactScan;
            _highlightArtifactSpots = _config.HighlightArfiactSpots;

            _location = new List<Vector2>();

            //Events
            helper.Events.GameLoop.SaveLoaded += OnSaveLoad;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            //helper.Events.Display.RenderedHud += OnHudRendered;
            helper.Events.Display.RenderedWorld += OnHudRendered;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        }

        /// <summary>
        /// Void that is ran when a Save is loaded.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event args for SaveLoaded</param>
        public void OnSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            //Make sure that the activate key is valid
            if (!Enum.TryParse(_config.ArtifactScanKey, true, out _activateKey))
            {
                _activateKey = SButton.Z;
                Monitor.Log("Keybind was invalid. setting it to Z");
            }

            Game1.player.MagneticRadius = 128;
            _magneticRadius = Game1.player.MagneticRadius;
        }

        /// <summary>
        /// Void that is ran when a button is pressed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">ButtonPressed event args</param>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var str = SafeToRun();

            if (!Context.IsWorldReady)
                return;

            if (e.IsDown(_activateKey) && !_autoScan && str)
            {
                DoScan();

                if(_bushShaker)
                    ShakeBushes();

                if (_treeShaker)
                    ShakeTrees();
            }
                

            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<ModConfig>();
                _radius = _config.DigRadius;
                _treeShaker = _config.ShakeTrees;
                _bushShaker = _config.ShakeBushes;
                _autoScan = false;//_config.AutoArtifactScan;
                _highlightArtifactSpots = _config.HighlightArfiactSpots;
                Monitor.Log($"Mod Config was reloaded: {_config.DigRadius}");
            }
                
        }


        /// <summary>
        /// Event that happens after the Hud os Rendered
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">RenderedHud event args</param>
        public void OnHudRendered(object sender, RenderedWorldEventArgs e)
        {
            if (_buildingPlacementTiles == null)
                _buildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

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

            if (_highlightArtifactSpots || _isDebugging)
                ShowSpots();

            if (_autoScan)
            {
                DoScan();

                if (_bushShaker)
                    ShakeBushes();

                if (_treeShaker)
                    ShakeTrees();
            }
        }

        #region "Custom Voids"

        /// <summary>
        /// Void that will do the scanning for Artifacts.
        /// </summary>
        public void DoScan()
        {
            GameLocation currentLocation = Game1.currentLocation;
            Game1.player.MagneticRadius = _radius * _magneticRadius;
            if(_isDebugging)
                Monitor.Log($"Cur Radius: {Game1.player.MagneticRadius}, Old Radius: {_magneticRadius}");
            int sec = 0;
            foreach (var i in _location)
            {
                currentLocation.Objects.TryGetValue(i, out SObject @object);

                Hoe h = new Hoe();
                if (_isDebugging)
                    Monitor.Log($"Found something {@object.DisplayName} {@object.TileLocation.X}, {@object.TileLocation.Y}");
                h.DoFunction(currentLocation, Convert.ToInt32(i.X * 64f), Convert.ToInt32(i.Y * 64f), 0, Game1.player);
            }
            
            //Wait 5 Seconds and reset Magnetic Radius
            for (int num = 0; num < 0; num++)
                sec++;
            if (sec == 5)
                Game1.player.MagneticRadius = _magneticRadius;
            
        }

        /// <summary>
        /// Sets up the Hud to show the artifact spots.
        /// </summary>
        public void ShowSpots()
        {
            Vector2[] gridRadius = GetTileGrid(Game1.player.getTileLocation(), _radius).ToArray();

            GameLocation currentLocation = Game1.currentLocation;

            //Clear the location list when we start the scan
            _location.Clear();

            foreach (var i in gridRadius)
            {
                var g = i;
                currentLocation.Objects.TryGetValue(g, out SObject @object);

                if (@object != null && @object.ParentSheetIndex == 590)
                    _location.Add(g);
            }
        }

        /// <summary>
        /// Method to find and shake bushes
        /// </summary>
        public void ShakeBushes()
        {
            Vector2[] gridRadius = GetTileGrid(Game1.player.getTileLocation(), _radius).ToArray();
            GameLocation currentLocation = Game1.currentLocation;

            foreach (var i in gridRadius)
            {
                Rectangle rec = AbsoluteTile(i);

                if(currentLocation.largeTerrainFeatures.FirstOrDefault(b => b.getBoundingBox(b.tilePosition.Value).Intersects(rec)) is Bush bush)
                    if(!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
                        bush.performUseAction(bush.tilePosition.Value, currentLocation);


            }
        }

        /// <summary>
        /// Method to find and shake any Tree that has a seed waiting.
        /// </summary>
        public void ShakeTrees()
        {
            Vector2[] gridRadius = GetTileGrid(Game1.player.getTileLocation(), _radius).ToArray();
            GameLocation currentLocation = Game1.currentLocation;

            foreach (var i in gridRadius)
            {
                currentLocation.terrainFeatures.TryGetValue(i, out TerrainFeature @terrain);

                if (@terrain != null && @terrain is Tree tree)
                {
                    if (tree.hasSeed.Value)
                        tree.performUseAction(i, currentLocation);
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
                Rectangle area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);

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
