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
using System.Linq;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.Core.Integrations.DynamicGameAssets;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items
{
    internal class DynamicGameAssetsNamespace : INamespaceProvider
    {
        private readonly Lazy<IOptional<IDynamicGameAssetsApi>> dgaApiFactory;

        public string Name => "DGA";

        public DynamicGameAssetsNamespace(Lazy<IOptional<IDynamicGameAssetsApi>> dgaApiFactory)
        {
            this.dgaApiFactory =
                dgaApiFactory ?? throw new ArgumentNullException(nameof(dgaApiFactory));
        }

        public IEnumerable<string> GetKnownItemKeys()
        {
            // No way to list DGA IDs
            return Enumerable.Empty<string>();
        }

        public bool TryGetItemFactory(string key, [NotNullWhen(true)] out IItemFactory? itemFactory)
        {
            if (!this.dgaApiFactory.Value.TryGetValue(out var dgaApi))
            {
                itemFactory = default;
                return false;
            }

            // Simple test - see if DGA spawns an item
            if (dgaApi.SpawnDGAItem(key) is not Item item)
            {
                itemFactory = default;
                return false;
            }

            // Get item type
            var itemType = item switch
            {
                SObject { bigCraftable: { Value: true } } => ItemTypes.BigCraftable,
                Boots => ItemTypes.Boots,
                Clothing => ItemTypes.Clothing,
                Wallpaper { isFloor: { Value: true } } => ItemTypes.Flooring,
                Wallpaper => ItemTypes.Wallpaper,
                Furniture => ItemTypes.Furniture,
                Hat => ItemTypes.Hat,
                SObject => ItemTypes.Object,
                Ring => ItemTypes.Ring,
                MeleeWeapon => ItemTypes.Weapon,
                Tool => ItemTypes.Tool,
                _ => ItemTypes.Unknown,
            };

            // Create factory
            itemFactory = new ItemFactory(dgaApi, key, itemType);
            return true;
        }

        public void Reload()
        {
            // Nothing to do
        }

        private class ItemFactory : IItemFactory
        {
            private readonly IDynamicGameAssetsApi dgaApi;
            private readonly string fullId;

            public string ItemType { get; }

            public ItemFactory(IDynamicGameAssetsApi dgaApi, string fullId, string itemType)
            {
                this.dgaApi = dgaApi;
                this.fullId = fullId;
                this.ItemType = itemType;
            }

            public Item Create()
            {
                var spawnedItem = this.dgaApi.SpawnDGAItem(this.fullId);
                return spawnedItem is Item item
                    ? item
                    : throw new InvalidOperationException(
                        $"DGA returned a non-item for {this.fullId}: {spawnedItem}"
                    );
            }
        }
    }
}