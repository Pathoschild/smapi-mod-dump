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
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using Unlockable_Bundles.API;
using Unlockable_Bundles.Lib.ShopTypes;

namespace Unlockable_Bundles
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;
        public static UnlockableBundlesAPI _API;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            ContentPatcherHandling.Initialize();
            UnlockableBundlesAPI.Initialize();
            GenericModConfigMenuHandler.Initialize();
            Lib.Main.Initialize();

            helper.ConsoleCommands.Add("ub", "Debug Breakpoint", this.commands);
            helper.ConsoleCommands.Add("ub_debug", "Debug Breakpoint", this.debug);
            helper.ConsoleCommands.Add("ub_apitest", "", this.apiTest);
            helper.ConsoleCommands.Add("ub_eventtest", "", this.eventTest);
        }

        private void commands(string command, string[] args)
        {
            if (args.Length == 0)
                return;

            if (args[0] == "ok")
                debugPurchase();
        }

        private void debugPurchase()
        {
            if (Game1.activeClickableMenu is not DialogueShopMenu)
                return;

            DialogueShopMenu shopMenu = (DialogueShopMenu)Game1.activeClickableMenu;
            shopMenu.processPurchase();
        }

        private void debug(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        private void apiTest(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI testAPI = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>(Mod.ModManifest.UniqueID);

            var res = testAPI.purchasedUnlockables;
            var res2 = testAPI.purchasedUnlockablesByLocation;
        }

        private void eventTest(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>("DLX.Bundles");
            unlockableAreasAPI.shopPurchasedEvent += evenTestMethod;
        }

        private void evenTestMethod(object source, ShopPurchasedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public override object GetApi()
        {
            _API = new UnlockableBundlesAPI();
            return _API;
        }
    }
}
