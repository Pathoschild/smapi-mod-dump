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
        private readonly Dictionary<string, ManagedQuestApi> managers;
        private readonly QuestManager baseManager;
        private readonly QuestOfferManager scheduleManager;
        private readonly IMonitor monitor;

        internal QuestApi(QuestManager baseManager, EventManager eventManager, QuestOfferManager scheduleManager, IMonitor monitor)
        {
            this.managers = new Dictionary<string, ManagedQuestApi>();
            this.baseManager = baseManager;
            this.scheduleManager = scheduleManager;
            this.monitor = monitor;
            this.Events = new QuestFrameworkEvents(eventManager);
        }

        public State Status => QuestFrameworkMod.Instance.Status;

        public IQuestFrameworkEvents Events { get; }

        public IManagedQuestApi GetManagedApi(IManifest manifest)
        {
            if (!this.managers.TryGetValue(manifest.UniqueID, out ManagedQuestApi proxy))
            {
                proxy = new ManagedQuestApi(manifest.UniqueID, this.baseManager, this.scheduleManager);
                this.managers.Add(manifest.UniqueID, proxy);
                this.monitor.Log($"Mod {manifest.UniqueID} requested scoped API.");
            }

            return proxy;
        }
    }
}
