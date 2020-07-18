using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework
{
    internal static class Commands
    {
        private static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;
        private static QuestManager QuestManager => QuestFrameworkMod.Instance.QuestManager;

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
                    .AppendLine($"    Active: {(quest.id >= 0 ? "Yes" : "No")}");
            }

            Monitor.Log(builder.ToString(), LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Styl", "IDE0060", Justification = "Command handler")]
        internal static void ListLog(string arg1, string[] arg2)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Can't list quest log if the game is not loaded", LogLevel.Info);
                return;
            }

            var managedLog = Game1.player.questLog
                .Where(q => QuestManager.IsManaged(q.id.Value))
                .Select(q => QuestManager.GetById(q.id.Value));
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
                    .AppendLine($"    Cancelable: {quest.Cancelable}");
            }

            Monitor.Log(builder.ToString(), LogLevel.Info);
        }
    }
}
