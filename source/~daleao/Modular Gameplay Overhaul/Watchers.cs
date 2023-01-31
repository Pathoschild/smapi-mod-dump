/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul;

#region using directives

using DaLion.Shared.Watchers;

#endregion using directives

/// <summary>Holds global <see cref="IWatcher"/> instances.</summary>
internal static class Watchers
{
    /// <summary>Gets the <see cref="ICollectionWatcher{TValue}"/> which monitors changes to the local player's professions.</summary>
    internal static Lazy<ICollectionWatcher<int>> ProfessionsWatcher { get; } =
        new(() => WatcherFactory.ForNetIntList("professions", Game1.player.professions));
}
