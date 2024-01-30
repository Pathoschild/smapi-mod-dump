/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-desert-bloom-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Unlockable_Bundles.API;
using Desert_Bloom.Lib;

namespace Desert_Bloom.API
{
    public class UnlockableBundlesHandler
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static void main()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var unlockableBundlesAPI = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>("DLX.Bundles");
            unlockableBundlesAPI.BundlePurchasedEvent += onShopPurchased;
            unlockableBundlesAPI.IsReadyEvent += isReady;
        }

        private static void isReady(object sender, IIsReadyEventArgs e)
        {
            if (!ModEntry.IsMyFarm())
                return;

            assignMillTier();
            CustomTerrainSpawns.DayStarted();
        }
        private static void assignMillTier()
        {
            Lib.Mill.Tier = 0;
            var unlockableBundlesAPI = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>("DLX.Bundles");
            foreach (var bundle in unlockableBundlesAPI.PurchasedBundles.Where(el => el.StartsWith("DLX.Desert_Bloom.Mill_Tier"))) {
                var tier = int.Parse(bundle.Last().ToString());
                if (tier > Lib.Mill.Tier)
                    Lib.Mill.Tier = tier;
            }
        }

        private static void onShopPurchased(object source, IBundlePurchasedEventArgs e)
        {
            if (!ModEntry.IsMyFarm())
                return;

            if (e.Bundle.Key.StartsWith("DLX.Desert_Bloom.Mill_Tier"))
                Task.Delay(6000).ContinueWith(t =>
                    Lib.Mill.Tier = int.Parse(e.Bundle.Key.Last().ToString()));

        }

        public static bool unlocked(string id)
        {
            var unlockableBundlesAPI = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>("DLX.Bundles");
            return unlockableBundlesAPI.PurchasedBundles.Contains(id);
        }


    }
}
