/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Farm_Loader.GameLoopInjections
{//Hotfix for 1.6 busstop having been widened
    internal class BusStopHotfix
    {
        public static Mod Mod { get => ModEntry.Mod; }
        private static IMonitor Monitor { get => ModEntry._Monitor; }
        private static IModHelper Helper { get => ModEntry._Helper; }
        public static void Initialize()
        {
            var harmony = new Harmony(Mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.updateWarps)),
               postfix: new HarmonyMethod(typeof(BusStopHotfix), nameof(BusStopHotfix.updateWarps_Postfix))
            );

            harmony.Patch(
               original: AccessTools.DeclaredMethod(typeof(Game1), "performWarpFarmer", new[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) }),
               prefix: new HarmonyMethod(typeof(BusStopHotfix), nameof(BusStopHotfix.performWarpFarmer_Prefix))
            );
        }

        public static void performWarpFarmer_Prefix(LocationRequest locationRequest, ref int tileX, ref int tileY)
        {
            if (locationRequest is null || locationRequest.Name is null)
                return;

            var forbidden = new Rectangle(-1, 20, 10, 6);

            if (locationRequest.Name != "BusStop")
                return;

            if (forbidden.Contains(new Point(tileX, tileY))) {
                tileX = 11;
                tileY = 23;
            }

        }

        public static void updateWarps_Postfix(GameLocation __instance)
        {
            var forbidden = new Rectangle(-1, 20, 10, 6);

            foreach (var warp in __instance.warps) {
                if (warp.TargetName != "BusStop")
                    continue;

                if (forbidden.Contains(new Point(warp.TargetX, warp.TargetY))) {
                    warp.TargetX = 11;
                    warp.TargetY = 23;
                }
            }
        }
    }
}
