using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.FishingOverhaul.Api.Overridables {
    public class OverridableDictionary<TKey, TValue> : OverridableConst<IDictionary<TKey, TValue>>, IDictionary<TKey, TValue> {
        private readonly IDictionary<TKey, TValue> _defaultValues;
        private IDictionary<TKey, TValue> _replaced = new Dictionary<TKey, TValue>();
        private HashSet<TKey> _removed = new HashSet<TKey>();
        public override bool Overridden => this._replaced.Any() || this._removed.Any();

        public OverridableDictionary() : this(new Dictionary<TKey, TValue>()) { }
        public OverridableDictionary(IDictionary<TKey, TValue> defaultValues) : base(defaultValues) {
            this._defaultValues = defaultValues;
        }

        public override IDictionary<TKey, TValue> Get() {
            return this.AsEnumerable().ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public override void Set(IDictionary<TKey, TValue> value) {
            this._replaced = value.Where(kv => !this._defaultValues.Contains(kv)).ToDictionary(kv => kv.Key, kv => kv.Value);
            this._removed = new HashSet<TKey>(this._defaultValues.Keys.Except(value.Keys));
        }

        public override void Reset() {
            this._replaced.Clear();
            this._removed.Clear();
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> AsEnumerable() {
            HashSet<TKey> enumeratedKeys = new HashSet<TKey>();
            foreach (KeyValuePair<TKey, TValue> kv in this._replaced) {
                if (enumeratedKeys.Add(kv.Key)) {
                    yield return kv;
                }
            }

            foreach (KeyValuePair<TKey, TValue> kv in this._defaultValues) {
                if (!this._removed.Contains(kv.Key) && enumeratedKeys.Add(kv.Key)) {
                    yield return kv;
                }
            }
        }

        #region IDictionary<TKey, TValue>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return this.AsEnumerable().GetEnumerator();
        }

        public void Clear() {
            this._replaced.Clear();
            this._removed = new HashSet<TKey>(this._defaultValues.Select(kv => kv.Key));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return this._replaced.Contains(item) || this._defaultValues.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            this.Value.CopyTo(array, arrayIndex);
        }

        public bool ContainsKey(TKey key) {
            return this._replaced.ContainsKey(key) || (!this._removed.Contains(key) && this._defaultValues.ContainsKey(key));
        }

        public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

        public void Add(TKey key, TValue value) {
            this._replaced.Add(key, value);
            this._removed.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

        public bool Remove(TKey key) {
            return this._replaced.Remove(key) && this._removed.Add(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return this._replaced.TryGetValue(key, out value) || (!this._removed.Contains(key) && this._defaultValues.TryGetValue(key, out value));
        }

        public TValue this[TKey key] {
            get => this._replaced.TryGetValue(key, out TValue value) ? value : (!this._removed.Contains(key) ? this._defaultValues[key] : throw new IndexOutOfRangeException($"Invalid key {key} not defined"));
            set {
                this._replaced[key] = value;
                this._removed.Remove(key);
            }
        }

        public int Count => this._replaced.Count + this._defaultValues.Count(kv => !this._replaced.ContainsKey(kv.Key) && !this._removed.Contains(kv.Key));
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => this.Value.Keys;
        public ICollection<TValue> Values => this.Value.Values;
        #endregion
    }
}