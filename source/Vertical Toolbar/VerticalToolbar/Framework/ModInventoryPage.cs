using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SB_VerticalToolMenu.Framework
{
    internal class ModInventoryPage : StardewValley.Menus.InventoryPage
    {
        private readonly VerticalToolBar verticalToolBar;

        public ModInventoryPage(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            verticalToolBar = new VerticalToolBar(
                Orientation.LeftOfToolbar,
                VerticalToolBar.NUM_BUTTONS,
                true)
            {
                xPositionOnScreen = this.xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth * 2,
                yPositionOnScreen = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder - IClickableMenu.borderWidth / 2 + 4
            };
        }

        public override void performHoverAction(int x, int y)
        {
            verticalToolBar.performHoverAction(x, y);
            base.performHoverAction(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Item heldItem = Game1.player.CursorSlotItem;
            for (int i = Game1.player.MaxItems; i < StardewValley.Farmer.maxInventorySpace; i++)
            {
                if (Game1.player.Items[i] != null)
                {
                    Game1.player.CursorSlotItem = Game1.player.Items[i];
                    Game1.player.Items[i] = null;
                }
            }
            foreach (ClickableComponent button in verticalToolBar.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    if (heldItem != null)
                    {
                        if (Game1.player.Items[Convert.ToInt32(button.name)] == null || Game1.player.Items[Convert.ToInt32(button.name)].canStackWith(heldItem))
                        {
                            if (Game1.player.CurrentToolIndex == Convert.ToInt32(button.name))
                                heldItem.actionWhenBeingHeld(Game1.player);
                            Utility.addItemToInventory(heldItem, Convert.ToInt32(button.name), Game1.player.Items);
                            Game1.player.CursorSlotItem = null;
                            Game1.playSound("stoneStep");
                            return;
                        }
                        if (Game1.player.Items[Convert.ToInt32(button.name)] != null)
                        {
                            Item swapItem = Game1.player.CursorSlotItem;
                            Game1.player.CursorSlotItem = Game1.player.Items[Convert.ToInt32(button.name)];
                            Utility.addItemToInventory(swapItem, Convert.ToInt32(button.name), Game1.player.Items);
                            return;

                        }
                    }
                    if (Game1.player.Items[Convert.ToInt32(button.name)] != null)
                    {
                        Game1.player.CursorSlotItem = Game1.player.Items[Convert.ToInt32(button.name)];
                        Utility.removeItemFromInventory(Convert.ToInt32(button.name), Game1.player.Items);
                        return;
                    }
                }
            }
            if (this.organizeButton.containsPoint(x, y))
            {
                List<Item> items = Game1.player.Items.ToList();
                items.Sort(0, Game1.player.MaxItems, null);
                items.Reverse(0, Game1.player.MaxItems);
                Game1.player.setInventory(items);
                Game1.playSound("Ship");
                return;
            }

            base.receiveLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (verticalToolBar.isWithinBounds(x, y))
            {
                Item heldItem = Game1.player.CursorSlotItem;
                Game1.player.CursorSlotItem = verticalToolBar.RightClick(x, y, heldItem, playSound);
                return;
            }
            base.receiveRightClick(x, y, playSound);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch b)
        {
            for (int index = 0; index < VerticalToolBar.NUM_BUTTONS; ++index)
                verticalToolBar.buttons[index].bounds = new Rectangle(
                            //TODO: Use more reliable coordinates
                            verticalToolBar.xPositionOnScreen,
                            verticalToolBar.yPositionOnScreen + (index * Game1.tileSize),
                            Game1.tileSize,
                            Game1.tileSize);
            base.draw(b);
            verticalToolBar.draw(b);
        }
    }
}
