/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Extensions;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for treasure loot.
    /// </summary>
    /// <param name="AvailabilityInfo">The availability information.</param>
    /// <param name="ItemKeys">The possible namespaced keys for the loot. The item key is chosen randomly.</param>
    public record TreasureEntry(
        AvailabilityInfo AvailabilityInfo,
        [property: JsonRequired]
        ImmutableArray<NamespacedKey> ItemKeys
    ) : Entry<AvailabilityInfo>(AvailabilityInfo)
    {
        /// <summary>
        /// The minimum quantity of this item. This is only valid for stackable items.
        /// </summary>
        [DefaultValue(1)]
        public int MinQuantity { get; init; } = 1;

        /// <summary>
        /// The maximum quantity of this item. This is only valid for stackable items.
        /// </summary>
        [DefaultValue(1)]
        public int MaxQuantity { get; init; } = 1;

        /// <summary>
        /// Whether this can be found multiple times in one chest.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowDuplicates { get; init; } = true;

        /// <inheritdoc/>
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            var itemKey = this.ItemKeys.ToWeighted(_ => 1).ChooseOrDefault(Game1.random);
            if (itemKey is { Value: var key }
                && namespaceRegistry.TryGetItemFactory(key, out var factory))
            {
                item = new(factory.Create());
                if (item.Item is SObject obj)
                {
                    // Random quantity
                    obj.Stack = Game1.random.Next(this.MinQuantity, this.MaxQuantity);
                }

                return true;
            }

            item = default;
            return false;
        }
    }
}
