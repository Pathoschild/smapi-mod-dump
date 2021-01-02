/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

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
            /// <returns>True if objects are allowed to spawn. False if any extra conditions should prevent spawning.</returns>
            public static bool CheckExtraConditions(SpawnArea area, InternalSaveData save)
            {
                Monitor.Log($"Checking extra conditions for this area...", LogLevel.Trace);

                //check years
                if (area.ExtraConditions.Years != null && area.ExtraConditions.Years.Length > 0)
                {
                    Monitor.Log("Years condition(s) found. Checking...", LogLevel.Trace);

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
                    Monitor.Log("Seasons condition(s) found. Checking...", LogLevel.Trace);

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
                    Monitor.Log("Days condition(s) found. Checking...", LogLevel.Trace);

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
                    Monitor.Log("Yesterday's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherYesterday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (save.WeatherForYesterday) //compare to yesterday's weather
                        {
                            case Utility.Weather.Sunny:
                            case Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
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
                    Monitor.Log("Today's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherToday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (Utility.WeatherForToday()) //compare to today's weather
                        {
                            case Utility.Weather.Sunny:
                            case Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }
                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
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
                    Monitor.Log("Tomorrow's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherTomorrow) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (Game1.weatherForTomorrow) //compare to tomorrow's weather
                        {
                            case (int)Utility.Weather.Sunny:
                            case (int)Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case (int)Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
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

                //check EPU preconditions
                if (area.ExtraConditions.EPUPreconditions != null && area.ExtraConditions.EPUPreconditions.Length > 0)
                {
                    Monitor.Log($"EPU Preconditions found. Checking...", LogLevel.Trace);
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