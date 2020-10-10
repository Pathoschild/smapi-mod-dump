/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.FishingOverhaul.Api.Overridables {
    public class OverridableSet2<T> : Overridable2<ISet<T>>, ISet<T> {
        private readonly ISet<T> _baseSet;
        private readonly ISet<T> _added;
        private ISet<T> _removed;

        public OverridableSet2(ISet<T> baseSet) {
            this._baseSet = baseSet;
            this._added = new HashSet<T>();
            this._removed = new HashSet<T>();
        }

        public IEnumerator<T> GetEnumerator() {
            // Added items
            foreach (T item in this._added) {
                yield return item;
            }

            // Base items that haven't been removed
            foreach (T item in this._baseSet) {
                if (!this._removed.Contains(item)) {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        void ICollection<T>.Add(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            ((ISet<T>) this).Add(item);
        }

        public void UnionWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            // Try to add all items in other
            foreach (T item in other) {
                if (!this._removed.Remove(item) && !this._baseSet.Contains(item)) {
                    this._added.Add(item);
                }
            }
        }

        public void IntersectWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {B, D} = {A, C, E}
            // {C, D, F} (intersect with default: {D})
            // ---
            // {C} + {B, D, E} - {B, D, E} = {C}
            this._added.IntersectWith(other);
            this._removed.UnionWith(this._baseSet.Except(other));
        }

        public void ExceptWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            // Try to remove all items in other
            foreach (T item in other) {
                if (!this._added.Remove(item) && this._baseSet.Contains(item)) {
                    this._removed.Add(item);
                }
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {D, E} = {A, B, C}
            // {B, C, D, F} (intersect with default: {B, D})
            // ---
            // {A, F} + {B, D, E} - {B, E} = {A, D, F}
            this._added.SymmetricExceptWith(other.Except(this._baseSet));
            this._removed.SymmetricExceptWith(other.Intersect(this._baseSet));
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            ISet<T> otherSet = other as ISet<T> ?? new HashSet<T>(other);
            return this.All(otherSet.Contains);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            return other.All(this.Contains);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return new HashSet<T>(this).IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return new HashSet<T>(this).IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            if (other is ISet<T> otherSet) {
                return this._added.Overlaps(otherSet) || (otherSet.Overlaps(this._baseSet.Except(this._removed)));
            }

            return other.Any(this.Contains);
        }

        public bool SetEquals(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            if (other is ISet<T> otherSet) {
                return otherSet.SetEquals(this._baseSet.Except(this._removed).Concat(this._added).Distinct());
            }

            return new HashSet<T>(this).SetEquals(other);
        }

        bool ISet<T>.Add(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (this._removed.Remove(item)) {
                return this._baseSet.Contains(item);
            }

            if (!this._baseSet.Contains(item)) {
                return this._added.Add(item);
            }

            return false;
        }

        public void Clear() {
            this._added.Clear();
            this._removed = new HashSet<T>(this._baseSet);
        }

        public bool Contains(T item) {
            return this._added.Contains(item) || (this._baseSet.Contains(item) && !this._removed.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex) {
            new HashSet<T>(this).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            return this._added.Remove(item) || (this._baseSet.Contains(item) && this._removed.Add(item));
        }

        public int Count => this._added.Count + this._baseSet.Except(this._removed).Count();
        public bool IsReadOnly => false;
    }
}
