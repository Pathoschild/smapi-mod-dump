/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace HorseOverhaul
{
    internal class HighlightDelayedItemGrabMenu : ItemGrabMenu
    {
        private int delay;
        private readonly InventoryMenu.highlightThisItem highlightFunctionAfterDelay;

        public HighlightDelayedItemGrabMenu(int highlightDelay, IList<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunctionAfterDelay, behaviorOnItemSelect behaviorOnItemSelectFunction, string message, behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, Item sourceItem = null, int whichSpecialButton = -1, object context = null, ItemExitBehavior heldItemExitBehavior = ItemExitBehavior.ReturnToPlayer, bool allowExitWithHeldItem = false)
        : base(inventory, reverseGrab, showReceivingMenu, HighlightNoItems, behaviorOnItemSelectFunction, message, behaviorOnItemGrab, snapToBottom, canBeExitedWithKey, playRightClickSound, allowRightClick, showOrganizeButton, source, sourceItem, whichSpecialButton, context, heldItemExitBehavior, allowExitWithHeldItem)
        {
            delay = Math.Max(0, highlightDelay);

            this.highlightFunctionAfterDelay = highlightFunctionAfterDelay;
        }

        public static bool HighlightNoItems(Item i)
        {
            return false;
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (delay == 0)
            {
                inventory.highlightMethod = highlightFunctionAfterDelay;
                ItemsToGrabMenu.highlightMethod = highlightFunctionAfterDelay;
            }
            else if (delay > 0)
            {
                delay--;
            }
        }
    }
}