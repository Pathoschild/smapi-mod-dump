/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Tools;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Framework.Serialization
{
    internal class TaskDataMigrator
    {
        private LocalizedGameDataMaps LocalizedGameData { get; }

        public TaskDataMigrator(ITranslationHelper translation)
        {
            LocalizedGameData = new LocalizedGameDataMaps(translation);
        }

        /// <summary>Migrate task from data versions 1.0.x.</summary>
        /// <remarks>The <see cref="JObject"/> is modified during the migration.</remarks>
        /// <param name="taskJson">JSON object containing the task properties.</param>
        /// <param name="taskType">The type of task that should be instantiated using the JSON data.</param>
        /// <param name="task">Task instance.</param>
        /// <returns><c>true</c> if a valid task was produced; <c>false</c> if the operation was aborted due to insufficient or incorrect JSON data.</returns>
        /// <exception cref="JsonReaderException">Migrated task data could not be deserialized.</exception>
        public bool Migrate_1_0(JObject taskJson, Type taskType, out ITask? task)
        {
            string? targetDisplayName = taskJson["TargetDisplayName"]?.Value<string>();
            string? targetName = taskJson["TargetName"]?.Value<string>();
            int? targetIndex = taskJson["TargetIndex"]?.Value<int>();
            task = null;

            if (targetDisplayName == null || targetName == null || targetIndex == null)
            {
                return false;
            }

            if (taskType == typeof(BlacksmithTask))
            {
                if (!LocalizedGameData.LocalizedItems.TryGetValue(targetDisplayName, out var itemId))
                {
                    return false;
                }

                taskJson.Add(nameof(BlacksmithTask.ItemId), itemId);

                if (ItemRegistry.GetData(itemId)?.RawData is ToolData toolData)
                {
                    taskJson.Add(nameof(BlacksmithTask.ToolType), toolData.ClassName);
                    taskJson.Add(nameof(BlacksmithTask.UpgradeLevel), toolData.UpgradeLevel);
                }
            }
            else if (taskType == typeof(BuildTask))
            {
                if (!LocalizedGameData.LocalizedBuildings.TryGetValue(targetDisplayName, out var buildingType))
                {
                    return false;
                }

                taskJson.Add(nameof(BuildTask.BuildingType), buildingType);
            }
            else if (taskType == typeof(BuyTask)
                || taskType == typeof(CollectTask)
                || taskType == typeof(CraftTask)
                || taskType == typeof(SellTask))
            {
                if (!LocalizedGameData.LocalizedItems.TryGetValues(targetDisplayName, out var itemIds))
                {
                    return false;
                }

                taskJson.Add("ItemIds", JArray.FromObject(itemIds));
            }
            else if (taskType == typeof(GiftTask))
            {
                if (!LocalizedGameData.LocalizedNpcs.TryGetValue(targetName, out var npcName))
                {
                    return false;
                }

                if (targetIndex > 0)
                {
                    taskJson.Add(nameof(GiftTask.ItemIds), JArray.FromObject(new[] { ItemRegistry.type_object + targetIndex }));
                }

                taskJson.Add(nameof(GiftTask.NpcName), npcName);
            }

            if (taskJson.ToObject(taskType) is ITask deserializedTask)
            {
                task = deserializedTask;
                return true;
            }
            else
            {
                throw new JsonReaderException($"{nameof(Migrate_1_0)}: Unable to deserialize task of type {taskType.Name}.");
            }
        }
    }
}
