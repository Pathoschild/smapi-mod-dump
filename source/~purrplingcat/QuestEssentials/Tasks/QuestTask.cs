/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using QuestEssentials.Framework;
using QuestEssentials.Messages;
using QuestEssentials.Quests;
using QuestFramework.Extensions;
using StardewValley;
using System;
using System.Collections.Generic;

namespace QuestEssentials.Tasks
{
    [JsonConverter(typeof(QuestTaskConverter))]
    public abstract class QuestTask
    {
        internal static readonly Dictionary<string, Type> knownTypes;
        private SpecialQuest _quest;
        protected bool _complete;

        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> When { get; set; }
        public List<string> RequiredTasks { get; set; }
        public int Goal { get; set; } = 1;

        [JsonIgnore]
        public int CurrentCount
        {
            get
            {
                if (this._quest == null && this._quest.State != null)
                    return 0;

                if (!this._quest.State.progress.ContainsKey(this.Name))
                {
                    this._quest.State.progress[this.Name] = 0;
                    this._quest.Sync();
                }

                return this._quest.State.progress[this.Name];
            }
            set
            {
                if (this._quest != null && this._quest.State != null)
                {
                    if (this._quest.State.progress[this.Name] == value)
                        return;

                    this._quest.State.progress[this.Name] = value;
                    this._quest.Sync();
                    this.OnCurrentCountChanged();
                }
            }
        }

        static QuestTask()
        {
            knownTypes = new Dictionary<string, Type>();

            RegisterTaskType<BasicTask>("Basic");
            RegisterTaskType<EnterSpotTask>("EnterSpot");
            RegisterTaskType<CollectTask>("Collect");
            RegisterTaskType<CraftTask>("Craft");
            RegisterTaskType<DeliverTask>("Deliver");
            RegisterTaskType<TalkTask>("Talk");
            RegisterTaskType<SlayTask>("Slay");
            RegisterTaskType<FishTask>("Fish");
            RegisterTaskType<GiftTask>("Gift");
            RegisterTaskType<TileActionTask>("TileAction");
        }

        protected bool IsWhenMatched()
        {
            if (this._quest == null)
                return false;

            if (this.When == null || this.When.Count == 0)
                return true;

            return this._quest.CheckGlobalConditions(this.When);
        }

        public bool IsRegistered()
        {
            return this._quest != null && this._quest.State != null;
        }

        public bool IsActive()
        {
            if (!this.IsRegistered())
                return false;

            bool hasRequiredTasks = this.RequiredTasks != null && this.RequiredTasks.Count > 0;
            bool matchAllRequiredTasks = true;

            if (hasRequiredTasks)
            {
                foreach (string requiredTaskName in this.RequiredTasks)
                {
                    if (!this._quest.HasCompletedTask(requiredTaskName))
                        matchAllRequiredTasks = false;
                }
            }

            return !hasRequiredTasks || matchAllRequiredTasks;
        }

        public void IncrementCount(int amount)
        {
            int newCurrent = this.CurrentCount + amount;

            if (newCurrent > this.Goal)
                this.CurrentCount = this.Goal;
            else
                this.CurrentCount = newCurrent;
        }

        public bool IsCompleted()
        {
            return this._complete;
        }

        public void ForceComplete(bool playSound = true)
        {
            this.CurrentCount = this.Goal;
        }

        public virtual void Load()
        {
        }

        public virtual void Register(SpecialQuest quest)
        {
            if (quest.State == null)
            {
                quest.Reset();
            }

            this._quest = quest;
            this.Load();
            this.CheckCompletion(playSound: false);
        }

        protected virtual void OnCurrentCountChanged()
        {
            this.CheckCompletion();
        }

        protected virtual void OnTaskComplete()
        {
        }

        public abstract bool OnCheckProgress(IStoryMessage message);

        public virtual bool CheckCompletion(bool playSound = true)
        {
            if (!this.IsRegistered() || !this._quest.IsInQuestLogAndActive())
                return false;

            bool wasJustCompleted = false;

            if (this.CurrentCount >= this.Goal && !this.IsCompleted())
            {
                wasJustCompleted = true;
                this._complete = true;
                this.OnTaskComplete();
            }

            if (this._quest != null)
            {
                this._quest.CheckQuestCompletion();
                if (playSound && wasJustCompleted && !this._quest.State.complete)
                {
                    Game1.playSound("jingle1");
                }
            }

            return wasJustCompleted;
        }

        public virtual void DoAdjust(object toAdjust)
        {
        }

        public virtual bool ShouldShowProgress()
        {
            return true;
        }

        /// <summary>
        /// Register a class for deserialize for specified type name
        /// Unknown type names are deserialized as <see cref="QuestTask"/> class type.
        /// </summary>
        /// <typeparam name="T">Task class type</typeparam>
        /// <param name="name">Name of task type</param>
        public static void RegisterTaskType<T>(string name) where T : QuestTask
        {
            knownTypes.Add(name, typeof(T));
        }

        public static bool IsKnownTaskType(string name)
        {
            return knownTypes.ContainsKey(name);
        }

        public static bool IsKnownTaskType(Type type)
        {
            return knownTypes.ContainsValue(type);
        }

        public static bool IsKnownTaskType<T>() where T : QuestTask
        {
            return knownTypes.ContainsValue(typeof(T));
        }
    }

    public abstract class QuestTask<T> : QuestTask
    {
        public T Data { get; set; }
    }
}
