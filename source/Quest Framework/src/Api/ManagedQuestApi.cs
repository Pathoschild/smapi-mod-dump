/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework;
using QuestFramework.Framework.Controllers;
using QuestFramework.Framework.Structures;
using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuestFramework.Api
{
    internal class ManagedQuestApi : IManagedQuestApi
    {
        private readonly Regex allowedChars = new Regex("^[a-zA-Z0-9_-]*$");
        public string ModUid { get; }
        public QuestManager QuestManager { get; }
        public QuestOfferManager QuestOfferManager { get; }
        public ConditionManager ConditionManager { get; }
        public CustomBoardController CustomBoardController { get; }

        private static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;

        public ManagedQuestApi(string modUid, QuestManager questManager, QuestOfferManager questOfferManager, ConditionManager conditionManager, CustomBoardController customBoardController)
        {
            this.ModUid = modUid;
            this.QuestManager = questManager;
            this.QuestOfferManager = questOfferManager;
            this.ConditionManager = conditionManager;
            this.CustomBoardController = customBoardController;
        }

        public void AcceptQuest(string fullQuestName, bool silent = false)
        {
            if (!fullQuestName.Contains('@'))
            {
                fullQuestName = $"{fullQuestName}@{this.ModUid}";
            }

            this.QuestManager.AcceptQuest(fullQuestName, silent);
        }

        [Obsolete("This API is deprecated! Use IManagedQuestApi.GetQuestById instead.", true)]
        public CustomQuest GetById(int id)
        {
            return this.GetQuestById(id);
        }

        public CustomQuest GetQuestById(int id)
        {
            var managedQuest = this.QuestManager.GetById(id);

            if (managedQuest?.OwnedByModUid != this.ModUid)
                Monitor.LogOnce($"Mod {this.ModUid} accessed to quest `{managedQuest.Name}` managed by mod `{managedQuest.OwnedByModUid}`");

            return managedQuest;
        }

        public CustomQuest GetQuestByName(string questName)
        {
            if (!questName.Contains('@'))
            {
                questName = $"{questName}@{this.ModUid}";
            }

            return this.GetQuestById(this.QuestManager.ResolveGameQuestId(questName));
        }

        public void RegisterQuest(CustomQuest quest)
        {
            if (!this.allowedChars.IsMatch(quest.Name))
                throw new InvalidQuestException("Quest name contains unallowed characters.");

            quest.OwnedByModUid = this.ModUid;

            this.QuestManager.RegisterQuest(quest);
        }

        public void CompleteQuest(string questName)
        {
            int questId = this.QuestManager.ResolveGameQuestId($"{questName}@{this.ModUid}");

            if (questId > -1 && Game1.player.hasQuest(questId))
                Game1.player.completeQuest(questId);
        }

        public void OfferQuest(QuestOffer offer)
        {
            if (!offer.QuestName.Contains('@'))
            {
                offer.QuestName = $"{offer.QuestName}@{this.ModUid}";
            }

            this.QuestOfferManager.AddOffer(offer);
        }

        public IEnumerable<QuestOffer> GetTodayQuestOffers(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.QuestOfferManager.GetMatchedOffers(source);
        }

        public IEnumerable<QuestOffer<TAttributes>> GetTodayQuestOffers<TAttributes>(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.QuestOfferManager.GetMatchedOffers<TAttributes>(source);
        }

        public void ExposeGlobalCondition(string conditionName, Func<string, CustomQuest, bool> conditionHandler)
        {
            if (!this.allowedChars.IsMatch(conditionName))
                throw new ArgumentException("Condition name contains unallowed characters.", nameof(conditionName));

            string fullConditionName = $"{this.ModUid}/{conditionName}";

            this.ConditionManager.Conditions[fullConditionName] = (value, context) => conditionHandler(value, context as CustomQuest);
            Monitor.Log($"Exposed custom global condition `{fullConditionName}`");
        }

        public void ExposeQuestType<TQuest>(string type, Func<TQuest> factory) where TQuest : CustomQuest
        {
            if (!this.allowedChars.IsMatch(type))
                throw new ArgumentException("Quest type name contains unallowed characters.", nameof(type));

            this.QuestManager.RegisterQuestFactory($"{this.ModUid}/{type}", factory);
            Monitor.Log($"{this.ModUid} exposed quest type {this.ModUid}/{type}", LogLevel.Debug);
        }

        public void ExposeQuestType<TQuest>(string type) where TQuest : CustomQuest, new()
        {
            this.ExposeQuestType(type, () => new TQuest());
        }

        public bool HasQuestType(string type)
        {
            if (!type.Contains('/'))
                type = $"{this.ModUid}/{type}";

            return this.QuestManager.Factories.ContainsKey(type);
        }

        public void RegisterCustomBoard(CustomBoardTrigger boardTrigger)
        {
            if (boardTrigger == null)
            {
                throw new ArgumentNullException(nameof(boardTrigger));
            }

            this.CustomBoardController.RegisterBoardTrigger(boardTrigger);
        }

        public bool CheckForQuestComplete(ICompletionMessage completionMessage)
        {
            if (completionMessage == null)
            {
                throw new ArgumentNullException(nameof(completionMessage));
            }

            return this.QuestManager.CheckForQuestComplete(completionMessage);
        }

        public bool CheckForQuestComplete<TQuest>(ICompletionMessage completionMessage) where TQuest : CustomQuest
        {
            if (completionMessage == null)
            {
                throw new ArgumentNullException(nameof(completionMessage));
            }

            return this.QuestManager.CheckForQuestComplete<TQuest>(completionMessage);
        }

        public void AdjustQuest(object adjustMessage)
        {
            if (adjustMessage is null)
            {
                throw new ArgumentNullException(nameof(adjustMessage));
            }

            this.QuestManager.AdjustQuest(adjustMessage);
        }

        public IEnumerable<CustomQuest> GetAllManagedQuests()
        {
            return this.QuestManager.Quests.AsReadOnly();
        }

        public IEnumerable<T> GetAllManagedQuests<T>() where T : CustomQuest
        {
            return this.GetAllManagedQuests().OfType<T>();
        }
    }
}
