using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Input;

namespace SB_VerticalToolMenu
{
    class VerticalToolBar : IClickableMenu
    {
        public List<ClickableComponent> buttons = new List<ClickableComponent>();
        public static int NUM_BUTTONS = 5;
        private string hoverTitle = "";
        private float transparency = 1f;
        public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);
        public int numToolsinToolbar = 0;
        private Item hoverItem;
        public bool forceDraw = false;

        public VerticalToolBar(int x, int y, int numButtons = 5, bool forceDraw = false)
            : base(x, y, 
                  (Game1.tileSize * 3 / 2), 
                  ( (Game1.tileSize * numButtons) + (Game1.tileSize / 2) ), false)
        {
            NUM_BUTTONS = numButtons;
            this.forceDraw = forceDraw;
            for (int count = Game1.player.items.Count; count < (36 + VerticalToolBar.NUM_BUTTONS); count++)
            {
                Game1.player.items.Add((Item)null);
            }

            for (int index = 0; index < NUM_BUTTONS; ++index)
            {
                this.buttons.Add(
                    new ClickableComponent(
                        new Rectangle(
                            (Game1.viewport.Width / 2 - Game1.tileSize * 15 / 2 - Game1.pixelZoom * 4) + 16,
                            this.yPositionOnScreen + (index * Game1.tileSize),
                            Game1.tileSize, 
                            Game1.tileSize),
                        string.Concat((object)(index + 36))));
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.player.usingTool)
                return;
            foreach (ClickableComponent button in this.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    Game1.player.CurrentToolIndex = Convert.ToInt32(button.name);
                    if (Game1.player.ActiveObject != null)
                    {
                        Game1.player.showCarrying();
                        Game1.playSound("pickUpItem");
                        break;
                    }
                    Game1.player.showNotCarrying();
                    Game1.playSound("stoneStep");
                    break;
                }
            }
        }

        public Item rightClick(int x, int y, Item toAddTo, bool playSound = true)
        {
            foreach (ClickableComponent button in this.buttons)
            {
                int int32 = Convert.ToInt32(button.name);
                int x1 = x;
                int y1 = y;
                if (button.containsPoint(x1, y1) && Game1.player.items[int32] != null)
                {
                    if (Game1.player.items[int32] is Tool && (toAddTo == null || toAddTo is StardewValley.Object) && (Game1.player.items[int32] as Tool).canThisBeAttached((StardewValley.Object)toAddTo))
                        return (Item)(Game1.player.items[int32] as Tool).attach(toAddTo == null ? (StardewValley.Object)null : (StardewValley.Object)toAddTo);
                    if (toAddTo == null)
                    {
                        if (Game1.player.items[int32].maximumStackSize() != -1)
                        {
                            if (int32 == Game1.player.CurrentToolIndex && Game1.player.items[int32] != null && Game1.player.items[int32].Stack == 1)
                                Game1.player.items[int32].actionWhenStopBeingHeld(Game1.player);
                            Item one = Game1.player.items[int32].getOne();
                            if (Game1.player.items[int32].Stack > 1)
                            {
                                if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new InputButton[1] { new InputButton(Keys.LeftShift) }))
                                {
                                    one.Stack = (int)Math.Ceiling((double)Game1.player.items[int32].Stack / 2.0);
                                    Game1.player.items[int32].Stack = Game1.player.items[int32].Stack / 2;
                                    goto label_15;
                                }
                            }
                            if (Game1.player.items[int32].Stack == 1)
                                Game1.player.items[int32] = (Item)null;
                            else
                                --Game1.player.items[int32].Stack;
                            label_15:
                            if (Game1.player.items[int32] != null && Game1.player.items[int32].Stack <= 0)
                                Game1.player.items[int32] = (Item)null;
                            if (playSound)
                                Game1.playSound("dwop");
                            return one;
                        }
                    }
                    else if (Game1.player.items[int32].canStackWith(toAddTo) && toAddTo.Stack < toAddTo.maximumStackSize())
                    {
                        if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new InputButton[1] { new InputButton(Keys.LeftShift) }))
                        {
                            toAddTo.Stack += (int)Math.Ceiling((double)Game1.player.items[int32].Stack / 2.0);
                            Game1.player.items[int32].Stack = Game1.player.items[int32].Stack / 2;
                        }
                        else
                        {
                            ++toAddTo.Stack;
                            --Game1.player.items[int32].Stack;
                        }
                        if (playSound)
                            Game1.playSound("dwop");
                        if (Game1.player.items[int32].Stack <= 0)
                        {
                            if (int32 == Game1.player.CurrentToolIndex)
                                Game1.player.items[int32].actionWhenStopBeingHeld(Game1.player);
                            Game1.player.items[int32] = (Item)null;
                        }
                        return toAddTo;
                    }
                }
            }
            return toAddTo;
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverItem = (Item)null;
            foreach (ClickableComponent button in this.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    int int32 = Convert.ToInt32(button.name);
                    if (int32 < Game1.player.items.Count && Game1.player.items[int32] != null)
                    {
                        button.scale = Math.Min(button.scale + 0.05f, 1.1f);
                        this.hoverTitle = Game1.player.items[int32].Name;
                        this.hoverItem = Game1.player.items[int32];
                    }
                }
                else
                    button.scale = Math.Max(button.scale - 0.025f, 1f);
            }
        }

        public void shifted(bool right)
        {
            if (right)
            {
                for (int index = 0; index < this.buttons.Count; ++index)
                    this.buttons[index].scale = (float)(1.0 + (double)index * 0.0299999993294477);
            }
            else
            {
                for (int index = this.buttons.Count - 1; index >= 0; --index)
                    this.buttons[index].scale = (float)(1.0 + (double)(11 - index) * 0.0299999993294477);
            }
        }

        public override void update(GameTime time)
        {
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            for (int index = 0; index < NUM_BUTTONS; ++index)
                buttons[index].bounds = new Rectangle(
                            //TODO: Use more reliable coordinates
                            Game1.activeClickableMenu is GameMenu ? this.xPositionOnScreen : (Game1.viewport.Width / 2 - Game1.tileSize * 15 / 2 - Game1.pixelZoom * 4),
                            this.yPositionOnScreen + (index * Game1.tileSize),
                            Game1.tileSize,
                            Game1.tileSize);
        }

        public override bool isWithinBounds(int x, int y)
        {
            return new Rectangle(
                this.buttons.First<ClickableComponent>().bounds.X, 
                this.buttons.First<ClickableComponent>().bounds.Y,
                Game1.tileSize,
                this.buttons.Last<ClickableComponent>().bounds.Y - this.buttons.First<ClickableComponent>().bounds.Y + Game1.tileSize).Contains(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            //Checks if the player is on any other menu before drawing the tooltip
            if (Game1.activeClickableMenu != null && !forceDraw)
                return;
            //Checks and draws the buttons
            if (!forceDraw)
            {
                int positionOnScreen1 = this.yPositionOnScreen;
                if (Game1.options.pinToolbarToggle)
                {
                    this.yPositionOnScreen = Game1.viewport.Height - getInitialHeight();
                    this.transparency = Math.Min(1f, this.transparency + 0.075f);
                    if ((double)Game1.GlobalToLocal(Game1.viewport, new Vector2((float)Game1.player.GetBoundingBox().Center.X, (float)Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height - Game1.tileSize * 3))
                        this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
                }
                else
                    this.yPositionOnScreen = (double)Game1.GlobalToLocal(Game1.viewport, new Vector2((float)Game1.player.GetBoundingBox().Center.X, (float)Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height / 2 + Game1.tileSize) ? Game1.tileSize / 8 : Game1.viewport.Height - getInitialHeight() - Game1.tileSize / 8;
                int positionOnScreen2 = this.yPositionOnScreen;
                if (positionOnScreen1 != positionOnScreen2)
                {
                    for (int index = 0; index < NUM_BUTTONS; ++index)
                        this.buttons[index].bounds.Y = this.yPositionOnScreen + (index * Game1.tileSize);
                }
            }
            //Draws the backgound texture. 
            IClickableMenu.drawTextureBox(
                b, 
                Game1.menuTexture, 
                this.toolbarTextSource,
                //TODO: Use more reliable coordinates
                Game1.activeClickableMenu is GameMenu? this.xPositionOnScreen: (Game1.viewport.Width / 2 - Game1.tileSize * 15 / 2 - Game1.pixelZoom * 4),
                this.yPositionOnScreen,
                Game1.tileSize * 3 / 2,
                ((Game1.tileSize * NUM_BUTTONS) + (Game1.tileSize / 2)), 
                Color.White * this.transparency, 1f, false);
            int toolBarIndex = 0;
            for (int index = 0; index < NUM_BUTTONS; ++index)
            {
                this.buttons[index].scale = Math.Max(1f, this.buttons[index].scale - 0.025f);
                Vector2 location = new Vector2(
                    //TODO: Use more reliable coordinates
                    (Game1.activeClickableMenu is GameMenu ? this.xPositionOnScreen : (Game1.viewport.Width / 2 - Game1.tileSize * 15 / 2 - Game1.pixelZoom * 4)) + 16,
                    (float)(this.yPositionOnScreen + (index * Game1.tileSize + 16)));
                b.Draw(Game1.menuTexture, location, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, Game1.player.CurrentToolIndex == (index + 36) ? 56 : 10, -1, -1)), Color.White * transparency);
                // Need to customize it for toolset //string text = index == 9 ? "0" : (index == 10 ? "-" : (index == 11 ? "=" : string.Concat((object)(index + 1))));
                //b.DrawString(Game1.tinyFont, text, position + new Vector2(4f, -8f), Color.DimGray * this.transparency);
                if (Game1.player.items.Count > (index + 36) && Game1.player.items.ElementAt<Item>((index + 36)) != null)
                {
                    Game1.player.items[(index + 36)].drawInMenu(b, location, Game1.player.CurrentToolIndex == (index + 36) ? 0.9f : this.buttons.ElementAt<ClickableComponent>(index).scale * 0.8f, this.transparency, 0.88f);
                    toolBarIndex++;
                }
            }
            if (toolBarIndex != numToolsinToolbar)
                numToolsinToolbar = toolBarIndex;

            //If an item is hovered, shows its tooltip.
            if (this.hoverItem == null)
                return;
            IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.Name, this.hoverItem, false, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            this.hoverItem = (Item)null;
        }

        public static int getInitialWidth()
        {
            return (Game1.tileSize * 3 / 2) ;
        }

        public static int getInitialHeight()
        {
            return ((Game1.tileSize * NUM_BUTTONS) + (Game1.tileSize / 2));
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
