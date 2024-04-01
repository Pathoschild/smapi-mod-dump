/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Interfaces;

using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;

/// <inheritdoc />
/// <typeparam name="TSource">The source object type.</typeparam>
internal interface IStorageContainer<TSource> : IStorageContainer
    where TSource : class
{
    /// <summary>Gets a value indicating whether the source object is still alive.</summary>
    public bool IsAlive { get; }

    /// <summary>Gets a weak reference to the source object.</summary>
    public WeakReference<TSource> Source { get; }
}