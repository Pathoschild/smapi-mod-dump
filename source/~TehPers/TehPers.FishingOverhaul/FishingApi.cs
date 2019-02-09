using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Helpers;
using TehPers.Core.Helpers.Static;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Overridables;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class FishingApi : IFishingApi {
        private static readonly Regex _mineLocRegex = new Regex(@"^UndergroundMine\d+$", RegexOptions.IgnoreCase);

        private float? _fishChance;
        private float? _treasureChance;
        private float? _unawareChance;
        private bool? _farmFishing;
        private bool? _fishableFarmFishing;
        private readonly Dictionary<string, Dictionary<int, IFishData>> _fishDataOverrides = new Dictionary<string, Dictionary<int, IFishData>>();
        private readonly Dictionary<int, IFishTraits> _fishTraitsOverrides = new Dictionary<int, IFishTraits>();
        private readonly HashSet<ITreasureData> _added = new HashSet<ITreasureData>();
        private readonly HashSet<TreasureData> _removed = new HashSet<TreasureData>();
        private readonly HashSet<ITrashData> _trash = new HashSet<ITrashData>();
        private readonly Dictionary<int, string> _fishNames = new Dictionary<int, string>();
        private readonly Dictionary<Farmer, int> _streaks = new Dictionary<Farmer, int>();
        private readonly HashSet<int> _hidden = new HashSet<int>();
        private readonly int[] _defaultLegendary = { 159, 160, 163, 682, 775 };
        private readonly Dictionary<int, bool> _legendaryOverrides = new Dictionary<int, bool>();
        private bool _obsoleteWarning;

        internal FishingApi() {
            // Get raw content data for fish
            Dictionary<int, string> fishContent = ModFishing.Instance.Helper.Content.Load<Dictionary<int, string>>(@"Data\Fish.xnb", ContentSource.GameContent);

            // Store all the names
            foreach (KeyValuePair<int, string> fishData in fishContent) {
                string[] contentData = fishContent[fishData.Key].Split('/');
                this._fishNames[fishData.Key] = contentData.Length > 13 ? contentData[13] : contentData[0];
            }
        }
        
        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> BeforeFishCatching;

        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> FishCaught;

        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> TrashCaught;

        /// <inheritdoc />
        public void SetFishChance(float? chance) {
            this._fishChance = chance;
        }

        /// <inheritdoc />
        public float GetFishChance(Farmer who) {
            return (this._fishChance ?? FishHelper.GetRawFishChance(who))
                .Clamp(ModFishing.Instance.MainConfig.GlobalFishSettings.MinFishChance, ModFishing.Instance.MainConfig.GlobalFishSettings.MaxFishChance)
                .Clamp(0F, 1F); // Prevent invalid chances
        }

        /// <inheritdoc />
        public void SetTreasureChance(float? chance) {
            this._treasureChance = chance;
        }

        /// <inheritdoc />
        public float GetTreasureChance(Farmer who, FishingRod rod) {
            return this._treasureChance ?? FishHelper.GetRawTreasureChance(who, rod);
        }

        /// <inheritdoc />
        public void SetUnawareChance(float? chance) {
            this._unawareChance = chance;
        }

        /// <inheritdoc />
        public float GetUnawareChance(Farmer who, int fish) {
            if (this.IsLegendary(fish))
                return 0F;

            return this._unawareChance ?? FishHelper.GetRawUnawareChance(who);
        }

        /// <inheritdoc />
        public void SetFarmFishing(bool? allowFish) {
            this._farmFishing = allowFish;
        }

        /// <inheritdoc />
        public bool GetFarmFishing() {
            return this._farmFishing ?? ModFishing.Instance.MainConfig.GlobalFishSettings.AllowFishOnAllFarms;
        }

        /// <inheritdoc />
        public void SetFishableFarmFishing(bool? allowFish) {
            this._fishableFarmFishing = allowFish;
        }

        /// <inheritdoc />
        public bool GetFishableFarmFishing() {
            return this._fishableFarmFishing ?? true;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, IFishData> GetFishData(string location) {
            Dictionary<int, IFishData> result = new Dictionary<int, IFishData>();

            // Get mod location data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out Dictionary<int, FishData> modLocData)) {
                foreach (KeyValuePair<int, FishData> modData in modLocData) {
                    // Mod data
                    result[modData.Key] = modData.Value;
                }
            }

            // Get overridden location data
            if (this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData)) {
                foreach (KeyValuePair<int, IFishData> overrideData in overrideLocData) {
                    if (overrideData.Value == null) {
                        // Removed data
                        result.Remove(overrideData.Key);
                    } else {
                        // Added data
                        result[overrideData.Key] = overrideData.Value;
                    }
                }
            }

            // Format it as a dictionary
            return result.ToDictionary();
        }

        /// <inheritdoc />
        public IFishData GetFishData(string location, int fish) {
            // Overriden fish data
            if (this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData) && overrideLocData.TryGetValue(fish, out IFishData overrideData))
                return overrideData;

            // Mod fish data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out Dictionary<int, FishData> modLocData) && modLocData.TryGetValue(fish, out FishData modData))
                return modData;

            // None found
            return null;
        }

        /// <inheritdoc />
        public void SetFishData(string location, int fish, IFishData data) {
            // Get location data dictionary
            if (!this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData)) {
                overrideLocData = new Dictionary<int, IFishData>();
                this._fishDataOverrides.Add(location, overrideLocData);
            }

            // Set the fish data for that location
            overrideLocData[fish] = data;
        }

        /// <inheritdoc />
        public void ResetFishData() {
            this._fishDataOverrides.Clear();
        }

        /// <inheritdoc />
        public bool ResetFishData(string location) {
            return this._fishDataOverrides.Remove(location);
        }

        /// <inheritdoc />
        public bool ResetFishData(string location, int fish) {
            return this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData) && overrideLocData.Remove(fish);
        }

        /// <inheritdoc />
        public void SetFishTraits(int fish, IFishTraits traits) {
            if (traits == null) {
                this._fishTraitsOverrides.Remove(fish);
            } else {
                this._fishTraitsOverrides[fish] = traits;
            }
        }

        /// <inheritdoc />
        public IFishTraits GetFishTraits(int fish) {
            return this._fishTraitsOverrides.TryGetValue(fish, out IFishTraits traits) ? traits : null;
        }

        /// <inheritdoc />
        public bool AddTreasureData(ITreasureData data) {
            return this._added.Add(data);
        }

        /// <inheritdoc />
        public bool RemoveTreasureData(ITreasureData data) {
            if (data is TreasureData td) {
                return this._removed.Add(td);
            } else {
                return this._added.Remove(data);
            }
        }

        /// <inheritdoc />
        public IEnumerable<ITreasureData> GetTreasureData() {
            return ModFishing.Instance.TreasureConfig.PossibleLoot.Except(this._removed).Concat(this._added);
        }

        /// <inheritdoc />
        public bool AddTrashData(ITrashData data) {
            return this._trash.Add(data);
        }

        /// <inheritdoc />
        public bool RemoveTrashData(ITrashData data) {
            return this._trash.Remove(data);
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData() {
            return this._trash;
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData(Farmer who) {
            WaterType w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            int mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return this.GetTrashData(who, who.currentLocation?.Name ?? "", w, SDate.Now(), Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return this._trash.Where(t => t.MeetsCriteria(who, locationName, waterType, date, weather, time, fishingLevel, mineLevel));
        }

        /// <inheritdoc />
        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who) {
            WaterType w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            int mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return this.GetPossibleFish(who, who.currentLocation?.Name ?? "", w, SDate.Now(), Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        /// <inheritdoc />
        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who, string locationName, WaterType water, SDate date, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            if (!string.Equals(locationName, "Farm", StringComparison.OrdinalIgnoreCase) || !this.GetFishableFarmFishing())
                return this.GetPossibleFishWithoutFarm(who, locationName, water, date, weather, time, fishLevel, mineLevel);

            // Custom handling for farm maps
            switch (Game1.whichFarm) {
                case 0: {
                        // Standard: default farm fish
                        break;
                    }
                case 1: {
                        // Riverland: forest fish + town fish
                        IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.3);
                        IEnumerable<IWeightedElement<int?>> townFish = this.GetPossibleFish(who, "Town", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.7);
                        return forestFish.Concat(townFish);
                    }
                case 2: {
                        // Forest: forest fish + woodskip
                        float scale = 0.05F + (float) Game1.dailyLuck;
                        IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(1 - scale);
                        IWeightedElement<int?>[] woodSkip = { new WeightedElement<int?>(734, scale) };
                        return forestFish.Concat(woodSkip);
                    }
                case 3: {
                        // Hills: forest fish + default farm fish
                        IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel);
                        IEnumerable<IWeightedElement<int?>> farmFish = this.GetPossibleFishWithoutFarm(who, locationName, water, date, weather, time, fishLevel, mineLevel);
                        return forestFish.Concat(farmFish);
                    }
                case 4: {
                        // Wilderness: mountain fish + default farm fish
                        IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Mountain", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.35);
                        IEnumerable<IWeightedElement<int?>> farmFish = this.GetPossibleFishWithoutFarm(who, locationName, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                        return forestFish.Concat(farmFish);
                    }
                default:
                    return this.GetPossibleFishWithoutFarm(who, locationName, water, date, weather, time, fishLevel, mineLevel);
            }

            return this.GetPossibleFishWithoutFarm(who, locationName, water, date, weather, time, fishLevel, mineLevel);
        }

        private IEnumerable<IWeightedElement<int?>> GetPossibleFishWithoutFarm(Farmer who, string locationName, WaterType water, SDate date, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            // Check if this is the farm
            if (locationName == "Farm" && !this.GetFarmFishing())
                return new[] { new WeightedElement<int?>(null, 1) };

            // Get chance for fish
            float fishChance = this.GetFishChance(who);

            // Get fish data for location
            IEnumerable<KeyValuePair<int, IFishData>> locFish = this.GetFishData(locationName);
            if (FishingApi._mineLocRegex.IsMatch(locationName)) {
                locFish = locFish.Concat(this.GetFishData("UndergroundMine"));
            }

            // Filter all the fish that can be caught at that location
            IEnumerable<IWeightedElement<int?>> fish = locFish.Where(f => {
                // Legendary fish criteria
                if (this.IsLegendary(f.Key)) {
                    // If custom legendaries is disabled, then don't include legendary fish. They are handled in CustomFishingRod
                    if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                        return false;
                    }

                    // If recatchable legendaries is disabled, then make sure this fish hasn't been caught yet
                    if (!ModFishing.Instance.MainConfig.RecatchableLegendaries && who.fishCaught.ContainsKey(f.Key)) {
                        return false;
                    }
                }

                // Normal criteria check
                return f.Value.MeetsCriteria(f.Key, water, date, weather, time, fishLevel, mineLevel);
            }).ToWeighted(kv => kv.Value.GetWeight(who), kv => (int?) kv.Key);

            // Include trash
            IEnumerable<IWeightedElement<int?>> trash = new WeightedElement<int?>(null, 1).Yield();

            // Combine fish with trash
            return fish.NormalizeTo(fishChance).Concat(trash.NormalizeTo(1 - fishChance));
        }

        /// <inheritdoc />
        public string GetFishName(int fish) {
            return this._fishNames.TryGetValue(fish, out string name) ? name : null;
        }

        /// <inheritdoc />
        public void SetFishName(int fish, string name) {
            this._fishNames[fish] = name ?? string.Empty;
        }

        /// <inheritdoc />
        public int GetStreak(Farmer who) {
            return this._streaks.TryGetValue(who, out int streak) ? streak : 0;
        }

        /// <inheritdoc />
        public void SetStreak(Farmer who, int streak) {
            this._streaks[who] = streak;
        }

        /// <inheritdoc />
        public void HideFish(int fish) {
            this._hidden.Add(fish);
        }

        /// <inheritdoc />
        public bool RevealFish(int fish) {
            return this._hidden.Remove(fish);
        }

        /// <inheritdoc />
        public bool IsHidden(int fish) {
            return this._hidden.Contains(fish);
        }

        /// <inheritdoc />
        public bool IsLegendary(int fish) {
            return this._legendaryOverrides.TryGetValue(fish, out bool legendary) ? legendary : this._defaultLegendary.Contains(fish);
        }

        /// <inheritdoc />
        public void SetLegendary(int fish, bool? legendary) {
            if (legendary.HasValue) {
                this._legendaryOverrides[fish] = legendary.Value;
            } else {
                this._legendaryOverrides.Remove(fish);
            }
        }

        private void WarnObsolete() {
            if (this._obsoleteWarning)
                return;

            ModFishing.Instance.Monitor.Log("A mod is using an obsolete trash command! The Nuget package cannot be updated right now, so go to https://github.com/TehPers/StardewValleyMods/tree/2.0-dev/FishingOverhaulApi for the latest version of the API.");
            this._obsoleteWarning = true;
        }

        internal void OnBeforeFishCatching(FishingEventArgs e) {
            this.BeforeFishCatching?.Invoke(this, e);
        }

        internal virtual void OnFishCaught(FishingEventArgs e) {
            this.FishCaught?.Invoke(this, e);
        }

        internal virtual void OnTrashCaught(FishingEventArgs e) {
            this.TrashCaught?.Invoke(this, e);
        }
    }
}
