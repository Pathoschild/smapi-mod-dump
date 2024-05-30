/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;

namespace TimeWatch.Utils;

internal static class ObjectUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Let<T, R>(this T thiz, Func<T, R> block) => block(thiz);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Apply<T>(this T thiz, Action<T> block)
    {
        block(thiz);
        return thiz;
    }
}