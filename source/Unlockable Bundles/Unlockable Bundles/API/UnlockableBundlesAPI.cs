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
using static Unlockable_Bundles.API.IUnlockableBundlesAPI;

namespace Unlockable_Bundles.API
{
    public class UnlockableBundlesAPI : IUnlockableBundlesAPI
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static IList<string> CachedPurchasedBundles = null;
        private static IDictionary<string, IList<string>> CachedPurchasedBundlesByLocation = null;
        private static IList<string> CachedDiscoveredBundles = null;
        private static IDictionary<string, IList<IBundle>> CachedBundles = null;
        public IList<string> PurchasedBundles => getPurchasedUnlockables();

        public IDictionary<string, IList<string>> PurchaseBundlesByLocation => getPurchasedUnlockablesByLocation();

        public IDictionary<string, IList<IBundle>> getBundles() => getAllBundleStates();

        public event BundlesPurchasedDelegate BundlePurchasedEvent;
        public event IsReadyDelegate IsReadyEvent;
        public event BundlesContributedDelegate BundleContributedEvent;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public static IList<string> getPurchasedUnlockables()
        {
            //We are not currently loading a savegame and are not between daystart and dayending
            if (SaveGame.loaded is null && !Context.IsWorldReady)
                return null;

            if (ModData.Instance is null)
                if (Context.IsMainPlayer)
                    SaveDataEvents.LoadModData();
                else
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

        public static IDictionary<string, IList<string>> getPurchasedUnlockablesByLocation()
        {
            if (SaveGame.loaded is null && !Context.IsWorldReady)
                return null;

            if (ModData.Instance is null)
                if (Context.IsMainPlayer)
                    SaveDataEvents.LoadModData();
                else
                    return null;

            if (CachedPurchasedBundlesByLocation != null)
                return CachedPurchasedBundlesByLocation;

            var result = new Dictionary<string, IList<string>>();

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

        public static IList<string> getDiscoveredUnlockables()
        {
            //We are not currently loading a savegame and are not between daystart and dayending
            if (SaveGame.loaded is null && !Context.IsWorldReady)
                return null;

            if (ModData.Instance is null)
                if (Context.IsMainPlayer)
                    SaveDataEvents.LoadModData();
                else
                    return null;

            if (CachedDiscoveredBundles != null)
                return CachedDiscoveredBundles;

            var ret = new List<string>();

            foreach (var e in ModData.Instance.UnlockableSaveData)
                if (e.Value.Any(el => el.Value.Discovered))
                    ret.Add(e.Key);

            CachedDiscoveredBundles = ret;
            return CachedDiscoveredBundles;
        }

        public static IDictionary<string, IList<IBundle>> getAllBundleStates()
        {
            if (ModData.Instance is null)
                return null;

            if (CachedBundles is not null)
                return CachedBundles;

            var asset = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");

            var result = new Dictionary<string, IList<IBundle>>();

            foreach (var el in ModData.Instance.UnlockableSaveData) {
                var key = el.Key;
                var unlockable = asset.ContainsKey(key) ? asset[key] : null;
                var list = new List<IBundle>();

                foreach (var location in el.Value)
                    list.Add(getBundleForAPI(el.Key, location.Key, unlockable, location.Value));

                result.Add(key, list);
            }

            CachedBundles = result;
            return CachedBundles;
        }

        public static Bundle getBundleForAPI(string key, string location)
        {
            var asset = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            var unlockable = asset.ContainsKey(key) ? asset[key] : null;
            var saveData = ModData.getUnlockableSaveData(key, location);
            return getBundleForAPI(key, location, unlockable, saveData);
        }

        public static Bundle getBundleForAPI(string key, string location, UnlockableModel unlockable, UnlockableSaveData saveData)
        {
            return new Bundle(
                    key,
                    unlockable != null ? unlockable.Location : null,
                    location,
                    new Dictionary<string, int>(unlockable.RandomPriceEntries == 0 ? unlockable.Price : saveData.Price),
                    new Dictionary<string, int>(saveData.AlreadyPaid),
                    saveData.Purchased,
                    saveData.DayPurchased == -1 ? -1 : Game1.Date.TotalDays - saveData.DayPurchased,
                    unlockable != null,
                    saveData.Discovered
                   );
        }

        public void raiseShopContributed(BundleContributedEventArgs args) => BundleContributedEvent?.Invoke(this, args);
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
            CachedDiscoveredBundles = null;
            CachedBundles = null;
            ContentPatcherHandling.DaysSincePurchaseToken.RequiresContextUpdate = true;
        }
    }
}
