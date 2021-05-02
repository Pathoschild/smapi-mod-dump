/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Omegasis.SaveAnywhere.Framework
{
    public static class NPCExtensions
    {
        /// <summary>
        /// Fill in the npcs schedule with a billion end points and hope they find their way again?
        /// </summary>
        /// <param name="npc"></param>
        public static void fillInSchedule(this NPC npc)
        {
            if (npc.Schedule == null) return;



            IDictionary<string, string> rawSchedule = GetRawSchedule(npc.Name);
            if (rawSchedule == null)
                return;
            string schedulekey = GetScheduleKey(npc);
            rawSchedule.TryGetValue(schedulekey, out string scheduleForTheDay);

            if (string.IsNullOrEmpty(scheduleForTheDay))
            {
                return;
            }


            // parse entries
            string[] entries = scheduleForTheDay.Split('/');
            //make a class to get all of the schedule info.....


            int scheduleIndex = 0;

            SortedDictionary<int, SchedulePathInfo> actualScheduleData = new SortedDictionary<int, SchedulePathInfo>(); //time of day and the the path info.

            for (int i = 0; i < entries.Length; i++)
            {


                string[] fields = entries[i].Split(' ');

                // handle GOTO command
                if (fields[0].Equals("GOTO"))
                {
                    rawSchedule.TryGetValue(fields[1], out scheduleForTheDay);
                    entries = scheduleForTheDay.Split('/');
                    i = -1;
                }


                SchedulePathInfo info2 = new SchedulePathInfo(entries[i]);
                if (info2.timeToGoTo == 0) continue;

                else
                {
                    actualScheduleData.Add(info2.timeToGoTo, info2);
                }

            }

            int index = 0;
            List<KeyValuePair<int, SchedulePathInfo>> scheduleData = actualScheduleData.ToList();
            scheduleData.OrderBy(i => i.Key);

            for (int time = 600; time <= 2600; time += 10)
            {

                if (index >= scheduleData.Count)
                {
                    if (actualScheduleData.ContainsKey(time) == false)
                    {
                        actualScheduleData.Add(time, scheduleData[scheduleData.Count-1].Value);
                        continue;
                    }
                }

                if (index == scheduleData.Count - 1)
                {
                    if (actualScheduleData.ContainsKey(time)==false)
                    {
                        actualScheduleData.Add(time, scheduleData[index].Value);
                    }
                }
                else
                {
                    if (time == scheduleData[index + 1].Key)
                    {
                        index++;
                        continue;
                    }
                    else
                    {
                        if (actualScheduleData.ContainsKey(time) == false)
                        {
                            actualScheduleData.Add(time, scheduleData[index].Value);
                        }
                    }
                }
            }

            //SaveAnywhere.ModMonitor.Log("Count of schedule size is: " + npc.Name + " " + actualScheduleData.Count, StardewModdingAPI.LogLevel.Info);
            //npc.checkSchedule(Game1.timeOfDay);



            SchedulePathInfo info = actualScheduleData[Game1.timeOfDay];

            SchedulePathDescription schedulePathDescription;
            schedulePathDescription = SaveAnywhere.ModHelper.Reflection
    .GetMethod(npc, "pathfindToNextScheduleLocation")
    .Invoke<SchedulePathDescription>(npc.currentLocation.Name, npc.getTileX(), npc.getTileY(), info.endMap, info.endX, info.endY, info.endDirection, info.endBehavior, info.endMessage);

            npc.DirectionsToNewLocation = schedulePathDescription;
            npc.controller = new PathFindController(npc.DirectionsToNewLocation.route, npc, Utility.getGameLocationOfCharacter(npc))
            {
                finalFacingDirection = npc.DirectionsToNewLocation.facingDirection,
                endBehaviorFunction = null
            };


        }

        /// <summary>Get an NPC's raw schedule data from the XNB files.</summary>
        /// <param name="npcName">The NPC name whose schedules to read.</param>
        /// <returns>Returns the NPC schedule if found, else <c>null</c>.</returns>
        private static IDictionary<string, string> GetRawSchedule(string npcName)
        {
            try
            {
                return Game1.content.Load<Dictionary<string, string>>($"Characters\\schedules\\{npcName}");
            }
            catch
            {
                return null;
            }
        }

        private static string GetScheduleKey(NPC npc)
        {
            string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            string scheduleKey = "";
            if (npc.Name.Equals("Penny") && (str.Equals("Tue") || str.Equals("Wed") || str.Equals("Fri")) || (npc.Name.Equals("Maru") && (str.Equals("Tue") || str.Equals("Thu")) || npc.Name.Equals("Harvey") && (str.Equals("Tue") || str.Equals("Thu"))))
            {
                scheduleKey = "marriageJob";
            }
            if (!Game1.isRaining && npc.hasMasterScheduleEntry("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
            {
                scheduleKey = "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            }
            if (npc.hasMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()))
            {
                scheduleKey = Game1.currentSeason + "_" + Game1.dayOfMonth.ToString();
            }

            int playerFriendshipLevel1 = Utility.GetAllPlayerFriendshipLevel(npc);
            if (playerFriendshipLevel1 >= 0)
                playerFriendshipLevel1 /= 250;
            for (; playerFriendshipLevel1 > 0; --playerFriendshipLevel1)
            {
                if (npc.hasMasterScheduleEntry(Game1.dayOfMonth.ToString() + "_" + (object)playerFriendshipLevel1))
                    scheduleKey = Game1.dayOfMonth.ToString() + "_" + (object)playerFriendshipLevel1;
            }
            if (npc.hasMasterScheduleEntry(Game1.dayOfMonth.ToString()))
                scheduleKey = Game1.dayOfMonth.ToString();
            if (npc.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
                scheduleKey = "bus";

            if (Game1.isRaining)
            {
                if (Game1.random.NextDouble() < 0.5 && npc.hasMasterScheduleEntry("rain2"))
                    scheduleKey = "rain2";
                if (npc.hasMasterScheduleEntry("rain"))
                    scheduleKey = "rain";
            }

            List<string> stringList = new List<string>()
            {
                Game1.currentSeason,
                Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
            };
            int playerFriendshipLevel2 = Utility.GetAllPlayerFriendshipLevel(npc);
            if (playerFriendshipLevel2 >= 0)
                playerFriendshipLevel2 /= 250;
            while (playerFriendshipLevel2 > 0)
            {
                stringList.Add(string.Empty + (object)playerFriendshipLevel2);
                if (npc.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>)stringList)))
                {
                    scheduleKey = string.Join("_", (IEnumerable<string>)stringList);
                    break;
                }
                --playerFriendshipLevel2;
                stringList.RemoveAt(stringList.Count - 1);
            }
            if (npc.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>)stringList)))
                scheduleKey = string.Join("_", (IEnumerable<string>)stringList);
            if (npc.hasMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                scheduleKey = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (npc.hasMasterScheduleEntry(Game1.currentSeason))
                scheduleKey = Game1.currentSeason;
            if (npc.hasMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                scheduleKey = "spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);

            stringList.RemoveAt(stringList.Count - 1);
            stringList.Add("spring");
            int playerFriendshipLevel3 = Utility.GetAllPlayerFriendshipLevel(npc);
            if (playerFriendshipLevel3 >= 0)
                playerFriendshipLevel3 /= 250;
            while (playerFriendshipLevel3 > 0)
            {
                stringList.Add(string.Empty + (object)playerFriendshipLevel3);
                if (npc.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>)stringList)))
                    scheduleKey = string.Join("_", (IEnumerable<string>)stringList);
                --playerFriendshipLevel3;
                stringList.RemoveAt(stringList.Count - 1);
            }
            if (npc.hasMasterScheduleEntry("spring"))
                scheduleKey = "spring";
            else
            {
                scheduleKey = "";
            }
            return scheduleKey;

        }
    }


}


