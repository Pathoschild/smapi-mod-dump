/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-water-bot
**
*************************************************/

using BotFramework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using WaterBot.Framework;

namespace WaterBot
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class WaterBot : Mod
    {
        private WaterBotControler bot;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// 
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Logger.SetMonitor(Monitor);
            this.bot = new WaterBotControler(helper);
            // Set static reference to monitor for logging.

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// <summary>
        /// Raised after the player presses a button on the keyboard, controller, or mouse.
        /// </summary>
        /// 
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.bot.active)
            {
                Logger.Log("Player provided interrupt signal. Process stopped.");
                this.bot.stop();
            } else if (e.Button.IsActionButton()) // SButton.MouseRight 
            {
                if (this.isWateringHoedDirt())
                {
                    Logger.Log("Player provided trigger to begin bot.");
                    this.bot.start(this.console);
                }
            }
        }

        /// <summary>
        /// Determines if the event was watering a tile of hoed dirt
        /// </summary>
        private bool isWateringHoedDirt()
        {
            // Is the player using a Watering Can on their Farm?
            if (Game1.player.CurrentItem is WateringCan)
            {
                // Find action tiles
                Vector2 mousePosition = Utility.PointToVector2(Game1.getMousePosition()) + new Vector2(Game1.viewport.X, Game1.viewport.Y);
                Vector2 toolLocation = Game1.player.GetToolLocation(mousePosition);
                Vector2 tile = Utility.clampToTile(toolLocation);

                List<Vector2> tileLocations = this.Helper.Reflection
                    .GetMethod(Game1.player.CurrentItem, "tilesAffected")
                    .Invoke<List<Vector2>>(new Vector2(tile.X / 64, tile.Y / 64), 0, Game1.player);

                foreach (Vector2 tileLocation in tileLocations)
                {
                    Vector2 rounded = new Vector2((float)Math.Round(tileLocation.X), (float)Math.Round(tileLocation.Y));

                    // If they just watered Hoe Dirt, return true
                    if (Game1.currentLocation.terrainFeatures.ContainsKey(rounded) &&
                        Game1.currentLocation.terrainFeatures[rounded] is HoeDirt &&
                        (Game1.currentLocation.terrainFeatures[rounded] as HoeDirt).crop != null &&
                        (((Game1.currentLocation.terrainFeatures[rounded] as HoeDirt).crop.fullyGrown &&
                        (Game1.currentLocation.terrainFeatures[rounded] as HoeDirt).crop.dayOfCurrentPhase > 0) || 
                            ((Game1.currentLocation.terrainFeatures[rounded] as HoeDirt).crop.currentPhase < (Game1.currentLocation.terrainFeatures[rounded] as HoeDirt).crop.phaseDays.Count - 1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Debug messages
        /// </summary>
        /// 
        /// <param name="message">Message text.</param>
        public void console(string message)
        {
            Logger.Log(message, LogLevel.Debug);
        }
    }
}