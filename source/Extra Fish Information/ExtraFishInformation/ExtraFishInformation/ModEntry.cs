/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceasg/StardewValleyMods
**
*************************************************/

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
    public class ModEntry : Mod
    { 
        // PRIVATE VARIABLES
        private ModConfig Config;
        private ITranslationHelper i18n;
        private bool timeIn24Hours;


        // PUBLIC METHODS

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // set default time settings
            timeIn24Hours = true;

            // get data from config.json
            try // try-catch to handle errors
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
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

        } // end Entry method


        // PRIVATE METHODS

        /// <summary>"Raised when an asset is being requested from the content pipeline. The asset isn't necessarily being loaded yet (e.g. the game may be checking if it exists). " [see more at: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events] </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // process only if asset requested is Data/ObjectInformation
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {

                // attempt to edit the Data/ObjectInformation asset
                e.Edit(asset =>
                {
                    // get Data/ObjectInformation file
                    IDictionary<int, string> objectInfo = asset.AsDictionary<int, string>().Data;

                    // read in other content assets from game folder
                    IDictionary<int, string> fishInfo = this.Helper.GameContent.Load<Dictionary<int, string>>("Data/Fish");
                    IDictionary<string, string> locationInfo = this.Helper.GameContent.Load<Dictionary<string, string>>("Data/Locations");

                    // set up a more easy to handle version of the locations data
                    IDictionary<string, string[]> seasonalLocationInfo = new Dictionary<string, string[]>();
                    foreach (KeyValuePair<string, string> item in locationInfo)
                    {
                        string[] locationInfoSections = item.Value.Split('/');
                        string[] newArray = new string[4];
                        Array.Copy(locationInfoSections, 4, newArray, 0, 4);
                        seasonalLocationInfo[item.Key] = newArray;
                    }

                    // get fish info from each fish, then process the fish and update its description
                    foreach (KeyValuePair<int, string> item in objectInfo)
                    {

                        try // try-catch to handle exceptions
                        {
                            // get info from Data/ObjectInformation content
                            string objectItemInfo = item.Value;
                            if (!objectItemInfo.Contains("Fish -4")) continue;  // ignore non-fish items
                            int fishId = item.Key;
                            string[] objectItemSections = objectItemInfo.Split('/');
                            string description = objectItemSections[5];  // get object description

                            // get info from Fish
                            string fishItemInfo = fishInfo[fishId];  // get individual fish info 
                            string[] fishInfoSections = fishItemInfo.Split('/');  // sections are 0-12 (or 0-13 for localisations) for all except trapper fish which are 0-6 (or 0-7 for localisations) [see sdv wiki for more info]

                            // set up string for new fish description
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

                                newDescription = $"{description} {extraInfo}";
                            }
                            else  // handle all other fish
                            {
                                string locations = ParseLocation(fishId, seasonalLocationInfo);
                                string weather = ParseWeather(fishInfoSections[7]);
                                string schedule = ParseTimes(fishInfoSections[5], timeIn24Hours);
                                string extraInfo = i18n.Get("new-description.normal.fish", new { weather = weather, schedule = schedule, locations = locations });
                                newDescription = $"{description} {extraInfo}";
                            } // end if/else statement

                            // repack object sections
                            objectItemSections[5] = newDescription;
                            string newObjectInfo = string.Join("/", objectItemSections);

                            // update Data/ObjectInformation with new info
                            objectInfo[item.Key] = newObjectInfo;

                        } // end of try {}, moving on to catches:

                        catch (KeyNotFoundException exception) // key not found exception: likely from fish added from another mod
                        {
                            // should not crash game: fish that do not throw this exception should still be loaded
                            this.Monitor.Log($"Error! Key not found for fish: {item}\nExtra fish information will not be added for this fish.", LogLevel.Error);
                        }

                        catch (Exception exception) // catch all other exceptions: no other exceptions expected
                        {
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                            this.Monitor.Log($"Error! {exception.GetType().Name}: {exception.Message} [Line number {trace.GetFrame(0).GetFileLineNumber()}]", LogLevel.Error);
                        }
                        // end of try-catch section

                    } // end foreach fish

                }); // end e.Edit

            } // end if e.NameWithoutLocale

        } // end OnAssetRequested method


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

                // setup arrays for parsing the data from each season for the location
                string foundSeasons = "";
                string allSeasons = $"{seasonNames[0]}, {seasonNames[1]}, {seasonNames[2]}, {seasonNames[3]}";
                string[] seasonInfo = location.Value;

                // parse the fishes in each season
                for (int i = 0; i < seasonInfo.Length; i++)
                {
                    if (seasonInfo[i].Contains(fishId.ToString()))
                    {
                        // TODO: handle Forest and IslandWest differently - these have different areas within them
                        if (foundSeasons.Length == 0)
                        { foundSeasons = $"{seasonNames[i]}"; }
                        else
                        { foundSeasons = $"{foundSeasons}, {seasonNames[i]}"; }
                    } // end if
                } // end for loop

                // if there were fish found in the season, update the fish's description with the info
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

            // if the fish was not found in any location data, consider it unknown
            if (foundLocations.Length == 0)
            { foundLocations = "?"; }

            return foundLocations;

        } // end ParseLocation method


        /// <summary>A helper method to parse location names from namesLikeThis to Names Like This.</summary>
        /// <param name="locationName">String containing unparsed location name.</param>
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