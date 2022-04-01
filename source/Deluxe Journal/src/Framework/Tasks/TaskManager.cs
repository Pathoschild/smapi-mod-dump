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
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Tasks
{
    internal class TaskManager
    {
        private readonly ITaskEvents _events;
        private readonly IDataHelper _dataHelper;
        private readonly TaskData _data;
        private readonly List<ITask> _tasks;

        public TaskData Data => _data;

        public IReadOnlyList<ITask> Tasks => _tasks;

        public TaskManager(ITaskEvents events, IDataHelper dataHelper)
        {
            _events = events;
            _dataHelper = dataHelper;
            _data = _dataHelper.ReadGlobalData<TaskData>(DeluxeJournalMod.TASKS_DATA_KEY) ?? new TaskData();
            _tasks = new List<ITask>();
        }

        public void AddTask(ITask task)
        {
            InsertTask(0, task);
        }

        public void InsertTask(int index, ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Insert(index, task);
        }

        public void RemoveTask(ITask task)
        {
            task.EventUnsubscribe(_events);
            _tasks.Remove(task);
        }

        public void RemoveTaskAt(int index)
        {
            _tasks[index].EventUnsubscribe(_events);
            _tasks.RemoveAt(index);
        }

        public void ReplaceTask(ITask oldTask, ITask newTask)
        {
            int index = _tasks.IndexOf(oldTask);

            oldTask.EventUnsubscribe(_events);
            newTask.EventSubscribe(_events);

            _tasks.RemoveAt(index);
            _tasks.Insert(index, newTask);
        }

        public void OnDayEnding()
        {
            _tasks.RemoveAll(delegate(ITask task)
            {
                if (task.RenewPeriod != ITask.Period.Never)
                {
                    if (task.Complete)
                    {
                        task.Complete = task.Active = false;
                    }

                    if (!task.Active && task.DaysRemaining() <= 1)
                    {
                        task.Active = true;
                    }
                }

                return task.Complete;
            });
        }

        public void SortTasks()
        {
            // Preserve ordering among tasks in the same state
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].SetSortingIndex(i);
            }

            _tasks.Sort();
        }

        public void Load()
        {
            for (int i = _tasks.Count - 1; i >= 0; i--)
            {
                RemoveTaskAt(i);
            }

            if (_data.Tasks.ContainsKey(Constants.SaveFolderName))
            {
                foreach (ITask task in _data.Tasks[Constants.SaveFolderName])
                {
                    InsertTask(_tasks.Count, task);
                }

                SortTasks();
            }
        }

        public void Save()
        {
            if (_data != null)
            {
                _data.Tasks[Constants.SaveFolderName] = _tasks;
                _dataHelper.WriteGlobalData(DeluxeJournalMod.TASKS_DATA_KEY, _data);
            }
        }
    }
}
