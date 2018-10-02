using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HorseWhistle.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;

namespace HorseWhistle
{
    /// <inheritdoc />
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        private TileData[] _tiles;
        private bool _gridActive;
        private ISoundBank _customSoundBank;
        private WaveBank _customWaveBank;
        private bool _hasAudio;
        private ModConfigModel _config;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfigModel>();

            if (Constants.TargetPlatform == GamePlatform.Windows && _config.EnableWhistleAudio)
            {
                try
                {
                    _customSoundBank = new SoundBankWrapper(new SoundBank(Game1.audioEngine,
                        Path.Combine(helper.DirectoryPath, "assets", "CustomSoundBank.xsb")));
                    _customWaveBank = new WaveBank(Game1.audioEngine,
                        Path.Combine(helper.DirectoryPath, "assets", "CustomWaveBank.xwb"));
                    _hasAudio = true;
                }
                catch (ArgumentException ex)
                {
                    _customSoundBank = null;
                    _customWaveBank = null;
                    _hasAudio = false;

                    Monitor.Log("Couldn't load audio, so the whistle sound won't play.");
                    Monitor.Log(ex.ToString(), LogLevel.Trace);
                }
            }

            // add all event listener methods
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            if (!_config.EnableGrid) return;
            GameEvents.SecondUpdateTick += (sender, e) => UpdateGrid();
            GraphicsEvents.OnPostRenderEvent += (sender, e) => DrawGrid(Game1.spriteBatch);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == _config.TeleportHorseKey)
            {
                var horse = FindHorse();
                if (horse == null) return;
                PlayHorseWhistle();
                Game1.warpCharacter(horse, Game1.currentLocation, Game1.player.getTileLocation());
            }
            else if (_config.EnableGrid && e.Button == _config.EnableGridKey)
                _gridActive = !_gridActive;
        }

        /// <summary>Play the horse whistle sound.</summary>
        private void PlayHorseWhistle()
        {
            if (!_hasAudio || !_config.EnableWhistleAudio) return;

            var originalSoundBank = Game1.soundBank;
            var originalWaveBank = Game1.waveBank;
            try
            {
                Game1.soundBank = _customSoundBank;
                Game1.waveBank = _customWaveBank;
                Game1.audioEngine.Update();
                Game1.playSound("horseWhistle");
            }
            finally
            {
                Game1.soundBank = originalSoundBank;
                Game1.waveBank = originalWaveBank;
                Game1.audioEngine.Update();
            }
        }

        /// <summary>Find the current player's horse.</summary>
        private Horse FindHorse()
        {
            return (from stable in GetStables()
                where !Context.IsMultiplayer || stable.owner.Value == Game1.player.UniqueMultiplayerID
                select Utility.findHorse(stable.HorseId)).FirstOrDefault(horse => horse != null && horse.rider == null);
        }

        /// <summary>Get all stables in the game.</summary>
        private IEnumerable<Stable> GetStables()
        {
            return from location in Game1.locations.OfType<BuildableGameLocation>()
                from stable in location.buildings.OfType<Stable>()
                where stable.GetType().FullName?.Contains("TractorMod") != true
                select stable;
        }

        private void UpdateGrid()
        {
            if (!_gridActive || !Context.IsPlayerFree || Game1.currentLocation == null)
            {
                _tiles = null;
                return;
            }

            // get updated tiles
            var location = Game1.currentLocation;
            _tiles = CommonMethods
                .GetVisibleTiles(location, Game1.viewport)
                .Where(tile => location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile))
                .Select(tile => new TileData(tile, Color.Red))
                .ToArray();
        }

        private void DrawGrid(SpriteBatch spriteBatch)
        {
            if (!_gridActive || !Context.IsPlayerFree || _tiles == null || _tiles.Length == 0)
                return;

            // draw tile overlay
            const int tileSize = Game1.tileSize;
            foreach (var tile in _tiles.ToArray())
            {
                var position = tile.TilePosition * tileSize - new Vector2(Game1.viewport.X, Game1.viewport.Y);
                RectangleSprite.DrawRectangle(spriteBatch,
                    new Rectangle((int) position.X, (int) position.Y, tileSize, tileSize), tile.Color * .3f, 6);
            }
        }
    }
}