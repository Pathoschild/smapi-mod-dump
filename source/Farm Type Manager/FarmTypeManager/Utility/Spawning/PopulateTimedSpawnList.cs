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
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates specific spawn times for a list of objects and adds them to the Utility.TimedSpawns list.</summary>
            /// <param name="objects">A list of saved objects to be spawned during the current in-game day.</param>
            /// <param name="data">The FarmData from which these saved objects were generated.</param>
            /// <param name="area">The SpawnArea for which these saved objects were generated.</param>
            public static void PopulateTimedSpawnList(List<SavedObject> objects, FarmData data, SpawnArea area)
            {
                List<TimedSpawn> timedSpawns = new List<TimedSpawn>(); //the list of fully processed objects and associated data

                Dictionary<int, int> possibleTimes = new Dictionary<int, int>(); //a dictionary of valid spawn times (keys) and the number of objects assigned to them (values)

                if (area.SpawnTiming == null) //if the SpawnTiming setting is null
                {
                    possibleTimes.Add(600, 0); //spawn everything at 6:00AM
                }
                else
                {
                    for (StardewTime x = area.SpawnTiming.StartTime; x <= area.SpawnTiming.EndTime; x++) //for each 10-minute time from StartTime to EndTime
                    {
                        possibleTimes.Add(x, 0); //add this time to the list
                    }
                }

                foreach (SavedObject obj in objects) //for each provided object
                {
                    int index = RNG.Next(0, possibleTimes.Count); //randomly select an index for a valid time
                    obj.SpawnTime = possibleTimes.Keys.ElementAt(index); //assign the time to this object
                    timedSpawns.Add(new TimedSpawn(obj, data, area)); //add this object to the processed list
                    possibleTimes[obj.SpawnTime]++; //increment the number of objects assigned to this time

                    if (area.SpawnTiming.MaximumSimultaneousSpawns.HasValue && area.SpawnTiming.MaximumSimultaneousSpawns.Value <= possibleTimes[obj.SpawnTime]) //if "max spawns" exists and has been reached for this time
                    {
                        possibleTimes.Remove(obj.SpawnTime); //remove this time from the list
                    }
                    else if (area.SpawnTiming.MinimumTimeBetweenSpawns.HasValue && area.SpawnTiming.MinimumTimeBetweenSpawns.Value > 10) //if "time between" exists and is significant
                    {
                        int between = (area.SpawnTiming.MinimumTimeBetweenSpawns.Value - 10) / 10; //get the number of other possible times to remove before/after the selected time
                        StardewTime minTime = obj.SpawnTime; //the earliest time to be removed from the list
                        StardewTime maxTime = obj.SpawnTime; //the latest time to be removed from the list

                        for (int x = 0; x < between; x++) //for each adjacent time to be removed
                        {
                            minTime--; //select the previous time
                            maxTime++; //select the next time
                        }

                        for (int x = possibleTimes.Count - 1; x >= 0; x--) //for each possible time (looping backward for removal purposes)
                        {
                            int time = possibleTimes.Keys.ElementAt(x);
                            if (time != obj.SpawnTime && time >= minTime && time <= maxTime) //if this time isn't the selected time, and is within the range of minTime and maxTime
                            {
                                possibleTimes.Remove(time); //remove it from the list
                            }
                        }
                    }

                    if (possibleTimes.Count <= 0) //if no valid spawn times are left
                    {
                        break; //skip the rest of the objects
                    }
                }

                TimedSpawns.Add(timedSpawns); //add the processed list of timed spawns to Utility.TimedSpawns
            }
        }
    }
}