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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    public class ModDataCategoryRule
    {
        public ISet<string> Class { get; set; }
        public ISet<string> ObjType { get; set; }
        public ISet<int> ObjCategory { get; set; }
        public ISet<string> ItemId { get; set; }
        public ISet<string> UniqueKey { get; set; }

        public bool IsMatch(SearchableItem entry)
        {
            var item = entry.Item;
            var obj = item as Object;
            var key = Utils.Helpers.GetItemUniqueKey(item);

            if (UniqueKey.Any() && UniqueKey.Contains(key))
            {
                return true;
            }

            if (Class.Any() && GetClassFullNames(item).Any(className => Class.Contains(className)))
            {
                return true;
            }

            if (ObjCategory.Any() && ObjCategory.Contains(item.Category))
            {
                return true;
            }

            if (ObjType.Any() && obj != null && ObjType.Contains(obj.Type))
            {
                return true;
            }

            if (ItemId.Any() && ItemId.Contains($"{entry.Type}:{item.ParentSheetIndex}"))
            {
                return true;
            }

            return false;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Class = new HashSet<string>(Class ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ObjType = new HashSet<string>(ObjType ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ItemId = new HashSet<string>(ItemId ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            UniqueKey = new HashSet<string>(UniqueKey ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ObjCategory ??= new HashSet<int>();
        }

        private IEnumerable<string> GetClassFullNames(Item item)
        {
            for (var type = item.GetType(); type != null; type = type.BaseType)
            {
                yield return type.FullName;
            }
        }
    }
}