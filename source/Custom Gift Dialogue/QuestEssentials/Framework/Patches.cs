/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace QuestEssentials.Framework
{
    internal static class Patches
    {
        public static void Before_receiveLeftClick(ShopMenu __instance, int x, int y)
        {
            try
            {
                if (Game1.activeClickableMenu == null)
                    return;

                if (__instance is ShopMenu)
                {
                    if (__instance.heldItem == null && __instance.onSell == null)
                    {
                        Item itemToSell = __instance.inventory.leftClick(x, y, null, false);

                        if (itemToSell != null)
                        {
                            QuestCheckers.CheckSellQuests(itemToSell);
                            __instance.inventory.leftClick(x, y, itemToSell, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QuestEssentialsMod.ModMonitor
                    .Log(
                        $"Error in {nameof(Before_receiveLeftClick)} harmony patch: {ex}",
                        LogLevel.Error
                    );
            }
        }

        public static void Before_receiveRightClick(ShopMenu __instance, int x, int y)
        {
            try
            {
                if (Game1.activeClickableMenu == null)
                    return;

                if (__instance is ShopMenu)
                {
                    if (__instance.heldItem == null && __instance.onSell == null)
                    {
                        Item itemToSell = __instance.inventory.rightClick(x, y, null, false, false);

                        if (itemToSell != null)
                        {
                            QuestCheckers.CheckSellQuests(itemToSell);

                            if (itemToSell.Stack == 1)
                                __instance.inventory.leftClick(x, y, itemToSell, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QuestEssentialsMod.ModMonitor
                    .Log(
                        $"Error in {nameof(Before_receiveRightClick)} harmony patch: {ex}",
                        LogLevel.Error
                    );
            }
        }

        public static void Before_set_Money(Farmer __instance, int value)
        {
            int oldMoney = __instance._money;
            __instance._money = value;

            if (value <= oldMoney)
                return;

            QuestCheckers.CheckEarnQuests(value - oldMoney);
        }
    }
}
