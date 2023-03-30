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
    internal class GeodeMenuPatch
    {
        private static string geodeMenuQueryKey = "";

        internal static void DrawPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (narrateRecievedTreasure(__instance)) return;
                if (narrateHoveredButton(__instance, x, y)) return;

                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    geodeMenuQueryKey = "";
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool narrateRecievedTreasure(GeodeMenu __instance)
        {
            // Narrates the treasure recieved on breaking the geode
            if (__instance.geodeTreasure == null) return false;

            string name = __instance.geodeTreasure.DisplayName;
            int stack = __instance.geodeTreasure.Stack;

            string toSpeak = $"Recieved {stack} {name}";

            if (geodeMenuQueryKey != toSpeak)
            {
                geodeMenuQueryKey = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
            }
            return true;
        }

        private static bool narrateHoveredButton(GeodeMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.geodeSpot != null && __instance.geodeSpot.containsPoint(x, y))
            {
                toSpeak = "Place geode here";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "Drop item here";
                isDropItemButton = true;
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trash can";
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "Ok button";
            }
            else
            {
                return false;
            }

            if (geodeMenuQueryKey == toSpeak) return true;

            geodeMenuQueryKey = toSpeak;
            MainClass.ScreenReader.Say(toSpeak, true);
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        internal static void Cleanup()
        {
            geodeMenuQueryKey = "";
        }
    }
}
