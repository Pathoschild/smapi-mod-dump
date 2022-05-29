/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Events;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Api.Weighted;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Integrations.Emp;

namespace TehPers.FishingOverhaul.Services
{
    /// <summary>
    /// Default API for working with fishing.
    /// </summary>
    public sealed partial class FishingApi : BaseFishingApi
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly Func<IEnumerable<IFishingContentSource>> contentSourcesFactory;

        private readonly EntryManagerFactory<FishEntry, FishAvailabilityInfo>
            fishEntryManagerFactory;

        private readonly EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory;

        private readonly EntryManagerFactory<TreasureEntry, AvailabilityInfo>
            treasureEntryManagerFactory;

        private readonly FishingEffectManagerFactory fishingEffectManagerFactory;

        private readonly Lazy<IOptional<IEmpApi>> empApi;

        internal readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        internal readonly List<EntryManager<FishEntry, FishAvailabilityInfo>> fishManagers;
        internal readonly List<EntryManager<TrashEntry, AvailabilityInfo>> trashManagers;
        internal readonly List<EntryManager<TreasureEntry, AvailabilityInfo>> treasureManagers;
        internal readonly List<FishingEffectManager> fishingEffectManagers;
        private readonly string stateKey;

        private bool reloadRequested;

        internal FishingApi(
            IModHelper helper,
            IMonitor monitor,
            IManifest manifest,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            Func<IEnumerable<IFishingContentSource>> contentSourcesFactory,
            EntryManagerFactory<FishEntry, FishAvailabilityInfo> fishEntryManagerFactory,
            EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory,
            EntryManagerFactory<TreasureEntry, AvailabilityInfo> treasureEntryManagerFactory,
            FishingEffectManagerFactory fishingEffectManagerFactory,
            Lazy<IOptional<IEmpApi>> empApi
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.contentSourcesFactory = contentSourcesFactory
                ?? throw new ArgumentNullException(nameof(contentSourcesFactory));
            this.fishEntryManagerFactory = fishEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(fishEntryManagerFactory));
            this.trashEntryManagerFactory = trashEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(trashEntryManagerFactory));
            this.treasureEntryManagerFactory = treasureEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(treasureEntryManagerFactory));
            this.fishingEffectManagerFactory = fishingEffectManagerFactory
                ?? throw new ArgumentNullException(nameof(fishingEffectManagerFactory));
            this.empApi = empApi ?? throw new ArgumentNullException(nameof(empApi));

            this.fishTraits = new();
            this.fishManagers = new();
            this.trashManagers = new();
            this.treasureManagers = new();
            this.fishingEffectManagers = new();
            this.stateKey = $"{manifest.UniqueID}/fishing-state";

            this.CreatedDefaultFishingInfo += this.ApplyMapOverrides;
            this.CreatedDefaultFishingInfo += this.ApplyEmpOverrides;
            this.CreatedDefaultFishingInfo += FishingApi.ApplyMagicBait;
            this.PreparedFishChances += FishingApi.ApplyCuriosityLure;

            this.reloadRequested = true;
        }

        private void ApplyMapOverrides(object? sender, CreatedDefaultFishingInfoEventArgs e)
        {
            if (e.FishingInfo.User.currentLocation is Farm farm
                && GetFarmLocationOverride(farm) is var (overrideLocation, overrideChance)
                && Game1.random.NextDouble() < overrideChance)
            {
                e.FishingInfo = e.FishingInfo with
                {
                    Locations = FishingInfo.GetDefaultLocationNames(
                            Game1.getLocationFromName(overrideLocation)
                        )
                        .ToImmutableArray(),
                };
            }

            (string, float)? GetFarmLocationOverride(Farm farm)
            {
                var overrideLocationField =
                    this.helper.Reflection.GetField<string?>(farm, "_fishLocationOverride");
                var overrideChanceField =
                    this.helper.Reflection.GetField<float>(farm, "_fishChanceOverride");

                // Set override
                float overrideChance;
                if (overrideLocationField.GetValue() is not { } overrideLocation)
                {
                    // Read from the map properties
                    var mapProperty = farm.getMapProperty("FarmFishLocationOverride");
                    if (mapProperty == string.Empty)
                    {
                        overrideLocation = string.Empty;
                        overrideChance = 0.0f;
                    }
                    else
                    {
                        var splitProperty = mapProperty.Split(' ');
                        if (splitProperty.Length >= 2
                            && float.TryParse(splitProperty[1], out overrideChance))
                        {
                            overrideLocation = splitProperty[0];
                        }
                        else
                        {
                            overrideLocation = string.Empty;
                            overrideChance = 0.0f;
                        }
                    }

                    // Set the fields
                    overrideLocationField.SetValue(overrideLocation);
                    overrideChanceField.SetValue(overrideChance);
                }
                else
                {
                    overrideChance = overrideChanceField.GetValue();
                }

                if (overrideChance > 0.0)
                {
                    // Overridden
                    return (overrideLocation, overrideChance);
                }

                // No override
                return null;
            }
        }

        private void ApplyEmpOverrides(object? sender, CreatedDefaultFishingInfoEventArgs e)
        {
            if (!this.empApi.Value.TryGetValue(out var empApi))
            {
                return;
            }

            // Get EMP info
            empApi.GetFishLocationsData(
                e.FishingInfo.User.currentLocation,
                e.FishingInfo.BobberPosition,
                out var empLocationName,
                out var empZone,
                out _
            );

            // Override data
            e.FishingInfo = e.FishingInfo with
            {
                Locations = empLocationName switch
                {
                    null => e.FishingInfo.Locations,
                    _ when Game1.getLocationFromName(empLocationName) is { } empLocation =>
                        FishingInfo.GetDefaultLocationNames(empLocation).ToImmutableArray(),
                    _ => ImmutableArray.Create(empLocationName),
                },
                WaterTypes = empZone switch
                {
                    null => e.FishingInfo.WaterTypes,
                    -1 => WaterTypes.All,
                    0 => WaterTypes.River,
                    1 => WaterTypes.PondOrOcean,
                    2 => WaterTypes.Freshwater,
                    _ => WaterTypes.All,
                },
            };
        }

        private static void ApplyMagicBait(object? sender, CreatedDefaultFishingInfoEventArgs e)
        {
            // Check if magic bait is equipped
            if (e.FishingInfo.Bait != NamespacedKey.SdvObject(908))
            {
                return;
            }

            // Update fishing info to allow catches from all seasons, weathers, and times
            e.FishingInfo = e.FishingInfo with
            {
                Seasons = Core.Api.Gameplay.Seasons.All,
                Weathers = Core.Api.Gameplay.Weathers.All,
                Times = Enumerable.Range(600, 2600).ToImmutableArray(),
            };
        }

        private static void ApplyCuriosityLure(object? sender, PreparedFishEventArgs e)
        {
            // Check if curiosity lure is equipped
            if (e.FishingInfo.Bobber != NamespacedKey.SdvObject(856))
            {
                return;
            }

            e.FishChances = e.FishChances.ToWeighted(
                    weightedValue =>
                        weightedValue.Weight >= 0 ? Math.Log(weightedValue.Weight + 1) : 0,
                    weightedValue => weightedValue.Value
                )
                .ToList();
        }

        /// <inheritdoc/>
        public override FishingInfo CreateDefaultFishingInfo(Farmer farmer)
        {
            var fishingInfo = new FishingInfo(farmer);
            var eventArgs = new CreatedDefaultFishingInfoEventArgs(fishingInfo);
            this.OnCreatedDefaultFishingInfo(eventArgs);

            return eventArgs.FishingInfo;
        }

        private static IEnumerable<IWeightedValue<TEntry>>
            GetWeightedEntries<TEntry, TAvailability>(
                IEnumerable<EntryManager<TEntry, TAvailability>> managers,
                FishingInfo fishingInfo
            )
            where TEntry : Entry<TAvailability>
            where TAvailability : AvailabilityInfo
        {
            var chances = managers.SelectMany(
                manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                    .AsEnumerable()
                    .ToWeighted(weight => weight, _ => manager.Entry)
            );
            var highestTier = chances.GroupBy(entry => entry.Value.AvailabilityInfo.PriorityTier)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            return highestTier ?? Enumerable.Empty<IWeightedValue<TEntry>>();
        }

        /// <inheritdoc/>
        public override IEnumerable<IWeightedValue<FishEntry>> GetFishChances(
            FishingInfo fishingInfo
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get fish chances
            var chances = FishingApi.GetWeightedEntries(this.fishManagers, fishingInfo);

            // Invoke prepared chances event (some baits/bobbers may have effects applied here)
            var preparedChancesArgs = new PreparedFishEventArgs(fishingInfo, chances.ToList());
            this.OnPreparedFishChances(preparedChancesArgs);

            return preparedChancesArgs.FishChances;
        }

        /// <inheritdoc/>
        public override bool TryGetFishTraits(
            NamespacedKey fishKey,
            [NotNullWhen(true)] out FishTraits? traits
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            if (!this.fishTraits.TryGetValue(fishKey, out traits))
            {
                return false;
            }

            var dartFrequency =
                (int)(this.fishConfig.GlobalDartFrequencyFactor * traits.DartFrequency);
            traits = traits with {DartFrequency = dartFrequency};
            return true;
        }

        /// <inheritdoc/>
        public override IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(
            FishingInfo fishingInfo
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get trash chances
            var chances = FishingApi.GetWeightedEntries(this.trashManagers, fishingInfo);

            // Invoke prepared chances event (some baits/bobbers may have effects applied here)
            var preparedChancesArgs = new PreparedTrashEventArgs(fishingInfo, chances.ToList());
            this.OnPreparedTrashChances(preparedChancesArgs);

            return preparedChancesArgs.TrashChances;
        }

        /// <inheritdoc/>
        public override IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(
            FishingInfo fishingInfo
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get treasure chances
            var chances = FishingApi.GetWeightedEntries(this.treasureManagers, fishingInfo);

            // Invoke prepared chances event (some baits/bobbers may have effects applied here)
            var preparedChancesArgs = new PreparedTreasureEventArgs(fishingInfo, chances.ToList());
            this.OnPreparedTreasureChances(preparedChancesArgs);

            return preparedChancesArgs.TreasureChances;
        }

        /// <inheritdoc/>
        public override double GetChanceForFish(FishingInfo fishingInfo)
        {
            // Get chance for fish
            var streak = this.GetStreak(fishingInfo.User);
            var chanceForFish =
                this.fishConfig.FishChances.GetUnclampedChance(fishingInfo.User, streak);

            // Invoke event (in case some mod wants to change the chance)
            var eventArgs = new CalculatedFishChanceEventArgs(fishingInfo, chanceForFish);
            this.OnCalculatedFishChance(eventArgs);

            return this.ClampFishChance(fishingInfo, eventArgs.Chance);
        }

        /// <inheritdoc/>
        public override double GetChanceForTreasure(FishingInfo fishingInfo)
        {
            // Get chance for treasure
            var streak = this.GetStreak(fishingInfo.User);
            var chanceForTreasure =
                this.treasureConfig.TreasureChances.GetUnclampedChance(fishingInfo.User, streak);

            // Invoke event (in case some mod wants to change the chance)
            var eventArgs = new CalculatedTreasureChanceEventArgs(fishingInfo, chanceForTreasure);
            this.OnCalculatedTreasureChance(eventArgs);

            return this.ClampTreasureChance(fishingInfo, eventArgs.Chance);
        }

        /// <summary>
        /// Clamps the chance of finding a fish.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chance">The unclamped chance.</param>
        /// <returns>The minimum and maximum chance of catching a fish.</returns>
        private double ClampFishChance(FishingInfo fishingInfo, double chance)
        {
            var minArgs = new CalculatedFishChanceEventArgs(
                fishingInfo,
                this.fishConfig.FishChances.MinChance
            );
            this.OnCalculatedMinFishChance(minArgs);

            var maxArgs = new CalculatedFishChanceEventArgs(
                fishingInfo,
                this.fishConfig.FishChances.MaxChance
            );
            this.OnCalculatedMaxFishChance(maxArgs);

            return minArgs.Chance > maxArgs.Chance
                ? maxArgs.Chance
                : Math.Clamp(chance, minArgs.Chance, maxArgs.Chance);
        }

        /// <summary>
        /// Clamps the chance of finding treasure while fishing.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chance">The unclamped chance.</param>
        /// <returns>The minimum and maximum chance of finding a treasure chest.</returns>
        private double ClampTreasureChance(FishingInfo fishingInfo, double chance)
        {
            var minArgs = new CalculatedTreasureChanceEventArgs(
                fishingInfo,
                this.treasureConfig.TreasureChances.MinChance
            );
            this.OnCalculatedMinTreasureChance(minArgs);

            var maxArgs = new CalculatedTreasureChanceEventArgs(
                fishingInfo,
                this.treasureConfig.TreasureChances.MaxChance
            );
            this.OnCalculatedMaxTreasureChance(maxArgs);

            return minArgs.Chance > maxArgs.Chance
                ? maxArgs.Chance
                : Math.Clamp(chance, minArgs.Chance, maxArgs.Chance);
        }

        /// <inheritdoc/>
        public override bool IsLegendary(NamespacedKey fishKey)
        {
            return this.TryGetFishTraits(fishKey, out var traits) && traits.IsLegendary;
        }

        /// <inheritdoc/>
        public override int GetStreak(Farmer farmer)
        {
            var key = $"{this.stateKey}/streak";
            return farmer.modData.TryGetValue(key, out var rawData)
                && int.TryParse(rawData, out var streak)
                    ? streak
                    : 0;
        }

        /// <inheritdoc/>
        public override void SetStreak(Farmer farmer, int streak)
        {
            var key = $"{this.stateKey}/streak";
            farmer.modData[key] = streak.ToString();
        }

        /// <inheritdoc/>
        public override PossibleCatch GetPossibleCatch(FishingInfo fishingInfo)
        {
            // Choose a random fish if one hasn't been chosen yet
            var fishChance = this.GetChanceForFish(fishingInfo);
            var possibleFish =
                (IEnumerable<IWeightedValue<FishEntry?>>)this.GetFishChances(fishingInfo)
                    .Normalize(fishChance);
            var fishEntry = possibleFish.Append(new WeightedValue<FishEntry?>(null, 1 - fishChance))
                .ChooseOrDefault(Game1.random)
                ?.Value;

            // Return if a fish was chosen
            if (fishEntry is not null)
            {
                return new PossibleCatch.Fish(fishEntry);
            }

            // Trash
            var trashEntry = this.GetTrashChances(fishingInfo).ChooseOrDefault(Game1.random)?.Value;
            if (trashEntry is not null)
            {
                return new PossibleCatch.Trash(trashEntry);
            }

            // Default trash item
            this.monitor.Log("No valid trash, selecting a default item.", LogLevel.Warn);
            var defaultTrashKey = NamespacedKey.SdvObject(168);
            return new PossibleCatch.Trash(new(defaultTrashKey, new(0.0)));
        }

        /// <inheritdoc/>
        public override IEnumerable<TreasureEntry> GetPossibleTreasure(
            CatchInfo.FishCatch catchInfo
        )
        {
            // Get possible loot
            var possibleLoot = this.GetTreasureChances(catchInfo.FishingInfo).ToList();

            // Perfect catch + treasure inverts the chances
            if (this.treasureConfig.InvertChancesOnPerfectCatch && catchInfo.State.IsPerfect)
            {
                possibleLoot = possibleLoot.Normalize()
                    .ToWeighted(item => 1.0 - item.Weight, item => item.Value)
                    .ToList();
            }

            // Select rewards
            var streak = this.GetStreak(catchInfo.FishingInfo.User);
            var chance = 1d;
            var rewards = 0;
            var additionalLootChance = this.treasureConfig.AdditionalLootChances.GetUnclampedChance(
                catchInfo.FishingInfo.User,
                streak
            );
            additionalLootChance =
                this.treasureConfig.AdditionalLootChances.ClampChance(additionalLootChance);
            while (possibleLoot.Any()
                   && rewards < this.treasureConfig.MaxTreasureQuantity
                   && Game1.random.NextDouble() <= chance)
            {
                // Choose a reward
                var treasure = possibleLoot.Choose(Game1.random);

                // Yield it
                rewards += 1;
                yield return treasure.Value;

                // Check if this reward shouldn't be duplicated
                if (!this.treasureConfig.AllowDuplicateLoot || !treasure.Value.AllowDuplicates)
                {
                    possibleLoot.Remove(treasure);
                }

                // Update chance
                chance *= additionalLootChance;
            }
        }

        internal new void OnCaughtItem(CaughtItemEventArgs e)
        {
            base.OnCaughtItem(e);
        }

        internal new void OnOpeningChest(OpeningChestEventArgs e)
        {
            base.OnOpeningChest(e);
        }
    }
}
