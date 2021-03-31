/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework.Stats;
using QuestFramework.Hooks;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Structures;
using Newtonsoft.Json;
using System.Linq;
using QuestFramework.Extensions;
using QuestFramework.Framework.Helpers;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Custom quest definition
    /// </summary>
    public class CustomQuest
    {
        private int _customTypeId;
        private string _trigger;
        private string _name;
        private readonly CustomQuestObjective _defaultObjective;

        internal int id = -1;
        private List<CustomQuestObjective> _currentObjectives;

        public event EventHandler<IQuestInfo> Completed;
        public event EventHandler<IQuestInfo> Accepted;
        public event EventHandler<IQuestInfo> Removed;

        [JsonIgnore]
        internal bool NeedsUpdate { get; set; }
        public string OwnedByModUid { get; internal set; }
        public QuestType BaseType { get; set; } = QuestType.Basic;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public List<string> NextQuests { get; set; }
        public int Reward { get; set; }
        public RewardType RewardType { get; set; } = RewardType.Money;
        public int RewardAmount { get; set; }
        public string RewardDescription { get; set; }
        public bool Cancelable { get; set; }
        public string ReactionText { get; set; }
        public int DaysLeft { get; set; } = 0;
        public Texture2D Texture { get; set; }
        public QuestLogColors Colors { get; set; }
        public List<Hook> Hooks { get; set; }
        public Dictionary<string, int> FriendshipGain { get; }
        public Dictionary<string, string> Tags { get; }

        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != null)
                    throw new InvalidOperationException("Quest name can't be changed!");

                this._name = value;
            }
        }

        public string Trigger 
        {
            get => this._trigger;
            set
            {
                if (this is ITriggerLoader triggerLoader)
                {
                    triggerLoader.LoadTrigger(value);
                }

                this._trigger = value;
            }
        }

        public bool IsDailyQuest()
        {
            return this.DaysLeft > 0;
        }

        public int CustomTypeId 
        { 
            get => this.BaseType == QuestType.Custom ? this._customTypeId : -1; 
            set => this._customTypeId = value >= 0 ? value : 0; 
        }

        internal protected static IModHelper Helper => QuestFrameworkMod.Instance.Helper;
        internal protected static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;
        private static StatsManager StatsManager => QuestFrameworkMod.Instance.StatsManager;

        public List<CustomQuestObjective> Objectives { get; }

        public CustomQuest()
        {
            this.BaseType = QuestType.Custom;
            this.NextQuests = new List<string>();
            this.Hooks = new List<Hook>();
            this.Objectives = new List<CustomQuestObjective>();
            this.FriendshipGain = new Dictionary<string, int>();
            this.Tags = new Dictionary<string, string>();
            this._defaultObjective = new CustomQuestObjective("default", "");
            this._currentObjectives = new List<CustomQuestObjective>();
        }

        public CustomQuest(string name) : this()
        {
            this.Name = name;
        }

        internal void ConfirmComplete(IQuestInfo questInfo)
        {
            this.GainFriendshipPoints();
            StatsManager.AddCompletedQuest(this.GetFullName());
            this.Completed?.Invoke(this, questInfo);
        }

        private void GainFriendshipPoints()
        {
            NPC npc;

            foreach (var friendship in this.FriendshipGain)
            {
                npc = Game1.getCharacterFromName(friendship.Key);

                if (npc != null)
                {
                    Game1.player.changeFriendship(friendship.Value, npc);
                }
            }
        }

        internal void ConfirmAccept(IQuestInfo questInfo)
        {
            StatsManager.AddAcceptedQuest(this.GetFullName());
            this.Accepted?.Invoke(this, questInfo);
        }

        internal void ConfirmRemove(IQuestInfo questInfo)
        {
            StatsManager.AddRemovedQuest(this.GetFullName());
            this.Removed?.Invoke(this, questInfo);
        }

        /// <summary>
        /// Update quest when it needs update 
        /// (NeedsUpdate field is set to TRUE)
        /// </summary>
        internal virtual void Update()
        {
        }

        /// <summary>
        /// Reset this managed quest and their state.
        /// Primarily called before accept quest and after remove quest from quest log.
        /// If you override this method, be sure to call <code>base.Reset()</code>.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Get full quest name in format {questName}@{ownerModUniqueId}
        /// </summary>
        public string GetFullName()
        {
            return $"{this.Name}@{this.OwnedByModUid}";
        }

        /// <summary>
        /// Cast this custom quest as statefull custom quest if this quest contains state.
        /// </summary>
        /// <returns>A statefull custom quest or null if this quest not contains or hasn't defined type of state</returns>
        public IStatefull AsStatefull()
        {
            return this as IStatefull;
        }

        /// <summary>
        /// Cast this custom quest as statefull custom quest with specific state type.
        /// </summary>
        /// <typeparam name="TState">Type of state</typeparam>
        /// <returns>A statefull custom quest with specific type of state or null when quest has no state or doesn't match state type.</returns>
        public CustomQuest<TState> AsStatefull<TState>() where TState : class, new()
        {
            return this as CustomQuest<TState>;
        }

        public void UpdateCurrentObjectives()
        {
            this._currentObjectives.Clear();

            if (!this.IsInQuestLog())
                return;

            var questance = this.GetInQuestLog();

            if (!string.IsNullOrEmpty(questance?.currentObjective))
            {
                this._defaultObjective.Text = questance.currentObjective ?? "";
                this._currentObjectives.Add(this._defaultObjective);
            }

            if (this.Objectives.Any())
            {
                this._currentObjectives.AddRange(
                    this.Objectives.Select(o => Utils.Clone(o)));
            }

            this.UpdateCurrentObjectives(this._currentObjectives);
        }

        protected virtual void UpdateCurrentObjectives(List<CustomQuestObjective> currentObjectives)
        {
        }

        public List<CustomQuestObjective> GetCurrentObjectives()
        {
            return this._currentObjectives;
        }

        internal string NormalizeName(string name)
        {
            if (name.Contains("@"))
                return name;

            return $"{name}@{this.OwnedByModUid}";
        }
    }
}
