/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace Common;

public class CommonHelper
{
    public static T Clamp<T>(T min, T t, T max)
    {
        if (Comparer<T>.Default.Compare(min, t) > 0)
            return min;
        if (Comparer<T>.Default.Compare(max, t) < 0)
            return max;
        return t;
    }

    public static T Adjust<T>(T value, T interval)
    {
        if (value is float vFloat && interval is float iFloat)
            value = (T)(object)(float)((decimal)vFloat - ((decimal)vFloat % (decimal)iFloat));

        if (value is int vInt && interval is int iInt)
            value = (T)(object)(vInt - vInt % iInt);

        return value;
    }
}