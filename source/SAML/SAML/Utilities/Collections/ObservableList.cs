/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Events;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities.Collections
{
    /// <summary>
    /// A simplified implementation of <see cref="List{T}"/> which fires property- and collectionchanged events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableList<T> : IList<T>, IEnumerable<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly List<T> _items = [];

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public event CollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Add(T item)
        {
            _items.Add(item);
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(new[] { item });
        }

        public void AddRange(IEnumerable<T> items)
        {
            _items.AddRange(items);
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(items);
        }

        public void Clear()
        {
            var items = new List<T>(_items);
            _items.Clear();
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(removed: items);
        }

        public bool Contains(T item) => _items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        public int IndexOf(T item) => _items.IndexOf(item);

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(new[] { item });
        }

        public bool Remove(T item)
        {
            if (_items.Remove(item))
            {
                invokePropertyChanged(nameof(Count));
                invokeCollectionChanged(removed: new[] { item });
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            var item = _items[index];
            _items.RemoveAt(index);
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(removed: new[] { item });
        }

        public void RemoveRange(int index, int count)
        {
            List<T> items = [];
            for (int i = index; i < index + count; i++)
            {
                if (i > _items.Count)
                    break;
                items.Add(_items[i]);
            }
            _items.RemoveRange(index, count);
            invokePropertyChanged(nameof(Count));
            invokeCollectionChanged(removed: items);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void invokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        protected void invokeCollectionChanged(IEnumerable<T>? added = null, IEnumerable<T>? removed = null) => CollectionChanged?.Invoke(this, new(added, removed));
    }
}
