using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HorseWhistle.Common;
using HorseWhistle.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace HorseWhistle
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        private TileData[] Tiles;
        private bool GridActive = false;
        private SoundBank OriginalSoundBank;
        private WaveBank OriginalWaveBank;
        private SoundBank CustomSoundBank;
        private WaveBank CustomWaveBank;
        private bool HasAudio;
        private ModConfigModel Config;


        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfigModel>();

            try
            {
                CustomSoundBank = new SoundBank(Game1.audioEngine, Path.Combine(helper.DirectoryPath, "assets", "CustomSoundBank.xsb"));
                CustomWaveBank = new WaveBank(Game1.audioEngine, Path.Combine(helper.DirectoryPath, "assets", "CustomWaveBank.xwb"));
                HasAudio = true;
            }
            catch (ArgumentException ex)
            {
                this.Monitor.Log("Couldn't load audio (this is normal on Linux/Mac). The mod will work fine without audio.");
                this.Monitor.Log(ex.ToString(), LogLevel.Trace);
            }

            // add all event listener methods
            ControlEvents.KeyPressed += ReceiveKeyPress;
            GameEvents.SecondUpdateTick += ReceiveUpdateTick;
            GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent;
        }

        /// <summary>Update the mod's config.json file from the current <see cref="Config"/>.</summary>
        internal void SaveConfig()
        {
            Helper.WriteConfig(Config);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ReceiveKeyPress(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.KeyPressed.ToString() == Config.EnableGridKey)
            {
                GridActive = !GridActive;
            }
            if (e.KeyPressed.ToString() == Config.TeleportHorseKey)
            {
                NPC horse = Utility.findHorse();
                if (horse != null)
                {
                    if (OriginalSoundBank != null && OriginalWaveBank != null)
                    {
                        PlayHorseWhistle();
                    }
                    Game1.warpCharacter(horse, Game1.currentLocation.Name, Game1.player.getLeftMostTileX(), true, true);
                }
            }
        }

        private void PlayHorseWhistle()
        {
            if (!HasAudio)
                return;

            Game1.soundBank = CustomSoundBank;
            Game1.waveBank = CustomWaveBank;
            Game1.audioEngine.Update();
            Game1.playSound("horseWhistle");
            Game1.soundBank = OriginalSoundBank;
            Game1.waveBank = OriginalWaveBank;
            Game1.audioEngine.Update();
        }

        // <summary>The method called when the game finishes drawing components to the screen.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPostRenderEvent(object sender, EventArgs e)
        {
            Draw(Game1.spriteBatch);
        }

        private void ReceiveUpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null)
            {
                Tiles = new TileData[0];
                return;
            }

            if (OriginalSoundBank == null && Game1.soundBank != null)
                OriginalSoundBank = Game1.soundBank;
            if (OriginalWaveBank == null && Game1.waveBank != null)
                OriginalWaveBank = Game1.waveBank;

            // get updated tiles
            GameLocation location = Game1.currentLocation;
            Tiles = Update(location, CommonMethods.GetVisibleTiles(location, Game1.viewport)).ToArray();
        }

        private void Draw(SpriteBatch spriteBatch)
        {
            if (Tiles == null || Tiles.Length == 0 || !GridActive)
                return;

            // draw tile overlay
            int tileSize = Game1.tileSize;
            foreach (TileData tile in Tiles.ToArray())
            {
                Vector2 position = tile.TilePosition * tileSize - new Vector2(Game1.viewport.X, Game1.viewport.Y);
                RectangleSprite.DrawRectangle(spriteBatch, new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y, tileSize, tileSize), tile.Color * .3f, 6);
            }
        }

        private IEnumerable<TileData> Update(GameLocation location, IEnumerable<Vector2> visibleTiles)
        {
            foreach (Vector2 tile in visibleTiles)
            {
                if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile))
                    yield return new TileData(tile, Color.Red);
            }
        }
    }
}
