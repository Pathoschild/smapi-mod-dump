/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/SeaMonsterAlert
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using xTile.Dimensions;

namespace SeaMonsterAlert
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        bool seaMonsterHere;
        bool hasScreenshot = false;

        // Add a config
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.SoundTheAlarm;
            helper.Events.GameLoop.TimeChanged += this.TestSpawnMonster;
        }


        /*********
        ** Private methods
        *********/

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Don't do the warp if config says not to
            if (!this.Config.doBeachWarp)
            {
                return;
            }

            // Warp farmer to beach default entry location
            GameLocation beach = Game1.getLocationFromName("Beach");
            int xLoc = 28;
            int yLoc = 36;
            Game1.warpFarmer(new LocationRequest(beach.NameOrUniqueName, beach.uniqueName.Value != null, beach), xLoc, yLoc, 2);
        }

        private void SoundTheAlarm(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Check through the beach sprites for the seas monster
            GameLocation beach = Game1.getLocationFromName("Beach");
            bool tempSeaMonsterHere = false;
            foreach (var sprite in beach.temporarySprites)
            {
                //this.Monitor.Log($"{sprite.GetType()}", LogLevel.Debug);
                if (sprite is SeaMonsterTemporarySprite)
                {
                    tempSeaMonsterHere = true;
                    this.Monitor.Log($"He's here!!! At {sprite.position.X/16}, {sprite.position.Y/16}", LogLevel.Alert);
                    if (this.Config.doScream)
                    {
                        Game1.playSound("batScreech");
                    }
                }
            }
            // Only show red message when he first appears
            if (!seaMonsterHere && tempSeaMonsterHere && this.Config.showMessage) {
                Game1.showRedMessage("He's here!");
            }
            else if (seaMonsterHere && tempSeaMonsterHere && !hasScreenshot && this.Config.takeScreenshot)
            {
                // Take picture after 1 second when he appears
                Game1.game1.takeMapScreenshot(1f, null, null);
                Game1.playSound("cameraNoise");
                hasScreenshot = true;
            }
            // Clear screenshot bool if he disappeared
            if (seaMonsterHere && !tempSeaMonsterHere)
            {
                hasScreenshot = false;
            }
            // Set global variable
            seaMonsterHere = tempSeaMonsterHere;
        }

        private void TestSpawnMonster(object sender, TimeChangedEventArgs e)
        {
            if (seaMonsterHere || !this.Config.forceSpawn)
            {
                return;
            }

            GameLocation beach = Game1.getLocationFromName("Beach");
            Vector2 position = new Vector2(Game1.random.Next(15, 47) * 64, Game1.random.Next(29, 42) * 64);
            bool draw = true;
            for (float i = position.Y / 64f; i < (float)beach.map.GetLayer("Back").LayerHeight; i += 1f)
            {
                if (beach.doesTileHaveProperty((int)position.X / 64, (int)i, "Water", "Back") == null || beach.doesTileHaveProperty((int)position.X / 64 - 1, (int)i, "Water", "Back") == null || beach.doesTileHaveProperty((int)position.X / 64 + 1, (int)i, "Water", "Back") == null)
                {
                    draw = false;
                    break;
                }
            }
            if (draw)
            {
                beach.temporarySprites.Add(new SeaMonsterTemporarySprite(250f, 4, Game1.random.Next(7), position));
                this.Monitor.Log("Added sea monster",LogLevel.Debug);
            }
        }
    }
}