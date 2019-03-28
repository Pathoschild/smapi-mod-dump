using System.Collections.Generic;

namespace DeleteAnything.Framework
{
    static class Extenders
    {
        public static TValue SafeGet<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary, Tkey key, TValue defaultValue = default(TValue))
        {
            TValue value = defaultValue;

            if (dictionary != null)
            {
                if (!dictionary.TryGetValue(key, out value))
                    value = defaultValue;
            }

            return value;
        }
    }
}
