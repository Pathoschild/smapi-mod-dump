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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.Core.Integrations.JsonAssets;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items
{
    internal class JsonAssetsNamespace : INamespaceProvider
    {
        private readonly IMonitor monitor;
        private readonly Lazy<IOptional<IJsonAssetsApi>> jaApiFactory;
        private readonly Dictionary<string, IItemFactory> itemFactories;

        public string Name => "JA";

        public JsonAssetsNamespace(IMonitor monitor, Lazy<IOptional<IJsonAssetsApi>> jaApiFactory)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.jaApiFactory =
                jaApiFactory ?? throw new ArgumentNullException(nameof(jaApiFactory));

            this.itemFactories = new();
        }

        public bool TryGetItemFactory(string key, [NotNullWhen(true)] out IItemFactory? itemFactory)
        {
            return this.itemFactories.TryGetValue(key, out itemFactory);
        }

        public IEnumerable<string> GetKnownItemKeys()
        {
            return this.itemFactories.Keys;
        }

        public void Reload()
        {
            // Clear all item factories
            this.itemFactories.Clear();

            // Try to get the mod API
            if (!this.jaApiFactory.Value.TryGetValue(out var jaApi))
            {
                return;
            }

            foreach (var (key, itemFactory) in JsonAssetsNamespace.GetItemFactories(jaApi))
            {
                if (!this.itemFactories.TryAdd(key, itemFactory))
                {
                    this.monitor.Log(
                        $"Conflicting item key: '{key}'. Some items may not be created correctly.",
                        LogLevel.Warn
                    );
                }
            }
        }

        private static IEnumerable<(string key, IItemFactory itemFactory)> GetItemFactories(
            IJsonAssetsApi jaApi
        )
        {
            // Big craftables
            foreach (var (jaKey, id) in jaApi.GetAllBigCraftableIds())
            {
                var key = $"{ItemTypes.BigCraftable}/{jaKey}";
                var itemFactory = new SimpleItemFactory(
                    ItemTypes.BigCraftable,
                    () => new SObject(Vector2.Zero, id)
                );
                yield return (key, itemFactory);
            }

            // Clothing
            foreach (var (jaKey, id) in jaApi.GetAllClothingIds())
            {
                var key = $"{ItemTypes.Clothing}/{jaKey}";
                var itemFactory = new SimpleItemFactory(ItemTypes.Clothing, () => new Clothing(id));
                yield return (key, itemFactory);
            }

            // Hats
            foreach (var (jaKey, id) in jaApi.GetAllHatIds())
            {
                var key = $"{ItemTypes.Hat}/{jaKey}";
                var itemFactory = new SimpleItemFactory(ItemTypes.Hat, () => new Hat(id));
                yield return (key, itemFactory);
            }

            // Weapons
            foreach (var (jaKey, id) in jaApi.GetAllWeaponIds())
            {
                var key = $"{ItemTypes.Weapon}/{jaKey}";
                var itemFactory = new SimpleItemFactory(
                    ItemTypes.Weapon,
                    () => new MeleeWeapon(id)
                );
                yield return (key, itemFactory);
            }

            // Objects
            foreach (var (jaKey, id) in jaApi.GetAllObjectIds())
            {
                var key = $"{ItemTypes.Object}/{jaKey}";
                var itemFactory = new SimpleItemFactory(ItemTypes.Object, () => new SObject(id, 1));
                yield return (key, itemFactory);
            }
        }
    }
}