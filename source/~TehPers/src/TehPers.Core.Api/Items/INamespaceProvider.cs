/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Provides items within a namespace.
    /// </summary>
    public interface INamespaceProvider
    {
        /// <summary>
        /// The name of the namespace.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Tries to get an item factory for an item with the given key from this item namespace.
        /// </summary>
        /// <param name="key">The key of the item local to this namespace.</param>
        /// <param name="itemFactory">An instance of an item with the given key.</param>
        /// <returns><see langword="true"/> if the key is valid for an item in this namespace, <see langword="false"/> otherwise.</returns>
        bool TryGetItemFactory(string key, [NotNullWhen(true)] out IItemFactory? itemFactory);

        /// <summary>
        /// Gets all the known, predefined item keys within this namespace. This may not return
        /// every valid item key in this namespace. For example, dynamically created items may not
        /// be returned. This should be treated as a guide for mods to know what items are
        /// available within this namespace.
        /// </summary>
        /// <returns>The known item keys.</returns>
        IEnumerable<string> GetKnownItemKeys();

        /// <summary>
        /// Reloads the namespace provider.
        /// </summary>
        void Reload();
    }
}