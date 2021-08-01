/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ItemResearchSpawner.Components
{
    internal class SaveManager
    {
        public static SaveManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly IManifest _modManifest;

        private Dictionary<string, Dictionary<string, ResearchProgression>> _progressions;
        private Dictionary<string, ModState> _modStates;

        private Dictionary<string, int> _pricelist;
        private ICollection<ModDataCategory> _categories;

        public SaveManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;
            _modManifest = modManifest;

            _helper.Events.GameLoop.Saving += OnSave;
            _helper.Events.GameLoop.SaveLoaded += OnLoad;
        }

        public Dictionary<string, Dictionary<string, ResearchProgression>> GetProgressions()
        {
            return _progressions.DeepClone();
        }

        public void LoadProgressions(Dictionary<string, Dictionary<string, ResearchProgression>> progressToLoad)
        {
            _progressions = progressToLoad;
        }

        public void CommitProgression(string playerID, Dictionary<string, ResearchProgression> commitProgression)
        {
            var progression = GetProgression(playerID);

            foreach (var key in commitProgression.Keys.ToArray())
            {
                progression[key] = commitProgression[key];
            }

            _progressions[playerID] = progression;
        }

        public Dictionary<string, ResearchProgression> GetProgression(string playerID)
        {
            if (_progressions.ContainsKey(playerID))
            {
                return _progressions[playerID].DeepClone() ?? new Dictionary<string, ResearchProgression>();
            }

            return new Dictionary<string, ResearchProgression>();
        }

        public void CommitModState(string playerID, ModState modState)
        {
            _modStates[playerID] = modState;
        }

        public ModState GetModState(string playerID)
        {
            if (_modStates.ContainsKey(playerID))
            {
                return _modStates[playerID] ?? new ModState()
                {
                    ActiveMode = _helper.ReadConfig<ModConfig>().DefaultMode
                };
            }

            return new ModState()
            {
                ActiveMode = _helper.ReadConfig<ModConfig>().DefaultMode
            };
        }

        public void CommitPricelist(Dictionary<string, int> pricelist)
        {
            foreach (var key in pricelist.Keys.ToArray())
            {
                _pricelist[key] = pricelist[key];
            }
        }

        public Dictionary<string, int> GetPricelist()
        {
            return _pricelist.DeepClone();
        }

        public void CommitCategories(ModDataCategory[] categories)
        {
            _categories = categories;
        }

        public ModDataCategory[] GetCategories()
        {
            return _categories.ToArray();
        }

        private void OnSave(object sender, SavingEventArgs e)
        {
            _helper.Data.WriteSaveData(SaveHelper.ProgressionsKey, _progressions);

            _helper.Data.WriteSaveData(SaveHelper.ModStatesKey, _modStates);

            if (!_helper.ReadConfig<ModConfig>().UseDefaultConfig)
            {
                _helper.Data.WriteGlobalData(SaveHelper.PriceConfigKey, _pricelist);
                _helper.Data.WriteGlobalData(SaveHelper.CategoriesConfigKey, _categories);
            }
        }

        private void OnLoad(object sender, SaveLoadedEventArgs saveLoadedEventArgs)
        {
            LoadProgression();
            LoadState();
            LoadPricelist();
            LoadCategories();
        }

        private void LoadProgression()
        {
            try
            {
                _progressions =
                    _helper.Data.ReadSaveData<Dictionary<string, Dictionary<string, ResearchProgression>>>(SaveHelper
                        .ProgressionsKey)
                    ?? new Dictionary<string, Dictionary<string, ResearchProgression>>();
            }
            catch (Exception _)
            {
                _progressions = new Dictionary<string, Dictionary<string, ResearchProgression>>();
            }
        }

        private void LoadState()
        {
            try
            {
                _modStates = _helper.Data.ReadSaveData<Dictionary<string, ModState>>(SaveHelper.ModStatesKey) ??
                             new Dictionary<string, ModState>();
            }
            catch (Exception _)
            {
                _modStates = new Dictionary<string, ModState>();
            }
        }

        private void LoadPricelist()
        {
            if (!_helper.ReadConfig<ModConfig>().UseDefaultConfig)
            {
                try
                {
                    _pricelist = _helper.Data.ReadGlobalData<Dictionary<string, int>>(SaveHelper.PriceConfigKey);
                }
                catch (Exception _)
                {
                    _pricelist = null;
                }

                _pricelist ??= _helper.Data.ReadJsonFile<Dictionary<string, int>>(SaveHelper.PricelistConfigPath) ??
                               new Dictionary<string, int>();
            }
            else
            {
                _pricelist = _helper.Data.ReadJsonFile<Dictionary<string, int>>(SaveHelper.PricelistConfigPath) ??
                             new Dictionary<string, int>();
            }
        }

        private void LoadCategories()
        {
            if (!_helper.ReadConfig<ModConfig>().UseDefaultConfig)
            {
                try
                {
                    _categories = _helper.Data.ReadGlobalData<List<ModDataCategory>>(SaveHelper.CategoriesConfigKey);
                }
                catch (Exception _)
                {
                    _categories = null;
                }

                _categories ??= _helper.Data.ReadJsonFile<List<ModDataCategory>>(SaveHelper.CategoriesConfigPath) ??
                                new List<ModDataCategory>();
            }
            else
            {
                _categories = _helper.Data.ReadJsonFile<List<ModDataCategory>>(SaveHelper.CategoriesConfigPath) ??
                              new List<ModDataCategory>();
            }
        }
    }
}