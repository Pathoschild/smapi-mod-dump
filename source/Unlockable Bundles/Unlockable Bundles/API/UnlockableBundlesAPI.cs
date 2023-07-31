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
using StardewValley;
using StardewModdingAPI;
using Unlockable_Bundles.Lib;

namespace Unlockable_Bundles.API
{
    public class UnlockableBundlesAPI : IUnlockableBundlesAPI
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static List<string> CachedPurchasedBundles = null;
        private static Dictionary<string, List<string>> CachedPurchasedBundlesByLocation = null;
        private static Dictionary<string, List<Bundle>> CachedBundles = null;
        public List<string> PurchasedBundles => getPurchasedUnlockables();

        public Dictionary<string, List<string>> PurchaseBundlesByLocation => getPurchasedUnlockablesByLocation();

        public Dictionary<string, List<Bundle>> getBundles() => getAllBundleStates();

        public event BundlesPurchasedEvent BundlePurchasedEvent;
        public event IsReadyEvent IsReadyEvent;
        public event BundlesPurchasedEvent BundleContributedEvent;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public static List<string> getPurchasedUnlockables()
        {
            if (!Context.IsWorldReady || ModData.Instance is null)
                return null;

            if (CachedPurchasedBundles != null)
                return CachedPurchasedBundles;

            var ret = new List<string>();

            foreach (var e in ModData.Instance.UnlockableSaveData)
                if (e.Value.Any(el => el.Value.Purchased))
                    ret.Add(e.Key);

            CachedPurchasedBundles = ret;
            return CachedPurchasedBundles;
        }

        public static Dictionary<string, List<string>> getPurchasedUnlockablesByLocation()
        {
            if (ModData.Instance is null)
                return null;

            if (CachedPurchasedBundlesByLocation != null)
                return CachedPurchasedBundlesByLocation;

            var result = new Dictionary<string, List<string>>();

            foreach (var keyLocationPair in ModData.Instance.UnlockableSaveData) {
                var list = new List<string>();

                foreach (var locationStatePair in keyLocationPair.Value.Where(el => el.Value.Purchased))
                    list.Add(locationStatePair.Key);

                if (list.Count > 0)
                    result.Add(keyLocationPair.Key, list);
            }

            CachedPurchasedBundlesByLocation = result;
            return CachedPurchasedBundlesByLocation;
        }

        public static Dictionary<string, List<Bundle>> getAllBundleStates()
        {
            if (ModData.Instance is null)
                return null;

            if (CachedBundles is not null)
                return CachedBundles;

            var asset = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");

            var result = new Dictionary<string, List<Bundle>>();

            foreach (var el in ModData.Instance.UnlockableSaveData) {
                var key = el.Key;
                var bundle = asset.ContainsKey(key) ? asset[key] : null;
                var list = new List<Bundle>();


                foreach (var location in el.Value)
                    list.Add(new Bundle(
                            key,
                            bundle != null ? bundle.Location : null,
                            location.Key,
                            new Dictionary<string,int>(bundle.RandomPriceEntries == 0 ? bundle.Price : location.Value.Price),
                            new Dictionary<string, int>(location.Value.AlreadyPaid),
                            location.Value.Purchased,
                            location.Value.DayPurchased == -1 ? -1 :Game1.Date.TotalDays - location.Value.DayPurchased,
                            bundle != null
                        ));

                result.Add(key, list);
            }

            CachedBundles = result;
            return CachedBundles;
        }

        public void raiseShopContributed(BundlePurchasedEventArgs args) => BundleContributedEvent?.Invoke(this, args);
        public void raiseShopPurchased(BundlePurchasedEventArgs args) => BundlePurchasedEvent?.Invoke(this, args);
        public void raiseIsReady(IsReadyEventArgs args)
        {
            ContentPatcherHandling.DaysSincePurchaseToken.Ready = true;
            IsReadyEvent?.Invoke(this, args);
        }

        public static void clearCache()
        {
            CachedPurchasedBundles = null;
            CachedPurchasedBundlesByLocation = null;
            CachedBundles = null;
            ContentPatcherHandling.DaysSincePurchaseToken.RequiresContextUpdate = true;
        }
    }
}
