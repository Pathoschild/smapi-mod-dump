/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Tools;

namespace ConvenientChests.CategorizeChests.Framework {
    internal class ItemDataManager : IItemDataManager {
        /// <summary>
        /// A mapping of category names to the item keys belonging to that category.
        /// </summary>
        public Dictionary<string, IList<ItemKey>> Categories { get; } = CreateCategories();


        private static Dictionary<string, IList<ItemKey>> CreateCategories()
            => DiscoverItems()
              .Select(item => item.ToItemKey())
              .Where(key => !ItemBlacklist.Includes(key))
              .GroupBy(key => key.GetCategory())
              .ToDictionary(
                            g => g.Key,
                            g => (IList<ItemKey>) g.ToList()
                           );

        /// <summary>
        /// Generate every item in the games ItemRegistry
        /// </summary>
        private static IEnumerable<Item> DiscoverItems()
            => ItemRegistry.ItemTypes.SelectMany(ItemHelper.GetAllItems)
                           .Where(FilterTools);

        private static bool FilterTools(Item item) {
            switch (item) {
                case GenericTool:
                case MeleeWeapon { ItemId: not MeleeWeapon.scytheId } m when m.isScythe():
                case Tool { UpgradeLevel: not 0 } and (Axe or Pickaxe or Hoe or FishingRod or WateringCan):
                case Tool { UpgradeLevel: > 1 }:
                    return false;

                default:
                    return true;
            }
        }
    }
}