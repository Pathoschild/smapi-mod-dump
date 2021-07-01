/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Models
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class SearchableItem
    {
        public ItemType Type { get; }
        public Item Item { get; }
        public Func<Item> CreateItem { get; }
        public int ID { get; }
        public string Name => Item.Name;
        public string DisplayName => Item.DisplayName;

        public SearchableItem(ItemType type, int id, Func<SearchableItem, Item> createItem)
        {
            Type = type;
            ID = id;
            CreateItem = () => createItem(this);
            Item = createItem(this);
        }

        public SearchableItem(SearchableItem item)
        {
            Type = item.Type;
            ID = item.ID;
            CreateItem = item.CreateItem;
            Item = item.Item;
        }

        public bool EqualsToSItem(Item item)
        {
            if (!Item.Category.Equals(item.category))
            {
                return false;
            }

            if (!Item.Name.Equals(item.Name))
            {
                return false;
            }

            if (Item.ParentSheetIndex.Equals(item.ParentSheetIndex))
            {
                if (Item is Object item1 && Item is Object item2)
                {
                    return item1.quality.Equals(item2.quality);
                }

                return true;
            }

            return false;
        }
    }
}