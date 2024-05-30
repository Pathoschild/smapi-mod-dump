/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

namespace TimeWatch.Utils;

internal static class NumberUtils
{
    public static T CoerceIn<T>(this T value, T min, T max) where T : struct, IComparable<T> => value switch
    {
        _ when value.CompareTo(min) < 0 => min,
        _ when value.CompareTo(max) > 0 => max,
        _ => value,
    };
}