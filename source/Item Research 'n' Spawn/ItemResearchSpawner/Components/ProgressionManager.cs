/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Models.Messages;
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
        private readonly IManifest _modManifest;

        private Dictionary<string, ResearchProgression> _progression = new Dictionary<string, ResearchProgression>();

        public delegate void StackChanged(int newCount);

        public static event StackChanged OnStackChanged;

        public ProgressionManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
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

        public void ResearchItem(Item item)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out var progressionItem);

            if (itemProgressionRaw.max < 0)
            {
                return;
            }

            if (itemProgressionRaw.current >= itemProgressionRaw.max)
            {
                /*                if (ModManager.Instance.ModMode == ModMode.Buy)
                                {
                                    OnStackChanged?.Invoke(0);
                                }*/

                switch (ModManager.Instance.ModMode)
                {
                    case ModMode.BuySell:
                    case ModMode.Combined:
                        OnStackChanged?.Invoke(0);
                        break;
                    default: break;
                }

                return;
            }

            var needCount = itemProgressionRaw.max - itemProgressionRaw.current;

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

            //sync with host
            if (!Context.IsMainPlayer)
            {
                SendItemResearchMessage(item, progressionItem);
            }

            if (item.Stack >= progressCount)
            {
                OnResearchCompleted();
            }

/*            if (ModManager.Instance.ModMode == ModMode.Buy)
            {
                OnStackChanged?.Invoke(0);
            }
            else
            {
                OnStackChanged?.Invoke(item.Stack - progressCount);
            }*/


            switch (ModManager.Instance.ModMode)
            {
                case ModMode.BuySell:
                case ModMode.Combined:
                    OnStackChanged?.Invoke(0);
                    break;
                case ModMode.Research:
                    OnStackChanged?.Invoke(item.Stack - progressCount);
                    break;
                default: break;
            }
        }

        public bool ItemResearched(Item item)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out _);

            return itemProgressionRaw.max > 0 && itemProgressionRaw.current >= itemProgressionRaw.max;
        }

        public void UnlockAllProgression()
        {
            foreach (var item in ModManager.Instance.ItemRegistry.Values)
            {
                UnlockProgression(item.Item, false);
            }

            var message = new ResearchProgressionMessage()
            {
                Progression = _progression,
                PlayerID = Game1.player.UniqueMultiplayerID.ToString()
            };

            _helper.Multiplayer.SendMessage(message, MessageKeys.PROGRESSION_SAVE_REQUIRED,
                new[] {_modManifest.UniqueID});
        }

        public void UnlockProgression(Item activeItem, bool sendMessage = true)
        {
            var progression = TryInitAndReturnProgressionItem(activeItem);

            progression.ResearchCount = 999;

            if (activeItem is Object)
            {
                progression.ResearchCountSilver = 999;
                progression.ResearchCountGold = 999;
                progression.ResearchCountIridium = 999;
            }

            //sync with host
            if (!Context.IsMainPlayer && sendMessage)
            {
                SendItemResearchMessage(activeItem, progression);
            }

            OnResearchCompleted();
        }

        private void SendItemResearchMessage(Item item, ResearchProgression progression)
        {
            var itemResearchedMessage = new ItemResearchedMessage
            {
                Key = Helpers.GetItemUniqueKey(item),
                Progression = progression
            };

            _helper.Multiplayer.SendMessage(itemResearchedMessage, MessageKeys.PROGRESSION_ITEM_RESEARCHED,
                new[] {_modManifest.UniqueID});
        }

        public IEnumerable<ResearchableItem> GetResearchedItems()
        {
            return ModManager.Instance.ItemRegistry.Values
                .Select(i => new ResearchableItem
                {
                    Item = i,
                    Progression = TryInitAndReturnProgressionItem(i.Item)
                })
                .Where(i =>
                    i.Progression.ResearchCount >= i.Item.ProgressionLimit && i.Item.ProgressionLimit > 0);
        }

        public string GetItemProgression(Item item, bool itemActive = false)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out _, itemActive);

            if (itemProgressionRaw.max <= 0)
            {
                return "(X)";
            }

            if (ModManager.Instance.ModMode == ModMode.BuySell)
            {
                return "($$$)";
            }

            if (ModManager.Instance.ModMode == ModMode.Combined && itemProgressionRaw.current >= itemProgressionRaw.max)
            {
                return "($$$)";
            }

            return $"({itemProgressionRaw.current} / {itemProgressionRaw.max})";
        }

        private static void OnResearchCompleted()
        {
            ModManager.Instance.RequestMenuUpdate(true);
        }

        private ItemProgressionRaw GetItemProgressionRaw(Item item,
            out ResearchProgression progressionItem, bool itemActive = false)
        {
            var spawnableItem = ModManager.Instance.GetSpawnableItem(item, out _);

            if (spawnableItem == null)
            {
                progressionItem = null;

                return new ItemProgressionRaw
                {
                    current = -1,
                    max = -1
                };
            }

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            return GetItemProgressionRaw(spawnableItem, out progressionItem, itemQuality, itemActive);
        }

        private ItemProgressionRaw GetItemProgressionRaw(SpawnableItem item,
            out ResearchProgression progressionItem, ItemQuality quality = ItemQuality.Normal, bool itemActive = false)
        {
            /*            
            var category =
                ModManager.Instance.AvailableCategories.FirstOrDefault(c =>
                    I18n.GetByKey(c.Label).ToString().Equals(item.Category));

            // if (itemActive)
            // {
            //     _monitor.Log($"Current item - name: {item.Name}, ID: {item.ID}, category: {item.Category}",
            //         LogLevel.Alert);
            //     _monitor.Log($"Unique key: {Helpers.GetItemUniqueKey(item.Item)}",
            //         LogLevel.Alert);
            // }

            var maxProgression = ModManager.Instance.ModMode switch
            {
                ModMode.BuySell => 1,
                _ => category?.ResearchCount ?? 1
            };*/

            var maxProgression = item.ProgressionLimit;

            progressionItem = TryInitAndReturnProgressionItem(item.Item);

            var itemProgression = quality switch
            {
                ItemQuality.Silver => progressionItem.ResearchCountSilver,
                ItemQuality.Gold => progressionItem.ResearchCountGold,
                ItemQuality.Iridium => progressionItem.ResearchCountIridium,
                _ => progressionItem.ResearchCount
            };

            itemProgression = (int) MathHelper.Clamp(itemProgression, 0, maxProgression);

            return new ItemProgressionRaw
            {
                current = itemProgression,
                max = maxProgression
            };
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

        public void DumpPlayersProgression()
        {
            var onlinePlayers = Game1.getOnlineFarmers()
                .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            var offlinePlayers = Game1.getAllFarmers()
                .Where(farmer => !onlinePlayers.Keys.Contains(farmer.UniqueMultiplayerID.ToString()))
                .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            DumpPlayerProgression(Game1.player, _progression);
            
            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_DUMP_REQUIRED,
                    new[] {_modManifest.UniqueID});
            }

            var progressions = SaveManager.Instance.GetProgressions();

            foreach (var player in offlinePlayers)
            {
                DumpPlayerProgression(player.Value,
                    progressions.ContainsKey(player.Key)
                        ? progressions[player.Key]
                        : new Dictionary<string, ResearchProgression>());
            }
        }

        private void DumpPlayerProgression(Farmer player, Dictionary<string, ResearchProgression> progression)
        {
            _monitor.Log(
                $"Dumping progression - player: {player.Name}, location: {SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString())}",
                LogLevel.Info);

            _helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString()),
                progression);
        }

        public void LoadPlayersProgression()
        {
            var progressions = SaveManager.Instance.GetProgressions();
            var progressToLoad = new Dictionary<string, Dictionary<string, ResearchProgression>>();

            foreach (var playerID in progressions.Keys)
            {
                var playerData = _helper.Data.ReadJsonFile<Dictionary<string, ResearchProgression>>(
                    SaveHelper.ProgressionDumpPath(playerID));

                if (playerData != null)
                {
                    progressToLoad[playerID] = playerData;
                }
                else
                {
                    progressToLoad[playerID] = progressions[playerID];
                }
            }

            SaveManager.Instance.LoadProgressions(progressToLoad);

            if (Context.IsMainPlayer)
            {
                OnLoad(null, null);
            }
            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_MANAGER_SYNC,
                    new[] {_modManifest.UniqueID});
            }
        }

        #region SaveLoad

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != _modManifest.UniqueID) return;

            ResearchProgressionMessage researchProgressionMessage;
            ItemResearchedMessage itemResearchedMessage;

            var allowedTypes = new List<string> {"progression"};

            if (!allowedTypes.Any(t => e.Type.ToLower().Contains(t))) return;

            switch (e.Type)
            {
                case MessageKeys.PROGRESSION_ITEM_RESEARCHED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    itemResearchedMessage = e.ReadAs<ItemResearchedMessage>();

                    SaveManager.Instance.CommitResearch(e.FromPlayerID.ToString(), itemResearchedMessage.Key,
                        itemResearchedMessage.Progression);
                    break;

                case MessageKeys.PROGRESSION_SAVE_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    researchProgressionMessage = e.ReadAs<ResearchProgressionMessage>();
                    SaveManager.Instance.CommitProgression(researchProgressionMessage.PlayerID,
                        researchProgressionMessage.Progression);
                    break;

                case MessageKeys.PROGRESSION_LOAD_REQUIRED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    var playerID = e.FromPlayerID.ToString();
                    researchProgressionMessage = new ResearchProgressionMessage
                    {
                        Progression = SaveManager.Instance.GetProgression(playerID),
                        PlayerID = playerID
                    };

                    _helper.Multiplayer.SendMessage(researchProgressionMessage, MessageKeys.PROGRESSION_LOAD_ACCEPTED,
                        new[] {_modManifest.UniqueID}, new[] {long.Parse(researchProgressionMessage.PlayerID)});
                    break;

                case MessageKeys.PROGRESSION_LOAD_ACCEPTED:
                    researchProgressionMessage = e.ReadAs<ResearchProgressionMessage>();
                    OnLoadProgression(researchProgressionMessage.Progression);
                    break;

                case MessageKeys.PROGRESSION_DUMP_REQUIRED:
                    if (Context.IsMainPlayer)
                    {
                    }
                    else
                    {
                        researchProgressionMessage = new ResearchProgressionMessage
                        {
                            Progression = _progression,
                            PlayerID = Game1.player.UniqueMultiplayerID.ToString()
                        };

                        _helper.Multiplayer.SendMessage(researchProgressionMessage,
                            MessageKeys.PROGRESSION_DUMP_ACCEPTED,
                            new[] {_modManifest.UniqueID});
                    }

                    break;

                case MessageKeys.PROGRESSION_DUMP_ACCEPTED:
                    if (!Context.IsMainPlayer)
                    {
                        break;
                    }

                    researchProgressionMessage = e.ReadAs<ResearchProgressionMessage>();
                    
                    var farmer = Game1.getAllFarmers()
                        .FirstOrDefault(f =>
                            f.UniqueMultiplayerID.ToString().Equals(researchProgressionMessage.PlayerID));
                    
                    DumpPlayerProgression(farmer, researchProgressionMessage.Progression);
                    
                    break;

                case MessageKeys.PROGRESSION_MANAGER_SYNC:
                    OnLoad(null, null);
                    break;
            }
        }

        private void OnSave(object sender, DayEndingEventArgs dayEndingEventArgs)
        {
            if (!Context.IsMainPlayer) return;

            SaveManager.Instance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), _progression);
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                var progression =
                    SaveManager.Instance.GetProgression(Game1.player.UniqueMultiplayerID.ToString());

                OnLoadProgression(progression);
            }
            else
            {
                _helper.Multiplayer.SendMessage(0, MessageKeys.PROGRESSION_LOAD_REQUIRED,
                    new[] {_modManifest.UniqueID});
            }
        }

        private void OnLoadProgression(Dictionary<string, ResearchProgression> progression)
        {
            _progression = progression;
            ModManager.Instance.RequestMenuUpdate(true);
        }

        #endregion
    }
}