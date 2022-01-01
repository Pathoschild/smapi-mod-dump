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
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Content
{
    internal record GoldenWalnutEntry(AvailabilityInfo AvailabilityInfo) : TrashEntry(
        NamespacedKey.SdvObject(73),
        AvailabilityInfo
    )
    {
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            if (!Game1.IsMultiplayer)
            {
                return base.TryCreateItem(fishingInfo, namespaceRegistry, out item);
            }

            item = default;
            return false;
        }
    }
}