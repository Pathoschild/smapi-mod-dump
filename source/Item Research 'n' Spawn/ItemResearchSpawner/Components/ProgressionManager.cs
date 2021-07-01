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
using System.Text.RegularExpressions;
using Force.DeepCloner;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Components
{
    internal class ProgressionManager
    {
        public static ProgressionManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;

        private readonly ModDataCategory[] _categories;

        private Dictionary<string, ResearchProgression> _progression =
            new Dictionary<string, ResearchProgression>();

        public delegate void StackChanged(int newCount);

        public static event StackChanged OnStackChanged;

        public ProgressionManager(IMonitor monitor, IModHelper helper)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;

            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories-progress.json");

            _helper.Events.GameLoop.Saving += OnSaveProgression;
            _helper.Events.GameLoop.DayStarted += OnLoadProgression;
        }

        public void ResearchItem(Item item)
        {
            var (itemProgression, progressionMax) = GetItemProgressionRaw(item, out var progressionItem);

            if (progressionMax <= 0 || itemProgression >= progressionMax)
            {
                if (ModManager.Instance.ModMode == ModMode.Buy)
                {
                    OnStackChanged?.Invoke(0);
                }

                return;
            }

            var needCount = progressionMax - itemProgression;

            var progressCount = item.Stack > needCount ? needCount : item.Stack;

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            if (itemQuality >= ItemQuality.Normal)
            {
                progressionItem.ResearchCount += progressCount;
            }

            if (itemQuality >= ItemQuality.Silver)
            {
                progressionItem.ResearchCountSilver += progressCount;
            }

            if (itemQuality >= ItemQuality.Gold)
            {
                progressionItem.ResearchCountGold += progressCount;
            }

            if (itemQuality >= ItemQuality.Iridium)
            {
                progressionItem.ResearchCountIridium += progressCount;
            }

            if (ModManager.Instance.ModMode == ModMode.Buy)
            {
                OnStackChanged?.Invoke(0);
            }
            else
            {
                OnStackChanged?.Invoke(item.Stack - progressCount);
            }

            if (item.Stack >= progressCount)
            {
                OnResearchCompleted();
            }
        }

        public bool ItemResearched(Item item)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item, out _);

            return maxProgression > 0 && itemProgression >= maxProgression;
        }

        public void UnlockAllProgression()
        {
            foreach (var item in ModManager.Instance.ItemRegistry.Values)
            {
                var progression = TryInitAndReturnProgressionItem(item.Item);

                progression.ResearchCount = 999;

                if (item.Item is Object)
                {
                    progression.ResearchCountSilver = 999;
                    progression.ResearchCountGold = 999;
                    progression.ResearchCountIridium = 999;
                }

                OnResearchCompleted();
            }
        }

        public void UnlockProgression(Item activeItem)
        {
            var progression = TryInitAndReturnProgressionItem(activeItem);

            progression.ResearchCount = 999;

            if (activeItem is Object)
            {
                progression.ResearchCountSilver = 999;
                progression.ResearchCountGold = 999;
                progression.ResearchCountIridium = 999;
            }

            OnResearchCompleted();
        }

        public IEnumerable<ResearchableItem> GetResearchedItems()
        {
            return ModManager.Instance.ItemRegistry.Values
                .Select(item => new ResearchableItem
                {
                    Item = item,
                    Progression = TryInitAndReturnProgressionItem(item.Item)
                })
                .Where(item =>
                    item.Progression.ResearchCount >= item.Item.ProgressionLimit && item.Item.ProgressionLimit > 0);
        }

        public string GetItemProgression(Item item, bool itemActive = false)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item, out _, itemActive);

            if (maxProgression <= 0)
            {
                return "???";
            }

            return $"({itemProgression} / {maxProgression})";
        }

        private static void OnResearchCompleted()
        {
            ModManager.Instance.RequestMenuUpdate(true);
        }

        private (int current, int max) GetItemProgressionRaw(Item item,
            out ResearchProgression progressionItem, bool itemActive = false)
        {
            var spawnableItem = ModManager.Instance.GetSpawnableItem(item, out _);

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            return GetItemProgressionRaw(spawnableItem, out progressionItem, itemQuality, itemActive);
        }

        private (int current, int max) GetItemProgressionRaw(SpawnableItem item,
            out ResearchProgression progressionItem, ItemQuality quality = ItemQuality.Normal, bool itemActive = false)
        {
            var category = _categories.FirstOrDefault(c => I18n.GetByKey(c.Label).ToString().Equals(item.Category));

            if (itemActive)
            {
                _monitor.Log($"Current item - name: {item.Name}, ID: {item.ID}, category: {item.Category}",
                    LogLevel.Alert);
                _monitor.Log($"Unique key: {Helpers.GetItemUniqueKey(item.Item)}",
                    LogLevel.Alert);
            }

            var maxProgression = ModManager.Instance.ModMode switch
            {
                ModMode.Buy => 1,
                _ => category?.ResearchCount ?? 1
            };

            progressionItem = TryInitAndReturnProgressionItem(item.Item);

            var itemProgression = quality switch
            {
                ItemQuality.Silver => progressionItem.ResearchCountSilver,
                ItemQuality.Gold => progressionItem.ResearchCountGold,
                ItemQuality.Iridium => progressionItem.ResearchCountIridium,
                _ => progressionItem.ResearchCount
            };

            itemProgression = (int) MathHelper.Clamp(itemProgression, 0, maxProgression);

            return (itemProgression, maxProgression);
        }

        private ResearchProgression TryInitAndReturnProgressionItem(Item item)
        {
            var key = Helpers.GetItemUniqueKey(item);

            ResearchProgression progressionItem;

            if (_progression.ContainsKey(key))
            {
                progressionItem = _progression[key];
            }
            else
            {
                progressionItem = new ResearchProgression();
                _progression[key] = new ResearchProgression();
            }

            return progressionItem;
        }

        #region SaveLoad

        private void OnSaveProgression(object sender, SavingEventArgs e)
        {
            SaveProgression();
        }

        private void OnLoadProgression(object sender, DayStartedEventArgs e)
        {
            LoadProgression();
        }

        private void SaveProgression()
        {
            _helper.Data.WriteJsonFile($"save/{SaveHelper.DirectoryName}/progress.json", _progression);
            _monitor.Log("Progression saved! :)", LogLevel.Debug);
        }

        private void LoadProgression()
        {
            var progressions =
                _helper.Data.ReadJsonFile<Dictionary<string, ResearchProgression>>(
                    $"save/{SaveHelper.DirectoryName}/progress.json") ?? new Dictionary<string, ResearchProgression>();

            //save backward compatibility
            _progression = new Dictionary<string, ResearchProgression>();

            var regex = new Regex(@"([\d+-]+):(.+):([\d+-]+)", RegexOptions.IgnoreCase);

            foreach (var pair in progressions)
            {
                var key = pair.Key;
                var match = regex.Match(key);

                if (match.Success)
                {
                    key = $"{match.Groups[2]}:{match.Groups[3]}";
                }

                _progression[key] = pair.Value;
            }

            _monitor.Log("Progression loaded! :)", LogLevel.Debug);
        }

        #endregion
    }
}