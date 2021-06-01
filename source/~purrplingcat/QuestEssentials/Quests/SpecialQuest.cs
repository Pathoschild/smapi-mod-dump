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
using QuestFramework.Quests;
using QuestFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using System.Text;
using QuestEssentials.Messages;
using QuestEssentials.Tasks;

namespace QuestEssentials.Quests
{
    public class SpecialQuest : CustomQuest<SpecialQuest.StoryQuestState>
    {
        private bool _taskRegistrationDirty;

        public class StoryQuestState
        {
            public bool complete = false;
            public Dictionary<string, int> progress;

            public StoryQuestState()
            {
                this.progress = new Dictionary<string, int>();
            }
        }

        public List<QuestTask> Tasks { get; set; }

        private void OnAccepted(object sender, IQuestInfo e)
        {
            this._taskRegistrationDirty = true;
        }

        private void UpdateTaskRegistration()
        {
            this.Tasks.ForEach(t => t.Register(this));
        }

        private void OnGameUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this._taskRegistrationDirty)
            {
                this._taskRegistrationDirty = false;
                this.UpdateTaskRegistration();
            }

            if (Context.CanPlayerMove && this.State.complete && this.IsInQuestLogAndActive())
            {
                this.Complete();
            }
        }

        protected override void OnInitialize()
        {
            this.Accepted += this.OnAccepted;
            QuestEssentialsMod.ModHelper.Events.GameLoop.UpdateTicked += this.OnGameUpdateTicked;
            base.OnInitialize();
        }

        protected override void Dispose(bool disposing)
        {
            this.Accepted -= this.OnAccepted;
            QuestEssentialsMod.ModHelper.Events.GameLoop.UpdateTicked -= this.OnGameUpdateTicked;
            base.Dispose(disposing);
        }

        protected override void OnStateRestored()
        {
            this._taskRegistrationDirty = true;
            base.OnStateRestored();
        }

        public override void OnRegister()
        {
            this._taskRegistrationDirty = true;
            base.OnRegister();
        }

        public override bool OnCompletionCheck(ICompletionMessage completionMessage)
        {
            if (completionMessage is IStoryMessage storyMessage)
            {
                return this.CheckTasks(storyMessage);
            } 
            else if (completionMessage is ICompletionArgs completionArgs)
            {
                return this.CheckTasks(new VanillaCompletionMessage(completionArgs));
            }

            return false;
        }

        public override void OnAdjust(object toAdjust)
        {
            if (this.Tasks == null)
                return;

            foreach (var task in this.Tasks)
            {
                if (!task.IsRegistered() && !task.IsActive())
                    continue;

                task.DoAdjust(toAdjust);
            }

        }

        protected override void UpdateCurrentObjectives(List<CustomQuestObjective> currentObjectives)
        {
            currentObjectives.Clear();

            if (this.Tasks != null) {
                foreach (var task in this.Tasks)
                {
                    if (!task.IsActive())
                        continue;

                    StringBuilder text = new StringBuilder(task.Description);

                    if (task.ShouldShowProgress() && task.Goal > 1)
                    {
                        text.Append($" ({task.CurrentCount}/{task.Goal})");
                    }

                    currentObjectives.Add(new CustomQuestObjective(task.Name, text.ToString())
                    {
                        IsCompleted = task.IsCompleted()
                    });
                }
            }
        }

        public bool HasCompletedTask(string requiredTaskName)
        {
            return this.Tasks.Any(t => t.Name == requiredTaskName && t.IsCompleted());
        }

        public bool IsInQuestLogAndActive()
        {
            return this.IsInQuestLog() && !this.GetInQuestLog().completed.Value;
        }

        public void CheckQuestCompletion()
        {
            this.State.complete = this.HasAllTasksCompleted();
        }

        public bool HasAllTasksCompleted()
        {
            return !this.Tasks.Any(t => t.IsRegistered() && t.IsActive() && !t.IsCompleted());
        }

        private bool CheckTasks(IStoryMessage storyMessage)
        {
            if (this.Tasks == null || !this.IsInQuestLogAndActive())
                return false;

            bool worked = false;
            foreach (var task in this.Tasks)
            {
                if (!task.IsRegistered() || !task.IsActive())
                    continue;

                worked |= task.OnCheckProgress(storyMessage);
            }

            return worked;
        }
    }
}
