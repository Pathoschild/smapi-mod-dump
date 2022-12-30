/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

/// <summary>Specifies that a <see cref="IManagedEvent"/> should ignore its <see cref="IManagedEvent.IsEnabled"/> flag.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AlwaysEnabledEventAttribute : Attribute
{
}
