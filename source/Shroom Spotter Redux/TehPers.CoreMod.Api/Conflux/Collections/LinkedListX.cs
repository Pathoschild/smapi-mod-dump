/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public class LinkedListX<T> : IList<T> {
        private Node _head = null;
        private Node _tail = null;

        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public LinkedListX() {
            this.Count = 0;
        }

        public LinkedListX(IEnumerable<T> items) : this() {
            foreach (T item in items) {
                this.Add(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<T> GetEnumerator() {
            Node cur = this._head;
            while (cur != null) {
                yield return cur.Value;
                cur = cur.Next;
            }
        }

        public void Add(T item) {
            Node prevTail = this._tail;
            this._tail = new Node {
                Value = item
            };

            if (prevTail == null) {
                this._head = this._tail;
            } else {
                prevTail.Next = this._tail;
            }

            this.Count++;
        }

        public void AddFirst(T item) {
            this._head = new Node {
                Value = item,
                Next = this._head
            };

            if (this._tail == null) {
                this._tail = this._head;
            }

            this.Count++;
        }

        public T RemoveFirst() {
            if (this._head == null) {
                throw new InvalidOperationException("The collection is empty");
            }

            Node prevHead = this._head;
            this._head = this._head.Next;
            this.UpdateTail();

            this.Count--;
            return prevHead.Value;
        }

        public void Clear() {
            this.Count = 0;
            this._head = this._tail = null;
        }

        public bool Contains(T item) {
            return this.IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }

            for (int i = 0; i < this.Count; i++) {
                array[arrayIndex + i] = this[i];
            }
        }

        public bool Remove(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (this._head == null) {
                return false;
            }

            if (object.Equals(this._head, item)) {
                this.RemoveFirst();
                this.Count--;
                return true;
            }

            Node cur = this._head;
            while (cur.Next != null) {
                if (object.Equals(cur.Next.Value, item)) {
                    if (this._tail == cur.Next) {
                        this._tail = cur;
                    }

                    cur.Next = cur.Next.Next;
                    this.Count--;
                    return true;
                }

                cur = cur.Next;
            }

            return false;
        }

        public int IndexOf(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (this._head == null) {
                return -1;
            }

            int i = 0;
            Node cur = this._head;
            while (cur != null) {
                if (object.Equals(cur.Value, item)) {
                    return i;
                }

                i++;
                cur = cur.Next;
            }

            return -1;
        }

        public void Insert(int index, T item) {
            if (this._head == null) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index == 0) {
                this.AddFirst(item);
            } else {
                // Search for the node before the one being removed
                Node cur = this._head;
                while (--index > 0) {
                    cur = cur.Next ?? throw new ArgumentOutOfRangeException(nameof(index));
                }

                // Create the node being added
                Node added = new Node {
                    Value = item,
                    Next = cur.Next
                };

                // Update tail if needed
                if (this._tail == cur) {
                    this._tail = added;
                }

                // Update current node
                cur.Next = added;
            }
        }

        public void RemoveAt(int index) {
            if (this._head == null) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index == 0) {
                this.RemoveFirst();
            } else {
                // Search for the node before the one being removed
                Node cur = this._head;
                while (--index > 0) {
                    cur = cur.Next ?? throw new ArgumentOutOfRangeException(nameof(index));
                }

                // Make sure the node to be removed exists
                if (cur.Next == null) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                // Update tail if needed
                if (this._tail == cur.Next) {
                    this._tail = cur;
                }

                // Update current node
                cur.Next = null;

                // Update count
                this.Count--;
            }
        }

        private void UpdateHead() {
            if (this._tail == null) {
                this._head = null;
            }
        }

        private void UpdateTail() {
            if (this._head == null) {
                this._tail = null;
            }
        }

        private Node GetNodeAt(int index) {
            if (this._head == null) {
                throw new IndexOutOfRangeException(nameof(index));
            }

            Node cur = this._head;
            while (index-- > 0) {
                cur = cur.Next ?? throw new IndexOutOfRangeException(nameof(index));
            }

            return cur;
        }

        public T this[int index] {
            get => this.GetNodeAt(index).Value;
            set => this.GetNodeAt(index).Value = value;
        }

        private class Node {
            public T Value = default;
            public Node Next = null;
        }
    }
}
