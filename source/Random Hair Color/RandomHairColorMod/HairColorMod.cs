using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;

namespace RandomHairColorMod
{
    public class HairColorMod : Mod
    {
        /************ Public methods *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // event += method to call
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            int seed = DateTime.Now.Second;
            Random random = new Random(seed);

            int r = random.Next() % 256;
            int g = random.Next() % 256;
            int b = random.Next() % 256;
            Color current_color = new Color(r, g, b);

            this.Monitor.Log($"R is {r}.");
            this.Monitor.Log($"G is {g}.");
            this.Monitor.Log($"B is {b}.");
            Game1.player.changeHairColor(current_color);
        }
    }
}
