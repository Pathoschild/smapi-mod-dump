/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley.Menus;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Events;

internal static class GlobalMoneyCounter {
    private const string HarmonySuffix = ".GlobalMoneyCounter";

    private static Harmony? _harmony;
    private static readonly IList<Action<Item, int>> SellEvents = new List<Action<Item, int>>();

    private static int? _beforeMoney;
    private static Item? _soldItem;

    public static void AddSellEvent(Action<Item, int> onSellEvent) {
        if (_harmony == null) {
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(ShopMenu),
                    nameof(ShopMenu.receiveLeftClick)
                ),
                prefix: new HarmonyMethod(typeof(GlobalMoneyCounter), nameof(MenuReceivingLeftClick)),
                postfix: new HarmonyMethod(typeof(GlobalMoneyCounter), nameof(MenuReceivedInput))
            );
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(ShopMenu),
                    nameof(ShopMenu.receiveRightClick)
                ),
                prefix: new HarmonyMethod(typeof(GlobalMoneyCounter), nameof(MenuReceivingRightClick)),
                postfix: new HarmonyMethod(typeof(GlobalMoneyCounter), nameof(MenuReceivedInput))
            );
            _harmony.Patch(
                original: AccessTools.Method(
                    typeof(Game1),
                    "_newDayAfterFade"
                ),
                prefix: new HarmonyMethod(typeof(GlobalMoneyCounter), nameof(NewDayAfterFade))
            );
        }

        SellEvents.Add(onSellEvent);
    }

    private static bool MenuReceivingLeftClick(ShopMenu __instance, int x, int y, bool playSound = true) {
        if (Game1.activeClickableMenu == null)
            return true;
        if (__instance.heldItem == null && !__instance.readOnly) {
            _soldItem = __instance.inventory.getItemAt(x, y);
            _beforeMoney = Game1.player.Money;
        }
        return true;
    }
    
    private static void MenuReceivedInput() {
        if (_soldItem != null) {
            if (_beforeMoney < Game1.player.Money) {
                foreach (var sellEvent in SellEvents) {
                    sellEvent(_soldItem, _beforeMoney == null ? 0 : Game1.player.Money - (int) _beforeMoney);
                }
            }
            _soldItem = null;
            _beforeMoney = null;
        }
    }
    
    private static bool MenuReceivingRightClick(ShopMenu __instance, int x, int y, bool playSound = true) {
        if (Game1.activeClickableMenu == null)
            return true;
        if (__instance.heldItem == null && !__instance.readOnly) {
            _soldItem = __instance.inventory.rightClick(x, y, null, false);
            _beforeMoney = Game1.player.Money;
        }
        return true;
    }

    private static bool NewDayAfterFade() {
        foreach (var obj in Game1.getFarm().getShippingBin(Game1.player).OfType<SObject>()) {
            foreach (var sellEvent in SellEvents) {
                sellEvent(obj, obj.sellToStorePrice() * obj.Stack);
            }
        }
        return true;
    }

    public static void RemoveSellEvent(Action<Item, int> onSellEvent) {
        SellEvents.Remove(onSellEvent);

        if (SellEvents.Count == 0) {
            _harmony?.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony = null;
        }
    }
}