/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System;
using System.Collections.Generic;
using MailOrderPigs.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace MailOrderPigs
{
    public class MailOrderPigs : Mod
    {
        /*********
        ** Properties
        *********/
        private SButton MenuKey = SButton.PageUp;
        private ModConfig Config;
        private bool AllowOvercrowding;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                if (!Enum.TryParse(this.Config.KeyBind, true, out this.MenuKey))
                {
                    this.MenuKey = SButton.PageUp;
                    this.Monitor.Log($"Error parsing key binding; defaulted to {this.MenuKey}.");
                }

                this.AllowOvercrowding = this.Config.AllowOvercrowding;

                this.Monitor.Log("Mod loaded successfully.", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Mod not loaded successfully: {ex}", LogLevel.Error);
            }

        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == this.MenuKey)
            {
                this.Monitor.Log("Attempting to bring up menu.", LogLevel.Trace);
                if (Game1.currentLocation is AnimalHouse house)
                {
                    try
                    {
                        if (house.isFull() && !this.AllowOvercrowding)
                        {
                            this.Monitor.Log("Not bringing up menu: building is full.", LogLevel.Trace);
                            Game1.showRedMessage("This Building Is Full");
                        }
                        else
                        {
                            this.Monitor.Log("Bringing up menu.", LogLevel.Trace);
                            Game1.activeClickableMenu = new MailOrderPigMenu(this.GetPurchaseAnimalStock(), this.Helper.Multiplayer.GetNewID);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Monitor.Log($"Problem bringing up menu: {ex}", LogLevel.Error);
                    }
                }
                else
                {
                    this.Monitor.Log($"Problem bringing up menu: you are not in an animal house. Location name is: {Game1.currentLocation.Name}", LogLevel.Trace);
                }
            }
        }

        private List<Object> GetPurchaseAnimalStock()
        {
            string locationName = ((AnimalHouse)Game1.currentLocation).getBuilding().buildingType.Value;
            this.Monitor.Log($"Returning stock for building: {locationName}", LogLevel.Trace);

            return new List<Object>
            {
                new Object(100, 1, false, 400) { name = "Chicken", Type = locationName.Equals("Coop") || locationName.Equals("Deluxe Coop") || locationName.Equals("Big Coop") ? null : "You gotta be in a Coop" },
                new Object(100, 1, false, 750) { name = "Dairy Cow", Type = locationName.Equals("Barn") || locationName.Equals("Deluxe Barn") || locationName.Equals("Big Barn") ? null : "You gotta be in a Barn" },
                new Object(100, 1, false, 2000) { name = "Goat", Type = locationName.Equals("Big Barn") || locationName.Equals("Deluxe Barn") ? null : "You gotta be in a Big Barn" },
                new Object(100, 1, false, 2000) { name = "Duck", Type = locationName.Equals("Big Coop") || locationName.Equals("Deluxe Coop") ? null : "You gotta be in a Big Coop" },
                new Object(100, 1, false, 4000) { name = "Sheep", Type = locationName.Equals("Deluxe Barn") ? null : "You gotta be in a Deluxe Barn" },
                new Object(100, 1, false, 4000) { name = "Rabbit", Type = locationName.Equals("Deluxe Coop") ? null : "You gotta be in a Deluxe Coop" },
                new Object(100, 1, false, 8000) { name = "Pig", Type = locationName.Equals("Deluxe Barn") ? null : "You gotta be in a Deluxe Barn" }
            };
        }
    }
}
