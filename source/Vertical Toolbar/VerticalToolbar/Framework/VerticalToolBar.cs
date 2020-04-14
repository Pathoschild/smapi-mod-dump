using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SB_VerticalToolMenu.Framework
{
    public enum Orientation
    {
        LeftOfToolbar,
        RightOfToolbar,
        BottomLeft,
        BottomRight
    }

    internal class VerticalToolBar : IClickableMenu
    {
        public List<ClickableComponent> buttons = new List<ClickableComponent>();
        public static int NUM_BUTTONS = 5;
        public Orientation orientation;
        private string hoverTitle = "";
        private float transparency = 1f;
        public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);
        public int numToolsInToolbar = 0;
        private Item hoverItem;
        public bool forceDraw = false;
        private int baseMaxItems = Game1.player.MaxItems;

        public VerticalToolBar(Orientation o, int numButtons = 5, bool forceDraw = false)
            : base()
        {

            orientation = o;
            NUM_BUTTONS = numButtons;
            this.forceDraw = forceDraw;
            getDimensions();
            // For compatibility with Bigger Backpack
            int newInventory = baseMaxItems + VerticalToolBar.NUM_BUTTONS;
            for (int count = Game1.player.Items.Count; count < newInventory; count++)
            {
                Game1.player.Items.Add(null);
            }

            for (int index = 0; index < NUM_BUTTONS; ++index)
            {
                this.buttons.Add(
                    new ClickableComponent(
                        new Rectangle(
                            this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder,
                            this.yPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (index * Game1.tileSize),
                            Game1.tileSize, 
                            Game1.tileSize),
                        string.Concat(index + baseMaxItems)));
            }
        }

        public static Toolbar getToolbar()
        {
            return Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();
        }

        public void getDimensions()
        {
            Rectangle dimensionRectangle;
            dimensionRectangle.Width = Game1.tileSize * 3 / 2;
            dimensionRectangle.Height = Game1.tileSize* NUM_BUTTONS +(Game1.tileSize / 2);

            switch (orientation)
            {
                case Orientation.LeftOfToolbar:
                    dimensionRectangle.X = (Game1.viewport.Width / 2 - 384 - 64) - (getInitialWidth() / 2);
                    dimensionRectangle.Y = Game1.viewport.Height - getInitialHeight(); 
                    break;
                case Orientation.RightOfToolbar:
                    dimensionRectangle.X = (Game1.viewport.Width / 2 - 384 - 64) + getToolbar().width - (getInitialWidth() / 2);
                    dimensionRectangle.Y = Game1.viewport.Height - getInitialHeight();
                    break;
                case Orientation.BottomLeft:
                    dimensionRectangle.X = IClickableMenu.spaceToClearSideBorder;
                    dimensionRectangle.Y = Game1.viewport.Height - getInitialHeight();
                    break;
                case Orientation.BottomRight:
                    dimensionRectangle.X = Game1.viewport.Width - (getInitialWidth() /2) -  IClickableMenu.spaceToClearSideBorder - getInitialWidth() - (Game1.showingHealth? 64 : 0);
                    dimensionRectangle.Y = Game1.viewport.Height - getInitialHeight();
                    break;
                default:
                    throw new NotSupportedException("Error: Orientation Not Supported");
            }
            this.xPositionOnScreen = dimensionRectangle.X;
            this.yPositionOnScreen = dimensionRectangle.Y;
            this.width = dimensionRectangle.Width;
            this.height = dimensionRectangle.Height;    

        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.player.UsingTool)
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

        public Item RightClick(int x, int y, Item toAddTo, bool playSound = true)
        {
            foreach (ClickableComponent button in this.buttons)
            {
                int int32 = Convert.ToInt32(button.name);
                int x1 = x;
                int y1 = y;
                if (button.containsPoint(x1, y1) && Game1.player.Items[int32] != null)
                {
                    if (Game1.player.Items[int32] is Tool && (toAddTo == null || toAddTo is SObject) && (Game1.player.Items[int32] as Tool).canThisBeAttached((SObject)toAddTo))
                        return (Game1.player.Items[int32] as Tool).attach((SObject)toAddTo);
                    if (toAddTo == null)
                    {
                        if (Game1.player.Items[int32].maximumStackSize() != -1)
                        {
                            if (int32 == Game1.player.CurrentToolIndex && Game1.player.Items[int32] != null && Game1.player.Items[int32].Stack == 1)
                                Game1.player.Items[int32].actionWhenStopBeingHeld(Game1.player);
                            Item one = Game1.player.Items[int32].getOne();
                            if (Game1.player.Items[int32].Stack > 1)
                            {
                                if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] { new InputButton(Keys.LeftShift) }))
                                {
                                    one.Stack = (int)Math.Ceiling(Game1.player.Items[int32].Stack / 2.0);
                                    Game1.player.Items[int32].Stack = Game1.player.Items[int32].Stack / 2;
                                    goto label_15;
                                }
                            }
                            if (Game1.player.Items[int32].Stack == 1)
                                Game1.player.Items[int32] = null;
                            else
                                --Game1.player.Items[int32].Stack;
                            label_15:
                            if (Game1.player.Items[int32] != null && Game1.player.Items[int32].Stack <= 0)
                                Game1.player.Items[int32] = null;
                            if (playSound)
                                Game1.playSound("dwop");
                            return one;
                        }
                    }
                    else if (Game1.player.Items[int32].canStackWith(toAddTo) && toAddTo.Stack < toAddTo.maximumStackSize())
                    {
                        if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] { new InputButton(Keys.LeftShift) }))
                        {
                            toAddTo.Stack += (int)Math.Ceiling(Game1.player.Items[int32].Stack / 2.0);
                            Game1.player.Items[int32].Stack = Game1.player.Items[int32].Stack / 2;
                        }
                        else
                        {
                            ++toAddTo.Stack;
                            --Game1.player.Items[int32].Stack;
                        }
                        if (playSound)
                            Game1.playSound("dwop");
                        if (Game1.player.Items[int32].Stack <= 0)
                        {
                            if (int32 == Game1.player.CurrentToolIndex)
                                Game1.player.Items[int32].actionWhenStopBeingHeld(Game1.player);
                            Game1.player.Items[int32] = null;
                        }
                        return toAddTo;
                    }
                }
            }
            return toAddTo;
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverItem = null;
            foreach (ClickableComponent button in this.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    int int32 = Convert.ToInt32(button.name);
                    if (int32 < Game1.player.Items.Count && Game1.player.Items[int32] != null)
                    {
                        button.scale = Math.Min(button.scale + 0.05f, 1.1f);
                        this.hoverTitle = Game1.player.Items[int32].Name;
                        this.hoverItem = Game1.player.Items[int32];
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
                    this.buttons[index].scale = (float)(1.0 + index * 0.0299999993294477);
            }
            else
            {
                for (int index = this.buttons.Count - 1; index >= 0; --index)
                    this.buttons[index].scale = (float)(1.0 + (11 - index) * 0.0299999993294477);
            }
        }

        public override void update(GameTime time)
        {
            if (baseMaxItems != Game1.player.MaxItems)
            {
                var newInventory = Game1.player.MaxItems;
                if (Game1.player.Items.Count() < (newInventory + NUM_BUTTONS) )
                {
                    for (int i = Game1.player.Items.Count(); i < (newInventory + NUM_BUTTONS); i++)
                        Game1.player.Items.Add(null);
                }
                for (int i= 0; i< NUM_BUTTONS; i++)
                {
                    this.buttons[i].name = string.Concat(i + newInventory);
                    Game1.player.Items[newInventory + i] = Game1.player.Items[baseMaxItems + i];
                    Game1.player.Items[baseMaxItems + i] = null;
                }
                if (Game1.player.CurrentToolIndex > (baseMaxItems -1) )
                    Game1.player.CurrentToolIndex += (newInventory - baseMaxItems);

                baseMaxItems = newInventory;
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            getDimensions();
            for (int index = 0; index < NUM_BUTTONS; ++index)
                buttons[index].bounds = new Rectangle(
                            //TODO: Use more reliable coordinates
                            this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder,
                            this.yPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (index * Game1.tileSize),
                            Game1.tileSize,
                            Game1.tileSize);
        }

        public override bool isWithinBounds(int x, int y)
        {
            return new Rectangle(
                this.buttons.First().bounds.X,
                this.buttons.First().bounds.Y,
                Game1.tileSize,
                this.buttons.Last().bounds.Y - this.buttons.First().bounds.Y + Game1.tileSize
            ).Contains(x, y);
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
                if (Game1.options.pinToolbarToggle )
                {
                    this.yPositionOnScreen = Game1.viewport.Height - getInitialHeight();
                    this.transparency = Math.Min(1f, this.transparency + 0.075f);
                    if (Game1.GlobalToLocal(Game1.viewport, new Vector2(Game1.player.GetBoundingBox().Center.X, Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height - Game1.tileSize * 3))
                        this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
                }

                else if ( !(orientation == Orientation.BottomLeft || orientation == Orientation.BottomRight) )
                    this.yPositionOnScreen = (double)Game1.GlobalToLocal(Game1.viewport, new Vector2(Game1.player.GetBoundingBox().Center.X, Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height / 2 + Game1.tileSize) ? Game1.tileSize / 8 : Game1.viewport.Height - getInitialHeight() - Game1.tileSize / 8;
                if (orientation == Orientation.BottomRight && Game1.showingHealth)
                {
                    int newXPos = Game1.viewport.Width - (getInitialWidth() / 2) - IClickableMenu.spaceToClearSideBorder - getInitialWidth() - 64;
                    xPositionOnScreen = newXPos;
                    foreach (ClickableComponent button in this.buttons)
                    {
                        button.bounds.X = newXPos + IClickableMenu.spaceToClearSideBorder;
                    }

                }
                int positionOnScreen2 = this.yPositionOnScreen;
                if (positionOnScreen1 != positionOnScreen2)
                {
                    for (int index = 0; index < NUM_BUTTONS; ++index)
                        this.buttons[index].bounds.Y = this.yPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (index * Game1.tileSize);
                }
            }
            //Draws the background texture. 
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, this.toolbarTextSource, this.xPositionOnScreen, this.yPositionOnScreen, this.width,
                this.height, Color.White * this.transparency, 1f, false);
            int toolBarIndex = 0;
            for (int index = 0; index < NUM_BUTTONS; ++index)
            {
                this.buttons[index].scale = Math.Max(1f, this.buttons[index].scale - 0.025f);
                Vector2 location = new Vector2(
                    //TODO: Use more reliable coordinates
                    this.buttons[index].bounds.X,
                    this.buttons[index].bounds.Y);
                b.Draw(Game1.menuTexture, location, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, Game1.player.CurrentToolIndex == (index + baseMaxItems) ? 56 : 10)), Color.White * transparency);
                // Need to customize it for toolset //string text = index == 9 ? "0" : (index == 10 ? "-" : (index == 11 ? "=" : string.Concat((object)(index + 1))));
                //b.DrawString(Game1.tinyFont, text, position + new Vector2(4f, -8f), Color.DimGray * this.transparency);
                if (Game1.player.Items.Count <= (index + baseMaxItems) || Game1.player.Items.ElementAt<Item>((index + baseMaxItems)) == null)
                {
                    continue;
                }
                Game1.player.Items[(index + baseMaxItems)].drawInMenu(b, location, Game1.player.CurrentToolIndex == (index + baseMaxItems) ? 0.9f : this.buttons.ElementAt<ClickableComponent>(index).scale * 0.8f, this.transparency, 0.88f);
                toolBarIndex++;
            }
            if (toolBarIndex != numToolsInToolbar)
                numToolsInToolbar = toolBarIndex;

            //draw the tooltip if it's feasible, else allow another method to explicitly draw it

            if(Game1.activeClickableMenu == null)
            {
                drawToolTip(b);
            }
        }
		
		public void drawToolTip(SpriteBatch b)
        {
            //If an item is hovered, shows its tooltip.
            if (this.hoverItem == null)
                return;
            IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.Name, this.hoverItem);
            this.hoverItem = null;
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
