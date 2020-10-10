/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/StardewValleyModding
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrobusSellsLargerStacks
{
    public class KrobusItem
    {
        public int ItemQuantity { get; set; }
        public int ItemId { get; set; }
        public string Type { get; set; }

        public KrobusItem(int itemQuantity, int itemId = 0, string type = "")
        {
            ItemQuantity = itemQuantity;
            ItemId = itemId;
            Type = type;
        }
    }
}
