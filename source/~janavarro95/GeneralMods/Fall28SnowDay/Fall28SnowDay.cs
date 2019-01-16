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
            helper.Events.GameLoop.Saving += this.OnSaving;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnSaving(object sender, SavingEventArgs e)
        {
            if (Game1.IsFall && Game1.dayOfMonth == 27)
                Game1.weatherForTomorrow = Game1.weather_snow;
        }
    }
}
