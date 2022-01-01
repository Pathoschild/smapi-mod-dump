/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A registry of all registered namespaces. Namespaces are reloaded whenever a save is loaded,
    /// however they can also be reloaded on demand.
    /// </summary>
    public interface INamespaceRegistry
    {
        /// <summary>
        /// Gets the registered namespaces.
        /// </summary>
        /// <returns>The registered namespaces.</returns>
        IEnumerable<string> GetRegisteredNamespaces();

        /// <summary>
        /// Tries to get an <see cref="IItemFactory"/> for the given <see cref="NamespacedKey"/>.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="factory">The item factory</param>
        /// <returns><see langword="true"/> if the key is valid for an item, <see langword="false"/> otherwise.</returns>
        bool TryGetItemFactory(NamespacedKey key, [NotNullWhen(true)] out IItemFactory? factory);

        /// <summary>
        /// Gets all the known, predefined item keys within all namespaces. This may not return
        /// every valid item key. For example, dynamically created items may not be returned. This
        /// should be treated as a guide for  mods to know what items are available within all
        /// namespaces.
        /// </summary>
        /// <returns>The known item keys.</returns>
        IEnumerable<NamespacedKey> GetKnownItemKeys();

        /// <summary>
        /// Reloads each namespace provider.
        /// </summary>
        void RequestReload();

        /// <summary>
        /// Invoked whenever the namespace provider is reloaded.
        /// </summary>
        event EventHandler? OnReload;
    }
}