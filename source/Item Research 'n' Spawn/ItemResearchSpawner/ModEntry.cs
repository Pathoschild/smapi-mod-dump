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
using ItemResearchSpawner.Components.UI;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
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
        private SaveManager _saveManager;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();
            _itemData = helper.Data.ReadJsonFile<ModItemData>("assets/config/item-data.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/config/categories-progress.json");

            I18n.Init(helper.Translation);

            _saveManager ??= new SaveManager(Monitor, _helper, ModManifest);
            
            _modManager ??= new ModManager(Monitor, _helper, ModManifest);
            _progressionManager ??= new ProgressionManager(Monitor, _helper, ModManifest);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnLaunched;

            _ = new CommandManager(_helper, Monitor, _progressionManager, _modManager);
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
            
            api.RegisterSimpleOption(ModManifest, "Apply default config", "If true, mod will use predefined config in assets folder",
                () => _config.UseDefaultConfig, val => _config.UseDefaultConfig = val);
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.CanPlayerMove)
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
        
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _items = GetSpawnableItems().ToArray(); // some items exists only after day started ;_;
            _modManager.InitRegistry(_items);
        }
    }
}