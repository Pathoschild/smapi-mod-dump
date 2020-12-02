/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    /// <summary>
    /// Cache of recent checks for if a setting is active to improve performance.
    /// Adapted from https://stackoverflow.com/questions/47874173/dictionary-cache-with-expiration-time
    /// </summary>
    public class ConditionalSettingCheckCache
    {
        public static readonly int CACHE_LIFE_MS = 20;

        private readonly Dictionary<int, CacheItem<bool>> _cache = new Dictionary<int, CacheItem<bool>>();

        public void Store(int key, bool value)
        {
            TimeSpan expiresAfter = TimeSpan.FromMilliseconds(CACHE_LIFE_MS);
            _cache[key] = new CacheItem<Boolean>(value, expiresAfter);
        }

        public bool? Get(int key)
        {
            if (!_cache.ContainsKey(key)) return null;

            var cached = _cache[key];

            if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
            {
                _cache.Remove(key);
                return null;
            }

            return cached.Value;
        }
    }

    /// <summary>
    /// An item in the cache.
    /// Copied from https://stackoverflow.com/questions/47874173/dictionary-cache-with-expiration-time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheItem<T>
    {
        public CacheItem(T value, TimeSpan expiresAfter)
        {
            Value = value;
            ExpiresAfter = expiresAfter;
        }
        public T Value { get; }
        internal DateTimeOffset Created { get; } = DateTimeOffset.Now;
        internal TimeSpan ExpiresAfter { get; }
    }
}
