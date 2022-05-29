/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using TehPers.Core.Api.Gameplay;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Conditions for availability.
    /// </summary>
    public record AvailabilityConditions
    {
        /// <summary>
        /// Obsoletion warnings from initializing this <see cref="AvailabilityInfo"/>.
        /// </summary>
        [JsonIgnore]
        internal ImmutableList<string> ObsoletionWarnings { get; init; } =
            ImmutableList<string>.Empty;

        /// <summary>
        /// Time this becomes available (inclusive).
        /// </summary>
        [DefaultValue(600)]
        public int StartTime { get; init; } = 600;

        /// <summary>
        /// Time this is no longer available (exclusive).
        /// </summary>
        [DefaultValue(2600)]
        public int EndTime { get; init; } = 2600;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Seasons"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityConditions.Seasons))]
        [Description("Seasons this can be caught in.")]
        public IEnumerable<Seasons> SeasonsSplit
        {
            get => this.Seasons switch
            {
                Seasons.All => new[] {Seasons.All},
                _ => new[] {Seasons.Spring, Seasons.Summer, Seasons.Fall, Seasons.Winter}.Where(
                    s => this.Seasons.HasFlag(s)
                ),
            };
            init => this.Seasons = value.Aggregate(Seasons.None, (acc, cur) => acc | cur);
        }

        /// <summary>
        /// Seasons this can be caught in.
        /// </summary>
        [JsonIgnore]
        public Seasons Seasons { get; init; } = Seasons.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Weathers"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityConditions.Weathers))]
        [Description("Weathers this can be caught in.")]
        public IEnumerable<Weathers> WeathersSplit
        {
            get => this.Weathers switch
            {
                Weathers.All => new[] {Weathers.All},
                _ => new[] {Weathers.Sunny, Weathers.Rainy}.Where(w => this.Weathers.HasFlag(w)),
            };
            init => this.Weathers = value.Aggregate(Weathers.None, (acc, cur) => acc | cur);
        }

        /// <summary>
        /// Weathers this can be caught in.
        /// </summary>
        [JsonIgnore]
        public Weathers Weathers { get; init; } = Weathers.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="WaterTypes"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityConditions.WaterTypes))]
        [Description(
            "The type of water this can be caught in. Each location handles this differently."
        )]
        public IEnumerable<WaterTypes> WaterTypesSplit
        {
            get => this.WaterTypes switch
            {
                WaterTypes.All => new[] {WaterTypes.All},
                _ => new[] {WaterTypes.River, WaterTypes.PondOrOcean, WaterTypes.Freshwater}.Where(
                    w => this.WaterTypes.HasFlag(w)
                ),
            };
            init => this.WaterTypes = value.Aggregate(WaterTypes.None, (acc, cur) => acc | cur);
        }

        /// <summary>
        /// The type of water this can be caught in. Each location handles this differently.
        /// </summary>
        [JsonIgnore]
        public WaterTypes WaterTypes { get; init; } = WaterTypes.All;

        /// <summary>
        /// Required fishing level to catch this.
        /// </summary>
        [DefaultValue(0)]
        public int MinFishingLevel { get; init; } = 0;

        /// <summary>
        /// Maximum fishing level required to catch this, or null for no max.
        /// </summary>
        [DefaultValue(null)]
        public int? MaxFishingLevel { get; init; } = null;

        /// <summary>
        /// List of locations this should be available in. Leaving this empty will make this
        /// available everywhere. Some locations have special handling. For example, the mines use
        /// the location names "UndergroundMine" and "UndergroundMine/N", where N is the floor
        /// number (both location names are valid for the floor).
        /// </summary>
        public ImmutableArray<string> IncludeLocations { get; init; } =
            ImmutableArray<string>.Empty;

        /// <summary>
        /// List of locations this should not be available in. This takes priority over
        /// <see cref="IncludeLocations"/>.
        /// </summary>
        public ImmutableArray<string> ExcludeLocations { get; init; } =
            ImmutableArray<string>.Empty;

        /// <summary>
        /// Constraints on the bobber's position on the map when fishing.
        /// </summary>
        public PositionConstraint Position { get; init; } = new();

        /// <summary>
        /// Constraints on the farmer's position on the map when fishing.
        /// </summary>
        public PositionConstraint FarmerPosition { get; init; } = new();

        /// <summary>
        /// Minimum bobber depth required to catch this.
        /// </summary>
        [Obsolete("Use " + nameof(AvailabilityConditions.MinBobberDepth) + " instead")]
        [DefaultValue(0)]
        public int MinDepth
        {
            get => this.MinBobberDepth;
            init
            {
                this.MinBobberDepth = value;
                this.ObsoletionWarnings = this.ObsoletionWarnings.Add(
                    $"{nameof(AvailabilityConditions.MinDepth)} is obsolete. Use {nameof(AvailabilityConditions.MinBobberDepth)} instead."
                );
            }
        }

        /// <summary>
        /// Minimum bobber depth required to catch this.
        /// </summary>
        [DefaultValue(0)]
        public int MinBobberDepth { get; init; } = 0;

        /// <summary>
        /// Maximum bobber depth required to catch this.
        /// </summary>
        [Obsolete("Use " + nameof(AvailabilityConditions.MaxBobberDepth) + " instead")]
        [DefaultValue(null)]
        public int? MaxDepth
        {
            get => this.MaxBobberDepth;
            init
            {
                this.MaxBobberDepth = value;
                this.ObsoletionWarnings = this.ObsoletionWarnings.Add(
                    $"{nameof(AvailabilityConditions.MaxDepth)} is obsolete. Use {nameof(AvailabilityConditions.MaxBobberDepth)} instead."
                );
            }
        }

        /// <summary>
        /// Maximum bobber depth required to catch this.
        /// </summary>
        [DefaultValue(null)]
        public int? MaxBobberDepth { get; init; } = null;

        /// <summary>
        /// Content Patcher conditions for when this is available.
        /// </summary>
        public ImmutableDictionary<string, string?> When { get; init; } =
            ImmutableDictionary<string, string?>.Empty;

        /// <summary>
        /// Check whether these conditions are met.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <returns>Whether these conditions are met.</returns>
        public virtual bool IsAvailable(FishingInfo fishingInfo)
        {
            // Verify at least one time is valid
            if (fishingInfo.Times.All(t => t < this.StartTime || t >= this.EndTime))
            {
                return false;
            }

            // Verify season is valid
            if ((this.Seasons & fishingInfo.Seasons) == Seasons.None)
            {
                return false;
            }

            // Verify weather is valid
            if ((this.Weathers & fishingInfo.Weathers) == Weathers.None)
            {
                return false;
            }

            // Verify water type is valid
            if ((this.WaterTypes & fishingInfo.WaterTypes) == WaterTypes.None)
            {
                return false;
            }

            // Verify fishing level is valid
            if (fishingInfo.FishingLevel < this.MinFishingLevel
                || this.MaxFishingLevel is { } maxLevel && fishingInfo.FishingLevel > maxLevel)
            {
                return false;
            }

            // Verify location is valid
            var validLocation = fishingInfo.Locations.Aggregate(
                this.IncludeLocations.Any()
                    ? LocationSearchState.VacuouslyExcluded
                    : LocationSearchState.Included,
                (state, cur) => state switch
                {
                    LocationSearchState.Excluded => LocationSearchState.Excluded,
                    _ when this.ExcludeLocations.Contains(cur) => LocationSearchState.Excluded,
                    LocationSearchState.Included => LocationSearchState.Included,
                    _ when this.IncludeLocations.Contains(cur) => LocationSearchState.Included,
                    _ => state
                }
            );
            if (validLocation is not LocationSearchState.Included)
            {
                return false;
            }

            // Verify position is valid
            if (!this.Position.Matches(fishingInfo.BobberPosition))
            {
                return false;
            }

            // Verify farmer's position is valid
            if (!this.FarmerPosition.Matches(fishingInfo.User.getStandingPosition() / 64f))
            {
                return false;
            }

            // Verify depth is valid
            if (fishingInfo.BobberDepth < this.MinBobberDepth)
            {
                return false;
            }

            if (this.MaxBobberDepth is { } maxDepth && fishingInfo.BobberDepth > maxDepth)
            {
                return false;
            }

            return true;
        }

        private enum LocationSearchState
        {
            VacuouslyExcluded,
            Included,
            Excluded,
        }
    }
}
