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

        public List<string> purchasedUnlockables => getPurchasedUnlockables().ToList();

        public Dictionary<string, List<string>> purchasedUnlockablesByLocation => getPurchasedUnlockablesByLocation();

        public event ShopPurchasedEvent shopPurchasedEvent;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public static IEnumerable<string> getPurchasedUnlockables()
        {
            if (!Context.IsWorldReady)
                yield break;

            if (ModData.Instance != null)
                foreach (var e in ModData.Instance.UnlockablePurchased)
                    if (e.Value.Any(el => el.Value == true))
                        yield return e.Key;
        }

        public static Dictionary<string, List<string>> getPurchasedUnlockablesByLocation()
        {
            if (ModData.Instance is null)
                return null;

            var result = new Dictionary<string, List<string>>();

            foreach (var keyLocationPair in ModData.Instance.UnlockablePurchased) {
                var list = new List<string>();

                foreach (var locationStatePair in keyLocationPair.Value.Where(el => el.Value == true))
                        list.Add(locationStatePair.Key);

                if (list.Count > 0)
                    result.Add(keyLocationPair.Key, list);
            }

            return result;
        }

        public void raiseShopPurchased(ShopPurchasedEventArgs args)
        {
            shopPurchasedEvent?.Invoke(this, args);
        }
    }
}
