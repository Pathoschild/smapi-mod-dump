/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ShopMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ShopMenuPatch), nameof(ShopMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ShopMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.forSaleButtons.Count > 0)
                {
                    __instance.forSaleButtons[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.forSaleButtons[0].myID);
                }
                else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
                {
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                }

                if (NarrateHoveredButton(__instance, x, y)) return;

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory.inventory, __instance.inventory.actualInventory, inventoryMenu: __instance.inventory, hoverPrice: __instance.hoverPrice))
                {
                    return;
                }

                NarrateHoveredSellingItem(__instance);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in shop menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(ShopMenu __instance, int x, int y)
        {
            string translationKey = "";
            bool isDropItemButton = false;

            if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
            {
                translationKey = "common-ui-drop_item_button";
                isDropItemButton = true;
            }
            else if (__instance.upArrow != null && __instance.upArrow.containsPoint(x, y))
            {
                translationKey = "common-ui-scroll_up_button";
            }
            else if (__instance.downArrow != null && __instance.downArrow.containsPoint(x, y))
            {
                translationKey = "common-ui-scroll_down_button";
            }
            else
            {
                return false;
            }

            if (MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true))
                if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static void NarrateHoveredSellingItem(ShopMenu __instance)
        {
            if (__instance.hoveredItem == null) return;

            string name = __instance.hoveredItem.DisplayName;
            string price = Translator.Instance.Translate("menu-shop-buy_price_info", new { price = __instance.hoverPrice }, TranslationCategory.Menu);
            string description = __instance.hoveredItem.getDescription();

            int itemIndex = (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 2)
                ? __instance.itemPriceAndStock[__instance.hoveredItem][2]
                : -1;

            int itemAmount = (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 3)
                ? __instance.itemPriceAndStock[__instance.hoveredItem][3]
                : 5;

            string requirements = InventoryUtils.GetExtraItemInfo(itemIndex, itemAmount);

            string toSpeak = $"{name}, {requirements}, {price}, {description}";
            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
        }
    }
}
