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
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using Unlockable_Bundles.API;
using Unlockable_Bundles.Lib.ShopTypes;
using StardewValley.BellsAndWhistles;
using Unlockable_Bundles.Lib;
using StardewValley.Locations;
using System.Linq;
using Unlockable_Bundles.Lib.Enums;
using static StardewValley.BellsAndWhistles.ParrotUpgradePerch;
using Unlockable_Bundles.Lib.AdvancedPricing;
using StardewValley.Triggers;

namespace Unlockable_Bundles
{

    public class ModEntry : Mod
    {
        public static ModConfig Config;

        public static Mod ModInstance;
        public static new IMonitor Monitor;
        public static new IModHelper Helper;
        public static new IManifest ModManifest;
        public static UnlockableBundlesAPI ModAPI;

        public static LogLevel DebugLogLevel = LogLevel.Trace;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            ModInstance = this;
            Monitor = ModInstance.Monitor;
            Helper = ModInstance.Helper;
            ModManifest = ModInstance.ModManifest;

            API.Main.Initialize();
            Lib.Main.Initialize();

            DebugLogLevel = Config.DebugLogging ? LogLevel.Debug : LogLevel.Trace;
        }

        public override object GetApi()
        {
            ModAPI ??= new UnlockableBundlesAPI();
            return ModAPI;
        }
    }
}
