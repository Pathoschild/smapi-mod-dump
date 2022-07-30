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
using Harmony;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using xTile.Dimensions;

namespace BetterShipping
{
    public static class HarmonyPather
    {
        public static void Init(IModHelper helper)
        {
            var Instance = HarmonyInstance.Create(helper.ModRegistry.ModID);

            Instance.Patch(
                original: AccessTools.Method(typeof(ShippingBin), nameof(ShippingBin.doAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatches), nameof(ShippingBinPatches.doActionPostfix))
            );

            Instance.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.checkAction)),
                postfix: new HarmonyMethod(typeof(ShippingBinPatches), nameof(ShippingBinPatches.checkActionPostfix))
            );
        }
    }

    public class ShippingBinPatches
    {
        private static readonly IModHelper Helper = ModEntry.IHelper;
        private static readonly IMonitor Monitor = ModEntry.IMonitor;

        public static void doActionPostfix(Vector2 tileLocation, Farmer who)
        {
            try
            {
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    Game1.activeClickableMenu = new BinMenuOverride(Helper, Monitor);
            }
            catch (Exception ex) { Monitor.Log($"Failed to patch ShippingBin.doAction", LogLevel.Error); Monitor.Log($"{ex} - {ex.Message}"); return; }
        }

        public static void checkActionPostfix(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            try
            {
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu && (tileLocation.X >= 71 && tileLocation.X <= 72 && (tileLocation.Y >= 13 && tileLocation.Y <= 14)))
                    Game1.activeClickableMenu = new BinMenuOverride(Helper, Monitor);
            }
            catch (Exception ex) { Monitor.Log($"Failed to patch ShippingBin.doAction", LogLevel.Error); Monitor.Log($"{ex} - {ex.Message}"); return; }
        }
    }
}
