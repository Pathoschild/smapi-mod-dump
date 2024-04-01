/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;

namespace BushBloomMod {
    internal class Schedule {
        private static IModHelper Helper;
        private static IMonitor Monitor;
        private static Config Config;

        public static void Register(IModHelper helper, IMonitor monitor, Config config) {
            Helper = helper;
            Monitor = monitor;
            Config = config;
        }

        private static string MakeId(ContentEntry entry, string shakeOff = null) => $"{shakeOff ?? entry.ShakeOff};{Helpers.GetDayOfYear(entry.StartSeason, entry.StartDay)};{Helpers.GetDayOfYear(entry.EndSeason, entry.EndDay)};{entry.StartYear};{entry.EndYear};{entry.Chance};{string.Join(",", entry.Locations ?? Array.Empty<string>())};{string.Join(",", entry.ExcludeLocations ?? Array.Empty<string>())};{string.Join(",", entry.Weather ?? Array.Empty<string>())};{string.Join(",", entry.ExcludeWeather ?? Array.Empty<string>())};{string.Join(",", entry.DestroyWeather ?? Array.Empty<string>())};{entry.Texture}";

        private bool IsDefault;

        public bool IsEnabled() => (this.Entry.Enabled ?? true) && (Config.EnableDefaultSchedules || !this.IsDefault);

        public ContentEntry Entry { get; private set; }

        public Texture2D Texture { get; private set; }

        // need to validate this later is startup to ensure other mod content has loaded and can be found
        private string _shakeOffId;
        public string ShakeOffId {
            get {
                if (this._shakeOffId == null) {
                    this._shakeOffId = Helpers.GetItemIdFromName(this.Entry.ShakeOff);
                    if (this._shakeOffId == null) {
                        Monitor.Log($"Invalid shakeoff item: {MakeId(this.Entry)}", LogLevel.Error);
                        this._shakeOffId = "";
                        this.Entry.Enabled = false;
                    }
                }
                return this._shakeOffId;
            }
        }

        // need to delay id construction for the same reason as shake off id
        private string _id;
        public string Id {
            get => this._id ??= MakeId(this.Entry, this.ShakeOffId);
        }

        private static readonly List<Schedule> Entries = new();

        public static void ReloadEntries() {
            Entries.Clear();
            try {
                AddEntries(Helper.DirectoryPath, Helper.Data.ReadJsonFile<ContentEntry[]>("content.json"));
            } catch {
                Monitor.Log($"Unable to load content pack: {Path.Combine(Helper.DirectoryPath, "content.json")}. Review that file for syntax errors.", LogLevel.Error);
            }
            foreach (var pack in Helper.ContentPacks.GetOwned()) {
                try {
                    AddEntries(pack.DirectoryPath, pack.ReadJsonFile<ContentEntry[]>("content.json"));
                } catch {
                    Monitor.Log($"Unable to load content pack: {Path.Combine(pack.DirectoryPath, "content.json")}. Review that file for syntax errors.", LogLevel.Error);
                }
            }
        }

        private static void AddEntries(string contentDirectory, ContentEntry[] content) {
            var isDefault = Entries.Count == 0;
            foreach (var entry in content) {
                entry.EndSeason ??= entry.StartSeason;
                entry.EndDay ??= entry.StartDay;
                entry.StartYear ??= 1;
                entry.Chance ??= 0.2;
                var sched = new Schedule() {
                    IsDefault = isDefault,
                    Entry = entry
                };
                if (entry.Texture is not null) {
                    entry.Texture = string.Join(Path.DirectorySeparatorChar, entry.Texture.Split('/', '\\'));
                    var fullPath = Path.Combine(contentDirectory, entry.Texture);
                    try {
                        sched.Texture = Texture2D.FromFile(Game1.graphics.GraphicsDevice, fullPath);
                    } catch {
                        Monitor.Log($"Failed to load texture: {MakeId(entry)}", LogLevel.Error);
                        continue;
                    }
                }
                if (!entry.IsValid()) {
                    Monitor.Log($"Invalid bloom schedule: {MakeId(entry)}", LogLevel.Error);
                    continue;
                }
                //Monitor.Log($"Valid bloom schedule: {MakeId(entry)}", LogLevel.Error);
                Entries.Add(sched);
            }
        }

        public static IEnumerable<Schedule> GetAllCandidates(int year, int doy, bool ignoreWeather, bool allowExisting, GameLocation location = null) =>
            Entries.Where(e => e.IsEnabled() && e.Entry.CanBloomToday(year, doy, ignoreWeather, allowExisting, location));

        private const string KeyBushDay = "bush-day", KeyBushSchedule = "bush-schedule";

        public static void SetSchedule(Bush bush, string id) => bush.modData[$"{Helper.ModContent.ModID}/{KeyBushSchedule}"] = id;

        public static Schedule GetExistingSchedule(Bush bush) {
            if (bush.IsAbleToBloom()
                && bush.modData.TryGetValue($"{Helper.ModContent.ModID}/{KeyBushSchedule}", out var value)
                && value is not null
                && value != "-1"
            ) {
                return Entries.Where(e => e.IsEnabled() && string.Compare(e.Id, value) == 0).FirstOrDefault();
            }
            return null;
        }

        public static Schedule GetSchedule(Bush bush) {
            if (!bush.IsAbleToBloom())
                return null;
            Schedule entry = null;
            var doy = Helpers.GetDayOfYear(bush.Location.GetSeasonKey(), Game1.dayOfMonth);
            var yDays = (Game1.year - 1) * 112;
            // check if we already picked a schedule for this bush and it's still valid
            if (bush.modData.TryGetValue($"{Helper.ModContent.ModID}/{KeyBushDay}", out var value) && int.TryParse(value ?? "", out var i)
                && i == doy + yDays - 1 // it had to be yesterday's schedule
                && bush.modData.TryGetValue($"{Helper.ModContent.ModID}/{KeyBushSchedule}", out value) && value is not null && value != "-1"
            ) {
                // check for valid schedules
                entry = GetAllCandidates(Game1.year, doy, false, true, bush.Location).FirstOrDefault(e => e.IsEnabled() && string.Compare(e.Id, value) == 0);
            }
            if (entry == null) { // found no existing valid schedule
                var candidates = GetAllCandidates(Game1.year, doy, false, false, bush.Location);
                // check if bush is not blooming already, or is from an active schedule and use same
                if (bush.tileSheetOffset.Value == 0
                    || !bush.modData.TryGetValue($"{Helper.ModContent.ModID}/{KeyBushSchedule}", out value) || value is null
                    || (entry = candidates.FirstOrDefault(e => string.Compare(e.Id, value) == 0)) is null
                ) {
                    // else get a new active schedule
                    // calculate the chance of NOT blooming today, based on all schedules
                    var chance = candidates
                        .Select(e => 1.0 - e.Entry.Chance)
                        .DefaultIfEmpty(1.0)
                        .Aggregate((a, e) => a * e);
                    // is the bush blooming?
                    if (Game1.random.NextDouble() > chance) {
                        // select a schedule from the candidates
                        var selected = candidates.Sum(e => e.Entry.Chance) * Game1.random.NextDouble();
                        entry = candidates
                            // find the selected schedule
                            .FirstOrDefault(e => (selected -= e.Entry.Chance) <= 0.0);
                    }
                }
            }
            bush.modData[$"{Helper.ModContent.ModID}/{KeyBushDay}"] = (doy + yDays).ToString();
            SetSchedule(bush, entry?.Id ?? "-1");
            return entry;
        }
    }
}