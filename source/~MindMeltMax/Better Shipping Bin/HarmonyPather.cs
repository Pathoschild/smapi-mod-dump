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
        private static IModHelper Helper;
        private static Harmony Instance;

        public static void Init(IModHelper helper)
        {
            Helper = helper;
            Instance = new Harmony(Helper.ModRegistry.ModID);

            Instance.Patch(
                original: AccessTools.Method(typeof(ShippingBin), nameof(ShippingBin.doAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatch), nameof(ShippingBinPatch.doActionPostfix))
            );

            Instance.Patch(
                original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatch), nameof(ShippingBinPatch.checkActionPostfix))
            );
        }
    }

    internal static class ShippingBinPatch
    {
        private static readonly IModHelper Helper = ModEntry.IHelper;
        private static readonly IMonitor Monitor = ModEntry.IMonitor;

        private static readonly Location islandBinPosition = new Location(90, 39);

        public static void doActionPostfix(Vector2 tileLocation, Farmer who)
        {
            try
            {
                if (Game1.activeClickableMenu is not null and ItemGrabMenu) 
                    Game1.activeClickableMenu = new BinMenuOverride(Helper, Monitor);
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch ShippingBin.doAction", LogLevel.Error); Monitor.Log($"{ex} - {ex.Message}"); return; }
        }

        public static void checkActionPostfix(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            try
            {
                if ((tileLocation.X >= islandBinPosition.X || tileLocation.X <= islandBinPosition.X + 1) && (tileLocation.Y == islandBinPosition.Y || tileLocation.Y >= islandBinPosition.Y - 1))
                    if (Game1.activeClickableMenu is not null and ItemGrabMenu)
                        Game1.activeClickableMenu = new BinMenuOverride(Helper, Monitor);
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch IslandWest.checkAction", LogLevel.Error); Monitor.Log($"{ex} - {ex.Message}"); return; }
        }
    }
}
