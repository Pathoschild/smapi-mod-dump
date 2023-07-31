/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ItemGrabMenuPatch
    {
        internal static string itemGrabMenuQueryKey = "";
        internal static string hoveredItemQueryKey = "";

        internal static void DrawPatch(ItemGrabMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.ItemsToGrabMenu.inventory.Count > 0 && !__instance.shippingBin)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
                    __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                }
                else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                }

                if (NarrateHoveredButton(__instance, x, y))
                {
                    InventoryUtils.Cleanup();
                    return;
                }
                if (NarrateLastShippedItem(__instance, x, y))
                {
                    InventoryUtils.Cleanup();
                    return;
                }

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    itemGrabMenuQueryKey = "";
                    return;
                }

                if (InventoryUtils.NarrateHoveredSlot(__instance.ItemsToGrabMenu, __instance.ItemsToGrabMenu.inventory, __instance.ItemsToGrabMenu.actualInventory, x, y, true))
                {
                    itemGrabMenuQueryKey = "";
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(ItemGrabMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "Ok Button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trash Can";
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                toSpeak = "Organize Button";
            }
            else if (__instance.fillStacksButton != null && __instance.fillStacksButton.containsPoint(x, y))
            {
                toSpeak = "Add to existing stacks button";
            }
            else if (__instance.specialButton != null && __instance.specialButton.containsPoint(x, y))
            {
                toSpeak = "Special Button";
            }
            else if (__instance.colorPickerToggleButton != null && __instance.colorPickerToggleButton.containsPoint(x, y))
            {
                toSpeak = "Color Picker: " + (__instance.chestColorPicker.visible ? "Enabled" : "Disabled");
            }
            else if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
            {
                toSpeak = "Community Center Button";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "Drop Item";
                isDropItemButton = true;
            }
            else
            {
                return false;
            }

            // FIXME
            /*if (__instance.discreteColorPickerCC.Count > 0) {
                for (int i = 0; i < __instance.discreteColorPickerCC.Count; i++)
                {
                    if (__instance.discreteColorPickerCC[i].containsPoint(x, y))
                    {
                        MainClass.monitor.Log(i.ToString(), LogLevel.Debug);
                        string toSpeak = getChestColorName(i);
                        if (itemGrabMenuQueryKey != toSpeak)
                        {
                            itemGrabMenuQueryKey = toSpeak;
                            hoveredItemQueryKey = "";
                            ScreenReader.say(toSpeak, true);
                            Game1.playSound("sa_drop_item");
                        }
                        return;
                    }
                }
            }*/

            if (itemGrabMenuQueryKey == toSpeak) return true;

            itemGrabMenuQueryKey = toSpeak;
            hoveredItemQueryKey = "";
            MainClass.ScreenReader.Say(toSpeak, true);
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static bool NarrateLastShippedItem(ItemGrabMenu __instance, int x, int y)
        {
            if (!__instance.shippingBin || Game1.getFarm().lastItemShipped == null || !__instance.lastShippedHolder.containsPoint(x, y))
                return false;

            Item lastShippedItem = Game1.getFarm().lastItemShipped;
            int count = lastShippedItem.Stack;
            string name = Translator.Instance.Translate("common-util-pluralize_name", new {item_count = count, name = lastShippedItem.DisplayName});

            string toSpeak = $"Last Shipped: {name}";

            if (itemGrabMenuQueryKey != toSpeak)
            {
                itemGrabMenuQueryKey = toSpeak;
                hoveredItemQueryKey = "";
                MainClass.ScreenReader.Say(toSpeak, true);
            }
            return true;
        }

        // TODO Add color names
        private static string GetChestColorName(int i)
        {
            string toReturn = "";
            switch (i)
            {
                case 0:
                    toReturn = "Default chest color";
                    break;
                case 1:
                    toReturn = "Default chest color";
                    break;
                case 2:
                    toReturn = "Default chest color";
                    break;
                case 3:
                    toReturn = "Default chest color";
                    break;
                case 4:
                    toReturn = "Default chest color";
                    break;
                case 5:
                    toReturn = "Default chest color";
                    break;
                case 6:
                    toReturn = "Default chest color";
                    break;
                case 7:
                    toReturn = "Default chest color";
                    break;
                case 8:
                    toReturn = "Default chest color";
                    break;
                case 9:
                    toReturn = "Default chest color";
                    break;
                case 10:
                    toReturn = "Default chest color";
                    break;
                case 11:
                    toReturn = "Default chest color";
                    break;
                case 12:
                    toReturn = "Default chest color";
                    break;
                case 13:
                    toReturn = "Default chest color";
                    break;
                case 14:
                    toReturn = "Default chest color";
                    break;
                case 15:
                    toReturn = "Default chest color";
                    break;
                case 16:
                    toReturn = "Default chest color";
                    break;
                case 17:
                    toReturn = "Default chest color";
                    break;
                case 18:
                    toReturn = "Default chest color";
                    break;
                case 19:
                    toReturn = "Default chest color";
                    break;
                case 20:
                    toReturn = "Default chest color";
                    break;
            }
            return toReturn;
        }

        internal static void Cleanup()
        {
            hoveredItemQueryKey = "";
            itemGrabMenuQueryKey = "";
        }
    }
}
