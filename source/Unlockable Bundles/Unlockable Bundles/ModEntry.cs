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
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using Unlockable_Bundles.API;
using Unlockable_Bundles.Lib.ShopTypes;
using StardewValley.BellsAndWhistles;
using Unlockable_Bundles.Lib;

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
        }

        private void commands(string command, string[] args)
        {
            if (args.Length == 0)
                return;

            if (args[0] == "ok")
                debugPurchase();
            else if (args[0] == "apitest")
                apiTest();
            else if (args[0] == "eventtest")
                eventTest();
            else if (args[0] == "debug" && System.Diagnostics.Debugger.IsAttached)
                debug();
            else if (args[0] == "event")
                playEventScript(args[1]);
        }

        private void debug()
        {
            System.Diagnostics.Debugger.Break();
        }

        private void playEventScript(string key)
        {
            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            Game1.globalFadeToBlack(() => Game1.currentLocation.startEvent(new UBEvent(new Unlockable(unlockables[key]), unlockables[key].ShopEvent, -1, Game1.player)));
        }

        private void debugPurchase()
        {
            if (Game1.activeClickableMenu is DialogueShopMenu menu) {
                menu.Unlockable.processPurchase();
                menu.ScreenSwipe = new ScreenSwipe(0);
                menu.CompletionTimer = 800;
                menu.Complete = true;
                menu.CanClick = false;

            } else if (Game1.activeClickableMenu is BundleMenu ccMenu) {
                ccMenu.Unlockable.processPurchase();
                ccMenu.ScreenSwipe = new ScreenSwipe(0);
                ccMenu.CompletionTimer = 800;
                ccMenu.Complete = true;
                ccMenu.CanClick = false;
            }
        }

        private void apiTest()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI api = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>(Mod.ModManifest.UniqueID);

            var bundles = api.getBundles();
            var purchased = api.PurchasedBundles;
            var purchasedSince = api.PurchaseBundlesByLocation;
        }

        private void eventTest()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableBundlesAPI api = Helper.ModRegistry.GetApi<IUnlockableBundlesAPI>(Mod.ModManifest.UniqueID);
            api.BundlePurchasedEvent += evenTestMethod;
        }

        private void evenTestMethod(object source, BundlePurchasedEventArgs e)
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
