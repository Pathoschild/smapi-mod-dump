/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.API
{
    public class BundlePurchasedEventArgs : IBundlePurchasedEventArgs
    {
        public Farmer Who { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public IBundle Bundle { get; }
        public bool IsBuyer { get; }

        public BundlePurchasedEventArgs(Farmer who, string location, string locationOrUnique, string bundleKey, bool isBuyer)
        {
            this.Who = who;
            this.Location = location;
            this.LocationOrUnique = locationOrUnique;
            this.Bundle = UnlockableBundlesAPI.getBundleForAPI(bundleKey, locationOrUnique);
            this.IsBuyer = isBuyer;
        }
    }
}
