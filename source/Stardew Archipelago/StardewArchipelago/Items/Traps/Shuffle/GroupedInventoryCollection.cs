/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StardewArchipelago.Items.Traps.Shuffle
{
    public class GroupedInventoryCollection : IEnumerable<InventoryCollection>
    {
        public List<InventoryCollection> InventoryGroups { get; set; }
        public int Count => InventoryGroups.Count;

        public GroupedInventoryCollection(InventoryCollection inventories)
        {
            InventoryGroups = new List<InventoryCollection>();
            foreach (var inventory in inventories.Inventories)
            {
                InventoryGroups.Add(new InventoryCollection(inventory));
            }
        }

        public InventoryContent GetTotalContent()
        {
            return new InventoryContent(InventoryGroups.SelectMany(x => x.GetTotalContent()));
        }

        public void Add(InventoryCollection inventoryGroup)
        {
            InventoryGroups.Add(inventoryGroup);
        }

        public InventoryCollection this[int i] => InventoryGroups[i];

        public void Remove(int mergeIndex1, int mergeIndex2)
        {
            if (mergeIndex1 > mergeIndex2)
            {
                InventoryGroups.RemoveAt(mergeIndex1);
                InventoryGroups.RemoveAt(mergeIndex2);
            }
            else
            {
                InventoryGroups.RemoveAt(mergeIndex2);
                InventoryGroups.RemoveAt(mergeIndex1);
            }
        }

        public IEnumerator<InventoryCollection> GetEnumerator()
        {
            return InventoryGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
