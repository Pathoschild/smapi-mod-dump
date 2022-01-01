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
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Content
{
    internal record CarolineNecklaceEntry(AvailabilityInfo AvailabilityInfo) : TrashEntry(
        NamespacedKey.SdvObject(GameLocation.CAROLINES_NECKLACE_ITEM),
        AvailabilityInfo
    )
    {
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            item = new(
                new SObject(GameLocation.CAROLINES_NECKLACE_ITEM, 1)
                {
                    questItem = { Value = true }
                }
            );
            return true;
        }
    }
}