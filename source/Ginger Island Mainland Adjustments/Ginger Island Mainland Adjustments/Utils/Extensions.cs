/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Reflection;
using System.Text.RegularExpressions;

namespace GingerIslandMainlandAdjustments.Utils;

/// <summary>
/// Add some python-esque methods to the dictionaries.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// equivalent to python's dictionary.update().
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to update.</param>
    /// <param name="updateDict">Dictionary containing values to add to the first dictionary.</param>
    /// <returns>the dictionary (for chaining).</returns>
    public static Dictionary<TKey, TValue> Update<TKey, TValue>(
        [NotNull] this Dictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue>? updateDict)
        where TKey : notnull
        where TValue : notnull
    {
        if (updateDict is not null)
        {
            foreach (TKey key in updateDict.Keys)
            {
                dictionary[key] = updateDict[key];
            }
        }
        return dictionary;
    }

    /// <summary>
    /// equivalent to python's dictionary.setdefault().
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary.</param>
    /// <param name="key">Key to look for.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>Value from dictionary if one exists, else default value.</returns>
    /// <remarks>Function both sets state and returns value.</remarks>
    public static TValue? SetDefault<TKey, TValue>(
        [NotNull] this Dictionary<TKey, TValue> dictionary,
        [NotNull] TKey key,
        [NotNull] TValue defaultValue)
        where TKey : notnull
        where TValue : notnull
    {
        // add the value to the dictionary if it doesn't exist.
        dictionary.TryAdd(key, defaultValue);
        return dictionary[key];
    }

    /// <summary>
    /// similar to SetDefault, but will override a null value.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to search in.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Value to use.</param>
    /// <returns>Value from dictionary if it exists and is not null, defaultValue otherwise.</returns>
    public static TValue SetDefaultOverrideNull<TKey, TValue>(
        [NotNull] this Dictionary<TKey, TValue> dictionary,
        [NotNull] TKey key,
        [NotNull] TValue defaultValue)
        where TKey : notnull
        where TValue : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? value) && value is not null)
        {
                return value;
        }
        else
        {
            dictionary[key] = defaultValue;
            return defaultValue;
        }
    }

    /// <summary>
    /// Retrieves a value from the dictionary.
    /// Uses the default if the value is null, or if the key is not found.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to search in.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>Value from dictionary if not null, or else defaultValue.</returns>
    [Pure]
    public static TValue GetValueOrDefaultOverrideNull<TKey, TValue>(
        [NotNull] this Dictionary<TKey, TValue> dictionary,
        [NotNull] TKey key,
        [NotNull] TValue defaultValue)
      where TKey : notnull
      where TValue : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? value) && value is not null)
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }
}

/// <summary>
/// Adds some LINQ-esque methods to the Regex class.
/// </summary>
public static class RegexExtensions
{
    /// <summary>
    /// Converts a Match with named matchgroups into a dictionary.
    /// </summary>
    /// <param name="match">Regex matchgroup.</param>
    /// <returns>Dictionary with the name of the matchgroup as the key and the value as the value.</returns>
    [Pure]
    public static Dictionary<string, string> MatchGroupsToDictionary([NotNull] this Match match)
    {
        Dictionary<string, string> result = new();
        foreach (Group group in match.Groups)
        {
            result[group.Name] = group.Value;
        }
        return result;
    }

    /// <summary>
    /// Converts a Match with named matchgroups into a dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type for key.</typeparam>
    /// <typeparam name="TValue">Type for value.</typeparam>
    /// <param name="match">Match with named matchgroups.</param>
    /// <param name="keyselector">Function to apply to all keys.</param>
    /// <param name="valueselector">Function to apply to all values.</param>
    /// <returns>The dictionary.</returns>
    [Pure]
    public static Dictionary<TKey, TValue> MatchGroupsToDictionary<TKey, TValue>(
        [NotNull] this Match match,
        [NotNull] Func<string, TKey> keyselector,
        [NotNull] Func<string, TValue> valueselector)
        where TKey : notnull
        where TValue : notnull
    {
        Dictionary<TKey, TValue> result = new();
        foreach (Group group in match.Groups)
        {
            result[keyselector(group.Name)] = valueselector(group.Value);
        }
        return result;
    }
}

/// <summary>
/// Extension methods for SMAPI's logging service.
/// </summary>
internal static class LogExtensions
{
    /// <summary>
    /// Logs to level (DEBUG by default) if compiled with the DEBUG flag
    /// Logs to verbose otherwise.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="level">Level to log at.</param>
    public static void DebugLog(
        [NotNull] this IMonitor monitor,
        [NotNull] string message,
        [NotNull] LogLevel level = LogLevel.Debug)
    {
#if DEBUG
        monitor.Log(message, level);
#else
        monitor.VerboseLog(message);
#endif
    }
}

/// <summary>
/// Small extensions to get the full name of a method.
/// </summary>
internal static class MethodExtensions
{
    /// <summary>
    /// Gets the full name of a MethodBase.
    /// </summary>
    /// <param name="method">MethodBase to analyze.</param>
    /// <returns>Fully qualified name of a MethodBase.</returns>
    [Pure]
    public static string GetFullName([NotNull] this MethodBase method) => $"{method.DeclaringType}::{method.Name}";

    /// <summary>
    /// Gets the full name of a MethodInfo.
    /// </summary>
    /// <param name="method">MethodInfo to analyze.</param>
    /// <returns>Fully qualified name of a MethodInfo.</returns>
    [Pure]
    public static string GetFullName([NotNull] this MethodInfo method) => $"{method.DeclaringType}::{method.Name}";
}

/// <summary>
/// Small extensions to Stardew's NPC class.
/// </summary>
internal static class NPCExtensions
{
    /// <summary>
    /// Clears the NPC's current dialogue stack and pushes a new dialogue onto that stack.
    /// </summary>
    /// <param name="npc">NPC.</param>
    /// <param name="dialogueKey">Dialogue key.</param>
    public static void ClearAndPushDialogue(
        [NotNull] this NPC npc,
        [NotNull] string dialogueKey)
    {
        npc.CurrentDialogue.Clear();
        npc.CurrentDialogue.Push(new Dialogue(npc.Dialogue[dialogueKey], npc) { removeOnNextMove = true });
        Globals.ModMonitor.VerboseLog(I18n.FoundKey(dialogueKey, npc.Name));
    }

    /// <summary>
    /// Given a base key, gets a random dialogue from a set.
    /// </summary>
    /// <param name="npc">NPC.</param>
    /// <param name="basekey">Basekey to use.</param>
    /// <param name="random">Random to use, defaults to Game1.random if null.</param>
    /// <returns>null if no dialogue key found, a random dialogue key otherwise.</returns>
    public static string? GetRandomDialogue(
        [NotNull] this NPC npc,
        string? basekey,
        Random? random)
    {
        if (basekey is null)
        {
            return null;
        }
        if (random is null)
        {
            random = Game1.random;
        }
        if (npc.Dialogue.ContainsKey(basekey))
        {
            int index = 1;
            while (npc.Dialogue.ContainsKey($"{basekey}_{++index}"))
            {
            }
            int selection = random.Next(1, index);
            return (selection == 1) ? basekey : $"{basekey}_{selection}";
        }
        return null;
    }
}

/// <summary>
/// Extension methods on Stardew's SchedulePathDescription class.
/// </summary>
internal static class SchedulePointDescriptionExtensions
{
    /// <summary>
    /// Gets the expected travel time of a SchedulePathDescription.
    /// </summary>
    /// <param name="schedulePathDescription">Schedule Path Description.</param>
    /// <returns>Time in in-game minutes, not rounded.</returns>
    [Pure]
    public static int GetExpectedRouteTime([NotNull] this SchedulePathDescription schedulePathDescription)
    {
        return schedulePathDescription.route.Count * 32 / 42;
    }
}