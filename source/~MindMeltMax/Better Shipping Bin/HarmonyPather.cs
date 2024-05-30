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
using StardewValley;
using System;
using StardewValley.Menus;
using StardewValley.Locations;
using xTile.Dimensions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Reflection.Emit;
using StardewValley.Objects;
using StardewValley.BellsAndWhistles;

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

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), [typeof(SpriteBatch)]),
                transpiler: new(typeof(ShippingBinPatch), nameof(ShippingBinPatch.drawTranspiler))
            );
        }
    }

    internal static class ShippingBinPatch
    {
        private static readonly IMonitor Monitor = ModEntry.IMonitor;

        public static void doActionPostfix()
        {
            try
            {
                if (Game1.activeClickableMenu is ItemGrabMenu) 
                    Game1.activeClickableMenu = new BinMenuOverride();
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch ShippingBin.doAction", LogLevel.Error); Monitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}"); }
        }

        public static void checkActionPostfix(IslandWest __instance, Location tileLocation)
        {
            try
            {
                if (tileLocation.X >= __instance.shippingBinPosition.X && 
                    tileLocation.X <= __instance.shippingBinPosition.X + 1 && 
                    tileLocation.Y >= __instance.shippingBinPosition.Y - 1 && 
                    tileLocation.Y <= __instance.shippingBinPosition.Y && 
                    Game1.activeClickableMenu is ItemGrabMenu)
                    Game1.activeClickableMenu = new BinMenuOverride();
            }
            catch(Exception ex) { Monitor.Log($"Failed to patch IslandWest.checkAction", LogLevel.Error); Monitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}"); }
        }

        public static IEnumerable<CodeInstruction> drawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);
            var mi_ghi = AccessTools.PropertyGetter(typeof(MenuWithInventory), nameof(MenuWithInventory.heldItem));
            var mi_dtvmb = AccessTools.Method(typeof(ShippingBinPatch), nameof(drawTotalValueMiniBin));

            matcher.MatchStartForward([new(OpCodes.Ldarg_0), new(OpCodes.Call, mi_ghi), new(OpCodes.Dup)]);
            matcher.Advance(1);
            matcher.Insert([ //Use this order to avoid the method being jumped over by if statements earlier in the code
                new(OpCodes.Ldarg_1), //Inject SpriteBatch param
                new(OpCodes.Call, mi_dtvmb), //Call drawBanner
                new(OpCodes.Ldarg_0), //Inject ItemGrabMenu instance
            ]);

            return matcher.Instructions();
        }

        private static void drawTotalValueMiniBin(ItemGrabMenu menu, SpriteBatch b)
        {
            if (menu.sourceItem is not Chest c || c.SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin || !ModEntry.IConfig.ShowTotalValueMiniBin)
                return;

            int value = 0;
            string text = ModEntry.IHelper.Translation.Get("Menu.TotalValue");

            for (int i = 0; i < menu.ItemsToGrabMenu.actualInventory.Count; i++) 
            {
                Item? item = menu.ItemsToGrabMenu.actualInventory[i];
                if (item is null)
                    continue;
                value += item.sellToStorePrice(Game1.player.UniqueMultiplayerID) * item.Stack;
            }

            if (value <= 0)
                return;
            text += $"{value}";
            SpriteText.drawStringWithScrollCenteredAt(b, text, menu.ItemsToGrabMenu.xPositionOnScreen + (menu.ItemsToGrabMenu.width / 2), menu.ItemsToGrabMenu.yPositionOnScreen - 120);
        }
    }
}
