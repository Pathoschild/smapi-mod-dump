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
using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Unlockable_Bundles.Lib;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.API
{
    public class ContentPatcherHandling
    {
        internal static DaysSincePurchaseToken DaysSincePurchaseToken = new DaysSincePurchaseToken();

        public static void Initialize()
        {
            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
                Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(ModManifest, "Purchased", getPurchasedUnlockables);
            api.RegisterToken(ModManifest, "DaysSincePurchase", DaysSincePurchaseToken);
            api.RegisterToken(ModManifest, "Discovered", getDiscoveredUnlockables);
        }

        public static IEnumerable<string> getPurchasedUnlockables() => UnlockableBundlesAPI.getPurchasedUnlockables();
        public static IEnumerable<string> getDiscoveredUnlockables() => UnlockableBundlesAPI.getDiscoveredUnlockables();
    }
}