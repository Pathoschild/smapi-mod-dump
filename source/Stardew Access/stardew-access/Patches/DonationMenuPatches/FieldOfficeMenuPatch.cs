/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using stardew_access.Utils;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class FieldOfficeMenuPatch
    {
        private static string fieldOfficeMenuQuery = "";

        internal static void DrawPatch(FieldOfficeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = " ";

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
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
                }
                else
                {
                    if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                        return;

                    for (int i = 0; i < __instance.pieceHolders.Count; i++)
                    {
                        if (!__instance.pieceHolders[i].containsPoint(x, y))
                            continue;

                        if (__instance.pieceHolders[i].item == null)
                            toSpeak = i switch
                            {
                                0 => "Center skeleton slot",
                                1 => "Center skeleton slot",
                                2 => "Center skeleton slot",
                                3 => "Center skeleton slot",
                                4 => "Center skeleton slot",
                                5 => "Center skeleton slot",
                                6 => "Snake slot",
                                7 => "Snake slot",
                                8 => "Snake slot",
                                9 => "Bat slot",
                                10 => "Frog slot",
                                _ => "Donation slot"
                            };
                        else
                            toSpeak = $"Slot {i + 1} finished: {__instance.pieceHolders[i].item.DisplayName}";

                        if (!MainClass.Config.DisableInventoryVerbosity && __instance.heldItem != null && __instance.pieceHolders[i].item == null)
                        {
                            int highlight = GetPieceIndexForDonationItem(__instance.heldItem.ParentSheetIndex);
                            if (highlight != -1 && highlight == i)
                                toSpeak += "Donatable ";
                        }

                        if (fieldOfficeMenuQuery != $"{toSpeak}:{i}")
                        {
                            fieldOfficeMenuQuery = $"{toSpeak}:{i}";
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }

                        return;
                    }
                }

                if (fieldOfficeMenuQuery != toSpeak)
                {
                    fieldOfficeMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);

                    if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                        Game1.playSound("drop_item");
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static int GetPieceIndexForDonationItem(int itemIndex)
        {
            return itemIndex switch
            {
                820 => 5,
                821 => 4,
                822 => 3,
                823 => 0,
                824 => 1,
                825 => 8,
                826 => 7,
                827 => 9,
                828 => 10,
                _ => -1,
            };
        }

        internal static void Cleanup()
        {
            fieldOfficeMenuQuery = "";
        }
    }
}
