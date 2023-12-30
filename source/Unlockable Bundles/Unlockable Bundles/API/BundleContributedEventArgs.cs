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
    public class BundleContributedEventArgs : IBundleContributedEventArgs
    {
        public Farmer Who { get; }
        public KeyValuePair<string, int> Contribution { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public IBundle Bundle { get; }
        public bool IsContributor { get; }

        public BundleContributedEventArgs(Farmer who, KeyValuePair<string, int> contribution, string location, string locationOrUnique, string bundleKey, bool isContributor)
        {
            this.Who = who;
            this.Contribution = contribution;
            this.Location = location;
            this.LocationOrUnique = locationOrUnique;
            this.Bundle = UnlockableBundlesAPI.getBundleForAPI(bundleKey, locationOrUnique);
            this.IsContributor = isContributor;
        }

    }
}
