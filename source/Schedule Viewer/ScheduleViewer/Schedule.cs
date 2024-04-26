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
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleViewer
{
    static public class Schedule
    {
        private static WorldDate Date;
        private static Dictionary<string, NPCSchedule> NpcsWithSchedule;

        public class ScheduleEntry
        {
            public int Time { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
            public string Location { get; init; }
            public int FacingDirection { get; init; }
            public string Animation { get; init; }
            protected string _hoverText;
            public string HoverText
            {
                get
                {
                    if (_hoverText != null)
                    {
                        return _hoverText;
                    }
                    string facingDirection = ModEntry.ModHelper.Translation.Get($"facing_direction_{this.FacingDirection}");
                    List<string> lines = new()
                    {
                        ModEntry.ModHelper.Translation.Get("location_hover_text", new { x = X, y = Y, facingDirection })
                    };
                    if (this.Animation != null)
                    {
                        lines.Add(this.Animation);
                    }
                    _hoverText = string.Join(Environment.NewLine, lines);
                    return _hoverText;
                }
            }

            public ScheduleEntry(SchedulePathDescription schedulePathDescription)
            {
                Time = schedulePathDescription.time;
                X = schedulePathDescription.targetTile.X;
                Y = schedulePathDescription.targetTile.Y;
                Location = PrettyPrintLocationName(schedulePathDescription.targetLocationName);
                FacingDirection = schedulePathDescription.facingDirection;
                Animation = schedulePathDescription.endOfRouteBehavior;
            }

            [JsonConstructor]
            public ScheduleEntry(int time, Vector2 position, int facingDirection, string location, string animation)
            {
                Time = time;
                Location = PrettyPrintLocationName(location);
                FacingDirection = facingDirection;
                Animation = animation;
                // convert position (pixels) to tile coords
                Vector2 tile = position / 64f;
                X = (int)tile.X;
                Y = (int)tile.Y;
            }

            public override string ToString()
            {
                return $"{Game1.getTimeOfDayString(this.Time != 0 ? this.Time : 600)} {this.Location ?? "Unknown"}";
            }
        }

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

            public NPCSchedule(NPC npc, List<ScheduleEntry> entries)
            {
                DisplayName = npc.getName();
                Entries = entries;
                CanSocialize = npc.CanSocialize;
                CurrentLocation = PrettyPrintLocationName(npc.currentLocation);
                IsOnSchedule = npc.followSchedule && !npc.ignoreScheduleToday;
                NPC = npc;
            }

            [JsonConstructor]
            public NPCSchedule(string displayName, List<ScheduleEntry> entries, bool canSocialize, bool isOnSchedule, string currentLocation)
            {
                DisplayName = displayName;
                Entries = entries;
                CanSocialize = canSocialize;
                IsOnSchedule = isOnSchedule;
                CurrentLocation = currentLocation;
            }

            internal void Deconstruct(out List<ScheduleEntry> entries, out string currentLocation, out bool isOnSchedule, out string displayName, out NPC npc)
            {
                entries = Entries;
                currentLocation = CurrentLocation;
                isOnSchedule = IsOnSchedule;
                displayName = DisplayName;
                npc = NPC;
            }
        }

        internal static void SendSchedules() =>
            ModEntry.ModHelper.Multiplayer.SendMessage((Game1.Date.TotalDays + 1, GetSchedules()), ModEntry.ModMessageSchedule);


        internal static void ReceiveSchedules((int, Dictionary<string, NPCSchedule>) Message)
        {
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
                ModEntry.Console.Log($"Error when trying to update the current location for {Message.Item1}.", LogLevel.Warn);
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
            NpcsWithSchedule = new();
            Utility.ForEachVillager(npc =>
            {
                string name = npc.getName();
                List<ScheduleEntry> scheduleEntries = null;
                try
                {
                    if (npc.followSchedule)
                    {
                        scheduleEntries = new()
                        {
                            new(0, npc.DefaultPosition, npc.DefaultFacingDirection, npc.DefaultMap, null)
                        };
                        foreach (var item in npc.Schedule.Values)
                        {
                            scheduleEntries.Add(new ScheduleEntry(item));
                        }
                    }
                    NpcsWithSchedule.Add(npc.Name, new NPCSchedule(npc, scheduleEntries));
                }
                catch (ArgumentNullException)
                {
                    // this error might not be necessary anymore with 1.6 changes
                    ModEntry.Console.Log($"Warning! Couldn't find a schedule for {name}. Does the host not have Schedule Viewer installed?", LogLevel.Warn);
                }
                catch (ArgumentException)
                {
                    int count = NpcsWithSchedule.Keys.Where(key => key.StartsWith(name)).Count() + 1;
                    // there are 2 Mister Qi in vanilla - one at desert club and one in the walnut room
                    if (!name.Equals("Mister Qi") || count != 2)
                    {
                        ModEntry.Console.Log($"Warning! Found an NPC whose name is already in the list. There have been {count} instances of {name} observed currently. You may seen duplicates of {name} in the list.", LogLevel.Warn);
                    }
                    NpcsWithSchedule.Add($"{name} ({count})", new NPCSchedule(npc, scheduleEntries));
                }
                return true;
            });

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

        /// <summary>
        /// Try and get an override for a location name.
        /// </summary>
        /// <param name="location">The internal game location name</param>
        /// <param name="overrideName">The override location name if it exists</param>
        /// <returns>true if a value is found, otherwise false</returns>
        private static bool TryGetOverrideLocationName(string location, out string overrideName)
        {
            switch (location)
            {
                // override game's displayName in favor of address
                case "HaleyHouse":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11073");
                    return true;
                case "JoshHouse":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11092");
                    return true;
                case "QiNutRoom":
                    overrideName = Game1.content.LoadString("Strings\\WorldMap:GingerIsland_West_QiWalnutRoom");
                    return true;
                case "SamHouse":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11071");
                    return true;
                case "Trailer":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11091");
                    return true;
                case "Trailer_Big":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.PamHouse");
                    return true;
                // locations without a game provided displayName
                case "Club":
                case "HarveyRoom":
                case "IslandEast":
                case "IslandHut":
                case "IslandNorth":
                case "IslandSouth":
                case "IslandWest":
                case "LeoTreeHouse":
                case "SandyHouse":
                case "SebastianRoom":
                case "SkullCave":
                case "Sunroom":
                case "WitchSwamp":
                case "WitchWarpCave":
                    overrideName = ModEntry.ModHelper.Translation.Get($"location_names.{location}");
                    return true;
                default:
                    overrideName = null;
                    return false;
            }
        }

        public static string PrettyPrintLocationName(string location)
        {
            if (TryGetOverrideLocationName(location, out string overrideName))
            {
                return overrideName;
            }

            GameLocation gameLocation = Game1.getLocationFromName(location) ?? Game1.getLocationFromName(location, true);
            return gameLocation?.DisplayName ?? location;
        }

        public static string PrettyPrintLocationName(GameLocation location)
        {
            if (TryGetOverrideLocationName(location.Name, out string overrideName))
            {
                return overrideName;
            }
            return location.DisplayName;
        }
    }
}
