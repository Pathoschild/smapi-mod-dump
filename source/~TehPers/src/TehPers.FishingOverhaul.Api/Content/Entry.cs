/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for a fishing item.
    /// </summary>
    /// <typeparam name="T">The type of availability for this entry.</typeparam>
    /// <param name="AvailabilityInfo">The availability information.</param>
    public abstract record Entry<T>([property: JsonRequired] T AvailabilityInfo)
        where T : AvailabilityInfo
    {
        /// <summary>
        /// Actions to perform when this is caught.
        /// </summary>
        public CatchActions? OnCatch { get; init; } = null;

        /// <summary>
        /// Tries to create an instance of the item this entry represents.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <param name="namespaceRegistry">The namespace registry.</param>
        /// <param name="item">The item that was created, if possible.</param>
        /// <returns><see langword="true"/> if the item was created, <see langword="false"/> otherwise.</returns>
        public abstract bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        );
    }
}
