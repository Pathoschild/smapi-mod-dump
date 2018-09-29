using Omegasis.BuyBackCollectables.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.BuyBackCollectables
{
    /// <summary>The mod entry point.</summary>
    public class BuyBackCollectables : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsPlayerFree && e.KeyPressed.ToString() == this.Config.KeyBinding)
                Game1.activeClickableMenu = new BuyBackMenu(this.Config.CostMultiplier);
        }
    }
}
