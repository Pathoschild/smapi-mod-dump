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
        ** Fields
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

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // open menu
            if (e.Button == this.Config.ShowMenuKey)
            {
                if (Game1.activeClickableMenu != null)
                    return;
                if (Game1.player.currentLocation is LibraryMuseum)
                    Game1.activeClickableMenu = this.OpenMenu = new NewMuseumMenu(this.Helper.Reflection);
                else
                    this.Monitor.Log("You can't rearrange the museum here.");
            }

            // toggle inventory box
            if (e.Button == this.Config.ToggleInventoryKey)
                this.OpenMenu?.ToggleInventory();
        }
    }
}
