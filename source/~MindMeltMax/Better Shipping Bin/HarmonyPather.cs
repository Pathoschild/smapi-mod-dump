/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using StardewValley.Menus;
using StardewValley.Locations;
using xTile.Dimensions;

namespace BetterShipping
{
    internal static class HarmonyPather
    {
        public static void Init(IModHelper helper)
        {
            Harmony harmony = new(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(ShippingBin), nameof(ShippingBin.doAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatch), nameof(ShippingBinPatch.doActionPostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatch), nameof(ShippingBinPatch.checkActionPostfix))
            );
        }
    }

    internal static class ShippingBinPatch
    {
        private static readonly IMonitor Monitor = ModEntry.IMonitor;

        private static readonly Location islandBinPosition = new(90, 39);

        public static void doActionPostfix()
        {
            try
            {
                if (Game1.activeClickableMenu is ItemGrabMenu) 
                    Game1.activeClickableMenu = new BinMenuOverride();
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch ShippingBin.doAction", LogLevel.Error); Monitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}"); }
        }

        public static void checkActionPostfix(Location tileLocation)
        {
            try
            {
                if ((tileLocation.X >= islandBinPosition.X || tileLocation.X <= islandBinPosition.X + 1) && 
                    (tileLocation.Y == islandBinPosition.Y || tileLocation.Y >= islandBinPosition.Y - 1) &&
                    Game1.activeClickableMenu is ItemGrabMenu)
                    Game1.activeClickableMenu = new BinMenuOverride();
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch IslandWest.checkAction", LogLevel.Error); Monitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}"); }
        }
    }
}
