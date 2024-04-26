/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unlockable_Bundles.Lib;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.API
{
    internal class SaveAnywhereHandler
    {
        //SaveAnywhere saves the game after a random DayStarted event
        public static bool DelayBundlePlacement = false;

        public static void Initialize()
        {
            if (Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere"))
                Helper.Events.GameLoop.GameLaunched += GameLaunched;
        }

        private static void GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<SaveAnywhere.Framework.ISaveAnywhereApi>("Omegasis.SaveAnywhere");
            api.BeforeSave += Api_BeforeSave;
            api.AfterSave += Api_AfterSave;
        }

        private static void Api_BeforeSave(object sender, EventArgs e)
        {
            var shops = ShopObject.getAll();
            foreach (var shop in shops)
                shop.unsubscribeFromAllEvents();

            SaveDataEvents.DayEnding(null, null);
            ShopPlacement.cleanupDay();

            DelayBundlePlacement = true;
        }

        private static void Api_AfterSave(object sender, EventArgs e)
        {
            DelayBundlePlacement = false;
            ShopPlacement.dayStarted(null, null);
        }
    }
}
