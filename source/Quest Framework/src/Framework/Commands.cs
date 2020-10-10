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
using QuestFramework.Extensions;
using StardewModdingAPI;
using StardewValley;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace QuestFramework.Framework
{
    internal static class Commands
    {
        private static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;
        private static QuestManager QuestManager => QuestFrameworkMod.Instance.QuestManager;
        private static StatsManager StatsManager => QuestFrameworkMod.Instance.StatsManager;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        public static void ListQuests(string name, string[] args)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Quest framework has {QuestManager.Quests.Count} quests under management:");

            foreach (var quest in QuestManager.Quests)
            {
                builder.AppendLine(quest.GetFullName())
                    .AppendLine($"    Type: {quest.BaseType}")
                    .AppendLine($"    Custom type: {quest.CustomTypeId}")
                    .AppendLine($"    Name: {quest.Name}")
                    .AppendLine($"    Owned by: {quest.OwnedByModUid}")
                    .AppendLine($"    Current ID: {quest.id}")
                    .AppendLine($"    Trigger: {quest.Trigger ?? "null"}")
                    .AppendLine($"    Reward: {quest.Reward}g")
                    .AppendLine($"    Active: {(quest.id >= 0 ? "Yes" : "No")}")
                    .AppendLine($"    Is in quest log: {quest.IsInQuestLog()}")
                    .AppendLine($"    Next quests: {string.Join(", ", quest.NextQuests ?? new List<string>())}");
            }

            Monitor.Log(builder.ToString(), LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void ListLog(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't list quest log if the game is not loaded", LogLevel.Info);
                return;
            }

            var managedLog = Game1.player.questLog
                .Where(q => q.IsManaged())
                .Select(q => q.AsManagedQuest());
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Quest framework has {managedLog.Count()} managed quests in player's quest log:");

            foreach(var quest in managedLog)
            {
                builder.AppendLine(quest.GetFullName())
                    .AppendLine($"    Type: {quest.BaseType}")
                    .AppendLine($"    Custom type: {quest.CustomTypeId}")
                    .AppendLine($"    Name: {quest.Name}")
                    .AppendLine($"    Owned by: {quest.OwnedByModUid}")
                    .AppendLine($"    Current ID: {quest.id}")
                    .AppendLine($"    Trigger: {quest.Trigger ?? "null"}")
                    .AppendLine($"    Reward: {quest.Reward}g")
                    .AppendLine($"    Cancelable: {(quest.Cancelable ? "Yes" : "No")}")
                    .AppendLine($"    Completed: {(quest.GetInQuestLog().completed.Value ? "Yes" : "No")}")
                    .AppendLine($"    Next quests: {string.Join(", ", quest.NextQuests ?? new List<string>())}");
            }

            Monitor.Log(builder.ToString(), LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void QuestStats(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't show quest statistics if the game is not loaded", LogLevel.Info);
                return;
            }

            if (args.Length < 1)
            {
                Monitor.Log("Choose one of these statistics: accepted, completed, removed, summary, <fullQualifiedQuestName>", LogLevel.Info);
                return;
            }

            Stats.Stats stats = StatsManager.GetStats(Game1.player.UniqueMultiplayerID);
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"{args[0]} quest stats:");

            switch (args[0])
            {
                case "accepted":
                    builder.AppendLine($"Show {stats.AcceptedQuests.Count} accepted quests.");
                    builder.AppendLine();
                    stats.AcceptedQuests.ForEach(stat => builder.AppendLine($"{stat.FullQuestName}\t{stat.Date.ToLocaleString()}"));
                    break;
                case "completed":
                    builder.AppendLine($"Show {stats.CompletedQuests.Count} completed quests.");
                    builder.AppendLine();
                    stats.CompletedQuests.ForEach(stat => builder.AppendLine($"{stat.FullQuestName}\t{stat.Date.ToLocaleString()}"));
                    break;
                case "removed":
                    builder.AppendLine($"Show {stats.RemovedQuests.Count} removed quests.");
                    builder.AppendLine();
                    stats.RemovedQuests.ForEach(stat => builder.AppendLine($"{stat.FullQuestName}\t{stat.Date.ToLocaleString()}"));
                    break;
                case "summary":
                    builder.AppendLine($"{stats.AcceptedQuests.Count} accepted quests");
                    builder.AppendLine($"{stats.CompletedQuests.Count} completed quests");
                    builder.AppendLine($"{stats.RemovedQuests.Count} removed quests");
                    break;
                default:
                    if (QuestManager.Fetch(args[0]) == null)
                    {
                        builder.AppendLine($"`{args[0]}` is not known managed quest");
                        break;
                    }

                    var summary = stats.GetQuestStatSummary(args[0]);

                    builder.AppendLine($"Last accepted:                  {summary.LastAccepted?.ToLocaleString() ?? "never"}");
                    builder.AppendLine($"Last completed:                 {summary.LastCompleted?.ToLocaleString() ?? "never"}");
                    builder.AppendLine($"Last removed from quest log:    {summary.LastRemoved?.ToLocaleString() ?? "never"}");
                    builder.AppendLine();
                    builder.AppendLine($"{summary.AcceptalCount} times accepted");
                    builder.AppendLine($"{summary.CompletionCount} times completed");
                    builder.AppendLine($"{summary.RemovalCount} times removed from quest log");
                    break;
            }

            Monitor.Log(builder.ToString(), LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void AcceptQuest(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't accept quest if the game is not loaded", LogLevel.Info);
                return;
            }

            if (args.Length < 1)
            {
                Monitor.Log("Enter valid fullqualified quest name (<quest_name>@<mod_uid>).", LogLevel.Info);
                return;
            }

            if (QuestManager.Fetch(args[0]) == null)
            {
                Monitor.Log($"Unable to accept unknown quest `{args[0]}`.", LogLevel.Info);
                return;
            }

            QuestManager.AcceptQuest(args[0]);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void CompleteQuest(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't complete quest if the game is not loaded", LogLevel.Info);
                return;
            }

            if (args.Length < 1)
            {
                Monitor.Log("Enter valid fullqualified quest name (<quest_name>@<mod_uid>).", LogLevel.Info);
                return;
            }

            var customQuest = QuestManager.Fetch(args[0]);

            if (customQuest == null || !customQuest.IsInQuestLog())
            {
                Monitor.Log($"Unable to complete quest `{args[0]}` which is not accepted in player's questlog.", LogLevel.Info);
                return;
            }

            customQuest.Complete();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void RemoveQuest(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't remove quest from questlog if the game is not loaded", LogLevel.Info);
                return;
            }

            if (args.Length < 1)
            {
                Monitor.Log("Enter valid fullqualified quest name (<quest_name>@<mod_uid>).", LogLevel.Info);
                return;
            }

            var customQuest = QuestManager.Fetch(args[0]);

            if (customQuest == null || !customQuest.IsInQuestLog())
            {
                Monitor.Log($"Unable to remove quest `{args[0]}` which is not accepted in player's questlog.", LogLevel.Info);
                return;
            }

            customQuest.RemoveFromQuestLog();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void InvalidateCache(string name, string[] args)
        {
            QuestFrameworkMod.InvalidateCache();
            Monitor.Log("Quest assets cache invalidated.", LogLevel.Info);
        }
    }
}
