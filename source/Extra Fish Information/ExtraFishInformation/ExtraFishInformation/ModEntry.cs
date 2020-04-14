using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

// TODO: figure out why fish from content packs, e.g. more new fish, are not being handled

namespace ExtraFishInformation
{

    // MOD ENTRY CLASS

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        // PUBLIC VARIABLES
        IDictionary<int, string> updatedDescriptions = new Dictionary<int, string>();


        // PRIVATE VARIABLES
        private ModConfig Config;
        private ITranslationHelper i18n;
        private bool timeIn24Hours;


        // PUBLIC METHODS

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // get data frm config.json - try/catched to handle errors
            timeIn24Hours = true;
            try
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
                timeIn24Hours = this.Config.TimeIn24Hours;
            }
            catch (Exception e)
            {
                this.Monitor.Log($"There was an error with parsing config.json. 24 hour time will be used for fish info, by default.", LogLevel.Error);
            }

            // set up helper for translations
            i18n = helper.Translation;
            if (!i18n.GetTranslations().Any())
            {
                this.Monitor.Log($"There was an error in locating translation files. Please try re-installing this mod to fix this.", LogLevel.Error);
            }

            // load mod
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        } // end Entry method


        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/ObjectInformation");

        } // end CanEdit method


        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (!asset.AssetNameEquals("Data/ObjectInformation")) return;
            // updates descriptions with new descriptions
            IDictionary<int, string> objectData = asset.AsDictionary<int, string>().Data;
            foreach (KeyValuePair<int, string> item in updatedDescriptions)
            {
                objectData[item.Key] = item.Value;
            }

        } // end Edit method


        // PRIVATE METHODS

        /// <summary>"Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations." [see more at: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events] </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // read in required data files
            IDictionary<int, string> objectInfo = this.Helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);
            IDictionary<int, string> fishInfo = this.Helper.Content.Load<Dictionary<int, string>>("Data/Fish", ContentSource.GameContent);
            IDictionary<string, string> locationInfo = this.Helper.Content.Load<Dictionary<string, string>>("Data/Locations", ContentSource.GameContent);

            // set up a more easy to handle version of the locations data
            IDictionary<string, string[]> seasonalLocationInfo = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string> item in locationInfo)
            {
                string[] locationInfoSections = item.Value.Split('/');
                string[] newArray = new string[4];
                Array.Copy(locationInfoSections, 4, newArray, 0, 4);
                seasonalLocationInfo[item.Key] = newArray;
            }

            // set up dict for new info
            IDictionary<int, string> newInfo = new Dictionary<int, string>();

            // get fish info from each fish
            foreach (KeyValuePair<int, string> item in objectInfo)
            {
                try // catch exceptions
                {
                    // get info from ObjectInformation
                    string objectItemInfo = item.Value;
                    if (!objectItemInfo.Contains("Fish -4")) continue;  // ignore non-fish items
                    int fishId = item.Key;
                    string[] objectItemSections = objectItemInfo.Split('/');
                    string description = objectItemSections[5];  // get object description

                    // get info from Fish
                    string fishItemInfo = fishInfo[fishId];  // get individual fish info 
                    string[] fishInfoSections = fishItemInfo.Split('/');  // sections are 0-12 (or 0-13 for localisations) for all except trapper fish which are 0-6 (or 0-7 for localisations) [see sdv wiki for more info]
                    string newDescription;

                    // handle each fish
                    if ((fishInfoSections.Length == 7) || (fishInfoSections.Length == 8)) // handle trapper fish
                    {
                        string location = fishInfoSections[4];
                        string extraInfo = "";
                        if (location.Equals("ocean"))
                        {
                            extraInfo = i18n.Get("new-description.trapper.fish", new { location = i18n.Get("location.ocean") });
                        }
                        else if (location.Equals("freshwater"))
                        {
                            extraInfo = i18n.Get("new-description.trapper.fish", new { location = i18n.Get("location.freshwater") });
                        }
                        else
                        {
                            extraInfo = i18n.Get("new-description.trapper.fish", new { location = i18n.Get("location.unknown") });
                        }

                        newDescription = description + extraInfo;
                    }
                    else  // handle all other fish
                    {
                        string locations = ParseLocation(fishId, seasonalLocationInfo);
                        string weather = ParseWeather(fishInfoSections[7]);
                        string schedule = ParseTimes(fishInfoSections[5], timeIn24Hours);
                        string extraInfo = i18n.Get("new-description.normal.fish", new { weather = weather, schedule = schedule, locations = locations });
                        newDescription = description + extraInfo;
                    } // end if/else statement

                    // repack object sections
                    objectItemSections[5] = newDescription;
                    string newObjectInfo = string.Join("/", objectItemSections);
                    newInfo[item.Key] = newObjectInfo;

                }
                catch (KeyNotFoundException exception) // key not found exception: likely from fish added from another mod
                {
                    this.Monitor.Log($"Error! Key not found for fish: {item}\nExtra fish information will not be added for this fish.", LogLevel.Error);
                    // should not crash game: fish that do not throw this exception should still be loaded
                }
                catch (Exception exception) // catch all other exceptions: no other exceptions expected
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                    this.Monitor.Log($"Error! {exception.GetType().Name}: {exception.Message} [Line number {trace.GetFrame(0).GetFileLineNumber()}]", LogLevel.Error);
                }

            } // end foreach fish

            // update descriptions to include new info for each fish
            updatedDescriptions = newInfo;
            this.Helper.Content.InvalidateCache("Data/ObjectInformation");

        } // end OnGameLaunched method


        /// <summary>A helper method to parse location and season information from the edited version of Locations.xnb </summary>
        /// <param name="fishId">Integer denoting fish being handled.</param>
        /// <param name="seasonalLocationInfo">IDictionary of edited version of Locations.xnb information.</param>
        /// <param name="i18n">Helper for translations.</param>
        /// <returns>Parsed locations/seasons data, to add directly to description.</returns>
        private string ParseLocation(int fishId, IDictionary<string, string[]> seasonalLocationInfo)
        {
            string foundLocations = "";
            string[] seasonNames = { i18n.Get("season.spring"), i18n.Get("season.summer"), i18n.Get("season.fall"), i18n.Get("season.winter") };

            foreach (KeyValuePair<string, string[]> location in seasonalLocationInfo)
            {
                // TODO: convert namesLikeThis to Names Like This
                string locationName = ParseLocationName(location.Key);

                // locations to ignore: Temp, Fishing Game, 
                if (locationName == "Temp")
                { break; }
                if (locationName == "FishingGame") // TODO: change to "Fishing Game" once locations have been properly parsed
                { break; }

                string foundSeasons = "";
                string allSeasons = $"{seasonNames[0]}, {seasonNames[1]}, {seasonNames[2]}, {seasonNames[3]}";
                string[] seasonInfo = location.Value;

                for (int i = 0; i < seasonInfo.Length; i++)
                {
                    if (seasonInfo[i].Contains(fishId.ToString()))
                    {
                        // TODO: handle Forest differently - Forest has 3 different areas -1/0/1
                        if (foundSeasons.Length == 0)
                        { foundSeasons = $"{seasonNames[i]}"; }
                        else
                        { foundSeasons = $"{foundSeasons}, {seasonNames[i]}"; }
                    } // end if
                } // end for loop

                if (foundSeasons.Length != 0)
                {
                    // change seasons to either blank (all seasons) or to be in brackets
                    if (foundSeasons == allSeasons)
                    { foundSeasons = ""; }
                    else
                    { foundSeasons = $" ({foundSeasons})"; }
                    // add new seasons & locations info to info for description
                    if (foundLocations.Length != 0)
                    { foundLocations = $"{foundLocations}\n- {locationName}{foundSeasons}"; }
                    else
                    { foundLocations = $"- {locationName}{foundSeasons}"; }
                } // end if

            } // end foreach loop

            if (foundLocations.Length == 0)
            { foundLocations = "?"; }

            return foundLocations;

        } // end ParseLocation method


        /// <summary>A helper method to parse location names from namesLikeThis to Names Like This.</summary>
        /// <param name="weather">String containing unparsed location name.</param>
        /// <returns>String containing parsed location name.</returns>
        private string ParseLocationName(string locationName)
        {
            // convert to NamesLikeThis
            string parsedLocationName = $"{locationName.First().ToString().ToUpper()}{locationName.Substring(1)}";

            // TODO: convert to Names Like This

            // return parsed location name
            return parsedLocationName;

        } // end ParseLocationName method


        /// <summary>A helper method to parse weather information from Fish.xnb. </summary>
        /// <param name="weather">String containing unparsed weather data.</param>
        /// <param name="i18n">Helper for translations.</param>
        /// <returns>Parsed weather data, to add directly to description.</returns>
        private string ParseWeather(string weather)
        {
            string parsedWeather;
            switch (weather)
            {
                case "sunny":
                    parsedWeather = i18n.Get("weather.sunny");
                    break;
                case "rainy":
                    parsedWeather = i18n.Get("weather.rainy");
                    break;
                case "both":
                    parsedWeather = i18n.Get("weather.both");
                    break;
                default:
                    parsedWeather = i18n.Get("weather.unknown");
                    break;
            } // end switch/case

            return parsedWeather;

        } // end ParseWeather method


        /// <summary>A helper method to parse active times from from Fish.xnb.</summary>
        /// <param name="times">String containing unparsed time data.</param>
        /// <param name="timeIn24Hours">Boolean to show whether time should be in 24 or 12 hour format.</param>
        /// <param name="i18n">Helper for translations.</param>
        /// <returns>Parsed time data, to add directly to description.</returns>
        private string ParseTimes(string times, bool timeIn24Hours)
        {
            string parsedTimes = "";
            string[] splitTimes = times.Split(' ');

            if (splitTimes.Length == 2)
            {
                // split times into start/end times
                string startTime = splitTimes[0];
                string endTime = splitTimes[1];

                // parse time strings from XXX or XXXX to XX:XX
                startTime = ParseTimeString(startTime, timeIn24Hours);
                endTime = ParseTimeString(endTime, timeIn24Hours);

                // add parsed times to new string
                parsedTimes = i18n.Get("schedule.between.times", new { startTime = startTime, endTime = endTime });
            }
            else
            {
                for (int i = 0; i < splitTimes.Length; i += 2)
                {
                    // split times into start/end times
                    string startTime = splitTimes[i];
                    string endTime = splitTimes[i + 1];

                    // parse time strings from XXX or XXXX to XX:XX
                    startTime = ParseTimeString(startTime, timeIn24Hours);
                    endTime = ParseTimeString(endTime, timeIn24Hours);

                    // add parsed times to new string
                    if (i == 0)
                    {
                        parsedTimes = i18n.Get("schedule.between.times", new { startTime = startTime, endTime = endTime });
                    }
                    else if (i == splitTimes.Length - 2)
                    {
                        parsedTimes = i18n.Get("schedule.join.times", new { parsedTimes = parsedTimes, startTime = startTime, endTime = endTime });
                    }
                    else
                    {
                        parsedTimes = i18n.Get("schedule.more.times", new { parsedTimes = parsedTimes, startTime = startTime, endTime = endTime });
                    }
                    // end if/else

                } // end for loop

            } // end if/else

            return parsedTimes;

        } // end ParseTimes method


        /// <summary>A helper method for ParseTimes() to parse time strings from from Fish.xnb.</summary>
        /// <param name="timeToParse">String containing unparsed time data in XXX or XXXX form.</param>
        /// <param name="timeIn24Hours">Boolean to show whether time should be in 24 or 12 hour format.</param>
        /// <param name="i18n">Helper for translations.</param>
        /// <returns>Parsed time data in XX:XX.</returns>
        private string ParseTimeString(string timeToParse, bool timeIn24Hours)
        {
            string newTime = "";
            if (timeToParse.Length == 3)
            {
                string hour = timeToParse.Substring(0, 1);
                string minutes = timeToParse.Substring(1);
                newTime = i18n.Get("time.24hour.format", new { hour = hour, minutes = minutes });
            }
            else if (timeToParse.Length == 4)
            {
                string hour = timeToParse.Substring(0, 2);
                string minutes = timeToParse.Substring(2);
                if (Int32.Parse(hour) > 24)
                {
                    hour = (Int32.Parse(hour) - 24).ToString();
                }
                newTime = i18n.Get("time.24hour.format", new { hour = hour, minutes = minutes });
            }
            else
            {
                newTime = timeToParse;
            }

            // put those in X:XX format into XX:XX format -- only checking for a ":" because only English needs this done
            if (newTime.Length == 4 && newTime.Contains(":"))
            {
                newTime = newTime.Insert(0, "0");
            }

            if (timeIn24Hours == false)
                { newTime = ConvertTo12HourTime(newTime); }

            return newTime;

        } // end ParseTimeString method


        /// <summary>A helper method for ParseTimeString() to parse time strings from 24 hour format to 12 hour format.</summary>
        /// <param name="time24Hours">String containing 24 hour time in XX:XX format.</param>
        /// <returns>Time in 12 hour format in XX.XXam/pm format.</returns>
        private string ConvertTo12HourTime(string time24Hours)
        {
            string time12Hours = "";
            int hour = 0;
            int minutes = 0;

            // languages handle time differently
            switch (i18n.LocaleEnum.ToString())
            {
                case "en":
                    hour = Int32.Parse(time24Hours.Substring(0, 2));
                    minutes = Int32.Parse(time24Hours.Substring(3));
                    break;
                case "fr":
                    hour = Int32.Parse(time24Hours.Substring(0, time24Hours.IndexOf("h")));
                    minutes = Int32.Parse(time24Hours.Substring(time24Hours.IndexOf("h") + 1));
                    break;
                default: 
                    hour = Int32.Parse(time24Hours.Substring(0, 2));
                    minutes = Int32.Parse(time24Hours.Substring(3));
                    break;
            }

            // parse & convert to 12hr time
            if (hour >= 0 && hour <= 11)
            {
                if (minutes == 0)
                    { time12Hours = i18n.Get("time.morning.12hour", new { hour = hour }); }
                else
                    { time12Hours = i18n.Get("time.morning.12hour.minutes", new { hour = hour, minutes = minutes }); }
            }
            else if (hour == 12)
            {
                if (minutes == 0)
                    { time12Hours = i18n.Get("time.afternoon.12hour", new { hour = hour }); }
                else
                    { time12Hours = i18n.Get("time.afternoon.12hour.minutes", new { hour = hour, minutes = minutes }); }
            }
            else if (hour >= 13 && hour <= 23)
            {
                if (minutes == 0)
                { time12Hours = i18n.Get("time.afternoon.12hour", new { hour = hour - 12 }); }
                else
                { time12Hours = i18n.Get("time.afternoon.12hour.minutes", new { hour = hour - 12, minutes = minutes }); }
            }
            else if (hour == 24)
            {
                if (minutes == 0)
                    { time12Hours = i18n.Get("time.morning.12hour", new { hour = hour - 12 }); }
                else
                    { time12Hours = i18n.Get("time.morning.12hour.minutes", new { hour = hour - 12, minutes = minutes }); }
            }

            // return time in 12hr format
            return time12Hours;

        } // end ConvertTo12HourTime method


    } // end ModEntry class


    // MOD CONFIG CLASS

    /// <summary>Generates config.json.</summary>
    class ModConfig
    {
        public bool TimeIn24Hours { get; set; } = true;
    } // end Mod Config class


} // end namespace