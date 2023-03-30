/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Features;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ForgeMenuPatch
    {
        private static string forgeMenuQuery = "";

        internal static void DrawPatch(ForgeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (narrateHoveredButton(__instance, x, y)) return;

                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                {
                    Cleanup();
                }

            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occured in forge menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool narrateHoveredButton(ForgeMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.leftIngredientSpot != null && __instance.leftIngredientSpot.containsPoint(x, y))
            {
                if (__instance.leftIngredientSpot.item == null)
                {
                    toSpeak = "Input weapon or tool here";
                }
                else
                {
                    Item item = __instance.leftIngredientSpot.item;
                    toSpeak = $"Weapon slot: {item.Stack} {item.DisplayName}";
                }
            }
            else if (__instance.rightIngredientSpot != null && __instance.rightIngredientSpot.containsPoint(x, y))
            {
                if (__instance.rightIngredientSpot.item == null)
                {
                    toSpeak = "Input gemstone here";
                }
                else
                {
                    Item item = __instance.rightIngredientSpot.item;
                    toSpeak = $"Gemstone slot: {item.Stack} {item.DisplayName}";
                }
            }
            else if (__instance.startTailoringButton != null && __instance.startTailoringButton.containsPoint(x, y))
            {
                toSpeak = "Star forging button";
            }
            else if (__instance.unforgeButton != null && __instance.unforgeButton.containsPoint(x, y))
            {
                toSpeak = "Unforge button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trashcan";
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "ok button";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "drop item";
                isDropItemButton = true;
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[0].containsPoint(x, y))
            {
                toSpeak = "Left ring Slot";

                if (Game1.player.leftRing.Value != null)
                    toSpeak = $"{toSpeak}: {Game1.player.leftRing.Value.DisplayName}";
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[1].containsPoint(x, y))
            {
                toSpeak = "Right ring Slot";

                if (Game1.player.rightRing.Value != null)
                    toSpeak = $"{toSpeak}: {Game1.player.rightRing.Value.DisplayName}";
            }
            else
            {
                return false;
            }

            if (forgeMenuQuery != toSpeak)
            {
                forgeMenuQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);

                if (isDropItemButton) Game1.playSound("drop_item");
            }

            return true;
        }

        internal static void Cleanup()
        {
            forgeMenuQuery = "";
        }
    }
}
