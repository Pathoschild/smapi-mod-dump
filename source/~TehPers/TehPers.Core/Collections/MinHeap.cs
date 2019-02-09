using System;
using System.Collections;
using System.Collections.Generic;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Collections {
    public class MinHeap<T> : ICollection<T> {
        private readonly List<T> _storage;
        private readonly Comparer _comparer;

        public MinHeap(IComparer<T> comparer) {
            this._comparer = comparer.Compare;
        }

        public MinHeap(Comparer comparer) {
            this._storage = new List<T>();
            this._comparer = comparer;
        }

        public MinHeap(IEnumerable<T> source, Comparer comparer) {
            this._storage = new List<T>(source);
            this._comparer = comparer;
            this.Heapify(0);
        }

        private void Heapify(int root, bool percolating = false) {
            // Make sure this node exists
            if (root >= this._storage.Count)
                return;

            // Make sure it has children
            int child1 = root * 2 + 1;
            int child2 = root * 2 + 2;

            // Heapify children
            if (!percolating) {
                this.Heapify(child1);
                this.Heapify(child2);
            }

            // No children
            if (child1 >= this.Count)
                return;

            // One child
            if (child2 >= this.Count) {
                // Check if root is greater than its child
                if (this._comparer(this._storage[root], this._storage[child1]) > 0) {
                    // Swap them
                    this._storage.Swap(root, child1);
                    this.Heapify(child1, true);
                }

                return;
            }

            // Get the smallest child
            int swapChild = this._comparer(this._storage[child1], this._storage[child2]) < 0 ? child1 : child2;
            if (this._comparer(this._storage[root], this._storage[swapChild]) > 0) {
                this._storage.Swap(root, swapChild);
                this.Heapify(swapChild, true);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return this._storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(T item) {
            // Make sure item is not null
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cannot add null to heap.");

            // Add the item
            this._storage.Add(item);

            // Percolate the item up
            int i = this.Count - 1;
            int parentI = (i - 1) / 2;
            while (i > 0 && this._comparer(this._storage[i], this._storage[parentI]) < 0) {
                this._storage.Swap(i, parentI);
                i = parentI;
                parentI = (i - 1) / 2;
            }
        }

        /// <summary>Removes the first element and returns it.</summary>
        /// <returns>The first element in the heap.</returns>
        public T RemoveFirst() {
            // Can't remove the first if there's no first element
            if (this.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            // Keep track of the removed element
            T removed = this._storage[0];

            // Check if that was the only element and this can be shortcutted
            if (this.Count == 1) {
                this._storage.Clear();
                return removed;
            }

            // Swap the last and first elements and remove the (now last) element
            this._storage.Swap(0, this.Count - 1);
            this._storage.RemoveAt(this.Count - 1);

            // Percolate the top element down
            this.Heapify(0, true);

            // Return the removed element
            return removed;
        }

        public bool Remove(T item) {
            // Check if trying to remove null
            if (item == null)
                return false;

            // Check if trying to remove from an empty collection
            if (this.Count == 0)
                return false;

            // Check if trying to remove the first element
            if (item.Equals(this._storage[0])) {
                this.RemoveFirst();
                return true;
            }

            // Remove normally, rebuilding the heap afterwards if needed
            if (this._storage.Remove(item)) {
                this.Heapify(0);
                return true;
            }

            // Not found
            return false;
        }

        public void Clear() {
            this._storage.Clear();
        }

        public bool Contains(T item) {
            return this._storage.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this._storage.CopyTo(array, arrayIndex);
        }

        public int Count => this._storage.Count;
        public bool IsReadOnly => ((ICollection<T>) this._storage).IsReadOnly;

        /// <summary>Compares two elements together.</summary>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <returns>A positive number if the first element is greater than the second, a negative number if it's less than the second, or 0 if they're equal.</returns>
        public delegate int Comparer(T a, T b);
    }
}
