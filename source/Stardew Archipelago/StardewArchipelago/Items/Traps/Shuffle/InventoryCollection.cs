/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StardewArchipelago.Items.Traps.Shuffle
{
    public class InventoryCollection : IEnumerable<Inventory>
    {
        public List<Inventory> Inventories { get; set; }
        public int Count => Inventories.Count;

        public InventoryCollection()
        {
            Inventories = new List<Inventory>();
        }

        public InventoryCollection(Inventory inventory)
        {
            Inventories = new List<Inventory> { inventory };
        }

        public InventoryCollection(IEnumerable<Inventory> inventories)
        {
            Inventories = new List<Inventory>(inventories);
        }

        public InventoryContent GetTotalContent()
        {
            return new InventoryContent(Inventories.SelectMany(x => x.Content.Content));
        }

        public void Add(Inventory inventoryInfo)
        {
            Inventories.Add(inventoryInfo);
        }

        public void Add(Tuple<InventoryInfo, InventoryContent> inventoryInfo)
        {
            Add(new Inventory(inventoryInfo.Item1, inventoryInfo.Item2));
        }

        public void Add(InventoryInfo inventoryInfo, InventoryContent content)
        {
            Add(new Inventory(inventoryInfo, content));
        }

        public void AddRange(IEnumerable<Inventory> inventories)
        {
            foreach (var inventoryInfo in inventories)
            {
                Add(inventoryInfo);
            }
        }

        public void RemoveInvalidInventories()
        {
            Inventories.RemoveAll(x => x == null || x.Info == null || x.Content == null ||
                                       !x.Content.Any() || x.Content.All(y => y.Value == null));
        }

        public int Sum(Func<Inventory, int> sumFunction)
        {
            return Inventories.Sum(sumFunction);
        }

        public IEnumerator<Inventory> GetEnumerator()
        {
            return Inventories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public InventoryCollection GetMergedWith(InventoryCollection otherInventoryGroup)
        {
            return new InventoryCollection(Inventories.Union(otherInventoryGroup.Inventories));
        }
    }
}
