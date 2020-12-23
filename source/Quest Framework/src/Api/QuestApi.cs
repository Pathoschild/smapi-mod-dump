/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Events;
using QuestFramework.Framework.Events;
using StardewModdingAPI;
using System;

namespace QuestFramework.Api
{
    public interface IQuestApi
    {
        /// <summary>
        /// Get Quest Framework API for your mod scope
        /// </summary>
        /// <param name="manifest">Your mod manifest</param>
        /// <returns></returns>
        IManagedQuestApi GetManagedApi(IManifest manifest);

        /// <summary>
        /// Force refresh cache, managed questlog and bulletinboard quest offer
        /// </summary>
        void ForceRefresh();

        /// <summary>
        /// Returns an quest id of managed quest
        /// </summary>
        /// <param name="fullQuestName">A fullqualified name of quest (questName@youdid)</param>
        /// <returns>
        /// Quest id if the quest with <param>fullQuestName</param> exists and it's managed, otherwise returns -1
        /// </returns>
        int ResolveQuestId(string fullQuestName);

        /// <summary>
        /// Resolves a fullqualified name (questName@youdid) of managed quest with this id
        /// </summary>
        /// <param name="questId"></param>
        /// <returns>
        /// Fullname of managed quest if this quest is managed by QF, otherwise returns null
        /// </returns>
        string ResolveQuestName(int questId);

        /// <summary>
        /// Is the quest with this id managed by QF?
        /// </summary>
        /// <param name="questId">A number representing quest id</param>
        /// <returns>True if this quest is managed, otherwise False</returns>
        bool IsManagedQuest(int questId);

        void LoadContentPack(Mod provider, IContentPack pack);

        /// <summary>
        /// Get QF lifecycle status as string
        /// </summary>
        /// <returns></returns>
        string GetStatus();

        /// <summary>
        /// Provide Quest Framework events
        /// </summary>
        IQuestFrameworkEvents Events { get; }

        /// <summary>
        /// Quest Framework lifecycle status
        /// </summary>
        State Status { get; }
    }

    public class QuestApi : IQuestApi
    {
        private readonly QuestFrameworkMod mod;
        private readonly IMonitor monitor;

        public State Status => this.mod.Status;

        internal QuestApi(QuestFrameworkMod mod)
        {
            this.mod = mod;
            this.monitor = mod.Monitor;
            this.Events = new QuestFrameworkEvents(this.mod.EventManager);
        }

        public IQuestFrameworkEvents Events { get; }

        public IManagedQuestApi GetManagedApi(IManifest manifest)
        {
            this.monitor.Log($"Requested managed api for mod `{manifest.UniqueID}`");

            return new ManagedQuestApi(
                modUid: manifest.UniqueID,
                questManager: this.mod.QuestManager,
                questOfferManager: this.mod.QuestOfferManager,
                conditionManager: this.mod.ConditionManager);
        }

        public void ForceRefresh()
        {
            this.monitor.Log("Force refresh requested");

            QuestFrameworkMod.InvalidateCache();
            this.mod.QuestController.RefreshAllManagedQuestsInQuestLog();
            this.mod.QuestController.RefreshBulletinboardQuestOffer();
            this.mod.EventManager.Refreshed.Fire(new EventArgs(), this);
        }

        public int ResolveQuestId(string fullQuestName)
        {
            var quest = this.mod.QuestManager.Fetch(fullQuestName);

            if (quest == null)
            {
                return -1;
            }

            return quest.id;
        }

        public bool IsManagedQuest(int questId)
        {
            return this.mod.QuestManager.GetById(questId) != null;
        }

        public string ResolveQuestName(int questId)
        {
            return this.mod.QuestManager.GetById(questId)?.GetFullName();
        }

        public void LoadContentPack(Mod provider, IContentPack contentPack)
        {
            if (this.mod.Status != State.STANDBY)
            {
                throw new InvalidOperationException($"Cannot load content pack in QF state `{this.mod.Status}`");
            }

            this.monitor.Log($"Loading content pack {contentPack.Manifest.UniqueID} (provided by {provider.ModManifest.Name}) ...");

            var content = this.mod.ContentPackLoader.LoadContentPack(contentPack);

            if (content != null && this.mod.ContentPackLoader.ValidateContent(content))
            {
                var m = contentPack.Manifest;

                this.mod.ContentPackLoader.ValidContents.Add(content);
                provider.Monitor.Log($"Provided QF content pack: {m.Name} {m.Version} by {m.Author} | {m.Description}", LogLevel.Info);
            }
        }

        public string GetStatus()
        {
            return this.mod.Status.ToString();
        }
    }
}
