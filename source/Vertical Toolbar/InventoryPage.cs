using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace SB_VerticalToolMenu
{
    class InventoryPage : StardewValley.Menus.InventoryPage
    {
        VerticalToolBar verticalToolBar;
        public InventoryPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            verticalToolBar = new VerticalToolBar(
                xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth * 2,
                yPositionOnScreen + IClickableMenu.spaceToClearTopBorder - IClickableMenu.borderWidth / 2 + 4,
                VerticalToolBar.NUM_BUTTONS,
                true);
        }

        public override void performHoverAction(int x, int y)
        {
            verticalToolBar.performHoverAction(x, y);
            base.performHoverAction(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            FieldInfo fieldInfo = typeof(InventoryPage).BaseType.GetField("heldItem", BindingFlags.NonPublic | BindingFlags.Instance);
            Item heldItem = (Item)fieldInfo.GetValue(this);
            for (int i = Game1.player.maxItems; i < StardewValley.Farmer.maxInventorySpace; i++)
            {
                if (Game1.player.items[i] != null)
                {
                    fieldInfo.SetValue(this, Game1.player.items[i]);
                    Game1.player.items[i] = null;
                }
            }
            foreach (ClickableComponent button in verticalToolBar.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    if (heldItem != null)
                    {
                        if (Game1.player.items[Convert.ToInt32(button.name)] == null || Game1.player.items[Convert.ToInt32(button.name)].canStackWith(heldItem))
                        {
                            if (Game1.player.CurrentToolIndex == Convert.ToInt32(button.name) && heldItem != null)
                                heldItem.actionWhenBeingHeld(Game1.player);
                            heldItem = Utility.addItemToInventory(heldItem, Convert.ToInt32(button.name), Game1.player.items, (ItemGrabMenu.behaviorOnItemSelect)null);
                            fieldInfo.SetValue(this, null);
                            Game1.playSound("stoneStep");
                            return;
                        }
                        if (Game1.player.items[Convert.ToInt32(button.name)] != null)
                        {
                            Item swapItem = (Item)fieldInfo.GetValue(this);
                            fieldInfo.SetValue(this, Game1.player.items[Convert.ToInt32(button.name)]);
                            Utility.addItemToInventory(swapItem, Convert.ToInt32(button.name), Game1.player.items);
                            return;

                        }
                    }
                    if (Game1.player.items[Convert.ToInt32(button.name)] != null)
                    {
                        fieldInfo.SetValue(this, Game1.player.items[Convert.ToInt32(button.name)]);
                        Utility.removeItemFromInventory(Convert.ToInt32(button.name), Game1.player.items);
                        return;
                    }
                }
            }
            if (this.organizeButton.containsPoint(x, y))
            {
                (Game1.player.items).Sort(0, 36, null);
                (Game1.player.items).Reverse(0, 36);
                Game1.playSound("Ship");
                return;
            }

            base.receiveLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (verticalToolBar.isWithinBounds(x, y))
            {
                FieldInfo fieldInfo = typeof(InventoryPage).BaseType.GetField("heldItem", BindingFlags.NonPublic | BindingFlags.Instance);
                Item heldItem = (Item)fieldInfo.GetValue(this);
                fieldInfo.SetValue(this, verticalToolBar.rightClick(x, y, heldItem, playSound));
                return;
            }
            base.receiveRightClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
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
