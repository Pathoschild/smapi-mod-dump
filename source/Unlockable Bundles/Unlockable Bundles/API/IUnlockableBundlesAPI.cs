/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
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
        IList<string> PurchasedBundles { get; }

        /// <summary>
        /// Returns all Bundles as Keys that have been purchased including a list of all GameLocations as NameOrUniqueName where they have been purchased
        /// This makes sense when your bundle is in a building for example
        /// </summary>
        IDictionary<string, IList<string>> PurchaseBundlesByLocation { get; }

        /// <summary>
        /// Returns all bundle states as Dictionar<BundleKey, List<Bundle>>
        /// A bundle key can contain multiple bundles, if the location is a building
        /// </summary>
        IDictionary<string, IList<IBundle>> getBundles();

        /// <summary>Fires once for every player when a bundle contribution has been made</summary>
        event BundlesContributedDelegate BundleContributedEvent;

        /// <summary>Fires once for every player when a bundle has been purchased before the ShopEvent</summary>
        event BundlesPurchasedDelegate BundlePurchasedEvent;

        /// <summary> Returns the wallet currency value of a player </summary>
        /// <param name="currencyId"></param>
        /// <param name="who">The players unique multiplayer id</param>
        int getWalletCurrency(string currencyId, long who);

        /// <summary>Adds a value to a players wallet currency</summary>
        /// <param name="currencyId"></param>
        /// <param name="who">The players unique multiplayer id</param>
        /// <param name="addedValue">The value to add</param>
        /// <param name="broadcast">
        ///     Whether this change should be applied to every other players savedata.
        ///     This only affects 'who's wallet.
        ///     Unless you're intending to handle multiplayer yourself, always set this to true</param>
        /// <param name="registerBillboard">Whether the billboard should be shown to show the currency change</param>
        /// <returns>The new wallet currency value</returns>
        int addWalletCurrency(string currencyId, long who, int addedValue, bool broadcast, bool registerBillboard);

        // <summary>
        // Fires once after daystart for every player after the UnlockableBundles/Bundles asset has been read and processed.
        // Also fires for joining players after they received and processed all unlockables.
        // </summary>
        event IsReadyDelegate IsReadyEvent;

        public delegate void BundlesPurchasedDelegate(object sender, IBundlePurchasedEventArgs e);
        public delegate void BundlesContributedDelegate(object sender, IBundleContributedEventArgs e);
        public delegate void IsReadyDelegate(object sender, IIsReadyEventArgs e);
    }

    public interface IBundle
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
    }
    public interface IBundlePurchasedEventArgs
    {
        public Farmer Who { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public IBundle Bundle { get; }
        public bool IsBuyer { get; }
    }
    public interface IBundleContributedEventArgs
    {
        public Farmer Who { get; }
        public KeyValuePair<string, int> Contribution { get; }
        public string Location { get; }
        public string LocationOrUnique { get; }
        public IBundle Bundle { get; }
        public bool IsContributor { get; }
    }
    public interface IIsReadyEventArgs
    {
        public Farmer Who { get; }
    }
}
