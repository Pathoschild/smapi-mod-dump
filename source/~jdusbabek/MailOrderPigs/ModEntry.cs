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
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        //private SButton MenuKey = SButton.PageUp;
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            //helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /*private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                if (!Enum.TryParse(Config.ToggleKey, true, out MenuKey))
                {
                    MenuKey = SButton.PageUp;
                    Monitor.Log($"Error parsing key binding; defaulted to {MenuKey}.");
                }

                AllowOvercrowding = Config.AllowOvercrowding;

                Monitor.Log("Mod loaded successfully.");
            }
            catch (Exception ex)
            {
                Monitor.Log($"Mod not loaded successfully: {ex}", LogLevel.Error);
            }

        }*/

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;
            if (!Config.ToggleKey.JustPressed()) return;
            Monitor.Log("Attempting to bring up menu.");
            if (Game1.currentLocation is AnimalHouse house)
            {
                try
                {
                    if (house.isFull())
                    {
                        Monitor.Log("Not bringing up menu: building is full.");
                        Game1.showRedMessage(
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321"));
                    }
                    else
                    {
                        Monitor.Log("Bringing up menu.");
                        Game1.activeClickableMenu = new MailOrderPigMenu(GetPurchaseAnimalStock(),
                            Helper.Multiplayer.GetNewID);
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Problem bringing up menu: {ex}", LogLevel.Error);
                }
            }
            else
            {
                Monitor.Log(
                    $"Problem bringing up menu: you are not in an animal house. Location name is: {Game1.currentLocation.Name}");
            }
        }
        
        private List<Object> GetPurchaseAnimalStock()
        {
            var locationName = ((AnimalHouse) Game1.currentLocation).getBuilding().buildingType.Value;
            Monitor.Log($"Returning stock for building: {locationName}");
            return new List<Object>
            {
                new Object(100, 1, false, 400)
                {
                    name = "Chicken",
                    Type = locationName.Equals("Coop") || locationName.Equals("Deluxe Coop") ||
                           locationName.Equals("Big Coop")
                        ? null
                        : "You have to be in a Coop"
                },
                new Object(100, 1, false, 750)
                {
                    name = "Dairy Cow",
                    Type = locationName.Equals("Barn") || locationName.Equals("Deluxe Barn") ||
                           locationName.Equals("Big Barn")
                        ? null
                        : "You have to be in a Barn"
                },
                new Object(100, 1, false, 2000)
                {
                    name = "Goat",
                    Type = locationName.Equals("Big Barn") || locationName.Equals("Deluxe Barn")
                        ? null
                        : "You have to be in a Big Barn"
                },
                new Object(100, 1, false, 600)
                {
                    name = "Duck",
                    Type = locationName.Equals("Big Coop") || locationName.Equals("Deluxe Coop")
                        ? null
                        : "You have to be in a Big Coop"
                },
                new Object(100, 1, false, 4000)
                {
                    name = "Sheep",
                    Type = locationName.Equals("Deluxe Barn") ? null : "You have to be in a Deluxe Barn"
                },
                new Object(100, 1, false, 4000)
                {
                    name = "Rabbit",
                    Type = locationName.Equals("Deluxe Coop") ? null : "You have to be in a Deluxe Coop"
                },
                new Object(100, 1, false, 8000)
                {
                    name = "Pig",
                    Type = locationName.Equals("Deluxe Barn") ? null : "You have to be in a Deluxe Barn"
                }
            };
        }
    }
}