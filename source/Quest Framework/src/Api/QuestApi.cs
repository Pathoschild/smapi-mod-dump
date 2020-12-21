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
using QuestFramework.Framework;
using QuestFramework.Framework.Events;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public State Status => QuestFrameworkMod.Instance.Status;

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
    }
}
