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
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Framework.Task
{
    internal class TaskManager
    {
        /// <summary>Data key for the tasks save data.</summary>
        public const string TasksDataKey = "tasks-data";

        private readonly EventManager _eventManager;
        private readonly ITaskEvents _taskEvents;
        private readonly IDataHelper _dataHelper;
        private readonly Config _config;
        private readonly ISemanticVersion _version;
        private readonly IDictionary<long, EventManagedTaskList> _tasks;
        private TaskData? _data;
        private bool _loaded;

        /// <summary><c>true</c> if the tasks have been loaded for the first time. Tasks are loaded on save started.</summary>
        public bool Loaded
        {
            get => _loaded;
            private set => _loaded = value;
        }

        /// <summary>A list of tasks for the active player.</summary>
        public IList<ITask> Tasks
        {
            get
            {
                long umid = Game1.player.UniqueMultiplayerID;

                if (!_tasks.ContainsKey(umid))
                {
                    _tasks[umid] = new(umid, _taskEvents, _eventManager.TaskListChanged);
                }

                return _tasks[umid];
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
                    _data = _dataHelper.ReadGlobalData<TaskData>(TasksDataKey) ?? new(_version);
                    Loaded = true;
                }

                return _data;
            }
        }

        public TaskManager(EventManager eventManager, IDataHelper dataHelper, Config config, ISemanticVersion version)
        {
            _eventManager = eventManager;
            _taskEvents = new TaskEvents(eventManager);
            _dataHelper = dataHelper;
            _config = config;
            _version = version;
            _tasks = new Dictionary<long, EventManagedTaskList>();

            eventManager.ModEvents.GameLoop.SaveLoaded += OnSaveLoaded;
            eventManager.ModEvents.GameLoop.Saving += OnSaving;
            eventManager.ModEvents.GameLoop.DayStarted += OnDayStarted;
            eventManager.ModEvents.GameLoop.DayEnding += OnDayEnding;
        }

        /// <summary>Sort local player tasks.</summary>
        public void SortTasks()
        {
            ((EventManagedTaskList)Tasks).Sort();
        }

        /// <summary>Refresh the task groups.</summary>
        /// <remarks>
        /// Tasks are grouped by headers and are given the same <see cref="ITask.Group"/> value as the header
        /// they fall under.
        /// </remarks>
        public void RefreshGroups()
        {
            int group = 0;
            int colorIndex = -1;

            foreach (ITask task in Tasks)
            {
                if (task.IsHeader)
                {
                    group++;
                    colorIndex = task.ColorIndex;
                }

                task.Group = group;
                task.GroupColorIndex = colorIndex;
            }
        }

        /// <summary>Load the task list from save data.</summary>
        public void Load()
        {
            long umid;

            // Each TaskList must be cleared in order to unsubscribe from task events
            foreach (EventManagedTaskList tasks in _tasks.Values)
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
                        _tasks[umid] = new(umid, _taskEvents, _eventManager.TaskListChanged);
                    }

                    foreach (ITask task in Data.Tasks[saveFolderName][key])
                    {
                        task.OwnerUniqueMultiplayerID = umid;
                        _tasks[umid].Add(task.Copy());
                    }

                    _tasks[umid].Sort();
                }

                RefreshGroups();
            }
        }

        /// <summary>Save the task list.</summary>
        public void Save()
        {
            if (Loaded && Constants.SaveFolderName is string saveFolderName)
            {
                Data.Version = _version.ToString();
                Data.Tasks[saveFolderName] = _tasks
                    .Where(entry => entry.Value.Count > 0)
                    .ToDictionary(entry => entry.Key, entry => (IList<ITask>)entry.Value.ToList());

                _dataHelper.WriteGlobalData(TasksDataKey, Data);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Load();
            }
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Save();
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            int pushGroup = 0;
            int pushIndex = 0;

            RefreshGroups();

            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                ITask task = Tasks[i];

                if (!task.Active && task.RenewPeriod != ITask.Period.Never && task.DaysRemaining() == 0)
                {
                    task.Active = true;

                    if (_config.PushRenewedTasksToTheTop)
                    {
                        if (task.Group != pushGroup)
                        {
                            for (pushIndex = i; pushIndex > 0; pushIndex--)
                            {
                                ITask groupTask = Tasks[pushIndex - 1];

                                if (groupTask.IsHeader)
                                {
                                    pushGroup = groupTask.Group;
                                    break;
                                }
                            }
                        }

                        if (pushIndex < i)
                        {
                            Tasks.RemoveAt(i++);
                            Tasks.Insert(pushIndex, task);
                            continue;
                        }
                    }
                }

                task.Validate();
            }

            SortTasks();
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
