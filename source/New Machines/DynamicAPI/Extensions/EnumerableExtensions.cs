using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class EnumerableExtensions
    {
        public static string Serialize<T>(this IEnumerable<T> items)
        {
            return string.Join(" ", items);
        }
    }
}
