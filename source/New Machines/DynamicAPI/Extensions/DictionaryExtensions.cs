using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class 
        {
            TValue value;
            return dictionary.TryGetValue(key, out value)? value : null;
        }
    }
}
