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
using System.Collections.Immutable;

namespace Unlockable_Bundles.API
{
    public interface IUnlockableBundlesAPI
    {
        /// <summary>Returns all Bundles as Keys that have been purchased</summary>
        List<string> PurchasedBundles { get; }

        /// <summary>
        /// Returns all Bundles as Keys that have been purchased including a list of all GameLocations as NameOrUniqueName where they have been purchased
        /// This makes sense when your bundle is in a building for example
        /// </summary>
        Dictionary<string, List<string>> PurchaseBundlesByLocation { get; }

        /// <summary>
        /// Returns all bundle states as Dictionar<BundleKey, List<Bundle>>
        /// A bundle key can contain multiple bundles, if the location is a building
        /// </summary>
        Dictionary<string, List<Bundle>> getBundles();

        /// <summary>Fires once for every player when a bundle contribution has been made</summary>
        event BundlesPurchasedEvent BundleContributedEvent;

        /// <summary>Fires once for every player when a bundle has been purchased before the ShopEvent</summary>
        event BundlesPurchasedEvent BundlePurchasedEvent;

        // <summary>
        // Fires once after daystart for every player after the UnlockableBundles/Bundles asset has been read and processed.
        // Also fires for joining players after they received and processed all unlockables.
        // </summary>
        event IsReadyEvent IsReadyEvent;
    }

    public class Bundle
    {
        public string Key { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public Dictionary<string, int> Price { get; }
        public Dictionary<string, int> AlreadyPaid { get; }
        public bool Purchased { get; }
        public int DaysSincePurchase { get; }
        public bool AssetLoaded { get; }
        public bool Discovered { get; }

        public Bundle(string key, string loc, string unique, Dictionary<string, int>  price, Dictionary<string, int> paid, bool purchased, int daysSincePurchase, bool assetLoaded, bool discovered)
        {
            Key = key;
            Location = loc;
            LocationOrUnique = loc;
            Price = price;
            AlreadyPaid = paid;
            Purchased = purchased;
            DaysSincePurchase = daysSincePurchase;
            AssetLoaded = assetLoaded;
            Discovered = discovered;
        }
    }

    public delegate void BundlesPurchasedEvent(object sender, BundlePurchasedEventArgs e);
    public class BundlePurchasedEventArgs : EventArgs
    {
        public Farmer Who;
        public string Location;
        public string LocationOrUnique;
        public string BundleKey;
        public bool IsBuyer;

        public BundlePurchasedEventArgs(Farmer who, string location, string locationOrUnique, string bundleKey, bool isBuyer)
        {
            this.Who = who;
            this.Location = location;
            this.LocationOrUnique = locationOrUnique;
            this.BundleKey = bundleKey;
            this.IsBuyer = isBuyer;
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
