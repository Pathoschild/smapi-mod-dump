/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using FarmAutomation.Common;
using FarmAutomation.ItemCollector.Processors;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace FarmAutomation.ItemCollector
{
    public class ItemCollectorMod : Mod
    {
        private bool _gameLoaded;
        private readonly MachinesProcessor _machinesProcessor;
        private readonly AnimalHouseProcessor _animalHouseProcessor;
        private readonly ItemCollectorConfiguration _config;

        public ItemCollectorMod()
        {
            Log.Info($"Initalizing {nameof(ItemCollectorMod)}");
            _config = ConfigurationBase.LoadConfiguration<ItemCollectorConfiguration>();
            ItemFinder.ConnectorItems = new List<string>(_config.ItemsToConsiderConnectors.Split(',').Select(v => v.Trim()));
            ItemFinder.ConnectorFloorings = _config.FlooringsToConsiderConnectors;

            var machinesToCollectFrom = _config.MachinesToCollectFrom.Split(',').Select(v => v.Trim()).ToList();
            var locationsToSearch = _config.LocationsToSearch.Split(',').Select(v => v.Trim()).ToList();
            _machinesProcessor = new MachinesProcessor(machinesToCollectFrom, locationsToSearch,
                _config.AddBuildingsToLocations, _config.AllowDiagonalConnectionsForAllItems)
            {
                MuteWhileCollectingFromMachines = Math.Max(0, Math.Min(5000, _config.MuteWhileCollectingFromMachines))
            };


            _animalHouseProcessor = new AnimalHouseProcessor(_config.PetAnimals, _config.AdditionalFriendshipFromCollecting, _config.MuteAnimalsWhenCollecting);
        }

        public override void Entry(params object[] objects)
        {
            base.Entry(objects);
            GameEvents.GameLoaded += (s, e) => { _gameLoaded = true; };

            TimeEvents.DayOfMonthChanged += (s, e) =>
            {
                if (_config.EnableMod)
                {
                    Log.Debug("It's a new day. Resetting the Item Collector mod");
                    _machinesProcessor.ValidateGameLocations();
                    _animalHouseProcessor.DailyReset();
                    _machinesProcessor.DailyReset();
                }
            };
            TimeEvents.TimeOfDayChanged += (s, e) =>
            {

                if (_gameLoaded && _config.EnableMod)
                {
                    try
                    {
                        _animalHouseProcessor.ProcessAnimalBuildings();
                        _machinesProcessor.ProcessMachines();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"an error occured with the automation mod: {ex}");
                        _machinesProcessor.DailyReset();
                    }
                }
            };
            PlayerEvents.InventoryChanged += (s, c) =>
            {
                if (_gameLoaded && ItemFinder.HaveConnectorsInInventoryChanged(c))
                {
                    try
                    {
                        _animalHouseProcessor.DailyReset();
                        _machinesProcessor.InvalidateCacheForLocation(Game1.player.currentLocation);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"an error occured with the automation mod: {ex}");
                        _machinesProcessor.DailyReset();
                    }
                }
            };
#if DEBUG
            // allow keypresses to initiate events for easier debugging.
            ControlEvents.KeyPressed += (s, c) =>
            {
                if (_gameLoaded && c.KeyPressed == Keys.K)
                {
                    _animalHouseProcessor.ProcessAnimalBuildings();
                    _machinesProcessor.ProcessMachines();
                }
                if (_gameLoaded && c.KeyPressed == Keys.P)
                {
                    _animalHouseProcessor.DailyReset();
                    _machinesProcessor.DailyReset();
                }
            };
#endif
        }
    }
}
