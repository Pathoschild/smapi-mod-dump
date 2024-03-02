/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Items.Traps.Shuffle
{
    public class Inventory
    {
        public InventoryInfo Info { get; set; }
        public InventoryContent Content { get; set; }

        public Inventory(InventoryInfo info, InventoryContent content)
        {
            Info = info;
            Content = content;
        }
    }
}