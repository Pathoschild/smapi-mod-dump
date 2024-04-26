/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Checks whether objects should be spawned in a given SpawnArea based on its ExtraConditions settings.</summary>
            /// <param name="area">The SpawnArea to be checked.</param>
            /// <param name="save">The mod's save data for the current farm and config file.</param>
            /// <param name="packManifest">The manifest of the content pack providing the spawn area. Null for personal configuration files.</param>
            /// <returns>True if objects are allowed to spawn. False if any extra conditions should prevent spawning.</returns>
            public static bool CheckExtraConditions(SpawnArea area, InternalSaveData save, IManifest packManifest)
            {
                Monitor.Log($"Checking extra conditions for this area...", LogLevel.Trace);

                //check years
                if (area.ExtraConditions.Years != null && area.ExtraConditions.Years.Length > 0)
                {
                    Monitor.Log("Year conditions found. Checking...", LogLevel.Trace);

                    bool validYear = false;

                    foreach (string year in area.ExtraConditions.Years)
                    {
                        try //watch for errors related to string parsing
                        {
                            if (year.Equals("All", StringComparison.OrdinalIgnoreCase) || year.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                            {
                                validYear = true;
                                break; //skip the rest of the "year" checks
                            }
                            else if (year.Contains("+")) //contains a plus, so parse it as a single year & any years after it, e.g. "2+"
                            {
                                string[] split = year.Split('+'); //split into separate strings around the plus symbol
                                int minYear = Int32.Parse(split[0].Trim()); //get the number to the left of the plus (trim whitespace) 

                                if (minYear <= Game1.year) //if the current year is within the specified range
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                            else if (year.Contains("-")) //contains a delimiter, so parse it as a range of years, e.g. "1-10"
                            {
                                string[] split = year.Split('-'); //split into separate strings for each delimiter
                                int minYear = Int32.Parse(split[0].Trim()); //get the first number (trim whitespace)
                                int maxYear = Int32.Parse(split[1].Trim()); //get the second number (trim whitespace)

                                if (minYear <= Game1.year && maxYear >= Game1.year) //if the current year is within the specified range
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                            else //parse as a single year, e.g. "1"
                            {
                                int yearNum = Int32.Parse(year.Trim()); //convert to a number

                                if (yearNum == Game1.year) //if it matches the current year
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"Issue: This part of the extra condition \"Years\" for the {area.MapName} map isn't formatted correctly: \"{year}\"", LogLevel.Info);
                        }
                    }

                    if (validYear)
                    {
                        Monitor.Log("The current year matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current year did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false;
                    }
                }

                //check seasons
                if (area.ExtraConditions.Seasons != null && area.ExtraConditions.Seasons.Length > 0)
                {
                    Monitor.Log("Season conditions found. Checking...", LogLevel.Trace);

                    bool validSeason = false;

                    foreach (string season in area.ExtraConditions.Seasons)
                    {
                        if (season.Equals("All", StringComparison.OrdinalIgnoreCase) || season.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validSeason = true;
                            break; //skip the rest of the "season" checks
                        }
                        else if (season.Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase)) //if the current season is listed
                        {
                            validSeason = true;
                            break; //skip the rest of the "season" checks
                        }
                    }

                    if (validSeason)
                    {
                        Monitor.Log("The current season matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current season did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check days
                if (area.ExtraConditions.Days != null && area.ExtraConditions.Days.Length > 0)
                {
                    Monitor.Log("Day conditions found. Checking...", LogLevel.Trace);

                    bool validDay = false;

                    foreach (string day in area.ExtraConditions.Days)
                    {
                        try //watch for errors related to string parsing
                        {
                            if (day.Equals("All", StringComparison.OrdinalIgnoreCase) || day.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                            {
                                validDay = true;
                                break; //skip the rest of the "day" checks
                            }
                            else if (day.Contains("+")) //contains a plus, so parse it as a single day & any days after it, e.g. "2+"
                            {
                                string[] split = day.Split('+'); //split into separate strings around the plus symbol
                                int minDay = Int32.Parse(split[0].Trim()); //get the number to the left of the plus (trim whitespace) 

                                if (minDay <= Game1.dayOfMonth) //if the current day is within the specified range
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                            else if (day.Contains("-")) //contains a delimiter, so parse it as a range of dates, e.g. "1-10"
                            {
                                string[] split = day.Split('-'); //split into separate strings for each delimiter
                                int minDay = Int32.Parse(split[0].Trim()); //get the first number (trim whitespace)
                                int maxDay = Int32.Parse(split[1].Trim()); //get the second number (trim whitespace)

                                if (minDay <= Game1.dayOfMonth && maxDay >= Game1.dayOfMonth) //if the current day is within the specified range
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                            else //parse as a single date, e.g. "1" or "25"
                            {
                                int dayNum = Int32.Parse(day.Trim()); //convert to a number

                                if (dayNum == Game1.dayOfMonth) //if it matches the current day
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"Issue: This part of the extra condition \"Days\" for the {area.MapName} map isn't formatted correctly: \"{day}\"", LogLevel.Info);
                        }
                    }

                    if (validDay)
                    {
                        Monitor.Log("The current day matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current day did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check yesterday's weather
                if (area.ExtraConditions.WeatherYesterday != null && area.ExtraConditions.WeatherYesterday.Length > 0)
                {
                    Monitor.Log("Yesterday's weather conditions found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherYesterday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        if (weather.Equals(save.WeatherForYesterday, StringComparison.OrdinalIgnoreCase)) //if the given weather name matches (SDV v1.6+)
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        switch (save.WeatherForYesterday) //compare to yesterday's weather
                        {
                            case Game1.weather_sunny:
                            case Game1.weather_festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Game1.weather_wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of this check
                        }
                    }


                    if (validWeather)
                    {
                        Monitor.Log("Yesterday's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Yesterday's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check today's weather
                if (area.ExtraConditions.WeatherToday != null && area.ExtraConditions.WeatherToday.Length > 0)
                {
                    Monitor.Log("Today's weather conditions found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherToday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        string weatherToday = Game1.netWorldState.Value.GetWeatherForLocation("Default").Weather;

                        if (weather.Equals(weatherToday, StringComparison.OrdinalIgnoreCase)) //if the given weather name matches (SDV v1.6+)
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        switch (weatherToday) //compare to today's weather 
                        {
                            case Game1.weather_sunny:
                            case Game1.weather_festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Game1.weather_wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }
                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of this check
                        }
                    }

                    if (validWeather)
                    {
                        Monitor.Log("Today's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Today's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check tomorrow's weather
                if (area.ExtraConditions.WeatherTomorrow != null && area.ExtraConditions.WeatherTomorrow.Length > 0)
                {
                    Monitor.Log("Tomorrow's weather conditions found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherTomorrow) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        if (weather.Equals(Game1.weatherForTomorrow, StringComparison.OrdinalIgnoreCase)) //if the given weather name matches (SDV v1.6+)
                        {
                            validWeather = true;
                            break; //skip the rest of this check
                        }

                        switch (Game1.weatherForTomorrow) //compare to tomorrow's weather
                        {
                            case Game1.weather_sunny:
                            case Game1.weather_festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Game1.weather_wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Game1.weather_snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of this check
                        }
                    }

                    if (validWeather)
                    {
                        Monitor.Log("Tomorrow's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Tomorrow's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check game state queries (GSQs)
                if (area.ExtraConditions.GameStateQueries != null && area.ExtraConditions.GameStateQueries.Length > 0)
                {
                    Monitor.Log("GSQ conditions found. Checking...", LogLevel.Trace);

                    bool validGSQ = false;

                    foreach (string gsq in area.ExtraConditions.GameStateQueries)
                    {
                        if (GameStateQuery.CheckConditions(gsq)) //if this query is currently true (at the start of the day with default context info)
                        {
                            validGSQ = true;
                            break; //skip the rest of this check
                        }
                    }

                    if (validGSQ)
                    {
                        Monitor.Log("At least one game state query (GSQ) string was valid. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("All game state query (GSQ) strings were invalid. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check CP conditions
                if (area.ExtraConditions.CPConditions != null && area.ExtraConditions.CPConditions.Count > 0)
                {
                    Monitor.Log($"CP conditions found. Checking...", LogLevel.Trace);
                    if (ContentPatcherAPI == null) //if CP's API is not available
                    {
                        Monitor.LogOnce($"FTM cannot currently access the API for Content Patcher (CP), but at least one spawn area has CP conditions. Those areas will be disabled.", LogLevel.Warn);
                        Monitor.Log($"CP conditions could not be checked. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                    else //if CP's API is available
                    {
                        try
                        {
                            string[] modIDs = packManifest?.Dependencies.Select(dep => dep.UniqueID).ToArray(); //get the ID of each mod this content pack requires (null if no manifest was provided)

                            var conditions = ContentPatcherAPI.ParseConditions(Manifest, area.ExtraConditions.CPConditions, ContentPatcherVersion, modIDs);

                            if (conditions.IsMatch) //if this area's CP conditions all match the current game state
                            {
                                Monitor.Log("CP conditions all currently match. Spawn allowed.", LogLevel.Trace);
                            }
                            else if (!conditions.IsValid) //if the conditions are not formatted correctly
                            {
                                Monitor.Log($"Issue: A Content Patcher (CP) condition for area ID \"{area.UniqueAreaID}\" isn't formatted correctly. Spawn disabled. Error message: \n\"{conditions.ValidationError}\"", LogLevel.Info);
                                return false; //prevent spawning
                            }
                            else
                            {
                                Monitor.Log($"At least one CP condition does not currently match: \"{conditions.GetReasonNotMatched()}\". Spawn disabled.", LogLevel.Trace);
                                return false; //prevent spawning
                            }
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"An error occurred while FTM was using the API for Content Patcher (CP). Please report this to FTM's developer. Auto-generated error message:", LogLevel.Error);
                            Monitor.Log($"----------", LogLevel.Error);
                            Monitor.Log($"{ex.ToString()}", LogLevel.Error);
                            Monitor.Log($"CP conditions could not be checked. Spawn disabled.", LogLevel.Trace);
                            return false;
                        }
                    }
                }

                //check EPU preconditions
                if (area.ExtraConditions.EPUPreconditions != null && area.ExtraConditions.EPUPreconditions.Length > 0)
                {
                    Monitor.Log($"EPU conditions found. Checking...", LogLevel.Trace);
                    if (EPUConditionsChecker == null) //if EPU's API is not available
                    {
                        Monitor.LogOnce($"FTM cannot currently access the API for Expanded Preconditions Utility (EPU), but at least one spawn area has EPU preconditions. Those areas will be disabled. Please make sure EPU is installed.", LogLevel.Warn);
                        Monitor.Log($"EPU preconditions could not be checked. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                    else //if EPU's API is available
                    {
                        try
                        {
                            if (EPUConditionsChecker.CheckConditions(area.ExtraConditions.EPUPreconditions) == true) //if ANY of this area's precondition strings are true
                            {
                                Monitor.Log("At least one EPU precondition string was valid. Spawn allowed.", LogLevel.Trace);
                            }
                            else //if ALL of this area's precondition strings are false
                            {
                                Monitor.Log("All EPU precondition strings were invalid. Spawn disabled.", LogLevel.Trace);
                                return false; //prevent spawning
                            }
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"An error occurred while FTM was using the API for Expanded Preconditions Utility (EPU). Please report this to FTM's developer. Auto-generated error message:", LogLevel.Error);
                            Monitor.Log($"----------", LogLevel.Error);
                            Monitor.Log($"{ex.ToString()}", LogLevel.Error);
                            Monitor.Log($"EPU preconditions could not be checked. Spawn disabled.", LogLevel.Trace);
                            return false;
                        }
                    }
                }

                //check number of spawns
                //NOTE: it's important that this is the last condition checked, because otherwise it might count down while not actually spawning (i.e. while blocked by another condition)
                if (area.ExtraConditions.LimitedNumberOfSpawns != null)
                {
                    Monitor.Log("Limited Number Of Spawns condition found. Checking...", LogLevel.Trace);
                    if (area.ExtraConditions.LimitedNumberOfSpawns > 0) //if there's at least one spawn day for this area
                    {
                        //if save data already exists for this area
                        if (save.LNOSCounter.ContainsKey(area.UniqueAreaID))
                        {
                            Monitor.Log("Sava data found for this area; checking spawn days counter...", LogLevel.Trace);
                            //if there's still at least one spawn day remaining
                            if ((area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]) > 0)
                            {
                                Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]}. Spawn allowed.", LogLevel.Trace);
                                save.LNOSCounter[area.UniqueAreaID]++; //increment (NOTE: this change needs to be saved at the end of the day)
                            }
                            else //no spawn days remaining
                            {
                                Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]}. Spawn disabled.", LogLevel.Trace);
                                return false; //prevent spawning
                            }
                        }
                        else //no save file exists for this area; behave as if LNOSCounter == 0
                        {
                            Monitor.Log("No save data found for this area; creating new counter.", LogLevel.Trace);
                            save.LNOSCounter.Add(area.UniqueAreaID, 1); //new counter for this area, starting at 1
                            Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns}. Spawn allowed.", LogLevel.Trace);
                        }
                    }
                    else //no spawns remaining
                    {
                        Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns}. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                return true; //all extra conditions allow for spawning
            }
        }
    }
}