/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Extensions;
using QuestFramework.Framework.Menus;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework.Controllers
{
    internal class QuestController : IAssetEditor
    {
        private Dictionary<string, int> questIdCache;
        private readonly IMonitor monitor;

        public QuestController(QuestManager questManager, QuestOfferManager offerManager, IMonitor monitor)
        {
            this.QuestManager = questManager;
            this.OfferManager = offerManager;
            this.monitor = monitor;
            this.questIdCache = new Dictionary<string, int>();
        }

        public QuestManager QuestManager { get; }
        public QuestOfferManager OfferManager { get; }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Quests");
        }

        public void Edit<T>(IAssetData asset)
        {
            var questRegistry = asset.AsDictionary<int, string>();

            foreach(var questKv in this.QuestManager.Quests)
            {
                if (questKv.id < 0)
                    continue;

                if (questRegistry.Data.ContainsKey(questKv.id))
                    this.monitor.Log($"Assign quest `{questKv.GetFullName()}` to existing quest id #{questKv.id} in quest registry. Original quest overwritten.", LogLevel.Warn);

                questRegistry.Data[questKv.id] = this.ToQuestString(questKv);

                this.monitor.VerboseLog($"Injected quest #{questKv.id} aka `{questKv.Name}` to `Data\\Quests`");
                this.monitor.VerboseLog($"   {questRegistry.Data[questKv.id]}");
            }

            this.monitor.Log($"Injected {this.QuestManager.Quests.Count} managed quests into Data\\Quests");
        }

        public void RefreshAllManagedQuestsInQuestLog()
        {
            if (Context.IsMainPlayer)
            {
                // Refresh (de-sanitize) all managed quests in questlog
                foreach (var farmhand in Game1.getAllFarmers())
                {
                    var managedLog = farmhand.questLog
                        .Where(q => q.IsManaged())
                        .Select(q => new {
                            vanillaQuest = q,
                            managedQuest = q.AsManagedQuest()
                        });

                    foreach (var q in managedLog)
                    {
                        // Restore title, description and right cancellable flag
                        q.vanillaQuest._questTitle = q.managedQuest.Title;
                        q.vanillaQuest._questDescription = q.managedQuest.Description;
                        q.vanillaQuest.canBeCancelled.Value = q.managedQuest.Cancelable;
                        q.vanillaQuest.questTitle = q.managedQuest.Title; // For more safety
                    }

                    this.monitor.Log($"Refresh managed quests info in questlog for player `{farmhand.UniqueMultiplayerID}` aka `{farmhand.Name}`.");
                }
            }
        }

        public void SanitizeAllManagedQuestsInQuestLog(ITranslationHelper translation)
        {
            // Sanitize all managed quests in the log (for case quest framework will be removed in future or can't be loaded)
            foreach (var farmhand in Game1.getAllFarmers())
            {
                var managedLog = farmhand.questLog
                .Where(q => q.IsManaged())
                .Select(q => new {
                    vanillaQuest = q,
                    managedQuest = q.AsManagedQuest()
                });

                foreach (var q in managedLog)
                {
                    q.vanillaQuest.canBeCancelled.Value = true; // Allow cancelation
                    // Add "disclaimer" info
                    q.vanillaQuest._questTitle = $"{q.managedQuest.Name} {translation.Get("fallbackTitle")}";
                    q.vanillaQuest._questDescription = translation.Get("fallbackDescription");
                    q.vanillaQuest._currentObjective = translation.Get("fallbackObjective");
                    q.vanillaQuest.questTitle = q.vanillaQuest._questTitle; // For more safety
                }

                this.monitor.Log($"Managed quests in questlog for player `{farmhand.UniqueMultiplayerID}` aka `{farmhand.Name}` sanitized.");
            }
        }

        internal void SetQuestIdCache(Dictionary<string, int> questIdList)
        {
            if (Context.IsMainPlayer)
            {
                throw new InvalidOperationException("Cannot set quest id cache from external source in singleplayer or host game");
            }

            this.questIdCache = questIdList;
        }

        public void ReintegrateQuests(Dictionary<string, int> oldIds)
        {
            if (!Context.IsMainPlayer)
                throw new InvalidOperationException("Only main player can reintegrate quests.");

            this.ResetIds(this.QuestManager.Quests);
            var newIds = this.AssignIds(QuestManager.ID_ROOT, this.QuestManager.Quests);

            if (oldIds != null && Context.IsWorldReady)
            {
                foreach (var farmhand in Game1.getAllFarmers())
                {
                    foreach (var oldId in oldIds)
                    {
                        if (!farmhand.hasQuest(oldId.Value))
                            continue;

                        var relevantNameIds = newIds.Where(i => i.Key == oldId.Key);
                        if (relevantNameIds.Any())
                        {
                            var relevantNameIdPair = relevantNameIds.First();
                            var quest = farmhand.questLog.Where(q => q.id.Value == oldId.Value).FirstOrDefault();
                            var managedQuest = this.QuestManager.Fetch(relevantNameIdPair.Key);
                            var questType = this.QuestManager.Fetch(relevantNameIdPair.Key).CustomTypeId;

                            quest.id.Value = relevantNameIdPair.Value;
                            // quest.currentObjective = quest.id.Value.ToString();
                            quest.questType.Value = questType != -1
                                ? questType
                                : (int)managedQuest.BaseType;

                            this.monitor.Log($"Updated ID for quest in quest log: #{oldId.Value} -> #{relevantNameIdPair.Value} in {farmhand.Name}'s questlog (player id: {farmhand.UniqueMultiplayerID}).");
                        }
                        else
                        {
                            farmhand.removeQuest(oldId.Value);
                            this.monitor.Log($"Removed abandoned quest #{oldId.Value} from {farmhand.Name}'s questlog (player id: {farmhand.UniqueMultiplayerID}).");
                        }
                    }
                }
            }

            this.questIdCache = newIds;
        }

        internal void RefreshBulletinboardQuestOffer()
        {
            if (QuestFrameworkMod.Instance.Status == State.LAUNCHED)
            {
                this.monitor.VerboseLog("Try refresh offered quest of the day");

                Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + Game1.dayOfMonth);
                var offers = this.OfferManager.GetMatchedOffers("Bulletinboard").ToList();

                if (offers.Count > 0)
                {
                    int which = random.Next(offers.Count);
                    var offer = offers.ElementAtOrDefault(which < offers.Count ? which : 0);
                    var quest = offer != null ? this.QuestManager.Fetch(offer.QuestName) : null;

                    if (quest == null || Game1.player.hasQuest(quest.id))
                    {
                        this.monitor.VerboseLog("Offered quest is already accepted in questlog.");
                        return;
                    }

                    Game1.questOfTheDay = Quest.getQuestFromId(quest.id);
                    this.monitor.Log($"Added quest `{quest.Name}` to bulletin board as quest of the day. (Chosen from today {offers.Count} offered quests)");
                }

                CustomBoard.todayQuests.Clear();
            }
        }

        public void Reassign()
        {
            foreach (var q in this.questIdCache)
            {
                var managedQuest = this.QuestManager.Fetch(q.Key);

                if (managedQuest == null)
                    continue;

                managedQuest.id = q.Value;
            }
        }

        public void Reset()
        {
            this.ResetIds(this.QuestManager.Quests);
            this.questIdCache = new Dictionary<string, int>();
        }

        private Dictionary<string, int> AssignIds(int starting, List<CustomQuest> data, int capacity = 1000)
        {
            data.Sort((dni1, dni2) => dni1.GetFullName().CompareTo(dni2.GetFullName()));

            Dictionary<string, int> ids = new Dictionary<string, int>();
            int currId = starting;
            int maxId = starting + capacity - 1;

            foreach (var d in data)
            {
                while (data.Any(_d => _d.id == currId) && currId <= maxId)
                    ++currId;

                if (currId > maxId)
                {
                    this.monitor.Log($"Maximum capacity of ID space {starting}-{maxId} was reached! No more ids will be assigned", LogLevel.Error);
                    break;
                }

                if (d.id == -1)
                {
                    string name = d.GetFullName();
                    this.monitor.VerboseLog($"New ID: {name} = {currId}");
                    int id = currId++;

                    ids.Add(name, id);
                    d.id = ids[name];
                }
            }

            return ids;
        }

        private void ResetIds(List<CustomQuest> quests)
        {
            foreach (var quest in quests)
                quest.id = -1;
        }

        private string ToQuestString(CustomQuest quest)
        {
            var type = quest.BaseType == QuestType.Custom ? "Basic" : quest.BaseType.ToString();

            return $"{type}/{quest.Title}/{quest.Description}/{quest.Objective}./{quest.Trigger}/-1/{quest.Reward}/-1/true/{quest.ReactionText}";
        }

        public Dictionary<string, int> GetQuestIds()
        {
            return this.questIdCache;
        }
    }
}
