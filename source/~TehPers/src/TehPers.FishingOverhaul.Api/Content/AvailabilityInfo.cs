/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Information about the availability of a catchable item.
    /// </summary>
    /// <param name="BaseChance">
    /// The base chance this will be caught. This is not a percentage chance, but rather a weight
    /// relative to all available entries.
    /// </param>
    [JsonDescribe]
    public record AvailabilityInfo([property: JsonRequired] double BaseChance)
    {
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
        [JsonProperty(nameof(AvailabilityInfo.Seasons))]
        [Description("Seasons this can be caught in.")]
        public IEnumerable<Seasons> SeasonsSplit
        {
            get => this.Seasons switch
            {
                Seasons.All => new[] { Seasons.All },
                _ => new[] { Seasons.Spring, Seasons.Summer, Seasons.Fall, Seasons.Winter }.Where(
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
        [JsonProperty(nameof(AvailabilityInfo.Weathers))]
        [Description("Weathers this can be caught in.")]
        public IEnumerable<Weathers> WeathersSplit
        {
            get => this.Weathers switch
            {
                Weathers.All => new[] { Weathers.All },
                _ => new[] { Weathers.Sunny, Weathers.Rainy }.Where(w => this.Weathers.HasFlag(w)),
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
        [JsonProperty(nameof(AvailabilityInfo.WaterTypes))]
        [Description(
            "The type of water this can be caught in. Each location handles this differently."
        )]
        public IEnumerable<WaterTypes> WaterTypesSplit
        {
            get => this.WaterTypes switch
            {
                WaterTypes.All => new[] { WaterTypes.All },
                _ => new[] { WaterTypes.River, WaterTypes.PondOrOcean, WaterTypes.Freshwater }
                    .Where(w => this.WaterTypes.HasFlag(w)),
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
        [DefaultValue(0)]
        public int MinDepth { get; init; } = 0;

        /// <summary>
        /// Maximum bobber depth required to catch this.
        /// </summary>
        [DefaultValue(null)]
        public int? MaxDepth { get; init; } = null;

        /// <summary>
        /// Content Patcher conditions for when this is available.
        /// </summary>
        public ImmutableDictionary<string, string> When { get; init; } =
            ImmutableDictionary<string, string>.Empty;

        /// <summary>
        /// Gets the weighted chance of this being caught, if any. This does not test the
        /// conditions in <see cref="When"/>.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <returns>The weighted chance of this being caught, or <see langword="null"/> if not available.</returns>
        public virtual double? GetWeightedChance(FishingInfo fishingInfo)
        {
            // Verify at least one time is valid
            if (fishingInfo.Times.All(t => t < this.StartTime || t >= this.EndTime))
            {
                return null;
            }

            // Verify season is valid
            if ((this.Seasons & fishingInfo.Seasons) == Seasons.None)
            {
                return null;
            }

            // Verify weather is valid
            if ((this.Weathers & fishingInfo.Weathers) == Weathers.None)
            {
                return null;
            }

            // Verify water type is valid
            if ((this.WaterTypes & fishingInfo.WaterTypes) == WaterTypes.None)
            {
                return null;
            }

            // Verify fishing level is valid
            if (fishingInfo.FishingLevel < this.MinFishingLevel
                || this.MaxFishingLevel is { } maxLevel && fishingInfo.FishingLevel > maxLevel)
            {
                return null;
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
                return null;
            }

            // Verify position is valid
            if (!this.Position.Matches(fishingInfo.BobberPosition))
            {
                return null;
            }

            // Verify farmer's position is valid
            if (!this.FarmerPosition.Matches(fishingInfo.User.getStandingPosition() / 64f))
            {
                return null;
            }

            // Verify depth is valid
            if (fishingInfo.BobberDepth < this.MinDepth)
            {
                return null;
            }

            if (this.MaxDepth is { } maxDepth && fishingInfo.BobberDepth > maxDepth)
            {
                return null;
            }

            // Calculate spawn weight
            return this.BaseChance;
        }

        private enum LocationSearchState
        {
            VacuouslyExcluded,
            Included,
            Excluded,
        }
    }
}
