using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.FishingOverhaul.Api.Overridables {
    public class OverridableSet<T> : OverridableConst<ISet<T>>, ISet<T> {
        private readonly HashSet<T> _added = new HashSet<T>();
        private HashSet<T> _removed = new HashSet<T>();
        private readonly ISet<T> _defaultValues;

        public OverridableSet() : this(new HashSet<T>()) { }
        public OverridableSet(ISet<T> defaultValues) : base(defaultValues) {
            this._defaultValues = defaultValues;
        }

        private IEnumerable<T> AsEnumerable() {
            HashSet<T> enumerated = new HashSet<T>();

            foreach (T cur in this._added) {
                if (enumerated.Add(cur)) {
                    yield return cur;
                }
            }

            foreach (T cur in this._defaultValues) {
                if (!this._removed.Contains(cur) && enumerated.Add(cur)) {
                    yield return cur;
                }
            }
        }

        #region ISet<T>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<T> GetEnumerator() {
            return this.AsEnumerable().GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other) {
            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {B, D} = {A, C, E}
            // {C, D, F} (intersect with default: {D})
            // ---
            // {A, C, F} + {B, D, E} - {B} = {A, C, D, E, F}
            this._added.UnionWith(other.Except(this._defaultValues));
            this._removed.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other) {
            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {B, D} = {A, C, E}
            // {C, D, F} (intersect with default: {D})
            // ---
            // {C} + {B, D, E} - {B, D, E} = {C}
            this._added.IntersectWith(other);
            this._removed.UnionWith(this._defaultValues.Except(other));
        }

        public void ExceptWith(IEnumerable<T> other) {
            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {D} = {A, B, C, E}
            // {B, C, D, F} (intersect with default: {B, D})
            // ---
            // {A} + {B, D, E} - {B, D} = {A, E}
            this._added.ExceptWith(other);
            this._removed.UnionWith(other.Intersect(this._defaultValues));
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            other = new HashSet<T>(other);

            // {A, C} + {B, D, E} - {D, E} = {A, B, C}
            // {B, C, D, F} (intersect with default: {B, D})
            // ---
            // {A, F} + {B, D, E} - {B, E} = {A, D, F}
            this._added.SymmetricExceptWith(other.Except(this._defaultValues));
            this._removed.SymmetricExceptWith(other.Intersect(this._defaultValues));
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            return new HashSet<T>(other).IsSupersetOf(this.AsEnumerable());
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return new HashSet<T>(other).IsSubsetOf(this.AsEnumerable());
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return new HashSet<T>(other).IsProperSubsetOf(this.AsEnumerable());
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return new HashSet<T>(other).IsProperSupersetOf(this.AsEnumerable());
        }

        public bool Overlaps(IEnumerable<T> other) {
            return new HashSet<T>(other).Overlaps(this.AsEnumerable());
        }

        public bool SetEquals(IEnumerable<T> other) {
            return new HashSet<T>(other).SetEquals(this.AsEnumerable());
        }

        void ICollection<T>.Add(T item) => ((ISet<T>) this).Add(item);
        bool ISet<T>.Add(T item) {
            return this._removed.Remove(item) || (!this._defaultValues.Contains(item) && this._added.Add(item));
        }

        public void Clear() {
            this._added.Clear();
            this._removed = new HashSet<T>(this._defaultValues);
        }

        public bool Contains(T item) {
            return this._added.Contains(item) || (!this._removed.Contains(item) && this._defaultValues.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            foreach (T cur in this.AsEnumerable()) {
                array[arrayIndex] = cur;
            }
        }

        public bool Remove(T item) {
            return this._added.Remove(item) || (this._defaultValues.Contains(item) && this._removed.Add(item));
        }

        public int Count => this._added.Count + this._defaultValues.Count - this._removed.Count;
        public bool IsReadOnly => false;
        #endregion
    }
}