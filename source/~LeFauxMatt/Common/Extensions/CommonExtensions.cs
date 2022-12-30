/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using StardewMods.Common.Models;
using StardewValley.Objects;
using StardewValley.Tools;

/// <summary>Common extension methods.</summary>
internal static class CommonExtensions
{
    /// <summary>
    ///     Invokes all event handlers for an event.
    /// </summary>
    /// <param name="eventHandler">The event.</param>
    /// <param name="source">The source.</param>
    public static void InvokeAll(this EventHandler? eventHandler, object source)
    {
        if (eventHandler is null)
        {
            return;
        }

        foreach (var handler in eventHandler.GetInvocationList())
        {
            try
            {
                handler.DynamicInvoke(source);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    ///     Invokes all event handlers for an event.
    /// </summary>
    /// <param name="eventHandler">The event.</param>
    /// <param name="source">The source.</param>
    /// <param name="param">The event parameters.</param>
    /// <typeparam name="T">The event handler type.</typeparam>
    public static void InvokeAll<T>(this EventHandler<T>? eventHandler, object source, T param)
    {
        if (eventHandler is null)
        {
            return;
        }

        foreach (var handler in eventHandler.GetInvocationList())
        {
            try
            {
                handler.DynamicInvoke(source, param);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    ///     Test if an item is equivalent to another item.
    /// </summary>
    /// <param name="salable">The item to test.</param>
    /// <param name="other">The other item to test against.</param>
    /// <returns>Returns true if the items are equivalent.</returns>
    public static bool IsEquivalentTo(this ISalable salable, ISalable? other)
    {
        if (other is null || !salable.Name.Equals(other.Name))
        {
            return false;
        }

        return salable switch
        {
            ColoredObject coloredObj when other is not ColoredObject otherColoredObj
                                       || !coloredObj.color.Value.Equals(otherColoredObj.color.Value) => false,
            SObject obj when other is not SObject otherObj
                          || obj.ParentSheetIndex != otherObj.ParentSheetIndex
                          || obj.bigCraftable.Value != otherObj.bigCraftable.Value
                          || obj.orderData.Value != otherObj.orderData.Value
                          || obj.Quality != otherObj.Quality
                          || obj.Type != otherObj.Type => false,
            Item item when other is not Item otherItem
                        || item.Category != otherItem.Category
                        || item.ParentSheetIndex != otherItem.ParentSheetIndex => false,
            Stackable stackable when other is not Stackable otherStackable || !stackable.canStackWith(otherStackable) =>
                false,
            Tool when other is not Tool => false,
            _ => true,
        };
    }

    /// <summary>
    ///     Maps a float value from one range to the same proportional value in another integer range.
    /// </summary>
    /// <param name="value">The float value to map.</param>
    /// <param name="sourceRange">The source range of the float value.</param>
    /// <param name="targetRange">The target range to map to.</param>
    /// <returns>The integer value.</returns>
    public static int Remap(this float value, Range<float> sourceRange, Range<int> targetRange)
    {
        return targetRange.Clamp(
            (int)(targetRange.Minimum
                + (targetRange.Maximum - targetRange.Minimum)
                * ((value - sourceRange.Minimum) / (sourceRange.Maximum - sourceRange.Minimum))));
    }

    /// <summary>
    ///     Maps an integer value from one range to the same proportional value in another float range.
    /// </summary>
    /// <param name="value">The integer value to map.</param>
    /// <param name="sourceRange">The source range of the integer value.</param>
    /// <param name="targetRange">The target range to map to.</param>
    /// <returns>The float value.</returns>
    public static float Remap(this int value, Range<int> sourceRange, Range<float> targetRange)
    {
        return targetRange.Clamp(
            targetRange.Minimum
          + (targetRange.Maximum - targetRange.Minimum)
          * ((float)(value - sourceRange.Minimum) / (sourceRange.Maximum - sourceRange.Minimum)));
    }

    /// <summary>Rounds an int up to the next int by an interval.</summary>
    /// <param name="i">The integer to round up from.</param>
    /// <param name="d">The interval to round up to.</param>
    /// <returns>Returns the rounded value.</returns>
    public static int RoundUp(this int i, int d = 1)
    {
        return (int)(d * Math.Ceiling((float)i / d));
    }

    /// <summary>Shuffles a list randomly.</summary>
    /// <param name="source">The list to shuffle.</param>
    /// <typeparam name="T">The list type.</typeparam>
    /// <returns>Returns a shuffled list.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new());
    }

    private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (rng is null)
        {
            throw new ArgumentNullException(nameof(rng));
        }

        return source.ShuffleIterator(rng);
    }

    private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng)
    {
        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; ++i)
        {
            var j = rng.Next(i, buffer.Count);
            yield return buffer[j];
            buffer[j] = buffer[i];
        }
    }
}