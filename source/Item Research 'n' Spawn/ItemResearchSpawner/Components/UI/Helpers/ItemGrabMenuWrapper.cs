/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ItemResearchSpawner.Components
{
    public class ItemGrabMenuWrapper : ItemGrabMenu
    {
        private Item SourceItem =>
            (Item) typeof(ItemGrabMenu).GetField("sourceItem", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(this);

        private string Message =>
            (string) typeof(ItemGrabMenu).GetField("message", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(this);

        private TemporaryAnimatedSprite Poof =>
            (TemporaryAnimatedSprite) typeof(ItemGrabMenu)
                .GetField("poof", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(this);

        public ItemGrabMenuWrapper(IList<Item> inventory, object context = null) : base(inventory, context)
        {
        }

        public ItemGrabMenuWrapper(IList<Item> inventory, bool reverseGrab, bool showReceivingMenu,
            InventoryMenu.highlightThisItem highlightFunction, behaviorOnItemSelect behaviorOnItemSelectFunction,
            string message, behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false,
            bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true,
            bool showOrganizeButton = false, int source = 0, Item sourceItem = null, int whichSpecialButton = -1,
            object context = null) : base(inventory, reverseGrab, showReceivingMenu, highlightFunction,
            behaviorOnItemSelectFunction, message, behaviorOnItemGrab, snapToBottom, canBeExitedWithKey,
            playRightClickSound, allowRightClick, showOrganizeButton, source, sourceItem, whichSpecialButton, context)
        {
        }

        protected void DrawMenu(SpriteBatch b)
        {
             if (drawBG)
             {
                 b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
                     Color.Black * 0.5f);
             }

             draw(b, false, false);

             if (showReceivingMenu)
             {
                 b.Draw(Game1.mouseCursors,
                     new Vector2(xPositionOnScreen - 64,
                         yPositionOnScreen + height / 2 + 64 + 16),
                     new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(xPositionOnScreen - 64,
                         yPositionOnScreen + height / 2 + 64 - 16),
                     new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(xPositionOnScreen - 40,
                         yPositionOnScreen + height / 2 + 64 - 44),
                     new Rectangle(4, 372, 8, 11), Color.White, 0.0f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 if ((source != 1 || !(SourceItem is Chest) ||
                      ((Chest) SourceItem).SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin &&
                      ((Chest) SourceItem).SpecialChestType != Chest.SpecialChestTypes.JunimoChest &&
                      ((Chest) SourceItem).SpecialChestType != Chest.SpecialChestTypes.Enricher) &&
                     source != 0)
                 {
                     b.Draw(Game1.mouseCursors,
                         new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 + 16),
                         new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f,
                         SpriteEffects.None, 1f);
                     b.Draw(Game1.mouseCursors,
                         new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 - 16),
                         new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f,
                         SpriteEffects.None, 1f);
                     Rectangle rectangle = new Rectangle(sbyte.MaxValue, 412, 10, 11);
                     switch (source)
                     {
                         case 2:
                             rectangle.X += 20;
                             break;
                         case 3:
                             rectangle.X += 10;
                             break;
                     }

                     b.Draw(Game1.mouseCursors,
                         new Vector2(xPositionOnScreen - 52, yPositionOnScreen + 64 - 44),
                         rectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                 }

                 Game1.drawDialogueBox(
                     ItemsToGrabMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder,
                     ItemsToGrabMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder,
                     ItemsToGrabMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2,
                     ItemsToGrabMenu.height + spaceToClearTopBorder + borderWidth * 2,
                     false, true);
                 ItemsToGrabMenu.draw(b);
             }
             else if (Message != null)
             {
                 Game1.drawDialogueBox(Game1.uiViewport.Width / 2,
                     ItemsToGrabMenu.yPositionOnScreen + ItemsToGrabMenu.height / 2, false, false,
                     Message);
             }

             Poof?.draw(b, true);

             foreach (TransferredItemSprite transferredItemSprite in _transferredItemSprites)
             {
                 transferredItemSprite.Draw(b);
             }

             if (shippingBin && Game1.getFarm().lastItemShipped != null)
             {
                 lastShippedHolder.draw(b);
                 Game1.getFarm().lastItemShipped.drawInMenu(b,
                     new Vector2(lastShippedHolder.bounds.X + 16,
                         lastShippedHolder.bounds.Y + 16), 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(lastShippedHolder.bounds.X - 8,
                         lastShippedHolder.bounds.Bottom - 100),
                     new Rectangle(325, 448, 5, 14), Color.White, 0.0f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(lastShippedHolder.bounds.X + 84,
                         lastShippedHolder.bounds.Bottom - 100),
                     new Rectangle(325, 448, 5, 14), Color.White, 0.0f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(lastShippedHolder.bounds.X - 8,
                         lastShippedHolder.bounds.Bottom - 44),
                     new Rectangle(325, 452, 5, 13), Color.White, 0.0f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
                 b.Draw(Game1.mouseCursors,
                     new Vector2(lastShippedHolder.bounds.X + 84,
                         lastShippedHolder.bounds.Bottom - 44),
                     new Rectangle(325, 452, 5, 13), Color.White, 0.0f, Vector2.Zero, 4f,
                     SpriteEffects.None, 1f);
             }

             if (colorPickerToggleButton != null)
             {
                 colorPickerToggleButton.draw(b);
             }
             else
             {
                 specialButton?.draw(b);
             }

             chestColorPicker?.draw(b);

             organizeButton?.draw(b);

             fillStacksButton?.draw(b);

             junimoNoteIcon?.draw(b);

             if (hoverText != null && (hoveredItem == null || ItemsToGrabMenu == null))
             {
                 if (hoverAmount > 0)
                 {
                     drawToolTip(b, hoverText, "", null, true,
                         moneyAmountToShowAtBottom: hoverAmount);
                 }
                 else
                 {
                     drawHoverText(b, hoverText, Game1.smallFont);
                 }
             }

             if (hoveredItem != null)
             {
                 drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);
             }
             else if (hoveredItem != null && ItemsToGrabMenu != null)
             {
                 drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem,
                     heldItem != null);
             }

             heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

             Game1.mouseCursorTransparency = 1f;
             drawMouse(b);
        }
    }
}