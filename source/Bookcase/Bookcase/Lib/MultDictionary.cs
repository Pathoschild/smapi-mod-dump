using System.Collections.Generic;

namespace Bookcase.Lib {

    /// <summary>
    /// This class serves a basic alternative to MultiDictionary/MultiValueDictionary. 
    /// We don't hace access to those, so this is a stand in until we do.
    /// </summary>
    /// <typeparam name="TKey">The key for the map.</typeparam>
    /// <typeparam name="TValue">The value for the map.</typeparam>
    public class MultiDict<TKey, TValue> {

        /// <summary>
        /// The internal collection. It's just a normal dictionary.
        /// </summary>
        private Dictionary<TKey, List<TValue>> internalData = new Dictionary<TKey, List<TValue>>();

        /// <summary>
        /// Gets a value by key.
        /// </summary>
        /// <param name="key">The key you want.</param>
        /// <returns></returns>
        public List<TValue> Get(TKey key) {

            List<TValue> values;
            return this.internalData.TryGetValue(key, out values) ? values : new List<TValue>();
        }

        /// <summary>
        /// Adds a value to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value) {

            List<TValue> values;

            if (internalData.TryGetValue(key, out values)) {

                values.Add(value);
            }

            else {

                internalData.Add(key, new List<TValue>() { value });
            }
        }

        /// <summary>
        /// Removes a value from the dictionary. This will remove from all keys.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(TValue value) {

            foreach (TKey key in internalData.Keys) {

                this.Remove(value, key);
            }
        }

        /// <summary>
        /// Removes a value from the dictionary. Requires a specific key.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <param name="key">The key the value is linked to.</param>
        public void Remove(TValue value, TKey key) {

            this.Get(key).Remove(value);
        }
    }
}