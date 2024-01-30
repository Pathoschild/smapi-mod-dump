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
    internal sealed class ModEntry : Mod
    {
        private ModConfig _config;
        private IModHelper _helper;

        private ProgressionManager _progressionManager;
        private ModManager _modManager;
        private SaveManager _saveManager;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;

            try
            {
                _config = helper.ReadConfig<ModConfig>();
            }
            catch (Exception e)
            {
                _config = new ModConfig();
                helper.WriteConfig(_config);
                Monitor.LogOnce("Failed to load config.json, replaced with default one");
            }
            

            I18n.Init(helper.Translation);

            _saveManager ??= new SaveManager(Monitor, _helper, ModManifest);
            _modManager ??= new ModManager(Monitor, _helper, ModManifest, _config);
            _progressionManager ??= new ProgressionManager(Monitor, _helper, ModManifest);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.GameLaunched += OnLaunched;

            _ = new CommandManager(_helper, Monitor, _progressionManager, _modManager);
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null) return;

            api.RegisterModConfig(ModManifest, () => _config = new ModConfig(), () => Helper.WriteConfig(_config));

            api.RegisterLabel(ModManifest, "Description", "ModManifest.Description");
            api.RegisterParagraph(ModManifest, ModManifest.Description.ToString());

            api.RegisterLabel(ModManifest, "Mod config", ":)");

            // ---------------- config options ----------------

            api.RegisterSimpleOption(ModManifest, "Menu open key", "Key to open mod menu",
                () => _config.ShowMenuButton, val => _config.ShowMenuButton = val);

            var availableModes = new List<string>(){"Research (Spawn) mode", "Buy/Sell mode", "Combined (Research->Sell/Buy) mode" };
            
            api.RegisterChoiceOption(ModManifest, "Default mode", "Mod menu mode for the new games", 
                () => availableModes[(int)_config.DefaultMode], val => _config.DefaultMode = (ModMode) availableModes.IndexOf(val), availableModes.ToArray());
            
            api.RegisterSimpleOption(ModManifest, "Force default config", "If true, mod will use predefined config in assets folder such as pricelist and categories",
                () => _config.UseDefaultBalanceConfig, val => _config.UseDefaultBalanceConfig = val);

            api.RegisterClampedOption(ModManifest, "Base reseach amount multiplier", "increase or decrease reseach amount for all items",
                () => _config.ResearchAmountMultiplier, (value) => { _config.ResearchAmountMultiplier = MathF.Round(value, 2); }, 0.1f, 10f);

            api.RegisterClampedOption(ModManifest, "Base buy price multiplier", "increase or decrease buy price for all items (Sell/Buy mode)",
                () => _config.BuyPriceMultiplier, (value) => { _config.BuyPriceMultiplier = MathF.Round(value, 2); }, 0.0f, 10f);

            api.RegisterClampedOption(ModManifest, "Base sell price multiplier", "increase or decrease sell price for all items (Sell/Buy mode)",
                () => _config.SellPriceMultiplier, (value) => { _config.SellPriceMultiplier = MathF.Round(value, 2); }, 0.0f, 10f);
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.CanPlayerMove)
            {
                return;
            }

            if (_config.ShowMenuButton.JustPressed())
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