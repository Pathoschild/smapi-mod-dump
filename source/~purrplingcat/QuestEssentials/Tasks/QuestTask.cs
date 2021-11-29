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
        private static readonly Dictionary<string, Type> _knownTypes;
        private AdventureQuest _quest;
        protected bool _complete;

        /// <summary>
        /// Unique task name (in the quest scope)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Task type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Task description. Visible in quest objective
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Pre-conditions must be matched for successful completion of this task
        /// </summary>
        public Dictionary<string, string> When { get; set; }

        /// <summary>
        /// Other tasks must be completed to activate this task. 
        /// If some tasks defined here are not completed, this task is not shown
        /// in the quest objective list and can't be completed.
        /// </summary>
        public List<string> RequiredTasks { get; set; }

        /// <summary>
        /// How much pieces of subject of this task (by type) must be reached to complete this task.
        /// </summary>
        public int Count { get; set; } = 1;
        
        /// <summary>
        /// Alias for count (for keep it compatible with QE beta content packs)
        /// </summary>
        public int Goal
        {
            get => this.Count;
            set => this.Count = value;
        }

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
            _knownTypes = new Dictionary<string, Type>();

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

            if (newCurrent > this.Count)
                this.CurrentCount = this.Count;
            else
                this.CurrentCount = newCurrent;
        }

        public bool IsCompleted()
        {
            return this._complete;
        }

        public void ForceComplete(bool playSound = true)
        {
            this.CurrentCount = this.Count;
        }

        public virtual void Load()
        {
        }

        public virtual void Register(AdventureQuest quest)
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

            if (this.CurrentCount >= this.Count && !this.IsCompleted())
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
            _knownTypes.Add(name, typeof(T));
        }

        public static bool IsKnownTaskType(string name)
        {
            return _knownTypes.ContainsKey(name);
        }

        public static bool IsKnownTaskType(Type type)
        {
            return _knownTypes.ContainsValue(type);
        }

        public static bool IsKnownTaskType<T>() where T : QuestTask
        {
            return _knownTypes.ContainsValue(typeof(T));
        }

        public static Type GetTaskType(string typeName)
        {
            return _knownTypes[typeName];
        }
    }

    public abstract class QuestTask<T> : QuestTask
    {
        public T Data { get; set; }
    }
}
