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
using Newtonsoft.Json.Linq;
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
        /// <summary>The date that <see cref="NpcsWithSchedule">NpcsWithSchedule</see> was generated for</summary>
        private static WorldDate Date;
        /// <summary>Tile Areas that the player needs to have 2 or more hearts with to enter</summary>
        private static readonly List<TileArea> AccessTileAreas = new();
        /// <summary>Tile Areas that describe a named location</summary>
        private static readonly List<TileArea> GeneralTileAreas = new();
        /// <summary>Map of location internal names to display names that overrides the location's DisplayName</summary>
        private static readonly Dictionary<string, string> LocationOverrides = new();
        /// <summary>Map of internal NPC names to their schedule for <see cref="Date">Date</see></summary>
        private static Dictionary<string, NPCSchedule> NpcsWithSchedule;

        public class ScheduleEntry
        {
            public int Time { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
            /// <summary>Internal game location name. </summary>
            public string Location { get; init; }
            public int FacingDirection { get; init; }
            public string Animation { get; init; }

            [JsonIgnore]
            protected string _hoverText;
            [JsonIgnore]
            protected string _locationName;

            [JsonIgnore]
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
            [JsonIgnore]
            public string LocationName
            {
                get
                {
                    if (_locationName != null)
                    {
                        return _locationName;
                    }
                    string locationName = PrettyPrintLocationName(Location) ?? "???";
                    Point tile = new(X, Y);
                    foreach (TileArea tileArea in GeneralTileAreas)
                    {
                        if (tileArea.Location.Equals(this.Location) && IsTileInTileArea(tileArea, tile))
                        {
                            locationName = tileArea.OverrideLocationName ? tileArea.DisplayName : $"{locationName} ({tileArea.DisplayName})";
                            break;
                        }
                    }
                    _locationName = locationName;
                    return _locationName;
                }
            }

            [JsonIgnore]
            public bool CanAccess { get; set; }

            public ScheduleEntry(SchedulePathDescription schedulePathDescription)
            {
                Time = schedulePathDescription.time;
                X = schedulePathDescription.targetTile.X;
                Y = schedulePathDescription.targetTile.Y;
                Location = schedulePathDescription.targetLocationName;
                FacingDirection = schedulePathDescription.facingDirection;
                Animation = schedulePathDescription.endOfRouteBehavior;
                CanAccess = true;
            }

            [JsonConstructor]
            public ScheduleEntry(int time, Vector2 position, int facingDirection, string location, string animation)
            {
                Time = time;
                Location = location;
                FacingDirection = facingDirection;
                Animation = animation;
                CanAccess = true;
                // convert position (pixels) to tile coords
                Vector2 tile = position / 64f;
                X = (int)tile.X;
                Y = (int)tile.Y;
            }

            public override string ToString()
            {
                return $"{Game1.getTimeOfDayString(this.Time != 0 ? this.Time : 600)} {this.LocationName ?? "Unknown"}";
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

        public static void ClearSchedules()
        {
            ModEntry.Console.Log("Clearing any previously generated schedules.", LogLevel.Debug);
            Date = null; 
            NpcsWithSchedule = null;
        }

        internal static void UpdateCurrentLocation(string location, string[] npcsToUpdate)
        {
            foreach (var npc in npcsToUpdate)
            {
                try
                {
                    NpcsWithSchedule[npc].CurrentLocation = location;
                }
                catch
                {
                    ModEntry.Console.Log($"Error when trying to update the current location for {npc} to {location}.", LogLevel.Warn);
                }
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
                        if (npc.Schedule == null || !npc.Schedule.Any())
                        {
                            // if following schedule but don't have entries then don't add to list
                            return true;
                        }

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
                    NpcsWithSchedule.Add($"{name}-{ModEntry.ModHelper.ModRegistry.ModID}-{count}", new NPCSchedule(npc, scheduleEntries));
                }
                catch (Exception ex)
                {
                    ModEntry.Console.Log($"Something went wrong when trying to add {name}'s schedule. See below for more details:", LogLevel.Error);
                    ModEntry.Console.Log(ex.ToString(), LogLevel.Error);
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
            // check tile areas for an override name first
            if (LocationOverrides.TryGetValue(location, out string tileAreaOverrideName))
            {
                overrideName = tileAreaOverrideName;
                return true;
            }

            // if not using address then don't need to override vanilla location display names
            if (!ModEntry.Config.UseAddress)
            {
                overrideName = null;
                return false;
            }

            switch (location)
            {
                // override game's displayName in favor of address
                case "HaleyHouse":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11073");
                    return true;
                case "JoshHouse":
                    overrideName = Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11092");
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

        #region Tile Areas
        public record TileArea(string Location, Rectangle? TileRectangle, string DisplayName, string[] Npcs = null, Point[] Tiles = null, bool OverrideLocationName = false);

        public static void LoadTileAreas()
        {
            Dictionary<string, JObject> tileAreas = Game1.content.Load<Dictionary<string, JObject>>(ModEntry.CustomDataPath);
            foreach (var entry in tileAreas)
            {
                JObject tileArea = entry.Value;
                try
                {
                    Rectangle? tileRectangle = null;
                    if (tileArea.ContainsKey("TileRectangle"))
                    {
                        JObject tr = tileArea.Value<JObject>("TileRectangle");
                        tileRectangle = new(tr.Value<int>("X"), tr.Value<int>("Y"), tr.Value<int>("Width"), tr.Value<int>("Height"));
                    }
                    Point[] tiles = tileArea.ContainsKey("Tiles")
                        ? tileArea.Value<JArray>("Tiles").Select(tile => new Point(tile.Value<int>("X"), tile.Value<int>("Y"))).ToArray()
                        : null;
                    string[] npcs = tileArea.ContainsKey("Npcs")
                        ? tileArea.Value<JArray>("Npcs").Select(npc => npc.ToString()).ToArray()
                        : null;
                    string location = tileArea.Value<string>("Location");
                    string displayName = tileArea.Value<string>("DisplayName");
                    displayName = displayName.StartsWith("tile_area") ? ModEntry.ModHelper.Translation.Get(displayName) : displayName.StartsWith("Strings\\") ? Game1.content.LoadString(displayName) : displayName;
                    bool overrideLocationName = tileArea.ContainsKey("OverrideLocationName") && tileArea.Value<bool>("OverrideLocationName");

                    TileArea toAdd = new(location, tileRectangle, displayName, npcs, tiles, overrideLocationName);

                    if (toAdd.Npcs == null)
                    {
                        GeneralTileAreas.Add(toAdd);
                    }
                    else
                    {
                        AccessTileAreas.Add(toAdd);
                    }
                    if (overrideLocationName && tileRectangle == null && tiles == null)
                    {
                        LocationOverrides[location] = displayName;
                    }
                }
                catch
                {
                    ModEntry.Console.Log($"Failed to load TileArea \"{tileArea.Value<string>("DisplayName")}\" in \"{tileArea.Value<string>("Location")}\"", LogLevel.Warn);
                }
            }
        }

        public static void UpdateScheduleEntriesCanAccess(NPCSchedule schedule)
        {
            if (schedule.Entries == null || !schedule.Entries.Any()) return;

            List<TileArea> tileAreasForNpc = null;
            try
            {
                tileAreasForNpc = AccessTileAreas.FindAll(tileArea => tileArea.Npcs.Contains(schedule.NPC.Name));
            }
            catch (Exception ex)
            {
                ModEntry.Console.Log($"Error checking AccessTileAreas for {schedule.DisplayName}'s schedule. See details below:\n{ex}", LogLevel.Error);
            }
            if (tileAreasForNpc == null || !tileAreasForNpc.Any()) return;

            foreach (var entry in schedule.Entries)
            {
                try
                {
                    TileArea location = tileAreasForNpc.Find(tileArea => entry.Location.Equals(tileArea.Location));
                    if (location == null) continue;

                    if (IsTileInTileArea(location, new Point(entry.X, entry.Y)))
                    {
                        bool hasTwoHearts = location.Npcs.Any(name => Game1.player.getFriendshipHeartLevelForNPC(name) >= 2);
                        entry.CanAccess = hasTwoHearts;
                    }
                }
                catch (Exception ex)
                {
                    ModEntry.Console.Log($"Error checking AccessTileAreas for {schedule.DisplayName}'s schedule entry: {entry}. See details below:\n{ex}", LogLevel.Error);
                }
            }
        }

        private static bool IsTileInTileArea(TileArea tileArea, Point tile)
        {
            bool? inTileArea = null;
            if (tileArea.TileRectangle != null)
            {
                inTileArea = ((Rectangle)tileArea.TileRectangle).Contains(tile);
            }
            if (inTileArea != true && tileArea.Tiles != null)
            {
                inTileArea = tileArea.Tiles.Contains(tile);
            }
            inTileArea ??= true;
            return (bool)inTileArea;
        }
        #endregion
    }
}
