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
    public interface IUnlockableAreasAPI
    {
        /// <summary>Returns all Unlockables as Keys that have been purchased</summary>
        List<string> purchasedUnlockables { get; }

        /// <summary>
        /// Returns all Unlockables as Keys that have been purchased including a list of all GameLocations as NameOrUniqueName where they have been purchased
        /// This makes sense when your unlockable is in a building for example
        /// </summary>
        Dictionary<string, List<string>> purchasedUnlockablesByLocation { get; }

        /// <summary>Fires once for every player when a shop has been purchased before the ShopEvent</summary>
        event ShopPurchasedEvent shopPurchasedEvent;

        // <summary>
        // Fires once after daystart for every player after the UnlockableAreas/Unlockables asset has been read and processed.
        // Also fires for joining players after they received and processed all unlockables.
        // </summary>
        event IsReadyEvent isReadyEvent;
    }

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

    public delegate void IsReadyEvent(object sender, IsReadyEventArgs e);
    public class IsReadyEventArgs : EventArgs
    {
        public Farmer who;
        public IsReadyEventArgs(Farmer who)
        {
            this.who = who;
        }
    }
}
