using System;
using CalendarNotes;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace MenusEverywhere
{   
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if(Game1.activeClickableMenu is Billboard b && b.calendarDays != null && e.Button == SButton.MouseLeft)
            {

                int x = Game1.getMouseX();
                int y = Game1.getMouseY();
                
                b.calendarDays.ForEach(
                    delegate (ClickableTextureComponent day)
                    {
                        if(day.bounds.Contains(x, y))
                        {
                            Game1.activeClickableMenu = new NotesMenu(this.Monitor);
                            this.Monitor.Log(Game1.year + " " + Game1.currentSeason + " " + day.myID.ToString(), LogLevel.Debug);
                        }
                    }
                );
            }
        }
    }
}