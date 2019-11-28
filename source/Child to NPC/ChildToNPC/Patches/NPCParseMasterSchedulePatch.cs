using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Harmony;
using StardewValley;
using StardewValley.Network;

namespace ChildToNPC.Patches
{
    /* Prefix for parseMasterSchedule
     * Most of this code is directly translated from the original method,
     * and there are extra methods at the bottom which are recreating what the original does.
     * (I'd like to come back to this and see if I can find a better solution).
     * The parts I need to change are mixed in, so I have to re-execute most code.
     */
    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("parseMasterSchedule")]
    class NPCParseMasterSchedulePatch
    {
        public static bool Prefix(NPC __instance, ref Dictionary<int, SchedulePathDescription> __result, string rawData,
            List<List<string>> ___routesFromLocationToLocation)
        {
            if (!ModEntry.IsChildNPC(__instance))
                return true;

            string[] events = rawData.Split('/');
            int index = 0;
            Dictionary<int, SchedulePathDescription> dictionary = new Dictionary<int, SchedulePathDescription>();

            Dictionary<string, string> scheduleFromName = null;
            try
            {
                scheduleFromName = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name);
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log("An error occurred in ParseMasterSchedule while trying to load the schedule for " + __instance.Name + ": " + e.Message);
                return true;
            }

            //Example: "GOTO Tue" or "GOTO spring" says which entry to use
            //This replaces the rawData with whatever entry the GOTO requested
            if (events[0].Contains("GOTO"))
            {
                string whereToGo = events[0].Split(' ')[1];
                if (whereToGo.ToLower().Equals("season"))
                    whereToGo = Game1.currentSeason;
                try
                {
                    events = scheduleFromName[whereToGo].Split('/');
                }
                catch (Exception)
                {
                    __result = ModEntry.helper.Reflection.GetMethod(__instance, "parseMasterSchedule", true).Invoke<Dictionary<int, SchedulePathDescription>>(new object[] { scheduleFromName["spring"] });
                    return false;
                }
            }

            //Example: "NOT friendship Sam 6/" as first entry
            //Tells you to skip this entry if the friendship isn't set
            if (events[0].Contains("NOT"))
            {
                string[] friendshipData = events[0].Split(' ');
                if (friendshipData[1].ToLower() == "friendship")
                {
                    string name = friendshipData[2];
                    int hearts = Convert.ToInt32(friendshipData[3]);
                    bool farmerHasHearts = false;
                    foreach (Farmer allFarmer in Game1.getAllFarmers())
                    {
                        if (allFarmer.getFriendshipLevelForNPC(name) >= hearts)
                        {
                            farmerHasHearts = true;
                            break;
                        }
                    }

                    if (!farmerHasHearts)
                    {
                        __result = ModEntry.helper.Reflection.GetMethod(__instance, "parseMasterSchedule", true).Invoke<Dictionary<int, SchedulePathDescription>>(new object[] { scheduleFromName["spring"] });
                        return false;
                    }
                    //Otherwise, increment index by 1, continue with schedule
                    ++index;
                }
            }
            //Added in the 1.4 update, I haven't checked this dialogue yet
            else if (events[0].Contains("MAIL"))
            {
                string id = events[0].Split(' ')[1];
                if (Game1.MasterPlayer.mailReceived.Contains(id) || NetWorldState.checkAnywhereForWorldStateID(id))
                    index += 2;
                else
                    ++index;
            }

            //For the case of "NOT friendship Sam 6/GOTO 9" (I think)
            //Handles the GOTO if friendship change happened
            if (events[index].Contains("GOTO"))
            {
                string whereToGo = events[index].Split(' ')[1];
                if (whereToGo.ToLower().Equals("season"))
                    whereToGo = Game1.currentSeason;
                else if (whereToGo.ToLower().Equals("no_schedule"))
                {
                    __instance.followSchedule = false;
                    __result = null;
                    return false;
                }
                events = scheduleFromName[whereToGo].Split('/');
                index = 1;
            }

            //Point point = this.isMarried() ? new Point(0, 23) : new Point((int)this.defaultPosition.X / 64, (int)this.defaultPosition.Y / 64);
            //string startingLocation = this.isMarried() ? "BusStop" : (string)((NetFieldBase<string, NetString>)this.defaultMap);
            Point point = new Point(0, 23);
            string startingLocation = "BusStop";

            //Go through each of the events, parse them.
            for (int i = index; i < events.Length/* && events.Length > 1*/; ++i)
            {
                string[] currentEvent = events[i].Split(' ');

                int time = Convert.ToInt32(currentEvent[0]);
                string locationName = currentEvent[1];
                string endBehavior = null;
                string endMessage = null;

                //If there is no location name, skips straight to position
                if (int.TryParse(locationName, out int result))
                    locationName = startingLocation;

                int positionX = Convert.ToInt32(currentEvent[2]);
                int positionY = Convert.ToInt32(currentEvent[3]);

                int endIndex = 4;
                int facingDirection;

                try
                {
                    facingDirection = Convert.ToInt32(currentEvent[4]);
                    ++endIndex;
                }
                catch (Exception)
                {
                    facingDirection = 2;
                }

                object[] param = { locationName, positionX, positionY, facingDirection };
                bool accessibleChange = ModEntry.helper.Reflection.GetMethod(__instance, "changeScheduleForLocationAccessibility", true).Invoke<bool>(param);

                if (accessibleChange)
                {
                    if (scheduleFromName.ContainsKey("default"))
                    {
                        __result = ModEntry.helper.Reflection.GetMethod(__instance, "parseMasterSchedule", true).Invoke<Dictionary<int, SchedulePathDescription>>(new object[] { scheduleFromName["default"] });
                        return false;
                    }
                    __result = ModEntry.helper.Reflection.GetMethod(__instance, "parseMasterSchedule", true).Invoke<Dictionary<int, SchedulePathDescription>>(new object[] { scheduleFromName["spring"] });
                    return false;
                }

                //830 ArchaeologyHouse 17 9 2 penny_read \"Strings\\schedules\\Penny:marriageJob.000\"
                if (currentEvent.Length > endIndex)
                {
                    if (currentEvent[endIndex].Length > 0 && currentEvent[endIndex][0] == '"')
                    {
                        endMessage = events[i].Substring(events[i].IndexOf('"'));
                    }
                    else
                    {
                        endBehavior = currentEvent[endIndex];
                        if (endIndex + 1 < currentEvent.Length && currentEvent[endIndex + 1].Length > 0 && currentEvent[endIndex + 1][0] == '"')
                            endMessage = events[i].Substring(events[i].IndexOf('"')).Replace("\"", "");
                    }
                }

                //Add this time event to the dictionary
                dictionary.Add(time, ModEntry.helper.Reflection.GetMethod(__instance, "pathfindToNextScheduleLocation", true).Invoke<SchedulePathDescription>(new object[] { startingLocation, point.X, point.Y, locationName, positionX, positionY, facingDirection, endBehavior, endMessage }));
                //Then pretend this character has completed that appt, check next appt
                point.X = positionX;
                point.Y = positionY;
                startingLocation = locationName;
            }

            __result = dictionary;
            return false;
        }
    }
}