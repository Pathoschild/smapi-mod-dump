using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace BetterSkullCavernFalling
{
    /// <summary>The entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Intercept annoying popup messages when jumping into holes in skull cavern.
            // Shows HUD message instead.
            SkullCavernFallMessageIntercepter.Intercept(this.Helper.Reflection);
        }
    }
}
