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
using StardewModdingAPI;
using Unlockable_Areas.Lib;

namespace Unlockable_Areas.API
{
    public class UnlockableAreasAPI : IUnlockableAreasAPI
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static List<string> CachedPurchasedUnlockables = null;
        private static Dictionary<string, List<string>> CachedPurchasedUnlockablesByLocation = null;
        public List<string> purchasedUnlockables => getPurchasedUnlockables();

        public Dictionary<string, List<string>> purchasedUnlockablesByLocation => getPurchasedUnlockablesByLocation();

        public event ShopPurchasedEvent shopPurchasedEvent;
        public event IsReadyEvent isReadyEvent;

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

            if (CachedPurchasedUnlockables != null)
                return CachedPurchasedUnlockables;

            var ret = new List<string>();

            foreach (var e in ModData.Instance.UnlockablePurchased)
                if (e.Value.Any(el => el.Value == true))
                    ret.Add(e.Key);

            CachedPurchasedUnlockables = ret;
            return ret;
        }

        public static Dictionary<string, List<string>> getPurchasedUnlockablesByLocation()
        {
            if (ModData.Instance is null)
                return null;

            if (CachedPurchasedUnlockablesByLocation != null)
                return CachedPurchasedUnlockablesByLocation;

            var result = new Dictionary<string, List<string>>();

            foreach (var keyLocationPair in ModData.Instance.UnlockablePurchased) {
                var list = new List<string>();

                foreach (var locationStatePair in keyLocationPair.Value.Where(el => el.Value == true))
                    list.Add(locationStatePair.Key);

                if (list.Count > 0)
                    result.Add(keyLocationPair.Key, list);
            }

            CachedPurchasedUnlockablesByLocation = result;

            return result;
        }

        public void raiseShopPurchased(ShopPurchasedEventArgs args) => shopPurchasedEvent?.Invoke(this, args);
        public void raiseIsReady(IsReadyEventArgs args) => isReadyEvent?.Invoke(this, args);

        public static void clearCache()
        {
            CachedPurchasedUnlockables = null;
            CachedPurchasedUnlockablesByLocation = null;
        }
    }
}
