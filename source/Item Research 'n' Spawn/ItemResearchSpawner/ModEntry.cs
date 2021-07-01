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
using ItemResearchSpawner.Components;
using ItemResearchSpawner.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;
        private IModHelper _helper;
        private ModItemData _itemData;
        private ModDataCategory[] _categories;
        private SpawnableItem[] _items;

        private ProgressionManager _progressionManager;
        private ModManager _modManager;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();
            _itemData = helper.Data.ReadJsonFile<ModItemData>("assets/item-data.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories-progress.json");

            I18n.Init(helper.Translation);

            _modManager ??= new ModManager(Monitor, _helper);
            _progressionManager ??= new ProgressionManager(Monitor, _helper);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnLaunched;

            helper.ConsoleCommands.Add("research_unlock_all", "unlock all items research progression",
                UnlockAllProgression);

            helper.ConsoleCommands.Add("research_unlock_active", "unlock hotbar active item",
                UnlockActiveProgression);

            helper.ConsoleCommands.Add("research_set_mode", "change mode to \n 0 - Spawn Mode \n 1 - Buy/Sell Mode",
                SetMode);

            helper.ConsoleCommands.Add("research_set_price",
                "set hotbar active item price (globally, for mod menu only) \n 0+ values only",
                SetPrice);

            helper.ConsoleCommands.Add("research_reset_price",
                "reset hotbar active item price (globally, for mod menu only)",
                ResetPrice);

            helper.ConsoleCommands.Add("research_reload_prices", "reload pricelist file",
                ReloadPriceList);
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null) return;

            api.RegisterModConfig(ModManifest, () => _config = new ModConfig(), () => Helper.WriteConfig(_config));
            api.RegisterLabel(ModManifest, "Description", "ModManifest.Description");
            api.RegisterParagraph(ModManifest, ModManifest.Description.ToString());
            api.RegisterLabel(ModManifest, "Mod config", ":)");
            api.RegisterSimpleOption(ModManifest, "Menu open key", "Key to open mod menu",
                () => _config.ShowMenuKey, val => _config.ShowMenuKey = val);

            var availableModes = new List<string>(){"Spawn mode", "Buy/Sell mode"};
            
            api.RegisterChoiceOption(ModManifest, "Default mode", "Mod menu mode for the new games", 
                () => availableModes[(int)_config.DefaultMode], val => _config.DefaultMode = (ModMode) availableModes.IndexOf(val), availableModes.ToArray());
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                return;
            }

            if (_config.ShowMenuKey.JustPressed())
            {
                Game1.activeClickableMenu = GetSpawnMenu();
            }
        }

        private IClickableMenu GetSpawnMenu()
        {
            return new SpawnMenu(_items, Helper.Content, _helper, Monitor);
        }

        private IEnumerable<SpawnableItem> GetSpawnableItems()
        {
            var items = new ItemRepository().GetAll();

            if (_itemData?.ProblematicItems?.Any() == true)
            {
                var problematicItems =
                    new HashSet<string>(_itemData.ProblematicItems, StringComparer.OrdinalIgnoreCase);

                items = items.Where(item => !problematicItems.Contains($"{item.Type}:{item.ID}"));
            }

            foreach (var entry in items)
            {
                var category = _categories?.FirstOrDefault(rule => rule.IsMatch(entry));
                var label = category != null
                    ? I18n.GetByKey(category.Label).Default(category.Label)
                    : I18n.Category_Misc();

                yield return new SpawnableItem(entry, label ?? I18n.Category_Misc(), category?.BaseCost ?? 100,
                    category?.ResearchCount ?? 1);
            }
        }

        #region Commands

        private void ReloadPriceList(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            ModManager.Instance.ReloadPriceList();
        }

        private void ResetPrice(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                _modManager.SetItemPrice(activeItem, -1);
                Monitor.Log($"Price for {activeItem.DisplayName}, was resetted! ;)", LogLevel.Info);
            }
        }

        private void SetPrice(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                try
                {
                    var price = int.Parse(args[0]);

                    if (price < 0)
                    {
                        Monitor.Log($"Price must be a non-negative number", LogLevel.Info);
                    }

                    _modManager.SetItemPrice(activeItem, price);
                    Monitor.Log($"Price for {activeItem.DisplayName}, was changed to: {price}! ;)", LogLevel.Info);
                }
                catch (Exception)
                {
                    Monitor.Log($"Price must be a correct non-negative number", LogLevel.Info);
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _items = GetSpawnableItems().ToArray(); // some items exists only after day started ;_;
            _modManager.InitRegistry(_items);
        }

        private void UnlockAllProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            _progressionManager.UnlockAllProgression();
            Monitor.Log($"All researches were completed! :D", LogLevel.Info);
        }

        private void UnlockActiveProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                _progressionManager.UnlockProgression(activeItem);
                Monitor.Log($"Item - {activeItem.DisplayName}, was unlocked! ;)", LogLevel.Info);
            }
        }

        private void SetMode(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            try
            {
                _modManager.ModMode = (ModMode) int.Parse(args[0]);
                Monitor.Log($"Mode was changed to: {_modManager.ModMode.GetString()}", LogLevel.Info);
            }
            catch (Exception)
            {
                Monitor.Log($"Available modes: \n 0 - Spawn Mode \n 1 - Buy/Sell Mode", LogLevel.Info);
            }
        }

        private bool CheckCommandInGame()
        {
            if (!Game1.hasLoadedGame)
            {
                Monitor.Log($"Use this command in-game", LogLevel.Info);
                return false;
            }

            return true;
        }

        #endregion
    }
}