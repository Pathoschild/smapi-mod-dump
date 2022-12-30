/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using System.Collections.Generic;

namespace SlimeProduce
{
    public class DropTable
    {
        public ColorRange colorRange { get; set; }
        public List<ItemDrop> itemDrops { get; set; }
        public DropTable(ColorRange c, List<ItemDrop> d)
        {
            colorRange = c;
            itemDrops = d;
        }
    }
}
