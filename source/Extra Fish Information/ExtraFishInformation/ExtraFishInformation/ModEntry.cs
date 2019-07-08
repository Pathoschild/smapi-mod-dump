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

// TODO: change the size of the Description box to make it more readable with the new information

namespace ExtraFishInformation
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        // PUBLIC VARIABLES
        IDictionary<int, string> updatedDescriptions = new Dictionary<int, string>();


        // PUBLIC METHODS

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // read in required data files
            IDictionary<int, string> objectInfo = helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);
            IDictionary<int, string> fishInfo = helper.Content.Load<Dictionary<int, string>>("Data/Fish", ContentSource.GameContent);
            IDictionary<string, string> locationInfo = helper.Content.Load<Dictionary<string, string>>("Data/Locations", ContentSource.GameContent);

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
                // get info from ObjectInformation
                // TODO: change to include just "Fish" not "Fish -4" so as to include seaweed, etc. - crashes on joja cola
                string objectItemInfo = item.Value;
                if (!objectItemInfo.Contains("Fish -4")) continue;  // ignore non-fish items
                int fishId = item.Key;
                string[] objectItemSections = objectItemInfo.Split('/');
                string description = objectItemSections[5];  // get object description

                // get info from Fish
                string fishItemInfo = fishInfo[fishId];  // get individual fish info 
                string[] fishInfoSections = fishItemInfo.Split('/');  // sections are 0-12 for all except trapper fish which are 0-6 [see sdv wiki for more info]
                string newDescription;

                // handle each fish
                if (fishInfoSections.Length != 13) // handle trapper fish
                {
                    string location = fishInfoSections[4];
                    string extraInfo = $" Found in the {location}.";
                    newDescription = description + extraInfo;
                }
                else  // handle all other fish
                {
                    string locations = ParseLocation(fishId, seasonalLocationInfo);
                    string weather = ParseWeather(fishInfoSections[7]);
                    string schedule = ParseTimes(fishInfoSections[5]);
                    string extraInfo = $" Found {weather} from {schedule}.\n\nLocations:\n{locations}";
                    newDescription = description + extraInfo;

                } // end if/else statement

                // repack object sections
                objectItemSections[5] = newDescription;
                string newObjectInfo = string.Join("/", objectItemSections);
                newInfo[item.Key] = newObjectInfo;

            } // end foreach fish

            // update descriptions to include new info for each fish
            updatedDescriptions = newInfo;

        } // end Entry method


        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/ObjectInformation");
        }


        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (!asset.AssetNameEquals("Data/ObjectInformation")) return;
            IDictionary<int, string> objectData = asset.AsDictionary<int, string>().Data;
            foreach (KeyValuePair<int, string> item in updatedDescriptions)
            {
                objectData[item.Key] = item.Value;
            }
        }


        // PRIVATE METHODS

        /// <summary>A helper method to parse location and season information from the edited version of Locations.xnb</summary>
        /// <param name="fishId">Integer denoting fish being handled</param>
        /// <param name="seasonalLocationInfo">IDictionary of edited version of Locations.xnb information</param>
        /// <returns>Parsed locations/seasons data, to add directly to description</returns>
        private string ParseLocation(int fishId, IDictionary<string, string[]> seasonalLocationInfo)
        {
            string foundLocations = "";
            string[] seasonNames = { "Spring", "Summer", "Fall", "Winter" };

            foreach (KeyValuePair<string, string[]> location in seasonalLocationInfo)
            {
                // TODO: convert namesLikeThis to Names Like This
                string locationName = parseLocationName(location.Key);

                // locations to ignore: Temp, Fishing Game, 
                if (locationName == "Temp")
                { break; }
                if (locationName == "FishingGame") // TODO: change to "Fishing Game" once locations have been properly parsed
                { break; }

                string foundSeasons = "";
                string allSeasons = "Spring, Summer, Fall, Winter";
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


        /// <summary>A helper method to parse location names from namesLikeThis to Names Like This</summary>
        /// <param name="weather">String containing unparsed location name</param>
        /// <returns>String containing parsed location name</returns>
        private string parseLocationName(string locationName)
        {
            // convert to NamesLikeThis
            string parsedLocationName = $"{locationName.First().ToString().ToUpper()}{locationName.Substring(1)}";

            // TODO: convert to Names Like This

            // return parsed location name
            return parsedLocationName;

        }


        /// <summary>A helper method to parse weather information from Fish.xnb</summary>
        /// <param name="weather">String containing unparsed weather data</param>
        /// <returns>Parsed weather data, to add directly to description</returns>
        private string ParseWeather(string weather)
        {
            string parsedWeather;
            switch (weather)
            {
                case "sunny":
                    parsedWeather = "when it is sunny";
                    break;
                case "rainy":
                    parsedWeather = "when it is rainy";
                    break;
                case "both":
                    parsedWeather = "in all weathers";
                    break;
                default:
                    parsedWeather = "in unknown conditions";
                    break;
            } // end switch/case

            return parsedWeather;

        } // end ParseWeather method


        /// <summary>A helper method to parse active times from from Fish.xnb</summary>
        /// <param name="times">String containing unparsed time data</param>
        /// <returns>Parsed time data, to add directly to description</returns>
        private string ParseTimes(string times)
        {
            string parsedTimes = "";
            string[] splitTimes = times.Split(' ');

            if (splitTimes.Length == 2)
            {
                // split times into start/end times
                string startTime = splitTimes[0];
                string endTime = splitTimes[1];

                // parse time strings from XXX or XXXX to XX:XX
                startTime = ParseTimeString(startTime);
                endTime = ParseTimeString(endTime);

                // add parsed times to new string
                parsedTimes = $"{startTime} to {endTime}";
            }
            else
            {
                for (int i = 0; i < splitTimes.Length; i += 2)
                {
                    // split times into start/end times
                    string startTime = splitTimes[i];
                    string endTime = splitTimes[i + 1];

                    // parse time strings from XXX or XXXX to XX:XX
                    startTime = ParseTimeString(startTime);
                    endTime = ParseTimeString(endTime);

                    // add parsed times to new string
                    if (i == 0)
                    {
                        parsedTimes = $"{startTime} to {endTime}";
                    }
                    else if (i == splitTimes.Length - 2)
                    {
                        parsedTimes = $"{parsedTimes}, and {startTime} to {endTime}";
                    }
                    else
                    {
                        parsedTimes = $"{parsedTimes}, {startTime} to {endTime}";
                    }
                    // end if/else

                } // end for loop

            } // end if/else

            return parsedTimes;

        } // end ParseTimes method


        /// <summary>A helper method for ParseTimes() to parse time strings from from Fish.xnb</summary>
        /// <param name="timeToParse">String containing unparsed time data in XXX or XXXX form</param>
        /// <returns>Parsed time data in XX:XX</returns>
        private string ParseTimeString(string timeToParse)
        {
            string newTime = "";
            if (timeToParse.Length == 3)
            {
                string hour = timeToParse.Substring(0, 1);
                string minutes = timeToParse.Substring(1);
                newTime = $"{hour}:{minutes}";
            }
            else if (timeToParse.Length == 4)
            {
                string hour = timeToParse.Substring(0, 2);
                string minutes = timeToParse.Substring(2);
                if (Int32.Parse(hour) > 24)
                {
                    hour = (Int32.Parse(hour) - 24).ToString();
                }
                newTime = $"{hour}:{minutes}";
            }
            else
            {
                newTime = timeToParse;
            }

            // put those in X:XX format into XX:XX format
            if (newTime.Length == 4 && newTime.Contains(":"))
            {
                newTime = newTime.Insert(0, "0");
            }

            return newTime;

        } // end ParseTimeString method


    } // end ModEntry class


} // end namespace