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
    /// A fish availability entry.
    /// </summary>
    /// <param name="FishKey">The item key.</param>
    /// <param name="AvailabilityInfo">The availability information.</param>
    public record FishEntry(
        [property: JsonRequired] NamespacedKey FishKey,
        FishAvailabilityInfo AvailabilityInfo
    ) : Entry<FishAvailabilityInfo>(AvailabilityInfo)
    {
        /// <inheritdoc/>
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            if (namespaceRegistry.TryGetItemFactory(this.FishKey, out var factory))
            {
                item = new(factory.Create());
                return true;
            }

            item = default;
            return false;
        }
    }
}
