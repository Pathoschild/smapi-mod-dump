/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using System;
using StardewModdingAPI.Events;

namespace BushBloomMod {
    internal class Schedule {
        public static readonly string ContentRoot = "NCarigon.BushBloomMod";
        public static readonly string SchedulePath = PathUtilities.NormalizeAssetName(ContentRoot + "/Schedules");
        public static readonly string TexturePrefix = PathUtilities.NormalizeAssetName(ContentRoot + "/Textures");

        public static void Content_AssetRequested(object sender, AssetRequestedEventArgs e) {
            if (e.NameWithoutLocale.Name.Equals(SchedulePath, StringComparison.InvariantCultureIgnoreCase)) {
                e.LoadFrom(() => new Dictionary<string, ContentEntry>(ReadContentPacks().Select(c => new KeyValuePair<string, ContentEntry>(c.Id, c))), AssetLoadPriority.Exclusive);
            } else if (e.NameWithoutLocale.Name.StartsWith(TexturePrefix, StringComparison.InvariantCultureIgnoreCase)) {
                if (e.NameWithoutLocale.Name.Equals(TexturePrefix + "/WinterBerry")) {
                    e.LoadFrom(() => Texture2D.FromFile(Game1.graphics.GraphicsDevice, Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "assets", "winter.png")), AssetLoadPriority.Exclusive);
                } else {
                    var imgPath = LoadedSchedules
                        .Where(s => !string.IsNullOrWhiteSpace(s.Entry.Texture)
                            && e.NameWithoutLocale.Name.Equals(PathUtilities.NormalizeAssetName(TexturePrefix + "/" + s.Entry.Id), StringComparison.InvariantCultureIgnoreCase)
                            && File.Exists(s.Entry.Texture))
                        .Select(s => s.Entry.Texture)
                        .FirstOrDefault();
                    if (imgPath is not null) {
                        e.LoadFrom(() => Texture2D.FromFile(Game1.graphics.GraphicsDevice, imgPath), AssetLoadPriority.Exclusive);
                    } else {
                        // create a fake texture we can detect and ignore later. CP overwrites will modify it and bypass the ignore.
                        e.LoadFrom(() => new Texture2D(Game1.graphics.GraphicsDevice, 32, 1), AssetLoadPriority.Exclusive);
                    }
                }
            }
        }

        public bool IsEnabled() => (this.Entry.Enabled ?? true) && ModEntry.Instance.Config.EnableDefaultSchedules;

        public ContentEntry Entry { get; private set; }

        public Texture2D Texture {
            // check the cache, and ensure it's not a fake texture (height)
            get {
                var t = Game1.content.Load<Texture2D>(PathUtilities.NormalizeAssetName(TexturePrefix + "/" + this.Entry.Id));
                return (t?.Height ?? 0) > 1 ? t : null;
            }
        }

        public static Texture2D WinterBerry {
            get {
                var t = Game1.content.Load<Texture2D>(TexturePrefix + "/WinterBerry");
                return (t?.Height ?? 0) > 1 ? t : null;
            }
        }

        // need to validate this later in startup to ensure other mod content has loaded and can be found
        private string _shakeOffId;
        public string ShakeOffId {
            get => this._shakeOffId ??= Helpers.GetItemIdFromName(this.Entry.ShakeOff);
        }

        public static IEnumerable<Schedule> LoadedSchedules {
            get => Game1.content.Load<Dictionary<string, ContentEntry>>(SchedulePath)
                .Where(s => {
                    if (!s.Value.IsValid()) {
                        ModEntry.Instance.Monitor.Log($"Invalid bloom schedule: {s.Value.Id}", LogLevel.Error);
                        return false;
                    }
                    s.Value.Id = s.Key;
                    return true;
                }).Select(s => new Schedule() { Entry = s.Value });
        }

        public static bool IsReloading { get; private set; }

        public static void ReloadSchedules() {
            IsReloading = true;
            ModEntry.Instance.Helper.GameContent.InvalidateCache(SchedulePath);
            _ = LoadedSchedules.Any();
            IsReloading = false;
        }

        private static IEnumerable<ContentEntry> ReadContentPacks() {
            var entries = new List<ContentEntry>();
            try {
                ParseContentPackEntries(entries, ModEntry.Instance.Helper.DirectoryPath, ModEntry.Instance.Helper.Data.ReadJsonFile<ContentEntry[]>("content.json"));
            } catch {
                ModEntry.Instance.Monitor.Log($"Unable to load content pack: {new DirectoryInfo(ModEntry.Instance.Helper.DirectoryPath).Name}. Review that file for syntax errors.", LogLevel.Error);
            }
            foreach (var pack in ModEntry.Instance.Helper.ContentPacks.GetOwned()) {
                try {
                    ParseContentPackEntries(entries, pack.DirectoryPath, pack.ReadJsonFile<ContentEntry[]>("content.json"));
                } catch {
                    ModEntry.Instance.Monitor.Log($"Unable to load content pack: {new DirectoryInfo(pack.DirectoryPath).Name}. Review that file for syntax errors.", LogLevel.Error);
                }
            }
            return entries;
        }

        private static void ParseContentPackEntries(List<ContentEntry> Entries, string contentDirectory, ContentEntry[] content) {
            var isDefault = Entries.Count == 0;
            var dir = new DirectoryInfo(contentDirectory).Name;
            var names = new List<string>();
            foreach (var entry in content) {
                entry.IsDefault ??= isDefault;
                entry.Id ??= $"{(isDefault ? "default" : dir)}/{entry.StartSeason}_{entry.ShakeOff}";
                var count = 0;
                while (names.Contains(entry.Id, StringComparer.InvariantCultureIgnoreCase)) {
                    count++;
                    entry.Id = $"{(isDefault ? "default" : dir)}/{entry.StartSeason}_{entry.ShakeOff}_{count}";
                }
                names.Add(entry.Id);
                if (entry.Texture is not null) {
                    entry.Texture = Path.Combine(contentDirectory, string.Join(Path.DirectorySeparatorChar, entry.Texture.Split('/', '\\')));
                }
                if (!entry.IsValid()) {
                    ModEntry.Instance.Monitor.Log($"Error in content pack: {dir}; Invalid bloom schedule: {entry.Id}", LogLevel.Error);
                    continue;
                }
                Entries.Add(entry);
            }
        }

        public static IEnumerable<Schedule> GetAllCandidates(int year, int doy, bool ignoreWeather, bool allowExisting, GameLocation location = null, Vector2? tile = null) {
            var candidates = LoadedSchedules.Where(e => e.IsEnabled() && e.Entry.CanBloomToday(year, doy, ignoreWeather, allowExisting, location, tile));
            return candidates.Any(e => e.Entry.Chance > 1f)
                ? candidates.Where(e => e.Entry.Chance > 1f)
                : candidates;
        }

        public static bool TryGetExistingSchedule(Bush bush, out Schedule schedule) {
            schedule = null;
            if (bush.IsAbleToBloom() && bush.TryDataGetSchedule(out var value)) {
                schedule = LoadedSchedules.Where(e => e.IsEnabled() && string.Compare(e.Entry.Id, value, true) == 0).FirstOrDefault();
            }
            return schedule is not null;
        }

        public static Schedule GetSchedule(Bush bush) {
            if (!bush.IsAbleToBloom())
                return null;
            var doy = Helpers.GetDayOfYear(bush.Location.GetSeasonKey(), Game1.dayOfMonth);
            // check if bush already has a valid schedule
            if (!(TryGetExistingSchedule(bush, out var sched) && sched.Entry.CanBloomToday(Game1.year, doy, false, true, bush.Location, bush.Tile))) {
                sched = null;
                // if not, search for one
                var candidates = GetAllCandidates(Game1.year, doy, false, false, bush.Location, bush.Tile);
                // calculate the chance of NOT blooming today, based on all schedules
                if (candidates.Any()) {
                    var chance = candidates
                        .Select(e => 1.0 - e.Entry.Chance)
                        .DefaultIfEmpty(1.0)
                        .Aggregate((a, e) => a * e);
                    // is the bush blooming?
                    // we create a predicatable random for each bush/day check so we can be consistent if recalled
                    if (Utility.CreateRandom(Game1.uniqueIDForThisGame, bush.Tile.X * 100 + bush.Tile.Y * 1000, Game1.year, doy).NextDouble() > chance) {
                        // select a schedule from the candidates
                        var selected = candidates.Sum(e => e.Entry.Chance) * Game1.random.NextDouble();
                        sched = candidates.FirstOrDefault(e => (selected -= e.Entry.Chance) <= 0.0);
                    }
                }
            }
            bush.DataSetSchedule(sched?.Entry?.Id);
            return sched;
        }
    }
}