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
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Task
{
    /// <summary>A List wrapper for conveniently managing ITasks.</summary>
    internal class TaskList : IList<ITask>, IReadOnlyList<ITask>
    {
        private readonly ITaskEvents _events;
        private readonly List<ITask> _tasks;

        public ITask this[int index]
        {
            get => _tasks[index];
            set => _tasks[index] = value;
        }

        public int Count => _tasks.Count;

        public bool IsReadOnly => ((IList<ITask>)_tasks).IsReadOnly;

        public TaskList(ITaskEvents events)
        {
            _events = events;
            _tasks = new List<ITask>();
        }

        public void Add(ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Add(task);
        }

        public void Clear()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].EventUnsubscribe(_events);
            }

            _tasks.Clear();
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
        }

        public bool Remove(ITask task)
        {
            task.EventUnsubscribe(_events);
            return _tasks.Remove(task);
        }

        public void RemoveAt(int index)
        {
            _tasks[index].EventUnsubscribe(_events);
            _tasks.RemoveAt(index);
        }

        public void Sort()
        {
            // Preserve ordering among tasks in the same state
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].SetSortingIndex(i);
            }

            _tasks.Sort();
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
