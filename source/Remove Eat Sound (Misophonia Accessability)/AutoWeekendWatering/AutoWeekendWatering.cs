using AutoWeekendWatering.Configuration;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewObject = StardewValley.Object;

namespace AutoWeekendWatering
{
    public class AutoWeekendWatering : Mod
    {
        private readonly HashSet<HoeDirt> _tilledDirtPlots = new HashSet<HoeDirt>();
        private readonly HashSet<IndoorPot> _indoorPots = new HashSet<IndoorPot>();
        private bool _shouldWaterToday = true;
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            if (!_config.Enabled)
                return;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
        }

        /// <summary>
        /// Sets up the initial list of waterable locations for 
        /// </summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _shouldWaterToday = ShouldAutoWaterToday();

            var builtLocations = Game1.locations.OfType<BuildableGameLocation>()
                .SelectMany(location => location.buildings)
                .Select(building => building.indoors.Value)
                .Where(location => location != null);

            var allLocations = Game1.locations.Concat(builtLocations);

            foreach (GameLocation location in allLocations)
            {
                foreach (HoeDirt tilledDirt in location.terrainFeatures.Values.OfType<HoeDirt>())
                    SetupWaterableLocationListener(tilledDirt);

                foreach (IndoorPot indoorPot in location.objects.Values.OfType<IndoorPot>())
                    SetupWaterableLocationListener(indoorPot.hoeDirt.Value, indoorPot);
            }
        }

        /// <summary>
        /// Automatically waters all tilled dirt and garden pots
        /// </summary>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _shouldWaterToday = ShouldAutoWaterToday();

            if (!_shouldWaterToday)
                return;

            foreach (var tilledDirt in _tilledDirtPlots)
                Water(tilledDirt);

            foreach (var indoorPot in _indoorPots)
                Water(indoorPot.hoeDirt.Value, indoorPot);
        }

        /// <summary>
        /// Adds listeners for each plot of tillable dirt
        /// </summary>
        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            foreach (HoeDirt tilledDirt in e.Added.Select(kvp => kvp.Value).OfType<HoeDirt>())
            {
                SetupWaterableLocationListener(tilledDirt);
            }
        }

        /// <summary>
        /// Adds listeners for each garden pot
        /// </summary>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (IndoorPot indoorPot in e.Added.Select(kvp => kvp.Value).OfType<IndoorPot>())
            {
                SetupWaterableLocationListener(indoorPot.hoeDirt.Value, indoorPot);
            }
        }

        /// <summary>
        /// Sets up listeners on NetRef values for when a waterable item changes
        /// </summary>
        /// <param name="tilledDirt">Tilled dirt plot to listen for changes to</param>
        /// <param name="indoorPot">Garden pot to listen for changes to</param>
        private void SetupWaterableLocationListener(HoeDirt tilledDirt, IndoorPot indoorPot = null)
        {
            try
            {
                NetRef<Crop> netRefCrop = Helper.Reflection.GetField<NetRef<Crop>>(tilledDirt, "netCrop", true).GetValue();
                
                if (netRefCrop.Value != null)
                    TrackAndWater(tilledDirt, indoorPot);
                
                netRefCrop.fieldChangeVisibleEvent += (_, __, crop) =>
                {
                    if (crop != null)
                        TrackAndWater(tilledDirt, indoorPot);
                    else
                        RemoveTrack(tilledDirt, indoorPot);
                };
            }
            catch (Exception ex)
            {
                Monitor.Log($"Could not read crop data on dirt; possible new game version?\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Adds a waterable target to the corresponding collection, and then waters that target
        /// </summary>
        /// <param name="tilledDirt">Tilled dirt plot to track and water</param>
        /// <param name="indoorPot">Garden pot to track and water</param>
        private void TrackAndWater(HoeDirt tilledDirt, IndoorPot indoorPot = null)
        {
            if (indoorPot != null)
                _indoorPots.Add(indoorPot);
            else
                _tilledDirtPlots.Add(tilledDirt);

            Water(tilledDirt, indoorPot);
        }

        /// <summary>
        /// Removes a waterable target from the corresponding collection
        /// </summary>
        /// <param name="tilledDirt">Tilled dirt plot to untrack</param>
        /// <param name="indoorPot">Garden pot to untrack</param>
        private void RemoveTrack(HoeDirt tilledDirt, IndoorPot indoorPot = null)
        {
            if (indoorPot != null)
                _indoorPots.Remove(indoorPot);
            else
                _tilledDirtPlots.Remove(tilledDirt);
        }

        /// <summary>
        /// Waters either a plot of tilled dirt or a garden pot
        /// </summary>
        /// <param name="tilledDirt">Tilled dirt plot to water</param>
        /// <param name="indoorPot">Garden pot to water</param>
        private void Water(HoeDirt tilledDirt, IndoorPot indoorPot = null)
        {
            if (!_shouldWaterToday)
                return;

            tilledDirt.state.Value = HoeDirt.watered;

            if (_config.Fertilizer.HasValue)
                tilledDirt.fertilizer.Value = _config.Fertilizer.Value;
            if (indoorPot != null)
                indoorPot.showNextIndex.Value = true;
        }

        /// <summary>
        /// Determines if the current day should have crops auto-watered
        /// </summary>
        /// <returns>Whether or not crops should be auto-watered</returns>
        private bool ShouldAutoWaterToday()
        {
            // Stardew dates range from 1->28
            // The date mod 7 gives us the current day of the week
            switch (Game1.dayOfMonth % 7)
            {
                case 0: return _config.DaysToWater.Sunday;
                case 1: return _config.DaysToWater.Monday;
                case 2: return _config.DaysToWater.Tuesday;
                case 3: return _config.DaysToWater.Wednesday;
                case 4: return _config.DaysToWater.Thursday;
                case 5: return _config.DaysToWater.Friday;
                case 6: return _config.DaysToWater.Saturday;
                default: return true;
            }
        }
    }
}
