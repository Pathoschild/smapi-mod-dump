using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.Fall28SnowDay
{
    /// <summary>The mod entry point.</summary>
    public class Fall28SnowDay : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked just before the game saves.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (Game1.IsFall && Game1.dayOfMonth == 27)
                Game1.weatherForTomorrow = Game1.weather_snow;
        }
    }
}
