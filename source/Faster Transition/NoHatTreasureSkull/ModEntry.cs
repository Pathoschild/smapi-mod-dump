/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using HarmonyLib;

namespace NoHatTreasureSkull
{
    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public override void Entry(IModHelper helper)
        {
            // Skull cave
            //Game1.enterMine(121);

            SMonitor = Monitor;
            SHelper = Helper;
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.getTreasureRoomItem)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MineShaftGetTreasureRoomItem_postfix))
            );

            // Cheat to spawn chest every room
            //harmony.Patch(
            //   original: AccessTools.Method(typeof(MineShaft), "addLevelChests"),
            //   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MineShaftaddLevelChests_prefix))
            //);

        }
    }
}
