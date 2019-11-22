using System;
using System.Threading.Tasks;
using NpcAdventure.StateMachine.StateFeatures;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;

namespace NpcAdventure.StateMachine.State
{
    internal class ResetState : CompanionState, ICompanionIntegrator
    {
        private Point companionRescheduleDestinationPoint;
        private string companionRescheduleDestinationLocation;
        private string companionRescheduleEndRouteBehavior;
        private string companionRescheduleEndRouteDialogue;
        private int companionRescheduleFacingDirection;

        public ResetState(CompanionStateMachine stateMachine, IModEvents events, IMonitor monitor) : base(stateMachine, events, monitor)
        {
        }

        public void ReintegrateCompanionNPC()
        {
            NPC companion = this.StateMachine.Companion;
            Farmer farmer = this.StateMachine.CompanionManager.Farmer;

            companion.Schedule = this.GetCompanionSchedule(Game1.dayOfMonth);
            companion.controller = null;
            companion.temporaryController = null;
            companion.farmerPassesThrough = false;
            companion.followSchedule = true;

            if (companion.Schedule == null)
            {
                this.companionRescheduleDestinationLocation = companion.DefaultMap;
                if (farmer.spouse != null && farmer.spouse.Equals(companion.Name))
                {
                    this.companionRescheduleDestinationPoint =
                        (Game1.getLocationFromName(companion.DefaultMap) as FarmHouse)
                        .getKitchenStandingSpot();
                }
                else
                {
                    this.companionRescheduleDestinationPoint =
                        new Point((int)companion.DefaultPosition.X, (int)companion.DefaultPosition.Y);
                }
            }
            Game1.fadeScreenToBlack();
            companion.faceTowardFarmerTimer = 0;
            this.DelayedWarp(this.companionRescheduleDestinationLocation,
                this.companionRescheduleDestinationPoint, 500, new Action(this.CompanionEndCleanup));
        }

        private Dictionary<int, SchedulePathDescription> GetCompanionSchedule(int dayOfMonth)
        {
            NPC companion = this.StateMachine.Companion;

            if (!companion.Name.Equals("Robin") || Game1.player.currentUpgrade != null)
            {
                companion.IsInvisible = false;
            }
            if (companion.Name.Equals("Willy") && Game1.stats.DaysPlayed < 2u)
            {
                companion.IsInvisible = true;
            }
            else if (companion.Schedule != null)
            {
                companion.followSchedule = true;
            }
            Dictionary<string, string> masterSchedule = null;
            Dictionary<int, SchedulePathDescription> result;
            try
            {
                masterSchedule = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name);
            }
            catch (Exception)
            {
                result = null;
                return result;
            }
            if (companion.isMarried())
            {
                string day = Game1.shortDayNameFromDayOfSeason(dayOfMonth);
                if ((companion.Name.Equals("Penny") && (day.Equals("Tue") || day.Equals("Wed") || day.Equals("Fri"))) || (companion.Name.Equals("Maru") && (day.Equals("Tue") || day.Equals("Thu"))) || (companion.Name.Equals("Harvey") && (day.Equals("Tue") || day.Equals("Thu"))))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(companion, "marriageJob");
                    return this.MasterScheduleParse(masterSchedule["marriageJob"]);
                }
                if (!Game1.isRaining && masterSchedule.ContainsKey("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(companion, "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                    return this.MasterScheduleParse(masterSchedule["marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                companion.followSchedule = false;
                return null;
            }
            else
            {
                if (masterSchedule.ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth))
                {
                    return this.MasterScheduleParse(masterSchedule[Game1.currentSeason + "_" + Game1.dayOfMonth]);
                }
                int friendship;
                for (friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1); friendship > 0; friendship--)
                {
                    if (masterSchedule.ContainsKey(Game1.dayOfMonth + "_" + friendship))
                    {
                        return this.MasterScheduleParse(masterSchedule[Game1.dayOfMonth + "_" + friendship]);
                    }
                }
                if (masterSchedule.ContainsKey(string.Empty + Game1.dayOfMonth))
                {
                    return this.MasterScheduleParse(masterSchedule[string.Empty + Game1.dayOfMonth]);
                }
                if (companion.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
                {
                    return this.MasterScheduleParse(masterSchedule["bus"]);
                }
                if (Game1.isRaining)
                {
                    if (Game1.random.NextDouble() < 0.5 && masterSchedule.ContainsKey("rain2"))
                    {
                        return this.MasterScheduleParse(masterSchedule["rain2"]);
                    }
                    if (masterSchedule.ContainsKey("rain"))
                    {
                        return this.MasterScheduleParse(masterSchedule["rain"]);
                    }
                }
                List<string> key = new List<string>
                {
                    Game1.currentSeason,
                    Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
                };
                friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1);
                while (friendship > 0)
                {
                    key.Add(string.Empty + friendship);
                    if (masterSchedule.ContainsKey(string.Join("_", key)))
                    {
                        return this.MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (masterSchedule.ContainsKey(string.Join("_", key)))
                {
                    return this.MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                }
                if (masterSchedule.ContainsKey(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return this.MasterScheduleParse(masterSchedule[Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                if (masterSchedule.ContainsKey(Game1.currentSeason))
                {
                    return this.MasterScheduleParse(masterSchedule[Game1.currentSeason]);
                }
                if (masterSchedule.ContainsKey("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return this.MasterScheduleParse(masterSchedule["spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                key.RemoveAt(key.Count - 1);
                key.Add("spring");
                friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1);
                while (friendship > 0)
                {
                    key.Add(string.Empty + friendship);
                    if (masterSchedule.ContainsKey(string.Join("_", key)))
                    {
                        return this.MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (masterSchedule.ContainsKey("spring"))
                {
                    return this.MasterScheduleParse(masterSchedule["spring"]);
                }
                return null;
            }
        }

        private Dictionary<int, SchedulePathDescription> MasterScheduleParse(string scheduleString)
        {
            NPC companion = this.StateMachine.Companion;
            int timeOfDay = Game1.timeOfDay;
            string[] split = scheduleString.Split(new char[] { '/' });
            Dictionary<int, SchedulePathDescription> oneDaySchedule = new Dictionary<int, SchedulePathDescription>();
            Type[] pathfinderTypes = new Type[]
            {
                typeof(string), typeof(int), typeof(int), typeof(string), typeof(int), typeof(int), typeof(int),
                typeof(string), typeof(string)
            };
            MethodInfo pathfinder = typeof(NPC).GetMethod("pathfindToNextScheduleLocation",
                BindingFlags.NonPublic | BindingFlags.Instance, null, pathfinderTypes, null);
            int routesToSkip = 0;
            int previousTime = 0;
            if (split[0].Contains("GOTO"))
            {
                string newKey = split[0].Split(new char[] { ' ' })[1];

                if (newKey.ToLower().Equals("season"))
                {
                    newKey = Game1.currentSeason;
                }

                try
                {
                    split =
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)
                            [newKey].Split(new char[] { '/' });
                }
                catch (Exception)
                {
                    return this.MasterScheduleParse(
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[
                            "spring"]);
                }
            }

            if (split[0].Contains("NOT"))
            {
                string[] commandSplit = split[0].Split(new char[] { ' ' });
                string a = commandSplit[1].ToLower();
                if (a == "friendship")
                {
                    string who = commandSplit[2];
                    int level = Convert.ToInt32(commandSplit[3]);
                    bool conditionMet = false;
                    using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.getFriendshipLevelForNPC(who) >= level)
                            {
                                conditionMet = true;
                                break;
                            }
                        }
                    }
                    if (conditionMet)
                    {
                        return this.MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["spring"]);
                    }
                    routesToSkip++;
                }
            }

            if (split[routesToSkip].Contains("GOTO"))
            {
                string newKey2 = split[routesToSkip].Split(new char[] { ' ' })[1];
                if (newKey2.ToLower().Equals("season"))
                {
                    newKey2 = Game1.currentSeason;
                }
                split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[newKey2].Split(new char[] { '/' });
                routesToSkip = 1;
            }

            Point previousPosition = companion.isMarried() ? new Point(0, 23) : new Point((int)companion.DefaultPosition.X / 64, (int)companion.DefaultPosition.Y / 64);
            string previousGameLocation = companion.isMarried() ? "BusStop" : companion.DefaultMap;
            int i = routesToSkip;

            while (i < split.Length && split.Length > 1)
            {
                int index = 0;
                string[] newDestinationDescription = split[i].Split(new char[] { ' ' });
                int time = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                string location = newDestinationDescription[index];
                string endOfRouteAnimation = null;
                string endOfRouteMessage = null;
                if (int.TryParse(location, out int tmp))
                {
                    location = previousGameLocation;
                    index--;
                }
                index++;
                int xLocation = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                int yLocation = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                int localFacingDirection = 2;
                try
                {
                    localFacingDirection = Convert.ToInt32(newDestinationDescription[index]);
                    index++;
                }
                catch (Exception)
                {
                    localFacingDirection = 2;
                }
                if (this.changeScheduleForLocationAccessibility(ref location, ref xLocation, ref yLocation, ref localFacingDirection))
                {
                    if (Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name).ContainsKey("default"))
                    {
                        return this.MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["default"]);
                    }
                    return this.MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["spring"]);
                }
                else
                {
                    if (index < newDestinationDescription.Length)
                    {
                        if (newDestinationDescription[index].Length > 0 && newDestinationDescription[index][0] == '"')
                        {
                            endOfRouteMessage = split[i].Substring(split[i].IndexOf('"')).Replace("\"", "");
                        }
                        else
                        {
                            endOfRouteAnimation = newDestinationDescription[index];
                            index++;
                            if (index < newDestinationDescription.Length && newDestinationDescription[index].Length > 0 && newDestinationDescription[index][0] == '"')
                            {
                                endOfRouteMessage = split[i].Substring(split[i].IndexOf('"')).Replace("\"", "");
                            }
                        }
                    }

                    object[] parameters = new object[]
                    {
                        previousGameLocation, previousPosition.X, previousPosition.Y, location, xLocation,
                        yLocation, localFacingDirection, endOfRouteAnimation, endOfRouteMessage
                    };
                    SchedulePathDescription schedulePathDesc = (SchedulePathDescription)pathfinder.Invoke(companion, parameters);
                    oneDaySchedule.Add(time, schedulePathDesc);
                    previousPosition.X = xLocation;
                    previousPosition.Y = yLocation;
                    if (timeOfDay >= time && schedulePathDesc.route != null && schedulePathDesc.route.Count != 0)
                    {
                        Stack<Point> sp = oneDaySchedule[time].route;
                        Point p = new Point();
                        while (sp.Count > 1)
                            sp.Pop();
                        while (sp.Count != 0)
                            p = sp.Pop();

                        if (previousTime < time)
                        {
                            this.companionRescheduleDestinationPoint = p;
                            this.companionRescheduleDestinationLocation = location;
                            this.companionRescheduleEndRouteBehavior = endOfRouteAnimation;
                            this.companionRescheduleEndRouteDialogue = endOfRouteMessage;
                            this.companionRescheduleFacingDirection = localFacingDirection;
                        }
                    }
                    previousTime = time;
                    previousGameLocation = location;
                    i++;
                }
            }
            return oneDaySchedule;
        }

        /// <summary>
        /// Another helper for GetCompanionSchedule.
        /// </summary>
        private bool changeScheduleForLocationAccessibility(ref string locationName, ref int tileX, ref int tileY, ref int facingDirection)
        {
            NPC companion = this.StateMachine.Companion;
            string a = locationName;
            if (!(a == "JojaMart") && !(a == "Railroad"))
            {
                if (a == "CommunityCenter")
                {
                    return !Game1.isLocationAccessible(locationName);
                }
            }
            else if (!Game1.isLocationAccessible(locationName))
            {
                if (!Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name).ContainsKey(locationName + "_Replacement"))
                {
                    return true;
                }
                string[] split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[locationName + "_Replacement"].Split(new char[] { ' ' });
                locationName = split[0];
                tileX = Convert.ToInt32(split[1]);
                tileY = Convert.ToInt32(split[2]);
                facingDirection = Convert.ToInt32(split[3]);
            }
            return false;
        }

        private async void DelayedWarp(String location, Point tileLocation, int milliseconds, Action afterWarpAction)
        {
            await Task.Run(() => this.Timer(milliseconds));
            location = location != null ? location : this.StateMachine.CompanionManager.Farmer.currentLocation.Name;
            //tileLocation = tileLocation != Point.Zero ? tileLocation : stateMachine.manager.farmer.getTileLocationPoint();
            if (this.StateMachine.Companion.currentLocation != null)
                Game1.warpCharacter(this.StateMachine.Companion, location, tileLocation);
            afterWarpAction.Invoke();
        }

        private int Timer(int milliseconds)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < milliseconds) ;
            return 0;
        }

        private void CompanionEndCleanup()
        {
            NPC companion = this.StateMachine.Companion;

            typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(companion, companion.getTileLocationPoint());
            companion.checkSchedule(Game1.timeOfDay);

            // Set end of route behavior, message, and facingDirection
            MethodInfo getRouteEndBehaviorFunction = typeof(NPC).GetMethod("getRouteEndBehaviorFunction",
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string) }, null);
            if (this.companionRescheduleEndRouteBehavior != null && this.companionRescheduleEndRouteBehavior != "")
            {
                PathFindController.endBehavior eB = (PathFindController.endBehavior)getRouteEndBehaviorFunction.Invoke(companion,
                    new object[] { this.companionRescheduleEndRouteBehavior, this.companionRescheduleEndRouteDialogue });
                eB(companion, companion.currentLocation);
            }
            if (this.companionRescheduleEndRouteDialogue != null && this.companionRescheduleEndRouteDialogue != "")
                companion.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString(this.companionRescheduleEndRouteDialogue), companion));
            companion.faceDirection(this.companionRescheduleFacingDirection);

            if (companion.Schedule == null && this.StateMachine.CompanionManager.Farmer.spouse != null &&
                this.StateMachine.CompanionManager.Farmer.spouse.Equals(companion.Name))
            {
                companion.marriageDuties();
            }
        }
    }
}
