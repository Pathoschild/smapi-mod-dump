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
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Serialization
{
    internal class TaskDataConverter : JsonConverter<TaskData>
    {
        public override bool CanWrite => false;

        private static IList<ITask> DeserializeTasks(JArray taskArray, ISemanticVersion version)
        {
            IList<ITask> tasks = new List<ITask>();
            TaskDataMigrator? migrator = null;
            bool version_pre_1_2 = version.IsOlderThan(new SemanticVersion(1, 2, 0));

            if (version_pre_1_2 && DeluxeJournalMod.Translation is ITranslationHelper translation)
            {
                migrator = new TaskDataMigrator(translation);
            }

            foreach (JObject taskObject in taskArray)
            {
                if (!taskObject.Remove("ID", out JToken? token) || token?.Value<string>() is not string id)
                {
                    throw new JsonReaderException("No 'ID' property of type string in ITask JSON object");
                }

                Type taskType = TaskRegistry.GetTaskTypeOrDefault(id);
                ITask? task = null;

                if (!version_pre_1_2)
                {
                    task = taskObject.ToObject(taskType) as ITask;
                }
                else if (migrator != null)
                {
                    try
                    {
                        migrator.Migrate_1_0(taskObject, taskType, out task);
                    }
                    catch (JsonReaderException ex)
                    {
                        if (DeluxeJournalMod.Instance?.Monitor is IMonitor monitor)
                        {
                            monitor.Log($"Exception caught during task migration! Type-specific data may be lost.", LogLevel.Warn);
                            monitor.Log($"\t{ex.Message}", LogLevel.Warn);
                        }
                    }
                }

                if (task != null || (task = taskObject.ToObject(typeof(BasicTask)) as ITask) != null)
                {
                    tasks.Add(task);
                }
                else
                {
                    throw new JsonReaderException("Unable to deserialize ITask");
                }
            }

            return tasks;
        }

        public override TaskData ReadJson(JsonReader reader, Type objectType, TaskData? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            IDictionary<string, IDictionary<long, IList<ITask>>> map = new Dictionary<string, IDictionary<long, IList<ITask>>>();
            IDictionary<long, IList<ITask>> deserializedTasks;
            JObject taskDataObject = JObject.Load(reader);
            ISemanticVersion version = new SemanticVersion(taskDataObject["Version"]?.ToObject<string>() ?? TaskData.DefaultVersion);

            if (taskDataObject["Tasks"] is not JObject json)
            {
                throw new JsonReaderException("Object with key 'Tasks' not found");
            }

            foreach (var save in json)
            {
                // Legacy versions (<= 1.0.3) do not have a UMID mapping for each (local) player's task list
                if (save.Value is JArray legacyTaskArray)
                {
                    map.Add(save.Key, new Dictionary<long, IList<ITask>>()
                    {
                        { 0, DeserializeTasks(legacyTaskArray, version) }
                    });
                }
                else if (save.Value is JObject playerTasksObject)
                {
                    deserializedTasks = new Dictionary<long, IList<ITask>>();

                    foreach (var playerTasks in playerTasksObject)
                    {
                        if (playerTasks.Value is JArray taskArray && long.TryParse(playerTasks.Key, out long umid))
                        {
                            deserializedTasks.Add(umid, DeserializeTasks(taskArray, version));
                        }
                    }

                    map.Add(save.Key, deserializedTasks);
                }
            }

            return new TaskData() { Version = version.ToString(), Tasks = map };
        }

        public override void WriteJson(JsonWriter writer, TaskData? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
