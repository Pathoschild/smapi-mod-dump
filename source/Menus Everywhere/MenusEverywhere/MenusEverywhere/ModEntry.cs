using System;
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
        /// <summary> The mod configuration from the player. </summary>
        private ModConfig Config;

        /// <summary> The dimensions of the menu. </summary>
        int width;
        int height;

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
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // Only opens a menu if the player isn't in an animation
            if (Game1.player.canMove)
            {
                if (e.Button == this.Config.CalendarKey)
                {
                    Game1.activeClickableMenu = new Billboard();
                    return;
                }
                else if (e.Button == this.Config.RequestKey)
                {
                    Game1.activeClickableMenu = new Billboard(true);
                    return;
                }
                else if (e.Button == this.Config.MonsterEradicationKey)
                {
                    AdventureGuild ag = new AdventureGuild();
                    ag.showMonsterKillList();
                    return;
                }
                else if (this.Config.CanAccessBin && e.Button == this.Config.BinKey)
                {
                    // To open the shipping bin, a click is simulated on the bin on the farm
                    Game1.getFarm().checkAction(new xTile.Dimensions.Location(71, 13), Game1.viewport, Game1.player);
                    return;
                }
            }

            // Changes the bundles to allow deposits from anywhere when the menu is open
            if (this.Config.CanAccessBundles && Game1.activeClickableMenu is JunimoNoteMenu menu)
            {
                menu.bundles.ForEach(
                    delegate (Bundle bundle)
                    {
                        bundle.depositsAllowed = true;
                    }
                );
            }
        }
    }
}