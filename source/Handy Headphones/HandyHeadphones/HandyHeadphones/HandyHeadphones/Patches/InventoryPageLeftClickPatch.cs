/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;

namespace HandyHeadphones.Patches
{
    [HarmonyPatch]
    class InventoryPageLeftClickPatch
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static IModHelper helper = ModEntry.modHelper;
        private static ModConfig config = ModEntry.config;

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.Menus.InventoryPage), nameof(StardewValley.Menus.InventoryPage.receiveLeftClick));
        }

        internal static bool Prefix(InventoryPage __instance, out bool __state, int x, int y, bool playSound = true)
        {
            __state = false;

            ClickableComponent hatComponent = __instance.equipmentIcons.First(s => s.name == "Hat");
            if (hatComponent.containsPoint(x, y))
            {
                bool heldItemWasNull = Game1.player.CursorSlotItem is null;
                if (!IsHeadphoneHeld())
                {
                    if (IsWearingMusicPlayer())
                    {
                        if (heldItemWasNull || Game1.player.CursorSlotItem is Hat)
                        {
                            __state = true;
                        }
                    }
                    return true;
                }

                if (IsValidMusicPlayer(Game1.player.CursorSlotItem.Name))
                {
                    Hat tmp = (Hat)helper.Reflection.GetMethod(__instance, "takeHeldItem").Invoke<Item>();
                    Item heldItem = Game1.player.hat;
                    heldItem = Utility.PerformSpecialItemGrabReplacement(heldItem);
                    Game1.player.hat.Value = tmp;

                    if (Game1.player.hat.Value != null)
                    {
                        Game1.playSound("grassyStep");
                    }
                    else if (Game1.player.CursorSlotItem is null)
                    {
                        Game1.playSound("dwop");
                    }

                    if (heldItem != null && !Game1.player.addItemToInventoryBool(heldItem, false))
                    {
                        helper.Reflection.GetMethod(__instance, "setHeldItem").Invoke(heldItem);
                    }

                    ShowCorrectPrompt();
                }

                if (!heldItemWasNull || Game1.player.CursorSlotItem is null || !Game1.oldKBState.IsKeyDown(Keys.LeftShift))
                {
                    return false;
                }

                for (int l = 0; l < Game1.player.items.Count; l++)
                {
                    if (Game1.player.items[l] == null || Game1.player.CursorSlotItem != null && Game1.player.items[l].canStackWith(Game1.player.CursorSlotItem))
                    {
                        if (Game1.player.CurrentToolIndex == l && Game1.player.CursorSlotItem != null)
                        {
                            Game1.player.CursorSlotItem.actionWhenBeingHeld(Game1.player);
                        }
                        helper.Reflection.GetMethod(__instance, "setHeldItem").Invoke(Utility.addItemToInventory(helper.Reflection.GetMethod(__instance, "takeHeldItem").Invoke<Item>(), l, __instance.inventory.actualInventory));
                        if (Game1.player.CurrentToolIndex == l && Game1.player.CursorSlotItem != null)
                        {
                            Game1.player.CursorSlotItem.actionWhenStopBeingHeld(Game1.player);
                        }
                        Game1.playSound("stoneStep");
                        return false;
                    }
                }
            }

            if (IsSelectingHeadPhonesInInventory(__instance, x, y) && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
            {
                helper.Reflection.GetMethod(__instance, "setHeldItem").Invoke(__instance.inventory.leftClick(x, y, helper.Reflection.GetMethod(__instance, "takeHeldItem").Invoke<Item>(), !Game1.oldKBState.IsKeyDown(Keys.LeftShift)));
                if (Game1.player.CursorSlotItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
                {
                    if (Game1.player.hat.Value == null)
                    {
                        Game1.player.hat.Value = helper.Reflection.GetMethod(__instance, "takeHeldItem").Invoke<Item>() as Hat;

                        ShowCorrectPrompt();
                        return false;
                    }
                }
            }

            return true;
        }

        internal static void Postfix(InventoryPage __instance, bool __state, int x, int y, bool playSound = true)
        {
            ClickableComponent hatComponent = __instance.equipmentIcons.First(s => s.name == "Hat");
            if (hatComponent.containsPoint(x, y))
            {
                if (__state)
                {
                    Game1.changeMusicTrack("none");

                    if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
                    {
                        Game1.playMorningSong();
                    }
                }
            }
        }

        private static void ShowCorrectPrompt()
        {
            Hat wornHat = Game1.player.hat;
            if (wornHat is null)
            {
                return;
            }

            if (wornHat.Name == "Headphones" || wornHat.Name == "Earbuds")
            {
                ModEntry.ShowMusicMenu();
                return;
            }
            if (wornHat.Name == "Studio Headphones")
            {
                ModEntry.ShowSoundMenu();
                return;
            }
        }

        private static bool IsWearingMusicPlayer()
        {
            if (Game1.player.hat != null)
            {
                Hat wornHat = Game1.player.hat;
                if (wornHat != null && IsValidMusicPlayer(wornHat.Name))
                {
                    return true;
                }

            }
            return false;
        }

        private static bool IsValidMusicPlayer(string name)
        {
            return name == "Headphones" || name == "Earbuds" || name == "Studio Headphones";
        }

        private static bool IsHeadphoneHeld()
        {
            return Game1.player.CursorSlotItem != null && IsValidMusicPlayer(Game1.player.CursorSlotItem.Name);
        }

        private static bool IsSelectingHeadPhonesInInventory(InventoryPage page, int x, int y)
        {
            return page.inventory.getItemAt(x, y) != null && (page.inventory.getItemAt(x, y).Name == "Headphones" || page.inventory.getItemAt(x, y).Name == "Earbuds" || page.inventory.getItemAt(x, y).Name == "Studio Headphones");
        }
    }
}
