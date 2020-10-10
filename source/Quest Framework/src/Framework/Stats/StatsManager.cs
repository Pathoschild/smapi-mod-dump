/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Stats
{
    internal class StatsManager
    {
        private readonly IMultiplayerHelper multiplayer;
        private readonly IDataHelper data;
        private Dictionary<long, Stats> stats;

        private static long PlayerId => Game1.player.UniqueMultiplayerID;

        public StatsManager(IMultiplayerHelper multiplayer, IDataHelper data)
        {
            this.multiplayer = multiplayer;
            this.data = data;
            this.stats = new Dictionary<long, Stats>();
        }

        public void SetStats(long playerId, Stats stats)
        {
            this.stats[playerId] = stats;
        }

        public void SetStats(Dictionary<long, Stats> stats)
        {
            this.stats = stats;
        }

        public Stats GetStats(long playerId)
        {
            if (!this.stats.ContainsKey(playerId))
            {
                this.SetStats(playerId, new Stats());
            }

            return this.stats[playerId];
        }

        public Dictionary<long, Stats> GetStats()
        {
            return this.stats;
        }

        public void LoadStats()
        {
            if (!Context.IsMainPlayer)
                return;

            this.stats = this.data.ReadSaveData<Dictionary<long, Stats>>("questStats") 
                ?? new Dictionary<long, Stats>();
        }

        public void SaveStats()
        {
            if (!Context.IsMainPlayer)
                return;

            this.data.WriteSaveData("questStats", this.stats);
        }

        public void AddCompletedQuest(string questName)
        {
            this.AddQuestStat(
                questName, this.GetStats(PlayerId).CompletedQuests);
        }

        public void AddAcceptedQuest(string questName)
        {
            this.AddQuestStat(
                questName, this.GetStats(PlayerId).AcceptedQuests);
        }

        public void AddRemovedQuest(string questName)
        {
            this.AddQuestStat(
                questName, this.GetStats(PlayerId).RemovedQuests);
        }

        public void Clean()
        {
            this.stats = new Dictionary<long, Stats>();
        }

        private void Sync()
        {
            if (Context.IsMultiplayer && this.stats.ContainsKey(PlayerId))
            {
                this.multiplayer.SendMessage(
                    this.stats[PlayerId], "questStats", new[] { this.multiplayer.ModID });
            }
        }

        private void AddQuestStat(string questName, List<QuestStat> statStore)
        {
            if (!Context.IsWorldReady)
                return;

            statStore.Add(
                new QuestStat(questName, SDate.Now().DaysSinceStart));
            this.Sync();
        }
    }
}
