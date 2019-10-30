using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Harmony;
using StardewValley;

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

            string[] strArray1 = rawData.Split('/');
            Dictionary<int, SchedulePathDescription> dictionary = new Dictionary<int, SchedulePathDescription>();
            int index1 = 0;
            if (strArray1[0].Contains("GOTO"))
            {
                string currentSeason = strArray1[0].Split(' ')[1];
                if (currentSeason.ToLower().Equals("season"))
                    currentSeason = Game1.currentSeason;
                try
                {
                    strArray1 = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)[currentSeason].Split('/');
                }
                catch (Exception)
                {
                    Prefix(__instance, ref __result, Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)["spring"], ___routesFromLocationToLocation);
                    return false;
                }
            }
            if (strArray1[0].Contains("NOT"))
            {
                string[] strArray2 = strArray1[0].Split(' ');
                if (strArray2[1].ToLower() == "friendship")
                {
                    string name = strArray2[2];
                    int int32 = Convert.ToInt32(strArray2[3]);
                    bool flag = false;
                    foreach (Farmer allFarmer in Game1.getAllFarmers())
                    {
                        if (allFarmer.getFriendshipLevelForNPC(name) >= int32)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Prefix(__instance, ref __result, Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)["spring"], ___routesFromLocationToLocation);
                        return false;
                    }
                    ++index1;
                }
            }
            if (strArray1[index1].Contains("GOTO"))
            {
                string currentSeason = strArray1[index1].Split(' ')[1];
                if (currentSeason.ToLower().Equals("season"))
                    currentSeason = Game1.currentSeason;
                strArray1 = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)[currentSeason].Split('/');
                index1 = 1;
            }

            //setting values based on what it would be if married
            Point point = new Point(0, 23);
            string startingLocation = "BusStop";

            for (int index2 = index1; index2 < strArray1.Length && strArray1.Length > 1; ++index2)
            {
                int index3 = 0;
                string[] strArray2 = strArray1[index2].Split(' ');
                int int32_1 = Convert.ToInt32(strArray2[index3]);
                int index4 = index3 + 1;
                string locationName = strArray2[index4];
                string endBehavior = null;
                string endMessage = null;
                if (int.TryParse(locationName, out int result))
                {
                    locationName = startingLocation;
                    --index4;
                }
                int index5 = index4 + 1;
                int int32_2 = Convert.ToInt32(strArray2[index5]);
                int index6 = index5 + 1;
                int int32_3 = Convert.ToInt32(strArray2[index6]);
                int index7 = index6 + 1;
                int facingDirection;
                try
                {
                    facingDirection = Convert.ToInt32(strArray2[index7]);
                    ++index7;
                }
                catch (Exception)
                {
                    facingDirection = 2;
                }

                //trying to run (__instance.changeScheduleForLocationAccessibility(ref locationName, ref int32_2, ref int32_3, ref facingDirection
                if (ChangeScheduleForLocationAccessibility(__instance, ref locationName, ref int32_2, ref int32_3, ref facingDirection))
                {
                    if (Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name).ContainsKey("default"))
                    {
                        Prefix(__instance, ref __result, Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)["default"], ___routesFromLocationToLocation);
                        return false;
                    }
                    Prefix(__instance, ref __result, Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + __instance.Name)["spring"], ___routesFromLocationToLocation);
                    return false;
                }
                if (index7 < strArray2.Length)
                {
                    if (strArray2[index7].Length > 0 && strArray2[index7][0] == '"')
                    {
                        endMessage = strArray1[index2].Substring(strArray1[index2].IndexOf('"'));
                    }
                    else
                    {
                        endBehavior = strArray2[index7];
                        int index8 = index7 + 1;
                        if (index8 < strArray2.Length && strArray2[index8].Length > 0 && strArray2[index8][0] == '"')
                            endMessage = strArray1[index2].Substring(strArray1[index2].IndexOf('"')).Replace("\"", "");
                    }
                }

                dictionary.Add(int32_1, PathfindToNextScheduleLocation(__instance, ___routesFromLocationToLocation, startingLocation, point.X, point.Y, locationName, int32_2, int32_3, facingDirection, endBehavior, endMessage));
                point.X = int32_2;
                point.Y = int32_3;
                startingLocation = locationName;
            }
            __result = dictionary;
            return false;
        }

        public static bool ChangeScheduleForLocationAccessibility(NPC character, ref string locationName, ref int tileX, ref int tileY, ref int facingDirection)
        {
            string str = locationName;
            if (!(str == "JojaMart") && !(str == "Railroad"))
            {
                if (str == "CommunityCenter")
                    return !Game1.isLocationAccessible(locationName);
            }
            else if (!Game1.isLocationAccessible(locationName))
            {
                if (!Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + character.Name).ContainsKey(locationName + "_Replacement"))
                    return true;
                string[] strArray = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + character.Name)[locationName + "_Replacement"].Split(' ');
                locationName = strArray[0];
                tileX = Convert.ToInt32(strArray[1]);
                tileY = Convert.ToInt32(strArray[2]);
                facingDirection = Convert.ToInt32(strArray[3]);
            }
            return false;
        }

        public static SchedulePathDescription PathfindToNextScheduleLocation(NPC instance, List<List<string>> routesFromLocationToLocation, string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY, int finalFacingDirection, string endBehavior, string endMessage)
        {
            Stack<Point> pointStack = new Stack<Point>();
            Point startPoint = new Point(startingX, startingY);
            List<string> stringList = !startingLocation.Equals(endingLocation, StringComparison.Ordinal) ? GetLocationRoute(instance, routesFromLocationToLocation, startingLocation, endingLocation) : null;
            if (stringList != null)
            {
                for (int index = 0; index < stringList.Count; ++index)
                {
                    GameLocation locationFromName = Game1.getLocationFromName(stringList[index]);
                    if (locationFromName.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                        locationFromName = Game1.getLocationFromName("Trailer_Big");
                    if (index < stringList.Count - 1)
                    {
                        Point warpPointTo = locationFromName.getWarpPointTo(stringList[index + 1]);
                        if (warpPointTo.Equals(Point.Zero) || startPoint.Equals(Point.Zero))
                            throw new Exception("schedule pathing tried to find a warp point that doesn't exist.");
                        pointStack = AddToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, warpPointTo, locationFromName, 30000));
                        startPoint = locationFromName.getWarpPointTarget(warpPointTo);
                    }
                    else
                        pointStack = AddToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), locationFromName, 30000));
                }
            }
            else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
                pointStack = PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), Game1.getLocationFromName(startingLocation), 30000);
            return new SchedulePathDescription(pointStack, finalFacingDirection, endBehavior, endMessage);
        }

        //Used in pathfindToNextScheduleLocation
        public static List<string> GetLocationRoute(NPC instance, List<List<string>> routesFromLocationToLocation, string startingLocation, string endingLocation)
        {
            foreach (List<string> source in routesFromLocationToLocation)
            {
                if (source.First().Equals(startingLocation, StringComparison.Ordinal)
                    && source.Last().Equals(endingLocation, StringComparison.Ordinal)
                    && (instance.Gender == 0 || !source.Contains("BathHouse_MensLocker", StringComparer.Ordinal))
                    && (instance.Gender != 0 || !source.Contains("BathHouse_WomensLocker", StringComparer.Ordinal)))
                {
                    return source;
                }
            }
            return null;
        }

        //Used in pathfindToNextScheduleLocation
        public static Stack<Point> AddToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
        {
            if (toAdd == null)
                return original;
            original = new Stack<Point>(original);
            while (original.Count > 0)
                toAdd.Push(original.Pop());
            return toAdd;
        }
    }
}