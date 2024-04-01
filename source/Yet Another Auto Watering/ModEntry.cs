/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZhuoYun233/YetAnotherAutoWatering-StardrewValleyMod
**
*************************************************/

using YetAnotherAutoWatering.Configuration;
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
using GenericModConfigMenu;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YetAnotherAutoWatering
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {

        //list of hoeDirt to water
        private readonly HashSet<HoeDirt> _hoeDirts = new HashSet<HoeDirt>();
        //list of indoor pots to water
        private readonly HashSet<IndoorPot> _indoorPots = new HashSet<IndoorPot>();
        
        private ModConfig _config;
        private bool _shouldWaterToday = true;
        private bool _keyPressWater = false;

        //Defination of fertilizer ID.
        private static Dictionary<string, int?> _validFertilizerDict = new Dictionary<string, int?>();

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.World.TerrainFeatureListChanged += this.OnTerrainFeatureListChanged;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            I18n.Init(helper.Translation);

            Dictionary<string, int?> newDictionary = new Dictionary<string, int?>
            {
                { "Disabled", null },
                { "Remove all fertilizer", 0 },
                { "Basic Fertilizer", 368 },
                { "Quality Fertilizer", 369 },
                { "Basic Retaining Soil", 370 },
                { "Quality Retaining Soil", 371 },
                { "Speed-Gro", 465 },
                { "Deluxe Speed-Gro", 466 },
                { "Hyper Speed-Gro", 918 },
                { "Deluxe Fertilizer", 919 },
                { "Deluxe Retaining Soil", 920 }
            };
            _validFertilizerDict = newDictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this._config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this._config)
            );

            //Main options
            configMenu.AddSectionTitle(
                mod: this.ModManifest,         
                text: () => I18n.Label_MainOptions(),
                tooltip: () => I18n.Label_MainOptions_Tooltip()
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_EnableAutoWatering(),
                tooltip: () => I18n.Option_EnableAutoWatering_Tooltip(),
                getValue: () => this._config.Enabled,
                setValue: value => this._config.Enabled = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => I18n.Option_AutoFertilizing(),
                tooltip: () => I18n.Option_AutoFertilizing_Tooltip(),
                getValue: () => _config.FertilizerType,
                setValue: value => _config.FertilizerType = value,
                allowedValues: new string[] {
                    "Disabled",
                    "Remove all fertilizer",
                    "Basic Fertilizer",
                    "Quality Fertilizer",
                    "Basic Retaining Soil",
                    "Quality Retaining Soil",
                    "Speed-Gro",
                    "Deluxe Speed-Gro",
                    "Hyper Speed-Gro",
                    "Deluxe Fertilizer",
                    "Deluxe Retaining Soil"
                },
                formatAllowedValue: FertilizerTranslated
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => I18n.Option_WaterNowKey(),
                tooltip: () => I18n.Option_WaterNowKey_Tooltip(),
                getValue: () => _config.WaterNowKey,
                setValue: value => _config.WaterNowKey = value
            );

            //Days to water Configuration
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => I18n.Label_DaysToWater(),
                tooltip: () => I18n.Label_DaysToWater_Tooltip()
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Sunday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Sunday,
                setValue: value => this._config.DaysToWater.Sunday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Monday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Monday,
                setValue: value => this._config.DaysToWater.Monday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Tuesday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Tuesday,
                setValue: value => this._config.DaysToWater.Tuesday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Wednesday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Wednesday,
                setValue: value => this._config.DaysToWater.Wednesday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Tuesday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Thursday,
                setValue: value => this._config.DaysToWater.Thursday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Friday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Friday,
                setValue: value => this._config.DaysToWater.Friday = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Option_DaysToWater_Saturday(),
                tooltip: () => I18n.Option_DaysToWater_Tooltip(),
                getValue: () => this._config.DaysToWater.Saturday,
                setValue: value => this._config.DaysToWater.Saturday = value
            );

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //Old version Reload Key
            /*if (e.Button == _config.ConfigReloadKey)
            {
                _config = Helper.ReadConfig<ModConfig>();
                Monitor.Log("Config file reloaded", LogLevel.Warn);
            }*/
            if (e.Button == _config.WaterNowKey)
            {
                _keyPressWater = true;
                foreach (var hoeDirt in _hoeDirts)
                    this.Water(hoeDirt);
                foreach (var indoorPot in _indoorPots)
                    this.Water(indoorPot.hoeDirt.Value, indoorPot);
                _keyPressWater = false;
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _shouldWaterToday = ShouldAutoWaterToday();

            var builtLocations = Game1.locations.OfType<GameLocation>() //Changed BuildableGameLocation to GameLocation. It solved the Error but I don't know if it causes other problems. So far so good.
                .SelectMany(location => location.buildings)
                .Select(building => building.indoors.Value)
                .Where(location => location != null);
            var allLocations = Game1.locations.Concat(builtLocations);

            foreach (var location in allLocations)
            {
                foreach (var indoorPot in location.objects.Values.OfType<IndoorPot>())
                    SetupIndoorPotListeners(indoorPot);

                foreach (var hoeDirt in location.terrainFeatures.Values.OfType<HoeDirt>())
                    SetupHoeDirtListeners(hoeDirt);
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _shouldWaterToday = ShouldAutoWaterToday();

            if (!_shouldWaterToday)
                return;

            foreach (var hoeDirt in _hoeDirts)
                this.Water(hoeDirt);
            foreach (var indoorPot in _indoorPots)
                this.Water(indoorPot.hoeDirt.Value, indoorPot);
        }
        /// <summary>
        /// Check if the recent changed object is indoor pot. If so, setup indoor pot listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var kvp in e.Added)
            {
                var obj = kvp.Value;
                if (obj is IndoorPot indoorPot)
                    SetupIndoorPotListeners(indoorPot);
            }
        }
        /// <summary>
        /// Check if the recent changed terrain feature is hoeDirt. If so, setup hoeDirt listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            foreach (var kvp in e.Added)
            {
                var feature = kvp.Value;
                if (feature is HoeDirt hoeDirt)
                    SetupHoeDirtListeners(hoeDirt);
            }
        }

        private void SetupIndoorPotListeners(IndoorPot indoorPot)
        {
            SetupHoeDirtListeners(indoorPot.hoeDirt.Value, indoorPot);
        }

        private void SetupHoeDirtListeners(HoeDirt hoeDirt, IndoorPot pot = null)
        {
            try
            {
                var netCrop = Helper.Reflection.GetField<NetRef<Crop>>(hoeDirt, "netCrop", true).GetValue();
                //Check if the hoeDirt have crop grown, if so, water
                if (netCrop.Value != null)
                    TrackAndWater(hoeDirt, pot);
                //Add new method with new parameter 'crop' to auto TrackAndWater or RemoveTrack when the specific field object changes
                netCrop.fieldChangeVisibleEvent += (_, __, crop) =>
                {
                    if (crop != null)
                        TrackAndWater(hoeDirt, pot);
                    else
                        RemoveTrack(hoeDirt, pot);
                };
            }
            catch (Exception ex)
            {
                Monitor.Log($"Could not read crop data on dirt; possible new game version?\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Track if the hoe dirt is in pot or not, add it to the list correspondingly, and water all items in both lists.
        /// </summary>
        /// <param name="hoeDirt"></param>
        /// <param name="indoorPot"></param>
        private void TrackAndWater(HoeDirt hoeDirt, IndoorPot indoorPot = null)
        {
            if (indoorPot != null)
                _indoorPots.Add(indoorPot);
            else
                _hoeDirts.Add(hoeDirt);
            Water(hoeDirt, indoorPot);
        }

        private void RemoveTrack(HoeDirt hoeDirt, IndoorPot indoorPot = null)
        {
            if (indoorPot != null)
                _indoorPots.Remove(indoorPot);
            else
                _hoeDirts.Remove(hoeDirt);
        }

        private void Water(HoeDirt hoeDirt, IndoorPot pot = null)
        {
            if (!_keyPressWater && (!_config.Enabled || !_shouldWaterToday))
                return;

            hoeDirt.state.Value = HoeDirt.watered;
            if (_config.FertilizerType != "Disabled")
            {
                if (_validFertilizerDict[_config.FertilizerType] != 0)
                    hoeDirt.fertilizer.Value = ItemRegistry.QualifyItemId(_validFertilizerDict[_config.FertilizerType].ToString());
                else 
                    //Always remove all fertilizer
                    hoeDirt.fertilizer.Value = null;
            }

            if (pot != null)
                pot.showNextIndex.Value = true;
        }

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

        private string FertilizerTranslated(string fertilizer)
        {
            switch (fertilizer)
            {
                case "Disabled": return I18n.Option_AutoFertilizing_Item_Disabled();
                case "Remove all fertilizer": return I18n.Option_AutoFertilizing_Item_RemoveAllFertilizer();
                case "Basic Fertilizer": return I18n.Option_AutoFertilizing_Item_BasicFertilizer();
                case "Quality Fertilizer": return I18n.Option_AutoFertilizing_Item_QualityFertilizer();
                case "Basic Retaining Soil": return I18n.Option_AutoFertilizing_Item_BasicRetainingSoil();
                case "Quality Retaining Soil": return I18n.Option_AutoFertilizing_Item_QualityRetainingSoil();
                case "Speed-Gro": return I18n.Option_AutoFertilizing_Item_SpeedGro();
                case "Deluxe Speed-Gro": return I18n.Option_AutoFertilizing_Item_DeluxeSpeedGro();
                case "Hyper Speed-Gro": return I18n.Option_AutoFertilizing_Item_HyperSpeedGro();
                case "Deluxe Fertilizer": return I18n.Option_AutoFertilizing_Item_DeluxeFertilizer();
                case "Deluxe Retaining Soil": return I18n.Option_AutoFertilizing_Item_DeluxeRetainingSoil();
                default: return "";
            }
        }

    }
}
