/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Tasks
{
    internal class TaskRegistry
    {
        private struct RegistryEntry
        {
            public Type TaskType { get; }

            public Type FactoryType { get; }

            public TaskIcon Icon { get; }

            public int Priority { get; }

            public RegistryEntry(Type taskType, Type factoryType, TaskIcon icon, int priority)
            {
                TaskType = taskType;
                FactoryType = factoryType;
                Icon = icon;
                Priority = priority;
            }
        }

        private static readonly IDictionary<string, RegistryEntry> Registry = new Dictionary<string, RegistryEntry>();

        private static IMonitor? Monitor => DeluxeJournalMod.Instance?.Monitor;

        /// <summary>All registered task IDs.</summary>
        public static IEnumerable<string> Keys => Registry.Keys;

        /// <summary>All registered task IDs in order of priority.</summary>
        public static IEnumerable<string> PriorityOrderedKeys => Registry.Keys.OrderByDescending(id => Registry[id].Priority);

        /// <summary>Register a new task.</summary>
        public static void Register(string id, Type taskType, Type factoryType, TaskIcon icon, int priority = 0)
        {
            if (Registry.ContainsKey(id))
            {
                throw new InvalidOperationException("Attempted to overwrite task registry entry with key \"" + id + "\".");
            }

            Registry.Add(id, new RegistryEntry(taskType, factoryType, icon, priority));
        }

        public static Type GetTaskTypeOrDefault(string id, string defaultID = "basic")
        {
            if (!Registry.ContainsKey(id))
            {
                Monitor?.Log(string.Format("Task with ID \"{0}\" not found in the registry. Defaulting to \"{1}\"", id, defaultID), LogLevel.Warn);
                return GetTaskType(defaultID);
            }

            return GetTaskType(id);
        }

        public static Type GetTaskType(string id)
        {
            return Registry[id].TaskType;
        }

        public static Type GetFactoryType(string id)
        {
            return Registry[id].FactoryType;
        }

        public static TaskIcon GetTaskIcon(string id)
        {
            return Registry[id].Icon;
        }

        public static DeluxeJournal.Tasks.TaskFactory CreateFactoryInstance(string id)
        {
            if (Activator.CreateInstance(Registry[id].FactoryType) is not DeluxeJournal.Tasks.TaskFactory factory)
            {
                throw new InvalidOperationException("Unable to instantiate TaskFactory: " + Registry[id].FactoryType.FullName);
            }

            return factory;
        }
    }
}
