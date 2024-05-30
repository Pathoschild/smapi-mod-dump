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
using DeluxeJournal.Task.Tasks;

namespace DeluxeJournal.Task
{
    public static class TaskRegistry
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

        /// <summary>Basic task factory instance.</summary>
        public static readonly TaskFactory BasicFactory = new BasicFactory<BasicTask>();

        /// <summary>Number of registered tasks.</summary>
        public static int Count => Registry.Count;

        /// <summary>All registered task IDs.</summary>
        public static IEnumerable<string> Keys => Registry.Keys;

        /// <summary>All registered task IDs in order of priority.</summary>
        public static IEnumerable<string> PriorityOrderedKeys => Registry.Keys.OrderByDescending(id => Registry[id].Priority);

        /// <summary>Task registry map.</summary>
        private static readonly IDictionary<string, RegistryEntry> Registry = new Dictionary<string, RegistryEntry>();

        /// <summary>Monitor for logging messages.</summary>
        private static IMonitor? Monitor => DeluxeJournalMod.Instance?.Monitor;

        /// <summary>Register a new task.</summary>
        public static void Register(string id, Type taskType, Type factoryType, TaskIcon icon, int priority = 0)
        {
            if (Registry.ContainsKey(id))
            {
                throw new InvalidOperationException("Attempted to overwrite task registry entry with key \"" + id + "\".");
            }

            Registry.Add(id, new RegistryEntry(taskType, factoryType, icon, priority));
        }

        /// <summary>Get the class type of a task for a given ID, or a default ID if not found.</summary>
        /// <param name="id">Registered task ID.</param>
        /// <param name="defaultId">Default registered task ID.</param>
        /// <exception cref="KeyNotFoundException">Neither the <paramref name="id"/> nor <paramref name="defaultId"/> are registered.</exception>
        public static Type GetTaskTypeOrDefault(string id, string defaultId = "basic")
        {
            if (!Registry.ContainsKey(id))
            {
                Monitor?.Log(string.Format("Task with ID \"{0}\" not found in the registry. Defaulting to \"{1}\"", id, defaultId), LogLevel.Warn);
                return GetTaskType(defaultId);
            }

            return GetTaskType(id);
        }

        /// <summary>Get the task type for a registered task ID.</summary>
        /// <param name="id">Registered task ID.</param>
        /// <exception cref="KeyNotFoundException">The task with <paramref name="id"/> was not registered.</exception>
        public static Type GetTaskType(string id)
        {
            return Registry[id].TaskType;
        }

        /// <summary>Get the task factory type for a registered task ID.</summary>
        /// <param name="id">Registered task ID.</param>
        /// <exception cref="KeyNotFoundException">The task with <paramref name="id"/> was not registered.</exception>
        public static Type GetFactoryType(string id)
        {
            return Registry[id].FactoryType;
        }

        /// <summary>Get the task icon for a registered task ID.</summary>
        /// <param name="id">Registered task ID.</param>
        /// <exception cref="KeyNotFoundException">The task with <paramref name="id"/> was not registered.</exception>
        public static TaskIcon GetTaskIcon(string id)
        {
            return Registry[id].Icon;
        }

        /// <summary>Create a factory instance from a registered task ID.</summary>
        /// <param name="id">Registered task ID.</param>
        /// <exception cref="KeyNotFoundException">The task with <paramref name="id"/> was not registered.</exception>
        /// <exception cref="InvalidOperationException">The factory could not be instantiated.</exception>
        public static TaskFactory CreateFactoryInstance(string id)
        {
            if (id == TaskTypes.Basic)
            {
                return BasicFactory;
            }
            
            if (Activator.CreateInstance(Registry[id].FactoryType) is not TaskFactory factory)
            {
                throw new InvalidOperationException("Unable to instantiate TaskFactory: " + Registry[id].FactoryType.FullName);
            }

            return factory;
        }
    }
}
