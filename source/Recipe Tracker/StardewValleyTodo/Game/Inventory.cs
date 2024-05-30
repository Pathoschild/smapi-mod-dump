/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using NativeInventory = StardewValley.Inventories.Inventory;

namespace StardewValleyTodo.Game {
    public class Inventory {
        private readonly NativeInventory _nativeInventory;
        private Dictionary<string, int> _items;
        private Dictionary<string, HashSet<string>> _byCategory;

        /// <summary>
        /// Represents player's inventory.
        /// </summary>
        /// <param name="inventory">Native inventory structure</param>
        public Inventory(NativeInventory inventory) {
            _nativeInventory = inventory;
            Startup(inventory);
        }

        private void Startup(NativeInventory inventory) {
            _items = new Dictionary<string, int>();
            _byCategory = new Dictionary<string, HashSet<string>>();

            foreach (var item in inventory) {
                if (item == null) {
                    // Skip empty slots
                    continue;
                }

                var itemId = item.ItemId;
                var categoryId = item.Category.ToString();
                var count = item.Stack;

                if (_byCategory.ContainsKey(categoryId)) {
                    _byCategory[categoryId].Add(itemId);
                } else {
                    _byCategory[categoryId] = new HashSet<string> { itemId };
                }

                if (_items.ContainsKey(itemId)) {
                    _items[itemId] += count;
                } else {
                    _items.Add(itemId, count);
                }
            }
        }

        public void Update() {
            Startup(_nativeInventory);
        }

        /// <summary>
        /// Returns amount of items of specified id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int Get(string itemId) {
            if (_items.ContainsKey(itemId)) {
                return _items[itemId];
            } else {
                return 0;
            }
        }

        public int GetCountByCategory(string categoryId) {
            if (_byCategory.ContainsKey(categoryId)) {
                var items = _byCategory[categoryId];

                return items.Select(Get).Sum();
            }

            return 0;
        }
    }
}
