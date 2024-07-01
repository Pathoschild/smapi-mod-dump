/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Collections;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Task
{
    /// <summary>A List wrapper for conveniently managing <see cref="ITask"/> instances.</summary>
    internal class EventManagedTaskList : IList<ITask>, IReadOnlyList<ITask>
    {
        private readonly ITaskEvents _events;
        private readonly List<ITask> _tasks;
        private readonly long _umid;

        public ITask this[int index]
        {
            get => _tasks[index];

            set
            {
                ITask old = _tasks[index];

                if (old != value)
                {
                    old.EventUnsubscribe(_events);
                    value.EventSubscribe(_events);
                    _tasks[index] = value;

                    if (TaskListChangedEvent.HasEventListeners)
                    {
                        TaskListChangedEvent.Raise(this, new(this, [value], [old], _umid));
                    }
                }
            }
        }

        public int Count => _tasks.Count;

        public bool IsReadOnly => ((IList<ITask>)_tasks).IsReadOnly;

        /// <summary>Managed event raised after this <see cref="IList{ITask}"/> is changed.</summary>
        public IManagedEvent<TaskListChangedArgs> TaskListChangedEvent { get; }

        public EventManagedTaskList(long umid, ITaskEvents events, IManagedEvent<TaskListChangedArgs> taskListChangedEvent)
        {
            _umid = umid;
            _events = events;
            _tasks = [];
            TaskListChangedEvent = taskListChangedEvent;
        }

        public void Add(ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Add(task);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [task], [], _umid));
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].EventUnsubscribe(_events);
            }

            IEnumerable<ITask> copy = [.._tasks];
            _tasks.Clear();

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [], copy, _umid));
            }
        }

        public bool Contains(ITask task)
        {
            return _tasks.Contains(task);
        }

        public void CopyTo(ITask[] array, int arrayIndex)
        {
            _tasks.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ITask task)
        {
            return _tasks.IndexOf(task);
        }

        public void Insert(int index, ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Insert(index, task);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [task], [], _umid));
            }
        }

        public bool Remove(ITask task)
        {
            bool removed = _tasks.Remove(task);
            task.EventUnsubscribe(_events);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [], [task], _umid));
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            ITask task = _tasks[index];
            task.EventUnsubscribe(_events);
            _tasks.RemoveAt(index);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [], [task], _umid));
            }
        }

        /// <summary>
        /// Sort the <see cref="ITask"/> instances by their current state while preserving the ordering
        /// among tasks in the same state.
        /// </summary>
        public void Sort()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].SetSortingIndex(i);
            }

            _tasks.Sort();

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, [], [], _umid));
            }
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_tasks).GetEnumerator();
        }
    }
}
