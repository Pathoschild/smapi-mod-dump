/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace SpecialOrdersExtended;

/// <summary>
/// LINQ-like extensions on enumerables.
/// </summary>
internal static class IEnumerableExtensions
{
    /// <summary>
    /// Similar to LINQ's ToDictionary, but ignores duplicates instead of erroring.
    /// </summary>
    /// <typeparam name="TEnumerable">The type of elements in the enumerable.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the resulting dictionary.</typeparam>
    /// <typeparam name="TValue">They type of the values in the resulting dictionary.</typeparam>
    /// <param name="enumerable">The enumerable to look at.</param>
    /// <param name="keyselector">Function that maps enumerable to key.</param>
    /// <param name="valueselector">Function that maps enumerable to value.</param>
    /// <returns>The dictionary.</returns>
    [Pure]
    public static Dictionary<TKey, TValue> ToDictionaryIgnoreDuplicates<TEnumerable, TKey, TValue>(
        [NotNull] this IEnumerable<TEnumerable> enumerable,
        [NotNull] Func<TEnumerable, TKey> keyselector,
        [NotNull] Func<TEnumerable, TValue> valueselector)
        where TEnumerable : notnull
        where TKey : notnull
        where TValue : notnull
    {
        Dictionary<TKey, TValue> result = new();
        foreach (TEnumerable item in enumerable)
        {
            if(!result.TryAdd(keyselector(item), valueselector(item)))
            {
                ModEntry.ModMonitor.DebugLog($"Recieved duplicate key {keyselector(item)}, ignoring");
            }
        }
        return result;
    }
}

/// <summary>
/// Extensions to SMAPI's IMonitor.
/// </summary>
internal static class LogExtensions
{
    /// <summary>
    /// Logs to level (DEBUG by default) if compiled with the DEBUG flag.
    /// Logs to verbose otherwise.
    /// </summary>
    /// <param name="monitor">The SMAPI logging Monitor.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="level">The <see cref="LogLevel"/> to log at.</param>
    public static void DebugLog(
        [NotNull] this IMonitor monitor,
        [NotNull] string message,
        [NotNull] LogLevel level = LogLevel.Debug) =>
#if DEBUG
        monitor.Log(message, level);
#else
        monitor.VerboseLog(message);
#endif

}
