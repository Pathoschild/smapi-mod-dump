using Omegasis.MuseumRearranger.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace Omegasis.MuseumRearranger
{
    /// <summary>The mod entry point.</summary>
    public class MuseumRearranger : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The open museum menu (if any).</summary>
        private NewMuseumMenu OpenMenu;


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
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;

            // open menu
            if (e.KeyPressed.ToString() == this.Config.ShowMenuKey)
            {
                if (Game1.activeClickableMenu != null)
                    return;
                if (Game1.player.currentLocation is LibraryMuseum)
                    Game1.activeClickableMenu = this.OpenMenu = new NewMuseumMenu(this.Helper.Reflection);
                else
                    this.Monitor.Log("You can't rearrange the museum here.");
            }

            // toggle inventory box
            if (e.KeyPressed.ToString() == this.Config.ToggleInventoryKey)
                this.OpenMenu?.ToggleInventory();
        }
    }
}
