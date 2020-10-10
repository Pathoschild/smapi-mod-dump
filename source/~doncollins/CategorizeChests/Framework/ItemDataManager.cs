/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewObject = StardewValley.Object;

namespace StardewValleyMods.CategorizeChests.Framework
{
    class ItemDataManager : IItemDataManager
    {
        private readonly int CustomIDOffset = 1000;

        private IMonitor Monitor;

        /// <summary>
        /// A mapping of category names to the item keys belonging to that category.
        /// </summary>
        public IDictionary<string, IEnumerable<ItemKey>> Categories => _Categories;
        IDictionary<string, IEnumerable<ItemKey>> _Categories;

        /// <summary>
        /// A mapping of item keys to a representative instance of the item they
        /// correspond to.
        /// </summary>
        private Dictionary<ItemKey, Item> PrototypeMap = new Dictionary<ItemKey, Item>();

        public ItemDataManager(IMonitor monitor)
        {
            Monitor = monitor;

            var categories = new Dictionary<string, IEnumerable<ItemKey>>();

            foreach (var result in DiscoverItems())
            {
                if (ItemBlacklist.Includes(result.ItemKey))
                    continue;

                PrototypeMap[result.ItemKey] = result.Item;

                // TODO: simplify this using a KeyBy-esque function
                var categoryName = ChooseCategoryName(result.ItemKey);

                if (!categories.ContainsKey(categoryName))
                    categories[categoryName] = new List<ItemKey>();
                (categories[categoryName] as List<ItemKey>).Add(result.ItemKey);
            }

            _Categories = categories;
        }

        /// <summary>
        /// Decide what category name the given item key should belong to.
        /// </summary>
        /// <returns>The chosen category name.</returns>
        /// <param name="itemKey">The item key to categorize.</param>
        private string ChooseCategoryName(ItemKey itemKey)
        {
            if (itemKey.ItemType == ItemType.Object)
            {
                var proto = GetItem(itemKey);
                var categoryName = proto.getCategoryName();
                return String.IsNullOrEmpty(categoryName) ? "Miscellaneous" : categoryName;
            }
            else
            {
                return Enum.GetName(typeof(ItemType), itemKey.ItemType);
            }
        }

        /// <summary>
        /// Retrieve the item key appropriate for representing the given item.
        /// </summary>
        public ItemKey GetKey(Item item)
        {
            var results = PrototypeMap.Where(p => MatchesPrototype(item, p.Value));

            if (!results.Any())
                throw new ItemNotImplementedException(item);

            return results.First().Key;
        }

        /// <summary>
        /// Retrieve the representative item corresponding to the given key.
        /// </summary>
        public Item GetItem(ItemKey itemKey)
        {
            return PrototypeMap[itemKey];
        }

        /// <summary>
        /// Check whether the repository contains an item corresponding to the
        /// given key.
        /// </summary>
        public bool HasItem(ItemKey itemKey)
        {
            return PrototypeMap.ContainsKey(itemKey);
        }

        /// <summary>
        /// Generate every item known to man, or at least those we're interested
        /// in using for categorization.
        /// </summary>
        /// <remarks>
        /// Substantially based on code from Pathoschild's LookupAnything mod.
        /// </remarks>
        /// <returns>A collection of all of the item entries.</returns>
        private IEnumerable<DiscoveredItem> DiscoverItems()
        {
            // get tools
            yield return new DiscoveredItem(ItemType.Tool, ToolFactory.axe,
                ToolFactory.getToolFromDescription(ToolFactory.axe, Tool.stone));
            yield return new DiscoveredItem(ItemType.Tool, ToolFactory.hoe,
                ToolFactory.getToolFromDescription(ToolFactory.hoe, Tool.stone));
            yield return new DiscoveredItem(ItemType.Tool, ToolFactory.pickAxe,
                ToolFactory.getToolFromDescription(ToolFactory.pickAxe, Tool.stone));
            yield return new DiscoveredItem(ItemType.Tool, ToolFactory.wateringCan,
                ToolFactory.getToolFromDescription(ToolFactory.wateringCan, Tool.stone));
            yield return new DiscoveredItem(ItemType.Tool, ToolFactory.fishingRod,
                ToolFactory.getToolFromDescription(ToolFactory.fishingRod, Tool.stone));
            yield return
                new DiscoveredItem(ItemType.Tool, CustomIDOffset,
                    new MilkPail()); // these don't have any sort of ID, so we'll just assign some arbitrary ones
            yield return new DiscoveredItem(ItemType.Tool, CustomIDOffset + 1, new Shears());
            yield return new DiscoveredItem(ItemType.Tool, CustomIDOffset + 2, new Pan());

            // equipment
            foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\Boots").Keys)
                yield return new DiscoveredItem(ItemType.Boots, id, new Boots(id));
            foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\hats").Keys)
                yield return new DiscoveredItem(ItemType.Hat, id, new Hat(id));
            foreach (int id in Game1.objectInformation.Keys)
            {
                if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange)
                    yield return new DiscoveredItem(ItemType.Ring, id, new Ring(id));
            }

            // weapons
            foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\weapons").Keys)
            {
                Item weapon = (id >= 32 && id <= 34)
                    ? (Item) new Slingshot(id)
                    : new MeleeWeapon(id);
                yield return new DiscoveredItem(ItemType.Weapon, id, weapon);
            }

            // furniture
            /*foreach (int id in Game1.content.Load<Dictionary<int, string>>("Data\\Furniture").Keys)
            {
                if (id == 1466 || id == 1468)
                    yield return new DiscoveredItem(ItemType.Furniture, id, new TV(id, Vector2.Zero));
                else
                    yield return new DiscoveredItem(ItemType.Furniture, id, new Furniture(id, Vector2.Zero));
            }*/

            // craftables
            /*foreach (int id in Game1.bigCraftablesInformation.Keys)
                yield return new DiscoveredItem(ItemType.BigCraftable, id, new StardewObject(Vector2.Zero, id));*/

            // objects
            foreach (int id in Game1.objectInformation.Keys)
            {
                if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange)
                    continue; // handled separated

                StardewObject item = new StardewObject(id, 1);
                yield return new DiscoveredItem(ItemType.Object, id, item);
            }
        }

        /// <summary>
        /// Check whether a given item should be classified as the same as the
        /// given representative item for the purposes of categorization.
        /// </summary>
        /// <param name="item">The item being checked.</param>
        /// <param name="prototype">The representative prototype item to check against.</param>
        public static bool MatchesPrototype(Item item, Item prototype)
        {
            return
                // same generic item type
                (
                    item.GetType() == prototype.GetType()
                    || prototype.GetType() == typeof(StardewObject) && item.GetType() == typeof(ColoredObject)
                )
                && item.category == prototype.category
                && item.parentSheetIndex == prototype.parentSheetIndex

                // same discriminators
                && (item as Boots)?.indexInTileSheet == (prototype as Boots)?.indexInTileSheet
                && (item as BreakableContainer)?.type == (prototype as BreakableContainer)?.type
                && (item as Fence)?.isGate == (prototype as Fence)?.isGate
                && (item as Fence)?.whichType == (prototype as Fence)?.whichType
                && (item as Hat)?.which == (prototype as Hat)?.which
                && (item as Ring)?.indexInTileSheet == (prototype as Ring)?.indexInTileSheet
                && (item as MeleeWeapon)?.type == (prototype as MeleeWeapon)?.type
                && (item as MeleeWeapon)?.initialParentTileIndex == (prototype as MeleeWeapon)?.initialParentTileIndex
                ;
        }
    }

    class DiscoveredItem
    {
        public readonly ItemKey ItemKey;
        public readonly Item Item;

        public DiscoveredItem(ItemType type, int index, Item item)
        {
            ItemKey = new ItemKey(type, index);
            Item = item;
        }
    }
}