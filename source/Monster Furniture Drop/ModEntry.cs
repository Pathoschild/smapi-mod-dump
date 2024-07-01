/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MangusuPixel/MonsterFurnitureDrop
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using KaimiraGames;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Monsters;
using System.Linq;

namespace MonsterFurnitureDrop
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public fields
        *********/
        public static IMonitor SMonitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        /*********
        ** Private fields
        *********/
        public static WeightedList<string> ItemDropPool = new();
        public EventHandler<AssetRequestedEventArgs> OnAssetRequested;
        public EventHandler<UpdateTickedEventArgs> OnContentPatcherDone;
        public int validationDelay = 15;

        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;
            Helper = helper;
            Config = Helper.ReadConfig<ModConfig>();

            OnContentPatcherDone = OnGameTicked_ContenPatcherDone;
            OnAssetRequested = OnAssetRequested_RemoveCatalogue;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnContentPatcherDone;

            if (Config.RemoveFurnitureCatalogue)
            {
                helper.Events.Content.AssetRequested += OnAssetRequested;
            }

            var harmony = new Harmony(ModManifest.UniqueID);
            GamePatches.Apply(harmony);
        }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Fill up the item drop pool with valid entries from the config file
        /// </summary>
        private static void RefreshDropPool()
        {
            ItemDropPool.Clear();

            var furnitureData = Helper.GameContent.Load<Dictionary<string, string>>("Data/Furniture");

            foreach (var (itemKey, dropChance) in Config.ItemDropRates)
            {
                if (!furnitureData.ContainsKey(itemKey))
                    continue; // Item isn't currently available

                if (dropChance <= 0)
                    continue; // Invalid drop chance

                ItemDropPool.Add(itemKey, dropChance);
            }
        }

        /// <summary>Raised after each tick until Content Patcher finished adding modded items.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameTicked_ContenPatcherDone(object? sender, UpdateTickedEventArgs e)
        {
            if (validationDelay-- < 0) // Wait for CP to finish loading up mods
            {
                var furnitureData = Helper.GameContent.Load<Dictionary<string, string>>("Data/Furniture");
                var monsterData = Helper.GameContent.Load<Dictionary<string, string>>("Data/Monsters");

                AddMissingFurnitureEntries(furnitureData);
                AddMissingMonsterEntries(monsterData);

                Helper.WriteConfig(Config);

                var configMenu = LoadConfigOptions();

                if (configMenu != null)
                {
                    LoadItemConfigOptions(configMenu, furnitureData);
                    LoadMonsterConfigOptions(configMenu, monsterData);
                }

                Helper.Events.GameLoop.UpdateTicked -= OnContentPatcherDone;
            }
        }

        /// <summary>Raised after a safe is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            RefreshDropPool();
        }

        /// <summary>Raised when an asset is requested.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested_RemoveCatalogue(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(asset =>
                {
                    Monitor.Log("Removing the furniture catalogue from Robin's shop.", LogLevel.Trace);
                    var shops = asset.AsDictionary<string, ShopData>();
                    var carpenterShop = shops.Data["Carpenter"];
                    var index = carpenterShop.Items.FindIndex(item => item.ItemId.Equals("(F)1226"));

                    if (index > 0)
                    {
                        carpenterShop.Items.RemoveAt(index);
                        Helper.Events.Content.AssetRequested -= OnAssetRequested;
                    }
                });
            }
        }

        internal void AddMissingFurnitureEntries(Dictionary<string, string> data)
        {
            var entriesLoaded = 0;

            foreach (var (key, entry) in data)
            {
                if (Config.ItemDropRates.ContainsKey(key))
                    continue; // Entry isn't missing

                var rawData = entry.Split('/', StringSplitOptions.None);

                if (rawData.Length > 7 && rawData[7] is null or "")
                    continue; // Invalid display name

                if (rawData.Length > 10 && bool.TryParse(rawData[10], out bool isOffLimits) && isOffLimits)
                    continue; // Item is marked as off limits for random sale

                Config.ItemDropRates.Add(key, Config.DefaultItemDropRate);
                entriesLoaded++;
            }

            if (entriesLoaded > 0)
                Monitor.Log($"Added {entriesLoaded} new furniture entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
        }

        internal void AddMissingMonsterEntries(Dictionary<string, string> data)
        {
            var entriesLoaded = 0;

            foreach (var (key, entry) in data)
            {
                if (Config.MonsterDropRates.ContainsKey(key))
                    continue; // Entry isn't missing

                var rawData = entry.Split('/', StringSplitOptions.None);

                if (rawData.Length > 14 && rawData[14] is null or "")
                    continue; // Invalid display name

                Config.MonsterDropRates.Add(key, Config.DefaultMonsterDropRate);
                entriesLoaded++;
            }

            if (entriesLoaded > 0)
                Monitor.Log($"Added {entriesLoaded} new monster entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
        }

        /// <summary>Creates the config menu in GMCM</summary>
        private IGenericModConfigMenuApi? LoadConfigOptions()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return null;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config.ResetToDefaults(),
                save: () => {
                    Helper.WriteConfig(Config);
                    RefreshDropPool();
                }
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Global drop chance",
                tooltip: () => "Base percentage that an item will drop. 1 guarantees a drop. 0 disables drops.",
                getValue: () => Config.GlobalDropChance,
                setValue: (value) => Config.GlobalDropChance = value,
                interval: 0.001f,
                min: 0f,
                max: 1f,
                formatValue: (value) => string.Format("{0:P1}", value)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Remove furniture catalogue",
                tooltip: () => "Removes the furniture catalogue from Robin's store. Restart required.",
                getValue: () => Config.RemoveFurnitureCatalogue,
                setValue: (value) => Config.RemoveFurnitureCatalogue = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Log drop messages",
                tooltip: () => "Displays a log message when an item is dropped",
                getValue: () => Config.LogDropMessages,
                setValue: (value) => Config.LogDropMessages = value
            );

            configMenu.AddPageLink(ModManifest, "ItemDropRates", () => "[Item drop rates]");
            configMenu.AddPageLink(ModManifest, "MonsterDropRates", () => "[Monster drop rates]");

            return configMenu;
        }

        /// <summary>Creates the item drop config page in GMCM</summary>
        private void LoadItemConfigOptions(IGenericModConfigMenuApi configMenu, Dictionary<string, string> data)
        {
            configMenu.AddPage(ModManifest, "ItemDropRates", () => "Items");

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Default multiplier",
                tooltip: () => "Value assigned to newly added furnitures or when resetting to default settings. Default value is 100%.",
                getValue: () => Config.DefaultItemDropRate,
                setValue: (value) => Config.DefaultItemDropRate = value,
                min: 0,
                max: 5000,
                interval: 10,
                formatValue: (value) => $"{value} %"
            );

            configMenu.AddSectionTitle(ModManifest, () => "Multipliers");

            var sortedEntries = Config.ItemDropRates.OrderBy(pair => ItemRegistry.GetDataOrErrorItem("(F)" + pair.Key).DisplayName).ToDictionary(pair => pair.Key, pair => pair.Value);
            var entriesLoaded = 0;
            var entriesSkipped = 0;

            foreach (var (key, entry) in sortedEntries)
            {
                if (!data.ContainsKey(key))
                {
                    entriesSkipped++;
                    continue; // Currently unknown key;
                }

                var itemInfo = ItemRegistry.GetDataOrErrorItem("(F)" + key);

                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => itemInfo.DisplayName,
                    tooltip: () => "The drop chance multiplier of this item.",
                    getValue: () => Config.ItemDropRates[key],
                    setValue: (value) => Config.ItemDropRates[key] = value,
                    min: 0,
                    max: 5000,
                    interval: 10,
                    formatValue: (value) => $"{value} %"
                );
                entriesLoaded++;
            }

            if (entriesLoaded > 0)
                Monitor.Log($"Loaded {entriesLoaded} furniture config entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
            if (entriesSkipped > 0)
                Monitor.Log($"Skipped {entriesSkipped} furniture config entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
        }

        /// <summary>Creates the monster drop config page in GMCM</summary>
        private void LoadMonsterConfigOptions(IGenericModConfigMenuApi configMenu, Dictionary<string, string> data)
        {
            configMenu.AddPage(ModManifest, "MonsterDropRates", () => "Monsters");

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Default multiplier",
                tooltip: () => "Value assigned to newly added monsters or when resetting to default settings. Default value is 100%.",
                getValue: () => Config.DefaultMonsterDropRate,
                setValue: (value) => Config.DefaultMonsterDropRate = value,
                interval: 0.01f,
                min: 0f,
                max: 1f,
                formatValue: (value) => string.Format("{0:P0}", value)
            );

            configMenu.AddSectionTitle(ModManifest, () => "Multipliers");

            var sortedEntries = Config.MonsterDropRates.OrderBy(pair => Monster.GetDisplayName(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
            var entriesLoaded = 0;
            var entriesSkipped = 0;

            foreach (var (key, entry) in sortedEntries)
            {

                if (!data.ContainsKey(key))
                {
                    entriesSkipped++;
                    continue; // Currently unknown key;
                }

                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => Monster.GetDisplayName(key),
                    tooltip: () => "The drop chance multiplier of this monster.",
                    getValue: () => Config.MonsterDropRates[key],
                    setValue: (value) => Config.MonsterDropRates[key] = value,
                    interval: 0.01f,
                    min: 0f,
                    max: 1f,
                    formatValue: (value) => string.Format("{0:P0}", value)
                );
                entriesLoaded++;
            }

            if (entriesLoaded > 0)
                Monitor.Log($"Loaded {entriesLoaded} monster config entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
            if (entriesSkipped > 0)
                Monitor.Log($"Skipped {entriesSkipped} monster config entr{(entriesLoaded > 1 ? "ies" : "y")}.", LogLevel.Info);
        }
    }
}