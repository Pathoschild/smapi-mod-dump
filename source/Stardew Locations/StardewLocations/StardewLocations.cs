using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewLocations
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    internal class StardewLocations : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += onWarped;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer && Game1.player?.currentLocation != null)
                Game1.showGlobalMessage(getLocationName(Game1.player.currentLocation.Name));
        }

        /// <summary>Get the display name for an in-game location.</summary>
        /// <param name="name">The in-game location name.</param>
        private string getLocationName(string name)
        {
            var i18n = Helper.Translation;
            return "Current Location:\n\r" + i18n.Get(name, new
            {
                farm_name = Game1.player.farmName,
                player_name = Game1.player.Name
            });
        }
    }
}
