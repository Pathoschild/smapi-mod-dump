/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Attributes;

using System;
using StardewModdingAPI.Events;

/// <inheritdoc />
[AttributeUsage(AttributeTargets.Method)]
public class SortedEventPriorityAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SortedEventPriorityAttribute" /> class.
    /// </summary>
    /// <param name="priority">The event priority for handlers.</param>
    public SortedEventPriorityAttribute(EventPriority priority)
    {
        this.Priority = priority;
    }

    /// <summary>
    ///     Gets the event priority for the handler.
    /// </summary>
    internal EventPriority Priority { get; }
}