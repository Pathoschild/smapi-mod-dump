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
using Unlockable_Areas.API;

namespace Desert_Bloom.API
{
    public class UnlockableAreas
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
            var unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>("DeLiXx.Unlockable_Areas");
            unlockableAreasAPI.shopPurchasedEvent += onShopPurchased;
            unlockableAreasAPI.isReadyEvent += isReady;
        }

        private static void isReady(object sender, IsReadyEventArgs e)
        {
            if (!ModEntry.IsMyFarm())
                return;

            assignMillTier();
        }
        private static void assignMillTier()
        {
            Lib.Mill.Tier = 0;
            var unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>("DeLiXx.Unlockable_Areas");
            foreach (var unlocked in unlockableAreasAPI.purchasedUnlockables.Where(el => el.StartsWith("DLX.Desert_Bloom.Mill_Tier"))) {
                var tier = int.Parse(unlocked.Last().ToString());
                if (tier > Lib.Mill.Tier)
                    Lib.Mill.Tier = tier;
            }
        }

        private static void onShopPurchased(object source, ShopPurchasedEventArgs e)
        {
            if (!ModEntry.IsMyFarm())
                return;

            if (e.unlockableKey.StartsWith("DLX.Desert_Bloom.Mill_Tier"))
                Task.Delay(1000).ContinueWith(t =>
                    Lib.Mill.Tier = int.Parse(e.unlockableKey.Last().ToString()));

            if (e.unlockableKey == "DLX.Desert_Bloom.Mining_Area_1" && e.isBuyer)
                e.who.Money += 5000;

        }

        public static bool unlocked(string id)
        {
            var unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>("DeLiXx.Unlockable_Areas");
            return unlockableAreasAPI.purchasedUnlockables.Contains(id);
        }


    }
}
