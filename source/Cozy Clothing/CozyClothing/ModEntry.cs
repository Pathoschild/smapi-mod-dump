/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CompSciLauren/stardew-valley-cozy-clothing-mod
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CozyClothing
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        private bool currentlyInPajamas = false;

        // previous clothes
        private int previousShirt;
        private int previousPantStyle;
        private Color previousPantsColor;
        private int previousShoeColor;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after the save file is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        /// <summary>Raised after the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.GameLoop.DayStarted -= OnDayStarted;
            Helper.Events.Player.Warped -= OnWarped;
            Helper.Events.GameLoop.DayEnding -= OnDayEnding;

            if (currentlyInPajamas)
            {
                ChangeIntoRegularClothes();
            }
        }

        /// <summary>Raised after the day has started.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!currentlyInPajamas)
            {
                ChangeIntoPajamas();
            }
        }

        /// <summary>Raised after the day is ending.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (currentlyInPajamas && Game1.currentLocation is StardewValley.Locations.FarmHouse)
            {
                ChangeIntoRegularClothes();
            }
        }

        /// <summary>Raised after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is Farm && e.OldLocation is StardewValley.Locations.FarmHouse && currentlyInPajamas)
            {
                ChangeIntoRegularClothes();
            }
            else if (e.NewLocation is StardewValley.Locations.FarmHouse && e.OldLocation is Farm && !currentlyInPajamas)
            {
                ChangeIntoPajamas();
            }
        }

        /// <summary>Removes pajamas and replaces them with previously worn clothes.</summary>
        private void ChangeIntoRegularClothes()
        {
            // Change out of pajamas and back into previous clothes
            Game1.player.changeShirt(previousShirt);
            Game1.player.changePantStyle(previousPantStyle);
            Game1.player.changePants(previousPantsColor);
            Game1.player.changeShoeColor(previousShoeColor);

            currentlyInPajamas = false;
        }

        /// <summary>Removes current clothes and replaces them with pajamas.</summary>
        private void ChangeIntoPajamas()
        {
            // save current clothes to change back into later
            previousShirt = Game1.player.shirt.Value;
            previousPantStyle = Game1.player.pants.Value;
            previousPantsColor = Game1.player.pantsColor;
            previousShoeColor = Game1.player.shoes.Value;

            // change current clothes to be pajamas
            Game1.player.changePantStyle(0);
            Game1.player.changeShoeColor(4);

            switch (Config.PajamaColor)
            {
                case "Pink":
                    Game1.player.changeShirt(36);
                    Game1.player.changePants(Color.PaleVioletRed);
                    break;
                case "Purple":
                    Game1.player.changeShirt(40);
                    Game1.player.changePants(Color.MediumPurple);
                    break;
                case "Green":
                    Game1.player.changeShirt(96);
                    Game1.player.changePants(Color.LimeGreen);
                    break;
                case "Water-Blue":
                    Game1.player.changeShirt(105);
                    Game1.player.changePants(Color.RoyalBlue);
                    break;
                case "Blue":
                default:
                    Game1.player.changeShirt(9);
                    Game1.player.changePants(Color.DarkTurquoise);
                    Game1.player.changeShoeColor(6);
                    break;
            }

            currentlyInPajamas = true;
        }
    }
}
