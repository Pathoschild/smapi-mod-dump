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
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Models.Messages;
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
        private readonly IManifest _modManifest;

        public readonly Dictionary<string, SpawnableItem> ItemRegistry =
            new Dictionary<string, SpawnableItem>();

        public Dictionary<string, int> Pricelist =
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

        public ModManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
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

            _helper.Events.GameLoop.DayEnding += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
            _helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
        }

        public void InitRegistry(SpawnableItem[] items)
        {
            foreach (var spawnableItem in items)
            {
                var key = Helpers.GetItemUniqueKey(spawnableItem.Item);

                // fix copper pan
                if (key.Equals("Copper Pan:-1") && spawnableItem.Type == ItemType.Hat)
                {
                    continue;
                }

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

            if (Pricelist.ContainsKey(key))
            {
                price = Pricelist[key];
            }

            if (price < 0)
            {
                price = Utility.getSellToStorePriceOfItem(item, false);
            }

            if (price <= 0 && !Pricelist.ContainsKey(key))
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

            if (price < 0 && Pricelist.ContainsKey(key))
            {
                Pricelist.Remove(key);
            }
            else
            {
                Pricelist[key] = price;
            }

            _helper.Data.WriteJsonFile($"price-config.json", Pricelist);
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
        
        public void DumpPricelist(){
        
        }
        
        public void LoadPricelist(){
        
        }
        
        public void DumpCategories(){
        
        }
        
        public void LoadCategories(){
        
        }

        #region Save/Load

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == _modManifest.UniqueID)
            {
                ModStateMessage message;
                switch (e.Type)
                {
                    case "ModState:SaveRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        message = e.ReadAs<ModStateMessage>();
                        SaveManager.Instance.CommitModState(message.PlayerID, message.ModState);
                        break;
                    case "ModState:LoadRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        var playerID = e.ReadAs<string>();
                        message = new ModStateMessage
                        {
                            ModState = SaveManager.Instance.GetModState(playerID),
                            PlayerID = playerID
                        };
                        _helper.Multiplayer.SendMessage(message, "ModState:LoadAccepted",
                            new[] {_modManifest.UniqueID}, new[] {long.Parse(message.PlayerID)});
                        break;
                    case "ModState:LoadAccepted":
                        message = e.ReadAs<ModStateMessage>();
                        OnLoadState(message.ModState);
                        break;
                }
            }
        }

        private void OnSave(object sender, DayEndingEventArgs dayEndingEventArgs)
        {
            var state = new ModState
            {
                ActiveMode = ModMode,
                Quality = Quality,
                SortOption = SortOption,
                SearchText = SearchText,
                Category = Category
            };

            if (Context.IsMainPlayer)
            {
                SaveManager.Instance.CommitModState(Game1.player.uniqueMultiplayerID.ToString(), state);
            }
            else
            {
                var message = new ModStateMessage
                {
                    ModState = state,
                    PlayerID = Game1.player.uniqueMultiplayerID.ToString()
                };

                _helper.Multiplayer.SendMessage(message, "ModState:SaveRequired", new[] {_modManifest.UniqueID});
            }

            if (Context.IsMainPlayer)
            {
                SaveManager.Instance.CommitPricelist(Pricelist);
            }
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                var state = SaveManager.Instance.GetModState(Game1.player.uniqueMultiplayerID.ToString());
                OnLoadState(state);
            }
            else
            {
                _helper.Multiplayer.SendMessage(Game1.player.uniqueMultiplayerID, "ModState:LoadRequired",
                    new[] {_modManifest.UniqueID});
            }

            OnLoadPrices();
        }

        private void OnLoadState(ModState state)
        {
            ModMode = state.ActiveMode;
            Quality = state.Quality;
            SortOption = state.SortOption;
            SearchText = state.SearchText;
            Category = state.Category;
        }

        private void OnLoadPrices()
        {
            Pricelist = SaveManager.Instance.GetPricelist();
        }

        #endregion
    }
}