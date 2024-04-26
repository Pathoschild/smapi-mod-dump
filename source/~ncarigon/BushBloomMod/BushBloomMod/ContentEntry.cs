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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace BushBloomMod {
    internal class ContentEntry {
#pragma warning disable CS0649
        public bool? Enabled, IsDefault;
        public string Id;
        public string ShakeOff;
        public string StartSeason, EndSeason;
        public int? StartDay, EndDay;
        public int? StartYear, EndYear;
        public string Texture;
        public double? Chance;
        public string[] Locations, ExcludeLocations;
        public string[] Weather, ExcludeWeather, DestroyWeather;
        public Vector2[] Tiles;
#pragma warning restore CS0649

        public bool CanBloomToday(int year, int dayOfYear, bool ignoreWeather, bool allowExisting, GameLocation location = null, Vector2? tile = null) {
            if (!(this.Enabled ?? true)) {
                return false;
            }
            var firstDayOfYear = Helpers.GetDayOfYear(this.StartSeason, this.StartDay);
            var lastDayOfYear = Helpers.GetDayOfYear(this.EndSeason, this.EndDay);
            var weather = location?.GetWeather().Weather;
            var bloom = ((this.Locations?.Length ?? 0) < 1 || location is null || this.Locations.Contains(location?.NameOrUniqueName, StringComparer.OrdinalIgnoreCase))
                && ((this.ExcludeLocations?.Length ?? 0) < 1 || location is null || !this.ExcludeLocations.Contains(location?.NameOrUniqueName, StringComparer.OrdinalIgnoreCase))
                && !(location?.InIslandContext() ?? false)
                && ((firstDayOfYear <= dayOfYear && dayOfYear <= lastDayOfYear)  // xxxxF----D----Lxxxxx, on a day between first and last day of same year, or same day
                || (firstDayOfYear > lastDayOfYear && dayOfYear >= firstDayOfYear)  // ----LxxxxF----D-----, on a day after first day, last day is next year
                || (dayOfYear <= lastDayOfYear && firstDayOfYear > lastDayOfYear)) // ----D----LxxxxF-----, on a day before last day, first day is next year
                && (!this.StartYear.HasValue || year >= this.StartYear.Value)
                && (!this.EndYear.HasValue || year <= this.EndYear.Value)
                && ((this.Tiles?.Length ?? 0) < 1 || tile is null || this.Tiles.Contains(tile.Value));// tile.Value.X + "," + tile.Value.Y)));
            if (bloom && weather is not null && !ignoreWeather) {
                bloom = ((this.DestroyWeather?.Length ?? 0) < 1 || !this.DestroyWeather.Contains(weather, StringComparer.OrdinalIgnoreCase));
                if (bloom && !allowExisting) {
                    bloom = ((this.Weather?.Length ?? 0) < 1 || this.Weather.Contains(weather, StringComparer.OrdinalIgnoreCase))
                        && ((this.ExcludeWeather?.Length ?? 0) < 1 || !this.ExcludeWeather.Contains(weather, StringComparer.OrdinalIgnoreCase));
                }
            }
            return bloom;
        }

        public bool IsValid() {
            this.IsDefault ??= false;
            this.EndSeason ??= this.StartSeason;
            this.EndDay ??= this.StartDay;
            this.StartYear ??= 1;
            this.Chance ??= 0.2;
            return this.FirstDay is not null
                && this.LastDay is not null
                && Helpers.IsValidDayOfYear(this.StartSeason, this.StartDay)
                && Helpers.IsValidDayOfYear(this.EndSeason, this.EndDay);
        }

        public WorldDate FirstDay {
            get {
                try {
                    return new(this.StartYear ?? Game1.year, this.StartSeason, this.StartDay ?? 0);
                } catch {
                    return null;
                }
            }
        }

        public WorldDate LastDay {
            get {
                try {
                    var ld = new WorldDate(this.EndYear ?? Game1.year, this.EndSeason ?? this.StartSeason, this.EndDay ?? 0);
                    if (this.FirstDay > ld) {
                        ld.Year++;
                    }
                    return ld;
                } catch {
                    return null;
                }
            }
        }
    }
}