/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/ScheduleViewer
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScheduleViewer
{
    static public class Schedule
    {
        private static WorldDate Date;
        private static Dictionary<string, NPCSchedule> NpcsWithSchedule;
        private static Dictionary<string, string> LocationNames;
        public static bool HasHostSchedules;

        public record ScheduleEntry(
            int Time,
            int X,
            int Y,
            int FacingDirection,
            string Location,
            string Animation,
            bool IsArrivalTime = false
        )
        {
            public override string ToString()
            {
                return $"{(this.IsArrivalTime ? "@" : String.Empty)}{Game1.getTimeOfDayString(this.Time != 0 ? this.Time : 600)} {(this.Location ?? "Unknown")}";
            }

            public string GetHoverText()
            {
                string facingDirection = ModEntry.ModHelper.Translation.Get($"facing_direction_{this.FacingDirection}");
                List<string> lines = new()
            {
                ModEntry.ModHelper.Translation.Get("location_hover_text", new { x = X, y = Y, facingDirection })
            };
                if (this.Animation != null)
                {
                    lines.Add(this.Animation);
                }
                return string.Join(Environment.NewLine, lines);
            }
        };

        public class NPCSchedule
        {
            public string DisplayName { get; init; }
            /// <summary>The schedule entries for today. If null, then the NPC is not following a schedule.</summary>
            public List<ScheduleEntry> Entries { get; init; }
            public bool CanSocialize { get; init; }
            /// <summary>If true, then the NPC is either not following a schedule or they are ignoring it today.</summary>
            public bool IsOnSchedule { get; init; }
            public string CurrentLocation { get; set; }
            [JsonIgnore]
            public NPC NPC { get; set; }

            /// <summary>Main creation method</summary>
            public NPCSchedule(NPC npc, List<ScheduleEntry> entries)
            {
                DisplayName = npc.getName();
                Entries = entries;
                CanSocialize = npc.CanSocialize;
                CurrentLocation = PrettyPrintLocationName(npc.currentLocation);
                IsOnSchedule = npc.followSchedule && !npc.ignoreScheduleToday;
                NPC = npc;
            }

            internal void Deconstruct(out List<ScheduleEntry> entries, out string currentLocation, out bool isOnSchedule, out string displayName, out NPC npc)
            {
                entries = Entries;
                currentLocation = CurrentLocation;
                isOnSchedule = IsOnSchedule;
                displayName = DisplayName;
                npc = NPC;
            }

            internal void Deconstruct(out List<ScheduleEntry> entries, out string currentLocation, out bool isOnSchedule, out string displayName)
            {
                entries = Entries;
                currentLocation = CurrentLocation;
                isOnSchedule = IsOnSchedule;
                displayName = DisplayName;
            }
        }

        internal static void SendSchedules() =>
            ModEntry.ModHelper.Multiplayer.SendMessage((Game1.Date.TotalDays + 1, GetSchedules()), ModEntry.ModMessageSchedule);


        internal static void ReceiveSchedules((int, Dictionary<string, NPCSchedule>) Message)
        {
            HasHostSchedules = true;
            Date = SDate.FromDaysSinceStart(Message.Item1).ToWorldDate();
            NpcsWithSchedule = Message.Item2;
        }

        internal static void UpdateCurrentLocation((string, string) Message)
        {
            try
            {
                NpcsWithSchedule[Message.Item1].CurrentLocation = Message.Item2;
            }
            catch
            {
                ModEntry.Console.Log($"Error when trying to update the current location for ${Message.Item1}.", LogLevel.Warn);
            }
        }

        public static bool HasSchedules() => Game1.Date == Date && NpcsWithSchedule != null;


        /// <summary>
        /// Get, parse, and filter NPCs with a schedule. 
        /// </summary>
        /// <param name="onlyShowSocializableNPCs">Filter out NPCs the player can't socialize with (ex: Gunther or Sandy before the bus is repaired)</param>
        /// <param name="onlyShowMetNPCs">Filter out NPCs the player hasn't talked to before.</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, NPCSchedule>> GetSchedules(bool onlyShowSocializableNPCs = false, bool onlyShowMetNPCs = false)
        {
            if (HasSchedules())
            {
                return FilterNPCSchedules(onlyShowSocializableNPCs, onlyShowMetNPCs);
            }
            ModEntry.Console.Log($"Calculating the NPCs' schedule for {Game1.Date}.", LogLevel.Debug);
            List<NPC> npcs = new();
            foreach (var npc in Utility.getAllCharacters())
            {
                if (!npc.followSchedule || (npc.Schedule != null && npc.Schedule.Any()))
                {
                    npcs.Add(npc);
                }
            }
            NpcsWithSchedule = new();
            foreach (var npc in npcs)
            {
                string name = npc.getName();
                string dayScheduleName = string.Empty;
                List<ScheduleEntry> scheduleEntries = null;
                try
                {
                    if (npc.followSchedule)
                    {
                        Dictionary<string, string> rawMasterSchedule = npc.getMasterScheduleRawData();
                        dayScheduleName = npc.dayScheduleName.Value ?? ModEntry.ModHelper.Reflection.GetField<string>(npc, "_lastLoadedScheduleKey").GetValue();
                        string rawSchedule = rawMasterSchedule[dayScheduleName];

                        scheduleEntries = ParseMasterSchedule(rawSchedule, npc);
                    }
                    NpcsWithSchedule.Add(npc.Name, new NPCSchedule(npc, scheduleEntries));
                }
                catch (ArgumentNullException)
                {
                    ModEntry.Console.Log($"Warning! Couldn't find a schedule for {name}. Does the host not have Schedule Viewer installed?", LogLevel.Warn);
                }
                catch (ArgumentException)
                {
                    ModEntry.Console.Log($"Warning! Found an NPC whose name is already in the list. This means you have 2 or more NPCs with the same name. The schedule for {name} might not be accurate.", LogLevel.Warn);
                }
                catch (KeyNotFoundException)
                {
                    ModEntry.Console.Log($"Warning! Couldn't find the schedule called \"{dayScheduleName}\" for {name}. One of your other mods must be messing with {name}'s schedule.", LogLevel.Warn);
                }
            }

            Date = Game1.Date;
            return FilterNPCSchedules(onlyShowSocializableNPCs, onlyShowMetNPCs);
        }

        private static IEnumerable<KeyValuePair<string, NPCSchedule>> FilterNPCSchedules(bool onlyShowSocializableNPCs = false, bool onlyShowMetNPCs = false) =>
            onlyShowSocializableNPCs || onlyShowMetNPCs ?
                NpcsWithSchedule.Where(s =>
                {
                    bool hasMet = Game1.player.friendshipData.ContainsKey(s.Key);
                    return (s.Value.CanSocialize || !onlyShowSocializableNPCs) && (hasMet || !onlyShowMetNPCs);
                }) :
                NpcsWithSchedule.AsEnumerable();

        /// <summary>Slightly adjusted version on the original in-game method NPC::parseMasterSchedule</summary>
        /// <param name="rawData">The raw string data for the days' schedule.</param>
        /// <param name="npc">The npc who's schedule entries we're getting.</param>
        private static List<ScheduleEntry> ParseMasterSchedule(string rawData, NPC npc)
        {
            Dictionary<string, string> masterScheduleRawData = npc.getMasterScheduleRawData();
            string[] split = rawData.Split('/');
            List<ScheduleEntry> entries = new();
            int routesToSkip = 0;
            if (split[0].Contains("GOTO"))
            {
                string newKey2 = split[0].Split(' ')[1];
                if (newKey2.ToLower().Equals("season"))
                {
                    newKey2 = Game1.currentSeason;
                }
                try
                {
                    split = masterScheduleRawData[newKey2].Split('/');
                }
                catch (Exception)
                {
                    // don't think this should happen since npc.dayScheduleName would've been updated when the game hit the original code
                    ModEntry.Console?.Log("Hit code I didn't think we'd hit! - 1", LogLevel.Warn);
                    return ParseMasterSchedule(masterScheduleRawData["spring"], npc);
                }
            }
            if (split[0].Contains("NOT"))
            {
                string[] commandSplit = split[0].Split(' ');
                if (commandSplit[1].ToLower() == "friendship")
                {
                    int index2 = 2;
                    bool conditionMet = false;
                    for (; index2 < commandSplit.Length; index2 += 2)
                    {
                        string who = commandSplit[index2];
                        int level = 0;
                        if (int.TryParse(commandSplit[index2 + 1], out level))
                        {
                            foreach (Farmer allFarmer in Game1.getAllFarmers())
                            {
                                if (allFarmer.getFriendshipHeartLevelForNPC(who) >= level)
                                {
                                    conditionMet = true;
                                    break;
                                }
                            }
                        }
                        if (conditionMet)
                        {
                            break;
                        }
                    }
                    if (conditionMet)
                    {
                        // don't think this should happen since npc.dayScheduleName would've been updated when the game hit the original code
                        ModEntry.Console?.Log("Hit code I didn't think we'd hit! - 2", LogLevel.Warn);
                        return ParseMasterSchedule(masterScheduleRawData["spring"], npc);
                    }
                    routesToSkip++;
                }
            }
            else if (split[0].Contains("MAIL"))
            {
                string mailID = split[0].Split(' ')[1];
                routesToSkip = ((!Game1.MasterPlayer.mailReceived.Contains(mailID) && !NetWorldState.checkAnywhereForWorldStateID(mailID)) ? (routesToSkip + 1) : (routesToSkip + 2));
            }
            if (split[routesToSkip].Contains("GOTO"))
            {
                string newKey = split[routesToSkip].Split(' ')[1];
                if (newKey.ToLower().Equals("season"))
                {
                    newKey = Game1.currentSeason;
                }
                else if (newKey.ToLower().Equals("no_schedule"))
                {
                    //this.followSchedule = false;
                    return null;
                }
                // don't think this should happen since npc.dayScheduleName would've been updated when the game hit the original code
                ModEntry.Console?.Log("Hit code I didn't think we'd hit! - 3", LogLevel.Warn);
                return ParseMasterSchedule(masterScheduleRawData[newKey], npc);
            }

            Point previousPosition = (npc.isMarried() ? new Point(0, 23) : new Point((int)npc.DefaultPosition.X / 64, (int)npc.DefaultPosition.Y / 64));
            string previousGameLocation = npc.isMarried() ? "BusStop" : npc.DefaultMap;
            string default_map = npc.DefaultMap;
            int default_x = (int)(npc.DefaultPosition.X / 64f);
            int default_y = (int)(npc.DefaultPosition.Y / 64f);
            int default_facingDirection = npc.DefaultFacingDirection;
            for (int i = routesToSkip; i < split.Length; i++)
            {
                if (split.Length <= 1)
                {
                    break;
                }
                int index = 0;
                string[] newDestinationDescription = split[i].Split(' ');
                int time;
                bool time_is_arrival_time = false;
                string time_string = newDestinationDescription[index];
                if (time_string.Length > 0 && newDestinationDescription[index][0] == 'a')
                {
                    time_is_arrival_time = true;
                    time_string = time_string.Substring(1);
                }
                time = Convert.ToInt32(time_string);
                index++;
                string location = newDestinationDescription[index];
                string endOfRouteAnimation = null;
                int xLocation = 0;
                int yLocation = 0;
                int localFacingDirection = 2;
                if (location == "bed")
                {
                    if (npc.isMarried())
                    {
                        location = "BusStop";
                        xLocation = -1;
                        yLocation = 23;
                        localFacingDirection = 3;
                    }
                    else
                    {
                        string default_schedule = null;
                        if (npc.hasMasterScheduleEntry("default"))
                        {
                            default_schedule = masterScheduleRawData["default"];
                        }
                        else if (npc.hasMasterScheduleEntry("spring"))
                        {
                            default_schedule = masterScheduleRawData["spring"];
                        }
                        if (default_schedule != null)
                        {
                            try
                            {
                                string[] last_schedule_split = default_schedule.Split('/')[^1].Split(' ');
                                location = last_schedule_split[1];
                                if (last_schedule_split.Length > 3)
                                {
                                    if (!int.TryParse(last_schedule_split[2], out xLocation) || !int.TryParse(last_schedule_split[3], out yLocation))
                                    {
                                        default_schedule = null;
                                    }
                                }
                                else
                                {
                                    default_schedule = null;
                                }
                            }
                            catch (Exception)
                            {
                                default_schedule = null;
                            }
                        }
                        if (default_schedule == null)
                        {
                            location = default_map;
                            xLocation = default_x;
                            yLocation = default_y;
                        }
                    }
                    index++;
                    Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
                    string sleep_behavior = npc.Name.ToLower() + "_sleep";
                    if (dictionary.ContainsKey(sleep_behavior))
                    {
                        endOfRouteAnimation = sleep_behavior;
                    }
                }
                else
                {
                    if (int.TryParse(location, out var _))
                    {
                        location = previousGameLocation;
                        index--;
                    }
                    index++;
                    xLocation = Convert.ToInt32(newDestinationDescription[index]);
                    index++;
                    yLocation = Convert.ToInt32(newDestinationDescription[index]);
                    index++;
                    try
                    {
                        if (newDestinationDescription.Length > index)
                        {
                            if (int.TryParse(newDestinationDescription[index], out localFacingDirection))
                            {
                                index++;
                            }
                            else
                            {
                                localFacingDirection = 2;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        localFacingDirection = 2;
                    }
                }
                if (ModEntry.ModHelper.Reflection.GetMethod(npc, "changeScheduleForLocationAccessibility").Invoke<bool>(location, xLocation, yLocation, localFacingDirection))
                {
                    if (masterScheduleRawData.ContainsKey("default"))
                    {
                        return ParseMasterSchedule(masterScheduleRawData["default"], npc);
                    }
                    return ParseMasterSchedule(masterScheduleRawData["spring"], npc);
                }
                if (index < newDestinationDescription.Length)
                {
                    if (newDestinationDescription[index].Length > 0 && newDestinationDescription[index][0] != '"')
                    {
                        endOfRouteAnimation = newDestinationDescription[index];
                    }
                }
                if (time == 0)
                {
                    default_map = location;
                    default_x = xLocation;
                    default_y = yLocation;
                    default_facingDirection = localFacingDirection;
                    previousGameLocation = location;
                    previousPosition.X = xLocation;
                    previousPosition.Y = yLocation;
                    continue;
                }
                entries.Add(new ScheduleEntry(time, xLocation, yLocation, localFacingDirection, PrettyPrintLocationName(location), endOfRouteAnimation, time_is_arrival_time));
            }
            entries.Insert(0, new ScheduleEntry(0, default_x, default_y, default_facingDirection, PrettyPrintLocationName(default_map), null));
            return entries;
        }

        public static string PrettyPrintLocationName(string location)
        {
            Dictionary<string, string> locationNames = GetLocationNames();
            if (!locationNames.TryGetValue(location, out string locationDisplayName))
            {
                ModEntry.Console.LogOnce($"Couldn't find a display name for location: {location}", LogLevel.Debug);
            }
            return locationDisplayName ?? GetUnknownLocationName(location);
        }

        public static string PrettyPrintLocationName(GameLocation location) =>
            location.IsFarm && location.isStructure.Value ?
                $"{PrettyPrintLocationName("Farm")} - {location.Name}" :
                PrettyPrintLocationName(location.Name);

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string GetFarmName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11064", Game1.player.IsLocalPlayer ? Game1.player.farmName.Value : "")?.Trim();

        public static string GetUnknownLocationName(string location)
        {
            if (location.StartsWith("Custom"))
            {
                return SplitCamelCase(location.Substring(7));
            }
            if (location.Equals("FarmHouse"))
            {
                return $"{GetFarmName()} - {ModEntry.ModHelper.Translation.Get("farmhouse")}";
            }
            if (location.StartsWith("Island"))
            {
                return $"{Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandName")} - {SplitCamelCase(location.Substring(6))}";
            }
            return SplitCamelCase(location);
        }

        public static Dictionary<string, string> GetLocationNames()
        {
            if (LocationNames?.Count > 0) return LocationNames;
            LocationNames = new Dictionary<string, string>
            {
                { "AdventureGuild", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11099") },
                { "AnimalShop", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11068") },
                { "ArchaeologyHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11086") },
                { "BathHouse_Entry", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") },
                { "BathHouse_MensLocker", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") },
                { "BathHouse_Pool", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") },
                { "BathHouse_WomensLocker", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11110") },
                { "Beach", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11174") },
                { "Blacksmith", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11081") },
                { "BusStop", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11066") },
                { "Club", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") },
                { "CommunityCenter", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11117") },
                { "Desert", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") },
                { "ElliottHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11088") },
                { "Farm", GetFarmName() },
                { "FishShop", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11107") },
                { "Forest", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11186") },
                { "HaleyHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11073") },
                { "HarveyRoom", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11076") },
                { "Hospital", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11076") },
                { "JojaMart", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11105") },
                { "JoshHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11092") },
                { "LeahHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11070") },
                { "ManorHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11085") },
                { "Mine", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11098") },
                { "Mountain", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11176") },
                { "Railroad", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11119") },
                { "Saloon", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11172") },
                { "SamHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11071") },
                { "SandyHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") },
                { "SandyShop", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") },
                { "ScienceHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11094") },
                { "SebastianRoom", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11094") },
                { "SeedShop", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11078") },
                { "SkullCave", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11062") },
                { "Sunroom", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11078") },
                { "Tent", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11097") },
                { "Town", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11190") },
                { "Trailer", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11091") },
                { "Trailer_Big", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.PamHouse") },
                { "UndergroundMine", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11098") },
                { "WitchWarpCave", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11119") },
                { "WizardHouse", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11067") },
                { "WizardHouseBasement", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11067") },
                { "Woods", Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11114") }
            };
            // Add custom locations (i.e. from other mods)
            foreach (var customLocation in ModEntry.CustomLocationNames)
            {
                LocationNames[customLocation.Key] = customLocation.Value;
            }
            return LocationNames;
        }
    }
}
