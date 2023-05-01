/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace SaveAnywhere.Framework
{
    public static class NpcExtensions
    {
        
        public static SchedulePathDescription pathfindToNextScheduleLocation(this NPC npc,
            string startingLocation,
            int startingX,
            int startingY,
            string endingLocation,
            int endingX,
            int endingY,
            int finalFacingDirection,
            string endBehavior,
            string endMessage)
        {
            var pointStack = new Stack<Point>();
            var startPoint = new Point(startingX, startingY);
            var stringList = !startingLocation.Equals(endingLocation, StringComparison.Ordinal)
                ? npc.getLocationRoute(startingLocation, endingLocation)
                : null;
            if (stringList != null)
            {
                for (var index = 0; index < stringList.Count; ++index)
                {
                    var locationFromName = Game1.getLocationFromName(stringList[index]);
                    if (locationFromName.Name.Equals("Trailer") &&
                        Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                        locationFromName = Game1.getLocationFromName("Trailer_Big");
                    if (index < stringList.Count - 1)
                    {
                        var warpPointTo = locationFromName.getWarpPointTo(stringList[index + 1]);
                        if (warpPointTo.Equals(Point.Zero) || startPoint.Equals(Point.Zero))
                            throw new Exception("schedule pathing tried to find a warp point that doesn't exist.");
                        pointStack = addToStackForSchedule(pointStack,
                            PathFindController.findPathForNPCSchedules(startPoint, warpPointTo, locationFromName,
                                30000));
                        startPoint = locationFromName.getWarpPointTarget(warpPointTo, npc);
                    }
                    else
                    {
                        pointStack = addToStackForSchedule(pointStack,
                            PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY),
                                locationFromName, 30000));
                    }
                }
            }
            else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
            {
                var locationFromName = Game1.getLocationFromName(startingLocation);
                if (locationFromName.Name.Equals("Trailer") &&
                    Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                    locationFromName = Game1.getLocationFromName("Trailer_Big");
                pointStack = PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY),
                    locationFromName, 30000);
            }

            return new SchedulePathDescription(pointStack, finalFacingDirection, endBehavior, endMessage);
        }

        private static List<string> getLocationRoute(this NPC npc, string startingLocation, string endingLocation)
        {
            var field = typeof(NPC)
                .GetField("routesFromLocationToLocation",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance);
            //get routesFromLocationToLocation field from fields and get its value
            var routes = (List<List<string>>)field.GetValue(null);
            foreach (var source in routes)
                if (source.First().Equals(startingLocation, StringComparison.Ordinal) &&
                    source.Last().Equals(endingLocation, StringComparison.Ordinal) &&
                    ((int)(NetFieldBase<int, NetInt>)npc.gender == 0 ||
                     !source.Contains("BathHouse_MensLocker", StringComparer.Ordinal)) &&
                    ((int)(NetFieldBase<int, NetInt>)npc.gender != 0 ||
                     !source.Contains("BathHouse_WomensLocker", StringComparer.Ordinal)))
                    return source;
            return null;
        }

        private static Stack<Point> addToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
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