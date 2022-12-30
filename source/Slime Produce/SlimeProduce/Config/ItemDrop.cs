/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

namespace SlimeProduce
{
    public class ItemDrop
    {
        public int parentSheetIndex { get; set; }
        public int minDrop { get; set; }
        public int maxDrop { get; set; }
        public double dropChance { get; set; }

        public ItemDrop(int index, int min, int max, double chance = 1.0)
        {
            parentSheetIndex = index;
            maxDrop = max;
            minDrop = min;
            dropChance = chance;
        }
    }
}
