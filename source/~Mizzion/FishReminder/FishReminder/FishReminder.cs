/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using FishReminder.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace FishReminder
{
    public class FishReminder : Mod, IAssetEditor//To do Change the fish dictionary to int, string. That wasy I can cross reference based on int.
    {
        //private string fishNeeded;

        private FishConfig _config;
        //Modify the Assets
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/mail");
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
                char[] trimmer = { ',' };
                var f = GetNeededFishLocation();
                string um = "", des = "", fo = "", t = "", m = "", bw = "", b = "", w = "", s = "", bl = "", ws = "";

                string outter = "";

                var o = from i in f
                    orderby i.Value
                    select i;


                foreach (var i in o)
                {
                    if (i.Value.Contains("Under"))
                        um += $"{i.Key}, ";
                    if (i.Value.Contains("Des"))
                        des += $"{i.Key}, ";
                    if (i.Value.Contains("For"))
                        fo += $"{i.Key}, ";
                    if (i.Value.Contains("Tow"))
                        t += $"{i.Key}, ";
                    if (i.Value.Contains("Moun"))
                        m += $"{i.Key}, ";
                    if (i.Value.Contains("Back"))
                        bw += $"{i.Key}, ";
                    if (i.Value.Contains("Bea"))
                        b += $"{i.Key}, ";
                    if (i.Value.Contains("Woods"))
                        w += $"{i.Key}, ";
                    if (i.Value.Contains("Sewer"))
                        s += $"{i.Key}, ";
                    if (i.Value.Contains("BugLand"))
                        bl += $"{i.Key}, ";
                    if (i.Value.Contains("Witch"))
                        ws += $"{i.Key}, ";
                }

                var undergroundmine = string.IsNullOrEmpty(um) ? "" : $"Underground Mine^{um.Trim().TrimEnd(trimmer)}^^";
                var desert = string.IsNullOrEmpty(des) ? "" : $"Desert^{des.Trim().TrimEnd(trimmer)}^^";
                var forest = string.IsNullOrEmpty(fo) ? "" : $"Forest^{fo.Trim().TrimEnd(trimmer)}^^";
                var town = string.IsNullOrEmpty(t) ? "" : $"Town^{t.Trim().TrimEnd(trimmer)}^^";
                var mountain = string.IsNullOrEmpty(m) ? "" : $"Mountain^{m.Trim().TrimEnd(trimmer)}^^";
                var backwoods = string.IsNullOrEmpty(bw) ? "" : $"Back Woods^{bw.Trim().TrimEnd(trimmer)}^^";
                var beach = string.IsNullOrEmpty(b) ? "" : $"Beach^{b.Trim().TrimEnd(trimmer)}^^";
                var woods = string.IsNullOrEmpty(w) ? "" : $"Woods^{w.Trim().TrimEnd(trimmer)}^^";
                var sewer = string.IsNullOrEmpty(s) ? "" : $"Sewer^{s.Trim().TrimEnd(trimmer)}^^";
                var bugland = string.IsNullOrEmpty(bl) ? "" : $"Bug Land^{bl.Trim().TrimEnd(trimmer)}^^";
                var witchswamp = string.IsNullOrEmpty(ws) ? "" : $"Witch Swamp^{ws.Trim().TrimEnd(trimmer)}^^";
                //populate outter
                outter =
                    $"{undergroundmine}{desert}{forest}{town}{mountain}{backwoods}{beach}{woods}{sewer}{bugland}{witchswamp}";
                Game1.activeClickableMenu = new LetterViewerMenu(outter);
            }
            else if (e.IsDown(SButton.F9))
            {
                var fish = Helper.Content.Load<Dictionary<int, string> > ("Data\\fish", ContentSource.GameContent);
                //string[] locData = loc.Value.Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                foreach (var f in fish)
                {
                    var fdata = f.Value.Split('/');
                    Game1.player.fishCaught.Add(f.Key, new int[2] {1, 1});
                    Monitor.Log($"Added: {fdata[0]} to the list.");
                }
            }
        }

        /// <summary>Event that fires before a world is saved</summary>
        /// <param name="sender">The sender</param>
        /// /// <param name="e">Information related to which button was pressed</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            var today = SDate.Now();
            var tomorrow = today.AddDays(1);

            string fishNed = GetNeededFish();
            if (string.IsNullOrEmpty(fishNed))
                return;

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
            char[] trimmer = { ',' };
            var f = GetNeededFishLocation();
            string um = "", des = "", fo = "", t = "", m = "", bw = "", b = "", w = "", s = "", bl = "", ws = "";

            string outter = "";

            var o = from i in f
                    orderby i.Value
                    select i;


            foreach (var i in o)
            {
                if (i.Value.Contains("Under"))
                    um += $"{i.Key}, ";
                if (i.Value.Contains("Des"))
                    des += $"{i.Key}, ";
                if (i.Value.Contains("For"))
                    fo += $"{i.Key}, ";
                if (i.Value.Contains("Tow"))
                    t += $"{i.Key}, ";
                if (i.Value.Contains("Moun"))
                    m += $"{i.Key}, ";
                if (i.Value.Contains("Back"))
                    bw += $"{i.Key}, ";
                if (i.Value.Contains("Bea"))
                    b += $"{i.Key}, ";
                if (i.Value.Contains("Woods"))
                    w += $"{i.Key}, ";
                if (i.Value.Contains("Sewer"))
                    s += $"{i.Key}, ";
                if (i.Value.Contains("BugLand"))
                    bl += $"{i.Key}, ";
                if (i.Value.Contains("Witch"))
                    ws += $"{i.Key}, ";
            }

            var undergroundmine = string.IsNullOrEmpty(um) ? "" : $"Underground Mine^{um.Trim().TrimEnd(trimmer)}^^";
            var desert = string.IsNullOrEmpty(des) ? "" : $"Desert^{des.Trim().TrimEnd(trimmer)}^^";
            var forest = string.IsNullOrEmpty(fo) ? "" : $"Forest^{fo.Trim().TrimEnd(trimmer)}^^";
            var town = string.IsNullOrEmpty(t) ? "" : $"Town^{t.Trim().TrimEnd(trimmer)}^^";
            var mountain = string.IsNullOrEmpty(m) ? "" : $"Mountain^{m.Trim().TrimEnd(trimmer)}^^";
            var backwoods = string.IsNullOrEmpty(bw) ? "" : $"Back Woods^{bw.Trim().TrimEnd(trimmer)}^^";
            var beach = string.IsNullOrEmpty(b) ? "" : $"Beach^{b.Trim().TrimEnd(trimmer)}^^";
            var woods = string.IsNullOrEmpty(w) ? "" : $"Woods^{w.Trim().TrimEnd(trimmer)}^^";
            var sewer = string.IsNullOrEmpty(s) ? "" : $"Sewer^{s.Trim().TrimEnd(trimmer)}^^";
            var bugland = string.IsNullOrEmpty(bl) ? "" : $"Bug Land^{bl.Trim().TrimEnd(trimmer)}^^";
            var witchswamp = string.IsNullOrEmpty(ws) ? "" : $"Witch Swamp^{ws.Trim().TrimEnd(trimmer)}^^";
            //populate outter
            //outter =
                //$"{undergroundmine}{desert}{forest}{town}{mountain}{backwoods}{beach}{woods}{sewer}{bugland}{witchswamp}";
            /*
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
            }*/
            //return neededFish
            //return neededFish.Trim().TrimEnd(trimmer);
            return $"{undergroundmine}{desert}{forest}{town}{mountain}{backwoods}{beach}{woods}{sewer}{bugland}{witchswamp}";
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
                        if (!fishNames.ContainsKey(fishData[0]) && 
                            !Game1.player.fishCaught.ContainsKey(Convert.ToInt32(locData[i])))
                            fishNames.Add(fishData[0], loc.Key);
                    }
                }
                    
            }

            return fishNames;
        }
    }
}
