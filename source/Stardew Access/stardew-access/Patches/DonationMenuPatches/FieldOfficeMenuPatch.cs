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
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class FieldOfficeMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(FieldOfficeMenu), nameof(FieldOfficeMenu.draw),
                    new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(FieldOfficeMenuPatch), nameof(FieldOfficeMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(FieldOfficeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    toSpeak = Translator.Instance.Translate("common-ui-trashcan_button", TranslationCategory.Menu);
                }
                else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    toSpeak = Translator.Instance.Translate("common-ui-ok_button", TranslationCategory.Menu);
                }
                else if (__instance.dropItemInvisibleButton != null &&
                         __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    toSpeak = Translator.Instance.Translate("common-ui-drop_item_button", TranslationCategory.Menu);
                }
                else
                {
                    if (InventoryUtils.NarrateHoveredSlot(__instance.inventory.inventory,
                            __instance.inventory.actualInventory, __instance.inventory))
                    {
                        return;
                    }

                    for (int i = 0; i < __instance.pieceHolders.Count; i++)
                    {
                        if (!__instance.pieceHolders[i].containsPoint(x, y))
                            continue;

                        if (__instance.pieceHolders[i].item == null)
                        {
                            toSpeak = Translator.Instance.Translate("menu-field_office-incomplete_slot_names",
                                new { slot_index = i }, TranslationCategory.Menu);
                        }
                        else
                        {
                            toSpeak = Translator.Instance.Translate("menu-field_office-completed_slot_info",
                                new
                                {
                                    slot_index = i + 1, item_name_in_slot = __instance.pieceHolders[i].item.DisplayName
                                }, TranslationCategory.Menu);
                        }

                        if (!MainClass.Config.DisableInventoryVerbosity && __instance.heldItem != null &&
                            __instance.pieceHolders[i].item == null)
                        {
                            int highlight = GetPieceIndexForDonationItem(__instance.heldItem.ParentSheetIndex);
                            if (highlight != -1 && highlight == i)
                            {
                                toSpeak = Translator.Instance.Translate(
                                    "menu-donation_common-donatable_item_in_inventory-prefix",
                                    new { content = toSpeak }, TranslationCategory.Menu);
                            }
                        }

                        MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, $"{toSpeak}:{i}");
                        return;
                    }
                }

                if (MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true))
                    if (__instance.dropItemInvisibleButton != null &&
                        __instance.dropItemInvisibleButton.containsPoint(x, y))
                        Game1.playSound("drop_item");
            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
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
    }
}