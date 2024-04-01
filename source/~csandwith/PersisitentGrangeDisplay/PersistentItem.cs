/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace PersisitentGrangeDisplay
{
    public class PersistentItem
    {
        public int id;
        public int quality;

        public PersistentItem()
        {
        }
        public PersistentItem(Item item)
        {
            id = item.parentSheetIndex;
            quality = (item as Object).Quality;
        }
    }
}