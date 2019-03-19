using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishReminder.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace FishReminder
{
    public class FishReminder : Mod, IAssetEditor
    {
        //private string fishNeeded;

        private FishConfig _config;
        //Modify the Assets
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
            {
                var i18n = Helper.Translation;
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["fishReminderDaily"] = i18n.Get("fishReminderDaily",
                    new {player = Game1.player.Name, fish = GetNeededFish()});

                data["fishReminderWeekly"] = i18n.Get("fishReminderWeekly",
                    new {player = Game1.player.Name, fish = GetNeededFish()});

                data["fishReminderMonthly"] = i18n.Get("fishReminderMonthly",
                    new {player = Game1.player.Name, fish = GetNeededFish()});
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<FishConfig>();
            //Events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            //helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        //Event methods
        /// <summary>Event that fires when a button is pressed</summary>
        /// <param name="sender">The sender</param>
        /// /// <param name="e">Information related to which button was pressed</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            //Process button pressed
            if (e.IsDown(SButton.F6))
            {
                string fishes = GetNeededFish(true, true);
                Monitor.Log(fishes);
            }
            else if (e.IsDown(SButton.F7))
            {
                
                var f = GetNeededFishLocation();
                string outter = "";

                foreach (var i in f)
                {
                    outter += $"{i.Key}({i.Value}), ";
                }
                Game1.activeClickableMenu = new LetterViewerMenu(outter);
            }
        }

        /// <summary>Event that fires before a world is saved</summary>
        /// <param name="sender">The sender</param>
        /// /// <param name="e">Information related to which button was pressed</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            var today = SDate.Now();
            var tomorrow = today.AddDays(1);

            switch (tomorrow.Day)
            {
                case 1:
                    if (_config.SendReminderMailMonthly)
                    {
                        Helper.Content.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add("fishReminderMonthly");
                    }
                    break;
                case 8:
                case 15:
                case 22:
                    if (_config.SendReminderMailWeekly)
                    {
                        Helper.Content.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add("fishReminderWeekly");
                    }
                    break;
                default:
                    if (_config.SendReminderMailDaily)
                    {
                        Helper.Content.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add("fishReminderDaily");
                    }

                    break;
            }
        }

        //Private Methods
        /// <summary>Grabs the fish that a player still needs.</summary>
        /// <param name="doSeason">Whether or not the current season should be accounted for.</param>
        /// <param name="doWeather">Whether or not the current weather should be accounted for. Not implemented yet</param>
        private string GetNeededFish(bool doSeason = true, bool doWeather = false)
        {
            string neededFish = "";
            char[] trimmer = {','};

            //Open the fish data file, and grab the needed info
            IDictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\fish");
            
            //Go through and grab what fish the player needs based on season
            foreach (var f in fish)
            {
                var fishSplit = f.Value.Split('/');
                var seasons = fishSplit[6].Split(' ');
                //var weather = fishSplit[7];

                //Make sure the player needs the fish.
                if (!Game1.player.fishCaught.ContainsKey(f.Key) && doSeason && seasons.Contains(Game1.currentSeason))
                {
                    neededFish += $"{fishSplit[0]}, ";
                }
            }
            //return neededFish
            return neededFish.Trim().TrimEnd(trimmer);
        }

        /// <summary>Grabs the fish that a player still needs.</summary>
        /// <param name="doSeason">Whether or not the current season should be accounted for.</param>
        /// <param name="doWeather">Whether or not the current weather should be accounted for. Not implemented yet</param>
        private Dictionary<string,string> GetNeededFishLocation(bool doSeason = true, bool doWeather = false)
        {
             Dictionary<string, string> fishNames = new Dictionary<string, string>();

            //Open the fish data file, and grab the needed info
            IDictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\fish");

            //Open Location, so we can gather the fish data based on it
            IDictionary<string, string> locations = Game1.content.Load<Dictionary<string, string>>("Data\\locations");

            foreach (var loc in locations)
            {
                string[] locData = loc.Value.Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                //go through the location data for the fish that can be gotten from there.
                if (locData.Length > 1)
                {
                    for (int i = 0; i < locData.Length; i += 2)
                    {
                        string[] fishData = fish[Convert.ToInt32(locData[i])].Split('/');
                        if (!fishNames.ContainsKey(fishData[0]))
                            fishNames.Add(fishData[0], loc.Key);
                    }
                }
                    
            }

            return fishNames;
        }
    }
}
