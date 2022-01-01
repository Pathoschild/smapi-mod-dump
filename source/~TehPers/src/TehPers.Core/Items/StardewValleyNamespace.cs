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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items
{
    internal class StardewValleyNamespace : INamespaceProvider
    {
        private readonly IMonitor monitor;
        private readonly IAssetProvider gameAssets;
        private readonly Dictionary<string, IItemFactory> itemFactories;

        public string Name => NamespacedKey.StardewValleyNamespace;

        public StardewValleyNamespace(
            IMonitor monitor,
            [ContentSource(ContentSource.GameContent)]
            IAssetProvider gameAssets
        )
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.gameAssets = gameAssets ?? throw new ArgumentNullException(nameof(gameAssets));

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
            this.itemFactories.Clear();
            foreach (var (key, itemFactory) in StardewValleyNamespace.GetItemFactories(this.gameAssets))
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
            IAssetProvider assetProvider
        )
        {
            foreach (var quality in Enumerable.Range(Tool.stone, Tool.iridium - Tool.stone + 1))
            {
                // Axe
                var axeKey = NamespacedKey.SdvTool(ToolTypes.Axe, quality).Key;
                var axeFactory = new SimpleItemFactory(
                    ItemTypes.Tool,
                    () => ToolFactory.getToolFromDescription(ToolFactory.axe, quality)
                );
                yield return (axeKey, axeFactory);

                // Hoe
                var hoeKey = NamespacedKey.SdvTool(ToolTypes.Hoe, quality).Key;
                var hoeFactory = new SimpleItemFactory(
                    ItemTypes.Tool,
                    () => ToolFactory.getToolFromDescription(ToolFactory.hoe, quality)
                );
                yield return (hoeKey, hoeFactory);

                // Pickaxe
                var pickKey = NamespacedKey.SdvTool(ToolTypes.Pickaxe, quality).Key;
                var pickFactory = new SimpleItemFactory(
                    ItemTypes.Tool,
                    () => ToolFactory.getToolFromDescription(ToolFactory.pickAxe, quality)
                );
                yield return (pickKey, pickFactory);

                // Watering can
                var canKey = NamespacedKey.SdvTool(ToolTypes.WateringCan, quality).Key;
                var canFactory = new SimpleItemFactory(
                    ItemTypes.Tool,
                    () => ToolFactory.getToolFromDescription(ToolFactory.wateringCan, quality)
                );
                yield return (canKey, canFactory);

                // Fishing rod
                if (quality < Tool.iridium)
                {
                    var rodKey = NamespacedKey.SdvTool(ToolTypes.FishingRod, quality).Key;
                    var rodFactory = new SimpleItemFactory(
                        ItemTypes.Tool,
                        () => ToolFactory.getToolFromDescription(ToolFactory.fishingRod, quality)
                    );
                    yield return (rodKey, rodFactory);
                }
            }

            yield return (NamespacedKey.SdvTool(ToolTypes.MilkPail).Key,
                new SimpleItemFactory(ItemTypes.Tool, () => new MilkPail()));
            yield return (NamespacedKey.SdvTool(ToolTypes.Shears).Key,
                new SimpleItemFactory(ItemTypes.Tool, () => new Shears()));
            yield return (NamespacedKey.SdvTool(ToolTypes.Pan).Key,
                new SimpleItemFactory(ItemTypes.Tool, () => new Pan()));
            yield return (NamespacedKey.SdvTool(ToolTypes.Wand).Key,
                new SimpleItemFactory(ItemTypes.Tool, () => new Wand()));

            // Clothing
            // TODO: dynamic clothing?
            var clothingInformation = assetProvider.Load<Dictionary<int, string>>(@"Data\ClothingInformation");
            var clothingIds = clothingInformation.Keys.ToHashSet();
            foreach (var id in clothingIds)
            {
                var key = NamespacedKey.SdvClothing(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Clothing, () => new Clothing(id));
                yield return (key, itemFactory);
            }

            // Wallpapers
            foreach (var id in Enumerable.Range(0, 112))
            {
                var key = NamespacedKey.SdvWallpaper(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Wallpaper, () => new Wallpaper(id));
                yield return (key, itemFactory);
            }

            // Flooring
            foreach (var id in Enumerable.Range(0, 56))
            {
                var key = NamespacedKey.SdvFlooring(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Flooring, () => new Wallpaper(id, true));
                yield return (key, itemFactory);
            }

            // Boots
            var boots = assetProvider.Load<Dictionary<int, string>>(@"Data\Boots");
            foreach (var id in boots.Keys)
            {
                var key = NamespacedKey.SdvBoots(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Boots, () => new Boots(id));
                yield return (key, itemFactory);
            }

            // Hats
            var hats = assetProvider.Load<Dictionary<int, string>>(@"Data\hats");
            foreach (var id in hats.Keys)
            {
                var key = NamespacedKey.SdvHat(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Hat, () => new Hat(id));
                yield return (key, itemFactory);
            }

            // Weapons
            var weapons = assetProvider.Load<Dictionary<int, string>>(@"Data\weapons");
            foreach (var id in weapons.Keys)
            {
                var key = NamespacedKey.SdvWeapon(id).Key;
                var itemFactory = id switch
                {
                    >= 32 and <= 34 => new SimpleItemFactory(ItemTypes.Weapon, () => new Slingshot(id)),
                    _ => new SimpleItemFactory(ItemTypes.Weapon, () => new MeleeWeapon(id)),
                };
                yield return (key, itemFactory);
            }

            // Furniture
            var furniture = assetProvider.Load<Dictionary<int, string>>(@"Data\Furniture");
            foreach (var id in furniture.Keys)
            {
                var key = NamespacedKey.SdvFurniture(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.Furniture, () => Furniture.GetFurnitureInstance(id));
                yield return (key, itemFactory);
            }

            // Big Craftables
            var bigCraftablesInformation =
                assetProvider.Load<Dictionary<int, string>>(@"Data\BigCraftablesInformation");
            foreach (var id in bigCraftablesInformation.Keys)
            {
                var key = NamespacedKey.SdvBigCraftable(id).Key;
                var itemFactory = new SimpleItemFactory(ItemTypes.BigCraftable, () => new SObject(Vector2.Zero, id));
                yield return (key, itemFactory);
            }

            // Objects
            var objectInformation = assetProvider.Load<Dictionary<int, string>>(@"Data\ObjectInformation");
            var secretNotes = assetProvider.Load<Dictionary<int, string>>(@"Data\SecretNotes");
            foreach (var (id, rawData) in objectInformation)
            {
                var data = rawData.Split('/');
                switch (id)
                {
                    // Secret notes
                    case 79:
                    {
                        foreach (var secretNoteId in secretNotes.Keys.Where(key => key < GameLocation.JOURNAL_INDEX))
                        {
                            var key = NamespacedKey.SdvCustom(ItemTypes.Object, $"SecretNotes/{secretNoteId}").Key;
                            var itemFactory = new SimpleItemFactory(
                                ItemTypes.Object,
                                () =>
                                {
                                    var note = new SObject(id, 1);
                                    note.name += $" #{secretNoteId}";
                                    return note;
                                }
                            );
                            yield return (key, itemFactory);
                        }

                        break;
                    }

                    // Journal scraps
                    case 842:
                    {
                        foreach (var journalId in secretNotes.Keys.Where(key => key >= GameLocation.JOURNAL_INDEX))
                        {
                            var key = NamespacedKey.SdvCustom(ItemTypes.Object, $"Journals/{journalId - GameLocation.JOURNAL_INDEX}").Key;
                            var itemFactory = new SimpleItemFactory(
                                ItemTypes.Object,
                                () =>
                                {
                                    var note = new SObject(id, 1);
                                    note.name += $" #{journalId - GameLocation.JOURNAL_INDEX}";
                                    return note;
                                }
                            );
                            yield return (key, itemFactory);
                        }

                        break;
                    }

                    // Rings
                    case not 801 when data.Length >= 4 && data[3] == "Ring":
                    {
                        var key = NamespacedKey.SdvRing(id).Key;
                        var itemFactory = new SimpleItemFactory(ItemTypes.Ring, () => new Ring(id));
                        yield return (key, itemFactory);
                        break;
                    }

                    // Roe
                    case 812:
                    {
                        // TODO: Variants?
                        var key = NamespacedKey.SdvObject(id).Key;
                        var itemFactory = new SimpleItemFactory(
                            ItemTypes.Object,
                            () => new ColoredObject(id, 1, Color.White)
                        );
                        yield return (key, itemFactory);
                        break;
                    }

                    // Other objects
                    default:
                    {
                        // TODO: Variants?
                        var key = NamespacedKey.SdvObject(id).Key;
                        var itemFactory = new SimpleItemFactory(ItemTypes.Object, () => new SObject(id, 1));
                        yield return (key, itemFactory);
                        break;
                    }
                }
            }
        }
    }
}