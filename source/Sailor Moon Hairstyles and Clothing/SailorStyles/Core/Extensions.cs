/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using System;
using System.Diagnostics.Contracts;
using StardewModdingAPI;
using StardewValley;

namespace SailorStyles.Core;

/// <summary>
/// Extension methods for IGameContentHelper.
/// </summary>
internal static class GameContentHelperExtensions
{
    /// <summary>
    /// Invalidates both an asset and the locale-specific version of an asset.
    /// </summary>
    /// <param name="helper">The game content helper.</param>
    /// <param name="assetName">The (string) asset to invalidate.</param>
    /// <returns>if something was invalidated.</returns>
    internal static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
        => helper.InvalidateCache(assetName)
            | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
}

internal static class StringExtensions
{
    /// <summary>
    /// Faster replacement for str.Split()[index];.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="deliminator">deliminator to use.</param>
    /// <param name="index">index of the chunk to get.</param>
    /// <returns>a readonlyspan char with the chunk, or an empty readonlyspan for failure.</returns>
    [Pure]
    internal static ReadOnlySpan<char> GetNthChunk(this string str, char deliminator, int index = 0)
        => str.GetNthChunk(new[] { deliminator }, index);

    /// <summary>
    /// Faster replacement for str.Split()[index];.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="deliminators">deliminators to use.</param>
    /// <param name="index">index of the chunk to get.</param>
    /// <returns>a readonlyspan char with the chunk, or an empty readonlyspan for failure.</returns>
    /// <remarks>Inspired by the lovely Wren.</remarks>
    [Pure]
    internal static ReadOnlySpan<char> GetNthChunk(this string str, char[] deliminators, int index = 0)
    {
        int start = 0;
        int ind = 0;
        while (index-- >= 0)
        {
            ind = str.IndexOfAny(deliminators, start);
            if (ind == -1)
            {
                // since we've previously decremented index, check against -1;
                // this means we're done.
                if (index == -1)
                {
                    return str.AsSpan()[start..];
                }

                // else, we've run out of entries
                // and return an empty span to mark as failure.
                return ReadOnlySpan<char>.Empty;
            }

            if (index > -1)
            {
                start = ind + 1;
            }
        }
        return str.AsSpan()[start..ind];
    }
}