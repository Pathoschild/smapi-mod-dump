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
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Desert_Bloom
{

    public class ModEntry : Mod
    {
        public static Mod Mod;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;
        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            Lib.Mill.main();
            Lib.AssetsRequested.main();
            Lib.CustomTerrainSpawns.main();
            API.UnlockableBundlesHandler.main();
            API.GenericModConfigMenuHandler.main();
        }

        public static bool IsMyFarm() => Game1.GetFarmTypeID() == "DeLiXx.Desert_Bloom_Farm_CFL/Desert Bloom";
    }
}
