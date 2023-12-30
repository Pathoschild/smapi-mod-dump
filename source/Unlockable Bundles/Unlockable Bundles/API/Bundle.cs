/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.API
{
    public class Bundle : IBundle
    {
        public string Key { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public IDictionary<string, int> Price { get; }
        public IDictionary<string, int> AlreadyPaid { get; }
        public bool Purchased { get; }
        public int DaysSincePurchase { get; }
        public bool AssetLoaded { get; }
        public bool Discovered { get; }

        public Bundle(string key, string loc, string unique, Dictionary<string, int> price, Dictionary<string, int> paid, bool purchased, int daysSincePurchase, bool assetLoaded, bool discovered)
        {
            Key = key;
            Location = loc;
            LocationOrUnique = unique;
            Price = price;
            AlreadyPaid = paid;
            Purchased = purchased;
            DaysSincePurchase = daysSincePurchase;
            AssetLoaded = assetLoaded;
            Discovered = discovered;
        }
    }
}
