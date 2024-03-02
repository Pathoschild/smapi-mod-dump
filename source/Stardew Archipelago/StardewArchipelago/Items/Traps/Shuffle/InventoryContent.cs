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
using StardewValley;

namespace StardewArchipelago.Items.Traps.Shuffle
{
    public class InventoryContent : IEnumerable<KeyValuePair<ItemSlot, Item>>
    {
        public Dictionary<ItemSlot, Item> Content { get; set; }
        public int Count => Content.Count;

        public InventoryContent()
        {
            Content = new Dictionary<ItemSlot, Item>();
        }

        public InventoryContent(Dictionary<ItemSlot, Item> content)
        {
            Content = content;
        }

        public InventoryContent(IEnumerable<KeyValuePair<ItemSlot, Item>> content) : this(content.ToDictionary(x => x.Key, x => x.Value))
        {
        }

        public void Add(ItemSlot slot, Item item)
        {
            Content.Add(slot, item);
        }

        public bool Any()
        {
            return Content.Any();
        }

        public bool All(Func<KeyValuePair<ItemSlot, Item>, bool> condition)
        {
            return Content.All(condition);
        }

        public IEnumerator<KeyValuePair<ItemSlot, Item>> GetEnumerator()
        {
            return Content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(ItemSlot slot)
        {
            return Content.ContainsKey(slot);
        }

        public void Remove(ItemSlot slot)
        {
            Content.Remove(slot);
        }
    }
}