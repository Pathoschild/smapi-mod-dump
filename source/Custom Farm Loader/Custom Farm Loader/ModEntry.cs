/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using Custom_Farm_Loader.Menus;
using System.Collections.Generic;

namespace Custom_Farm_Loader
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            Lib.Main.Initialize(this);
            GameLoopInjections.Main.Initialize(this);
            Menus.Main.Initialize(this);
        }
    }

}
