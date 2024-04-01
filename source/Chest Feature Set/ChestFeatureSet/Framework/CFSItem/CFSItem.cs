/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

namespace ChestFeatureSet.Framework.CFSItem
{
    public class CFSItem
    {
        public string ItemId { get; }
        public int Quality { get; }

        public CFSItem(string itemId, int quality)
        {
            this.ItemId = itemId;
            this.Quality = quality;
        }
    }

    public class SaveCFSItem
    {
        public readonly IEnumerable<CFSItem> Items;

        public SaveCFSItem(IEnumerable<CFSItem> items)
        {
            this.Items = items;
        }
    }
}
