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
using Unlockable_Areas.API;

namespace Unlockable_Areas
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;
        public static UnlockableAreasAPI _API;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            ContentPatcherHandling.Initialize();
            UnlockableAreasAPI.Initialize();
            Lib.Main.Initialize();
            Menus.ShopObjectMenu.Initialize();

            helper.ConsoleCommands.Add("ua_debug", "Debug Breakpoint", this.debug);
            helper.ConsoleCommands.Add("ua_apitest", "", this.apiTest);
            helper.ConsoleCommands.Add("ua_eventtest", "", this.eventTest);
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

            IUnlockableAreasAPI testAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>(Mod.ModManifest.UniqueID);

            var res = testAPI.purchasedUnlockables;
            var res2 = testAPI.purchasedUnlockablesByLocation;
        }

        private void eventTest(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            IUnlockableAreasAPI unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>("DeLiXx.Unlockable_Areas");
            unlockableAreasAPI.shopPurchasedEvent += evenTestMethod;
        }

        private void evenTestMethod(object source, ShopPurchasedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public override object GetApi()
        {
            _API = new UnlockableAreasAPI();
            return _API;
        }
    }
}
