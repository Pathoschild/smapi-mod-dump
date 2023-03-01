/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Unlockable_Areas.API
{
    public delegate void ShopPurchasedEvent(object sender, ShopPurchasedEventArgs e);

    public class ShopPurchasedEventArgs : EventArgs
    {
        public Farmer who;
        public string location;
        public string locationOrUnique;
        public string unlockableKey;
        public bool isBuyer;

        public ShopPurchasedEventArgs(Farmer who, string location, string locationOrUnique, string unlockableKey, bool isBuyer)
        {
            this.who = who;
            this.location = location;
            this.locationOrUnique = locationOrUnique;
            this.unlockableKey = unlockableKey;
            this.isBuyer = isBuyer;
        }
    }
}
