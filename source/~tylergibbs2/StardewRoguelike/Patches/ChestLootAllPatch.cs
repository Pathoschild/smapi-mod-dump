/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Objects;
using HarmonyLib;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Chest), "checkForAction")]
    internal class ChestLootAllPatch
    {
        public static bool Prefix(Chest __instance, ref bool __result, Farmer who, bool justCheckingForActivity)
        {
            if (justCheckingForActivity)
            {
                __result = true;
                return false;
            }

            Type chestType = __instance is TimedChest ? __instance.GetType().BaseType! : __instance.GetType();
            int currentLidFrame = (int)chestType.GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;

            if (currentLidFrame == __instance.startingLidFrame.Value && __instance.frameCounter.Value <= -1)
            {
                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                {
                    __instance.GetMutex().RequestLock(delegate
                    {
                        __instance.openChestEvent.Fire();
                    });
                }
                else
                    __instance.performOpenChest();
            }
            else if (currentLidFrame == __instance.getLastLidFrame() && __instance.items.Count > 0 && !__instance.synchronized.Value)
            {
                List<Item> items = __instance.items.ToList();
                __instance.items.Clear();

                if (items.Count > 1)
                    RoguelikeUtility.AddItemsByMenu(items);
                else
                    Game1.player.addItemByMenuIfNecessary(items[0]);

                IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
                ItemGrabMenu grab_menu = (ItemGrabMenu)activeClickableMenu;
                if (grab_menu is not null)
                {
                    ItemGrabMenu itemGrabMenu = grab_menu;
                    itemGrabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(itemGrabMenu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                    {
                        grab_menu.DropRemainingItems();
                    });
                }
            }

            if (__instance.items.Count == 0 && __instance.coins.Value == 0)
            {
                who.currentLocation.removeObject(__instance.TileLocation, showDestroyedObject: false);
                who.currentLocation.playSound("woodWhack");
            }

            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Chest), "dumpContents")]
    internal class ChestDumpContentsPatch
    {
        public static bool Prefix(Chest __instance, GameLocation location)
        {
            if (__instance.synchronized.Value && (__instance.GetMutex().IsLocked() || !Game1.IsMasterGame) && !__instance.GetMutex().IsLockHeld())
                return false;

            if (__instance.items.Count > 0 && __instance.items.Count >= 1 && (__instance.GetMutex().IsLockHeld()))
            {
                if (!__instance.synchronized.Value || __instance.GetMutex().IsLockHeld())
                {
                    List<Item> items = __instance.items.ToList();
                    __instance.items.Clear();

                    if (items.Count > 1)
                        RoguelikeUtility.AddItemsByMenu(items);
                    else
                        Game1.player.addItemByMenuIfNecessary(items[0]);

                    IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
                    ItemGrabMenu grab_menu = (ItemGrabMenu)activeClickableMenu;
                    if (grab_menu != null)
                    {
                        ItemGrabMenu itemGrabMenu = grab_menu;
                        itemGrabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(itemGrabMenu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                        {
                            grab_menu.DropRemainingItems();
                        });
                    }
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Chest), "performOpenChest")]
    internal class ChestOpenChest
    {
        public static void Postfix(Chest __instance)
        {
            List<Item> items = __instance.items.ToList();
            __instance.items.Clear();

            if (items.Count > 1)
                RoguelikeUtility.AddItemsByMenu(items);
            else
                Game1.player.addItemByMenuIfNecessary(items[0]);

            IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
            ItemGrabMenu grab_menu = (ItemGrabMenu)activeClickableMenu;
            if (grab_menu != null)
            {
                ItemGrabMenu itemGrabMenu = grab_menu;
                itemGrabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(itemGrabMenu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                {
                    grab_menu.DropRemainingItems();
                });
            }
        }
    }
}
