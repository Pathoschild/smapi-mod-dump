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
using System.Threading;
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

        public readonly Dictionary<string, SpawnableItem> ItemRegistry = new Dictionary<string, SpawnableItem>();

        private Dictionary<string, int> _pricelist;
        private List<ModDataCategory> _categories;

        private ModState _syncedModState;
        private ModConfig _config;

        #region Proprerties

        private ItemQuality _quality;
        private ModMode _modMode;
        private ItemSortOption _sortOption;
        private string _searchText;
        private string _category;

        public ModDataCategory[] AvailableCategories => _categories.ToArray();

        public ItemQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;

                SendStateSaveMessage();

                RequestMenuUpdate(false);
            }
        }

        public ModMode ModMode
        {
            get => _modMode;
            set
            {
                if (_modMode == value) return;

                _modMode = value;
                RequestMenuUpdate(true);

                //sync with multiplayer (only by host or infinite messages)
                if (Context.IsMultiplayer && Context.IsMainPlayer)
                {
                    _helper.Multiplayer.SendMessage("", MessageKeys.MOD_MANAGER_SYNC, new[] {_modManifest.UniqueID});
                }
            }
        }

        public ItemSortOption SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;

                SendStateSaveMessage();
                    
                RequestMenuUpdate(true);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                
                SendStateSaveMessage();
                
                RequestMenuUpdate(true);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                
                SendStateSaveMessage();
                
                RequestMenuUpdate(true);
            }
        }

        #endregion
        
        private void SendStateSaveMessage()
        {
            var modState = GetCurrentModState();

            if (!modState.Equals(_syncedModState))
            {
                var message = new ModStateMessage
                {
                    ModState = modState,
                    PlayerID = Game1.player.uniqueMultiplayerID.ToString()
                };

                _helper.Multiplayer.SendMessage(message, MessageKeys.MOD_STATE_SAVE_REQUIRED,
                    new[] {_modManifest.UniqueID});

                _syncedModState = modState;
            }
        }

        public delegate void UpdateMenuView(bool rebuild);

        public event UpdateMenuView OnUpdateMenuView;

        public ModManager(IMonitor monitor, IModHelper helper, IManifest modManifest, ModConfig config)
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

            _pricelist = new Dictionary<string, int>();
            _categories = new List<ModDataCategory>();

            _helper.Events.GameLoop.DayEnding += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
            _helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;

            _config = config;
        }

        private void InitRegistry(IEnumerable<SpawnableItem> items)
        {
            var banlist = _helper.Data.ReadJsonFile<List<string>>(SaveHelper.BannedItemsConfigPath) ??
                          new List<string>();

            foreach (var spawnableItem in items)
            {
                var key = Helpers.GetItemUniqueKey(spawnableItem.Item);

                // fix copper pan
                if (key.Equals("Copper Pan:-1") && spawnableItem.Type == ItemType.Hat)
                {
                    continue;
                }

                if (banlist.Contains(key))
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
            var price = GetItemBuyPrice(item, true);

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
            Game1.player._money += GetItemSellPrice(item, true);
        }

        public (int buy, int sell) GetItemPrices(Item item, bool countStack = false)
        {
            var price = GetItemPrice(item, false);

            var buyPrice = (int)MathF.Round((float)price * _config.BuyPriceMultiplier);
            buyPrice = buyPrice >= 0 ? buyPrice : 0;

            var sellPrice = (int)MathF.Round((float)price * _config.SellPriceMultiplier);
            sellPrice = sellPrice >= 0 ? sellPrice : 0;

            if (countStack)
            {
                buyPrice *= item.Stack;
                sellPrice *= item.Stack;
            }


            return new (buyPrice, sellPrice);
        }

        public int GetItemBuyPrice(Item item, bool countStack = false)
        {
            var buyPrice = GetItemPrice(item, countStack, _config.BuyPriceMultiplier);

            return buyPrice >= 0 ? buyPrice : 0;
        }

        public int GetItemSellPrice(Item item, bool countStack = false)
        {
            var sellPrice = GetItemPrice(item, countStack, _config.SellPriceMultiplier);

            return sellPrice >= 0 ? sellPrice : 0;
        }

        public int GetItemPrice(Item item, bool countStack = false, float multiplyBy=1.0f)
        {
            item.Stack = item.Stack > 0 ? item.Stack : 1;

            var spawnableItem = GetSpawnableItem(item, out var key);

            if (spawnableItem == null)
            {
                return 0;
            }

            var price = -1;

            if (_pricelist.ContainsKey(key))
            {
                price = _pricelist[key];
            }

            if (price < 0)
            {
                price = Utility.getSellToStorePriceOfItem(item, false);
            }

            if (price <= 0 && !_pricelist.ContainsKey(key))
            {
                price = spawnableItem.CategoryPrice;
            }

            if (multiplyBy != 1.0f)
            {
                price = (int)MathF.Round((float)price * multiplyBy);
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

            if (price < 0 && _pricelist.ContainsKey(key))
            {
                _pricelist.Remove(key);
            }
            else
            {
                _pricelist[key] = price;
            }
            
            SaveManager.Instance.CommitPricelist(_pricelist);

            //sync with multiplayer
            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.MOD_MANAGER_SYNC, new[] {_modManifest.UniqueID});
            }
        }

        public SpawnableItem GetSpawnableItem(Item item, out string key)
        {
            key = Helpers.GetItemUniqueKey(item);

            if (!ItemRegistry.TryGetValue(key, out var spawnableItem))
            {
                // _monitor.LogOnce(
                //     $"Item with - name: {item.Name}, ID: {item.parentSheetIndex}, key: {key} is missing in register!",
                //     LogLevel.Debug);
            }

            return spawnableItem;
        }

        public void DumpPricelist()
        {
            var prices = _pricelist;

            if (_helper.ReadConfig<ModConfig>().UseDefaultBalanceConfig)
            {
                prices = _helper.Data.ReadGlobalData<Dictionary<string, int>>(SaveHelper.PriceConfigKey) ?? _pricelist;
            }

            _helper.Data.WriteJsonFile(SaveHelper.PricelistDumpPath, prices);
        }

        public void LoadPricelist()
        {
            if (_helper.ReadConfig<ModConfig>().UseDefaultBalanceConfig)
            {
                _monitor.Log(
                    "Note: default config is being used, your changes will be ignored unless you turn the use of default config off");
                return;
            }

            SaveManager.Instance.CommitPricelist(_helper.Data.ReadJsonFile<Dictionary<string, int>>(
                SaveHelper.PricelistDumpPath));

            if (Context.IsMainPlayer)
            {
                OnLoad(null, null);
            }
            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.MOD_MANAGER_SYNC, new[] {_modManifest.UniqueID});
            }
        }

        public void DumpCategories()
        {
            var categories = _categories;

            if (_helper.ReadConfig<ModConfig>().UseDefaultBalanceConfig)
            {
                categories = _categories =
                    _helper.Data.ReadGlobalData<List<ModDataCategory>>(SaveHelper.CategoriesConfigKey) ?? _categories;
            }

            _helper.Data.WriteJsonFile(SaveHelper.CategoriesDumpPath, categories);
        }

        public void LoadCategories()
        {
            if (_helper.ReadConfig<ModConfig>().UseDefaultBalanceConfig)
            {
                _monitor.Log(
                    "Note: default config is being used, your changes will be ignored unless you turn the use of default config off");
                return;
            }

            SaveManager.Instance.CommitCategories(_helper.Data.ReadJsonFile<List<ModDataCategory>>(
                SaveHelper.CategoriesDumpPath));

            if (Context.IsMainPlayer)
            {
                OnLoad(null, null);
            }
            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.MOD_MANAGER_SYNC, new[] {_modManifest.UniqueID});
            }
        }

        #region Save/Load

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != _modManifest.UniqueID) return;

            var allowedTypes = new List<string> {"modstate", "pricelist", "categories", "modmanager"};

            if (!allowedTypes.Any(t => e.Type.ToLower().Contains(t))) return;

            ModStateMessage modStateMessage;
            PricelistMessage pricelistMessage;
            CategoriesMessage categoriesMessage;
            string playerID;

            switch (e.Type)
            {
                /*ModState messages*/
                case MessageKeys.MOD_STATE_SAVE_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    modStateMessage = e.ReadAs<ModStateMessage>();
                    SaveManager.Instance.CommitModState(modStateMessage.PlayerID, modStateMessage.ModState);
                    break;

                case MessageKeys.MOD_STATE_LOAD_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    playerID = e.FromPlayerID.ToString();
                    modStateMessage = new ModStateMessage
                    {
                        ModState = SaveManager.Instance.GetModState(playerID),
                        PlayerID = playerID
                    };

                    //sync mod mode
                    modStateMessage.ModState.ActiveMode = ModMode;

                    _helper.Multiplayer.SendMessage(modStateMessage, MessageKeys.MOD_STATE_LOAD_ACCEPTED,
                        new[] {_modManifest.UniqueID}, new[] {long.Parse(modStateMessage.PlayerID)});
                    break;

                case MessageKeys.MOD_STATE_LOAD_ACCEPTED:
                    modStateMessage = e.ReadAs<ModStateMessage>();
                    OnLoadState(modStateMessage.ModState);
                    break;

                /*Pricelist messages*/
                case MessageKeys.PRICELIST_LOAD_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    playerID = e.FromPlayerID.ToString();
                    pricelistMessage = new PricelistMessage
                    {
                        Pricelist = SaveManager.Instance.GetPricelist(),
                        PlayerID = playerID
                    };

                    _helper.Multiplayer.SendMessage(pricelistMessage, MessageKeys.PRICELIST_LOAD_ACCEPTED,
                        new[] {_modManifest.UniqueID}, new[] {long.Parse(pricelistMessage.PlayerID)});
                    break;
                case MessageKeys.PRICELIST_LOAD_ACCEPTED:
                    pricelistMessage = e.ReadAs<PricelistMessage>();
                    OnLoadPrices(pricelistMessage.Pricelist);
                    break;

                /*Categories messages*/
                case MessageKeys.CATEGORIES_LOAD_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    playerID = e.FromPlayerID.ToString();
                    categoriesMessage = new CategoriesMessage
                    {
                        Categories = SaveManager.Instance.GetCategories(),
                        PlayerID = playerID
                    };

                    _helper.Multiplayer.SendMessage(categoriesMessage, MessageKeys.CATEGORIES_LOAD_ACCEPTED,
                        new[] {_modManifest.UniqueID}, new[] {long.Parse(categoriesMessage.PlayerID)});
                    break;

                case MessageKeys.CATEGORIES_LOAD_ACCEPTED:
                    categoriesMessage = e.ReadAs<CategoriesMessage>();
                    OnLoadCategories(categoriesMessage.Categories);
                    break;

                /*Sync*/
                case MessageKeys.MOD_MANAGER_SYNC:
                    OnLoad(null, null);
                    break;
            }
        }

        private void OnSave(object sender, DayEndingEventArgs dayEndingEventArgs)
        {
            var state = GetCurrentModState();

            if (!Context.IsMainPlayer) return;

            SaveManager.Instance.CommitModState(Game1.player.uniqueMultiplayerID.ToString(), state);
            SaveManager.Instance.CommitPricelist(_pricelist);
            SaveManager.Instance.CommitCategories(_categories);
        }

        private ModState GetCurrentModState()
        {
            var state = new ModState
            {
                ActiveMode = ModMode,
                Quality = Quality,
                SortOption = SortOption,
                SearchText = SearchText,
                Category = Category
            };
            return state;
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                OnLoadState(SaveManager.Instance.GetModState(Game1.player.uniqueMultiplayerID.ToString()));
                OnLoadPrices(SaveManager.Instance.GetPricelist());
                OnLoadCategories(SaveManager.Instance.GetCategories());
            }
            else
            {
                _helper.Multiplayer.SendMessage(0, MessageKeys.MOD_STATE_LOAD_REQUIRED,
                    new[] {_modManifest.UniqueID});
                _helper.Multiplayer.SendMessage(0, MessageKeys.PRICELIST_LOAD_REQUIRED,
                    new[] {_modManifest.UniqueID});
                _helper.Multiplayer.SendMessage(0, MessageKeys.CATEGORIES_LOAD_REQUIRED,
                    new[] {_modManifest.UniqueID});
            }
        }

        private void OnLoadState(ModState state)
        {
            _modMode = state.ActiveMode;
            _quality = state.Quality;
            _sortOption = state.SortOption;
            _searchText = state.SearchText;
            _category = state.Category;
            
            RequestMenuUpdate(true);
        }

        private void OnLoadPrices(Dictionary<string, int> pricelist)
        {
            _pricelist = pricelist;
            RequestMenuUpdate(true);
        }

        private void OnLoadCategories(List<ModDataCategory> categories)
        {
            _categories = categories;
            InitRegistry(GetSpawnableItems().ToArray());
            RequestMenuUpdate(true);
        }

        #endregion

        private IEnumerable<SpawnableItem> GetSpawnableItems()
        {
            var items = new ItemRepository().GetAll();

            foreach (var entry in items)
            {
                var category = _categories?.FirstOrDefault(rule => rule.IsMatch(entry));
                var label = category != null
                    ? I18n.GetByKey(category.Label).Default(category.Label)
                    : I18n.Category_Misc();

                int baseResearchCount = category?.ResearchCount ?? 1;
                //float researchMultiplier = _helper.ReadConfig<ModConfig>().ResearchAmountMultiplier;

                yield return new SpawnableItem(entry, label ?? I18n.Category_Misc(), category?.BaseCost ?? 100,
                    (int)((float)baseResearchCount * _config.ResearchAmountMultiplier));
            }
        }
    }
}