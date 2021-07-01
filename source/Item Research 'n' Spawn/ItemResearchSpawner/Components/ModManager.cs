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
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    internal class ModManager
    {
        public static ModManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;

        public readonly Dictionary<string, SpawnableItem> ItemRegistry =
            new Dictionary<string, SpawnableItem>();

        public Dictionary<string, int> CustomItemPriceList =
            new Dictionary<string, int>();

        #region Proprerties

        private ItemQuality _quality;
        private ModMode _modMode;
        private ItemSortOption _sortOption;
        private string _searchText;
        private string _category;

        public ItemQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                RequestMenuUpdate(false);
            }
        }

        public ModMode ModMode
        {
            get => _modMode;
            set
            {
                _modMode = value;
                RequestMenuUpdate(true);
            }
        }

        public ItemSortOption SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;
                RequestMenuUpdate(true);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RequestMenuUpdate(true);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                RequestMenuUpdate(true);
            }
        }

        #endregion

        public delegate void UpdateMenuView(bool rebuild);

        public event UpdateMenuView OnUpdateMenuView;

        public ModManager(IMonitor monitor, IModHelper helper)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;

            _helper.Events.GameLoop.Saving += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
        }

        public void InitRegistry(SpawnableItem[] items)
        {
            foreach (var spawnableItem in items)
            {
                var key = Helpers.GetItemUniqueKey(spawnableItem.Item);

                ItemRegistry[key] = spawnableItem;
            }
        }

        public void RequestMenuUpdate(bool rebuild)
        {
            OnUpdateMenuView?.Invoke(rebuild);
        }

        public void BuyItem(Item item)
        {
            var price = GetItemPrice(item, true);

            if (price > Game1.player._money)
            {
                Game1.player._money = 0;
            }
            else
            {
                Game1.player._money -= price;
            }
        }

        public void SellItem(Item item)
        {
            Game1.player._money += GetItemPrice(item, true);
        }

        public int GetItemPrice(Item item, bool countStack = false)
        {
            item.Stack = item.Stack > 0 ? item.Stack : 1;

            var spawnableItem = GetSpawnableItem(item, out var key);
            var price = -1;

            if (CustomItemPriceList.ContainsKey(key))
            {
                price = CustomItemPriceList[key];
            }

            if (price < 0)
            {
                price = Utility.getSellToStorePriceOfItem(item, false);
            }

            if (price <= 0 && !CustomItemPriceList.ContainsKey(key))
            {
                price = spawnableItem.CategoryPrice;
            }

            if (countStack)
            {
                price *= item.Stack;
            }

            return price;
        }

        public void SetItemPrice(Item activeItem, int price)
        {
            var key = Helpers.GetItemUniqueKey(activeItem);

            if (price < 0 && CustomItemPriceList.ContainsKey(key))
            {
                CustomItemPriceList.Remove(key);
            }
            else
            {
                CustomItemPriceList[key] = price;
            }

            _helper.Data.WriteJsonFile($"price-config.json", CustomItemPriceList);
        }

        public SpawnableItem GetSpawnableItem(Item item, out string key)
        {
            key = Helpers.GetItemUniqueKey(item);

            if (!ItemRegistry.TryGetValue(key, out var spawnableItem))
            {
                _monitor.LogOnce(
                    $"Item with - name: {item.Name}, ID: {item.parentSheetIndex}, key: {key} is missing in register!",
                    LogLevel.Alert);
            }

            return spawnableItem;
        }

        #region Save/Load

        private void OnSave(object sender, SavingEventArgs e)
        {
            var state = new ModState
            {
                ActiveMode = ModMode,
                Quality = Quality,
                SortOption = SortOption,
                SearchText = SearchText,
                Category = Category
            };

            _helper.Data.WriteJsonFile($"save/{SaveHelper.DirectoryName}/state.json", state);
            _helper.Data.WriteJsonFile($"price-config.json", CustomItemPriceList);
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            var state = _helper.Data.ReadJsonFile<ModState>(
                $"save/{SaveHelper.DirectoryName}/state.json") ?? new ModState
            {
                ActiveMode = _helper.ReadConfig<ModConfig>().DefaultMode
            };

            ModMode = state.ActiveMode;
            Quality = state.Quality;
            SortOption = state.SortOption;
            SearchText = state.SearchText;
            Category = state.Category;

            CustomItemPriceList = _helper.Data.ReadJsonFile<Dictionary<string, int>>(
                $"price-config.json") ?? new Dictionary<string, int>();
        }

        #endregion

        public void ReloadPriceList()
        {
            CustomItemPriceList = _helper.Data.ReadJsonFile<Dictionary<string, int>>(
                $"price-config.json") ?? new Dictionary<string, int>();
        }
    }
}