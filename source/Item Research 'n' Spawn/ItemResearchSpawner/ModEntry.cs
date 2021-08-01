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

        private ProgressionManager _progressionManager;
        private ModManager _modManager;
        private SaveManager _saveManager;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();

            I18n.Init(helper.Translation);

            _saveManager ??= new SaveManager(Monitor, _helper, ModManifest);
            _modManager ??= new ModManager(Monitor, _helper, ModManifest);
            _progressionManager ??= new ProgressionManager(Monitor, _helper, ModManifest);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
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
            
            api.RegisterSimpleOption(ModManifest, "Force default config", "If true, mod will use predefined config in assets folder such as pricelist and categories",
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
            return new SpawnMenu(_modManager.ItemRegistry.Values.ToArray(), Helper.Content, _helper, Monitor);
        }
    }
}