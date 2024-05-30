/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Task
{
    internal class TaskManager
    {
        private readonly ITaskEvents _events;
        private readonly IDataHelper _dataHelper;
        private readonly Config _config;
        private readonly ISemanticVersion _version;
        private readonly IDictionary<long, TaskList> _tasks;
        private TaskData? _data;

        /// <summary>A list of tasks for the active player.</summary>
        public IList<ITask> Tasks
        {
            get
            {
                if (!_tasks.ContainsKey(Game1.player.UniqueMultiplayerID))
                {
                    _tasks[Game1.player.UniqueMultiplayerID] = new TaskList(_events);
                }

                return _tasks[Game1.player.UniqueMultiplayerID];
            }
        }

        /// <summary>Task save data.</summary>
        private TaskData Data
        {
            get
            {
                // Delayed deserialization to allow for file migration using localized game data.
                if (_data == null)
                {
                    _data = _dataHelper.ReadGlobalData<TaskData>(DeluxeJournalMod.TASKS_DATA_KEY) ?? new TaskData(_version);
                }

                return _data;
            }
        }

        public TaskManager(ITaskEvents events, IDataHelper dataHelper, Config config, ISemanticVersion version)
        {
            _events = events;
            _dataHelper = dataHelper;
            _config = config;
            _version = version;
            _tasks = new Dictionary<long, TaskList>();

            _events.ModEvents.GameLoop.DayStarted += OnDayStarted;
            _events.ModEvents.GameLoop.DayEnding += OnDayEnding;
        }

        /// <summary>Sort local player tasks.</summary>
        public void SortTasks()
        {
            ((TaskList)Tasks).Sort();
        }

        /// <summary>Load the task list from save data.</summary>
        public void Load()
        {
            long umid;

            // Each TaskList must be cleared in order to unsubscribe from task events
            foreach (TaskList tasks in _tasks.Values)
            {
                tasks.Clear();
            }

            _tasks.Clear();

            if (Constants.SaveFolderName is string saveFolderName && Data.Tasks.ContainsKey(saveFolderName))
            {
                foreach (long key in Data.Tasks[saveFolderName].Keys)
                {
                    // UMID is set to 0 when data is converted from legacy versions (<= 1.0.3)
                    umid = (key == 0) ? Game1.player.UniqueMultiplayerID : key;

                    if (!_tasks.ContainsKey(umid))
                    {
                        _tasks[umid] = new TaskList(_events);
                    }

                    foreach (ITask task in Data.Tasks[saveFolderName][key])
                    {
                        task.OwnerUMID = umid;
                        _tasks[umid].Add(task.Copy());
                    }

                    _tasks[umid].Sort();
                }
            }
        }

        /// <summary>Save the task list.</summary>
        public void Save()
        {
            if (Constants.SaveFolderName is string saveFolderName)
            {
                Data.Version = _version.ToString();
                Data.Tasks[saveFolderName] = _tasks
                    .Where(entry => entry.Value.Count > 0)
                    .ToDictionary(entry => entry.Key, entry => (IList<ITask>)entry.Value.ToList());

                _dataHelper.WriteGlobalData(DeluxeJournalMod.TASKS_DATA_KEY, Data);
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            for (int i = Tasks.Count - 1, pushed = 0; i >= pushed; i--)
            {
                ITask task = Tasks[i];

                if (task.RenewPeriod != ITask.Period.Never && !task.Active && task.DaysRemaining() == 0)
                {
                    task.Active = true;

                    if (_config.PushRenewedTasksToTheTop)
                    {
                        Tasks.RemoveAt(i++);
                        Tasks.Insert(0, task);
                        pushed++;
                    }
                }

                task.Validate();
            }
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                ITask task = Tasks[i];

                if (task.RenewPeriod != ITask.Period.Never && task.Complete)
                {
                    task.Complete = task.Active = false;
                }

                if (task.Complete)
                {
                    Tasks.RemoveAt(i);
                }
            }
        }
    }
}
