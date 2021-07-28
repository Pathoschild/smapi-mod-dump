/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalEffects
{

    public class ItemEatenEventArgs : EventArgs
    {
        public StardewValley.Item Item { get; set; }
        public StardewValley.Farmer Farmer { get; set; }
    }

    public class Events
    {
        //ItemEaten - called when a player starts eating an item. Not cancellable (because of how it's detected)
        public event ItemEatenHandler ItemEaten;
        public delegate void ItemEatenHandler(object sender, ItemEatenEventArgs args);
        internal ItemEatenEventArgs ItemEatenEvent(Farmer who, StardewValley.Item item)
        {
            var args = new ItemEatenEventArgs
            {
                Farmer = who,
                Item = item
            };
            ItemEaten?.Invoke(this, args);
            return args;
        }
    }
}
