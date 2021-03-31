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
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    internal class QuestManager
    {
        public const int ID_ROOT = 9000;
        private readonly IMonitor monitor;
        private readonly PerScreen<List<CustomQuest>> _quests;

        public List<CustomQuest> Quests => this._quests.Value;
        public Dictionary<string, Func<CustomQuest>> Factories { get; }

        public QuestManager(IMonitor monitor)
        {
            this.monitor = monitor;
            this._quests = new PerScreen<List<CustomQuest>>(() => new List<CustomQuest>());
            this.Factories = new Dictionary<string, Func<CustomQuest>>();
        }

        public void RegisterQuest(CustomQuest quest)
        {
            if (QuestFrameworkMod.Instance.Status != State.LAUNCHING)
            {
                throw new InvalidOperationException($"Cannot register new quest when in state `{QuestFrameworkMod.Instance.Status}`.");
            }

            if (string.IsNullOrEmpty(quest.Name) || string.IsNullOrEmpty(quest.OwnedByModUid))
            {
                throw new InvalidQuestException($"Quest name and category can't be empty or null!");
            }

            if (quest.Name.Contains(' ') || quest.Name.Contains('@'))
            {
                throw new InvalidQuestException("Quest name contains illegal characters (spaces or these reserved characters: @)");
            }

            if (this.Quests.Any(q => q.GetFullName() == quest.GetFullName()))
            {
                throw new InvalidQuestException($"Quest `{quest.GetFullName()}` is already registered!");
            }

            this.Quests.Add(quest);
            this.monitor.Log($"Added quest `{quest.Name}` to quest manager");
        }

        public void AcceptQuest(string fullQuestName, bool silent = false)
        {
            int id = this.ResolveGameQuestId(fullQuestName);

            if (!Context.IsWorldReady)
            {
                throw new InvalidOperationException("Unable to add quest to quest log if game is not loaded!");
            }

            if (id < 0)
            {
                throw new InvalidQuestException($"Unable to add unknown quest `{fullQuestName}` to player's quest log.");
            }

            if (!Game1.player.hasQuest(id)) {
                if (silent)
                    Game1.player.AddQuestQuiet(id);
                else
                    Game1.player.addQuest(id);

                this.monitor.Log($"Quest `{fullQuestName}` #{id} {(silent ? "silently " : "")}added to the quest log.");
            } else
            {
                this.monitor.Log($"Quest `{fullQuestName}` #{id} is already in quest log!");
            }
        }

        public void AcceptQuest(int id)
        {
            if (!Game1.player.hasQuest(id))
            {
                Game1.player.addQuest(id);
            }
            else
            {
                this.monitor.Log($"Quest `{this.GetById(id)?.GetFullName() ?? id.ToString()}` #{id} is already in quest log!");
            }
        }

        public void RegisterQuestFactory<T>(string name, Func<T> factory) where T : CustomQuest
        {
            if (this.Factories.ContainsKey(name))
                throw new InvalidOperationException($"Quest factory {name} is already registered!");

            this.Factories.Add(name, factory);
        }

        public bool IsManaged(int id)
        {
            if (id < 0)
                return false;

            return this.Quests.Any(q => q.id == id);
        }

        public CustomQuest GetById(int id)
        {
            if (id < 0)
                return null;

            return this.Quests.Where(q => q.id == id).SingleOrDefault();
        }

        public void Update()
        {
            foreach (var customQuest in this.Quests.Where(q => q.NeedsUpdate))
            {
                customQuest.Update();
                customQuest.NeedsUpdate = false;
            }
        }

        internal int ResolveGameQuestId(string fullName)
        {
            var quest = this.Fetch(fullName);

            if (quest == null)
                return -1;

            return quest.id;
        }

        internal CustomQuest Fetch(string fullName)
        {
            var quests = from q in this.Quests
                         where q.GetFullName() == fullName
                         select q;

            if (!quests.Any())
                return null;

            return quests.First();
        }

        internal CustomQuest CreateQuestOfType(string type)
        {
            if (!this.Factories.ContainsKey(type))
                throw new InvalidQuestException($"Quest type factory {type} doesn't exist!");

            CustomQuest quest = this.Factories[type]();

            if (quest.Name != null)
                throw new InvalidQuestException("Quest name is unexpectedly assigned by factory!");

            return quest;
        }
    }
}
