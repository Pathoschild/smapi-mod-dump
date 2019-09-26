using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class MenuTab : IClickableMenu
    {
        private TabName Name;
        private List<MenuTabItem> Items;
        private List<ClickableComponent> ViewableItems;
        public string HoverText;
        private const int BUFFER = 16;
        private const int NUM_VIEWABLE = 6;
        private readonly ClickableTextureComponent upArrow;
        private readonly ClickableTextureComponent downArrow;
        private readonly ClickableTextureComponent scrollBar;
        private Rectangle scrollBarRunner;
        private bool scrolling;
        /// <summary>
        /// Always points to the index of the top most
        /// displayed item (its index in Items)
        /// </summary>
        public int currentItemIndex;

        /// <summary>
        /// Where to draw the header text.
        /// </summary>
        private readonly Point headerBounds;
        /// <summary>
        /// The header itself.
        /// </summary>
        private readonly OptionsElement header;

        public MenuTab(int x, int y, int width, int height, TabName name, List<SObject> items) : base(x, y, width, height, true)
        {
            Name = name;
            HoverText = "";
            currentItemIndex = 0;
            Items = new List<MenuTabItem>();
            foreach(SObject obj in items)
            {
                Items.Add(new MenuTabItem(obj));
            }

            ViewableItems = new List<ClickableComponent>();

            Initialize();

            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom, false);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize / 4, yPositionOnScreen + height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom, false);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + Game1.pixelZoom * 3, upArrow.bounds.Y + upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom, false);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + Game1.pixelZoom, scrollBar.bounds.Width, height - Game1.tileSize * 2 - upArrow.bounds.Height - Game1.pixelZoom * 2);
            headerBounds = new Point(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom);

            header = new OptionsElement(TabNameToString());
        }

        private void Initialize()
        {
            int XCoord = xPositionOnScreen + Game1.tileSize / 4;
            for (int i = 0; i < NUM_VIEWABLE; ++i)
            {
                int YCoord = yPositionOnScreen + (Game1.tileSize + BUFFER) * (i + 2);
                Rectangle bounds = new Rectangle(XCoord, YCoord, width - Game1.tileSize /2, (height - Game1.tileSize * 2) / (NUM_VIEWABLE + 1) + Game1.pixelZoom);
                ClickableComponent component = new ClickableComponent(bounds, i.ToString());

                component.myID = i;
                component.upNeighborID = i > 0 ? i + 1 : -7777;
                component.downNeighborID = i < NUM_VIEWABLE - 1 ? i - 1 : -7777;
                component.fullyImmutable = true;
                ViewableItems.Add(component);
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            header.draw(b, headerBounds.X, headerBounds.Y);

            if (Items.Count > 0)
            {
                b.End();
                b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                for (int i = 0; i < NUM_VIEWABLE && currentItemIndex + i < Items.Count; ++i)
                {
                    Items[currentItemIndex + i].draw(b, ViewableItems[i].bounds.X, ViewableItems[i].bounds.Y, xPositionOnScreen);
                }
                if (!GameMenu.forcePreventClose)
                {
                    upArrow.draw(b);
                    downArrow.draw(b);
                    if (Items.Count > NUM_VIEWABLE)
                    {
                        drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, Game1.pixelZoom, false);
                        scrollBar.draw(b);
                    }
                }
            }
            else
            {
                DrawEmptyListMessage(b);
            }
        }

        private void DrawEmptyListMessage(SpriteBatch b)
        {
            int x = headerBounds.X + Game1.tileSize / 2;
            int y = yPositionOnScreen + Game1.tileSize * 5;
            int width = 850;

            SpriteText.drawString(b, Utilities.GetTranslation("EMPTY_LIST_MESSAGE"), x, y, width: width);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if(upArrow.containsPoint(x, y) && currentItemIndex > 0)
            {
                upArrow.scale = upArrow.baseScale;
                TryToMoveUpOneItem();
                Game1.playSound("shwip");
            }
            else if(downArrow.containsPoint(x, y) && currentItemIndex < Items.Count - NUM_VIEWABLE)
            {
                downArrow.scale = downArrow.baseScale;
                TryMoveDownOneItem();
                Game1.playSound("shwip");
            }
            else if(scrollBar.containsPoint(x, y))
            {
                scrolling = true;
            }
            else if(scrollBarRunner.Contains(x, y))
            {
                scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            scrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (scrolling)
            {
                int initialY = scrollBar.bounds.Y;
                int howManyItemsToMove = Math.Abs(initialY - y) / GetScrollBarDivisionSize();
                if (howManyItemsToMove > 0)
                {
                    if(initialY - y > 0)
                    {
                        //Its moving up to the start of the list
                        TryToMoveUpXItems(howManyItemsToMove);
                    }
                    else
                    {
                        TryToMoveDownXItems(howManyItemsToMove);
                    }
                }
            }
        }

        private void TryToMoveUpXItems(int x)
        {
            if (currentItemIndex > x - 1)
            {
                currentItemIndex -= x;
            }
            else
            {
                //The user moved so fast that it thinks there's nothing left to scroll
                //e.g trying to move up 4 items when theres only 3 left
                currentItemIndex = 0;
            }
            SetScrollBarToCurrentItem();
        }

        private void TryToMoveDownXItems(int x)
        {
            if (currentItemIndex < Items.Count - NUM_VIEWABLE - x + 1)
            {
                currentItemIndex += x;
            }
            else
            {
                currentItemIndex = Items.Count - NUM_VIEWABLE;
            }
            SetScrollBarToCurrentItem();
        }

        private void TryToMoveUpOneItem()
        {
            if(currentItemIndex > 0)
            {
                currentItemIndex -= 1;
                SetScrollBarToCurrentItem();
            }
        }

        private void TryMoveDownOneItem()
        {
            if(currentItemIndex < Items.Count - NUM_VIEWABLE)
            {
                currentItemIndex += 1;
                SetScrollBarToCurrentItem();
            }
        }

        private void SetScrollBarToCurrentItem()
        {
            if(currentItemIndex == Items.Count - NUM_VIEWABLE)
            {
                scrollBar.bounds.Y = downArrow.bounds.Y - scrollBar.bounds.Height - Game1.pixelZoom;
            }
            else
            {
                scrollBar.bounds.Y = GetScrollBarDivisionSize() * currentItemIndex + upArrow.bounds.Bottom + Game1.pixelZoom;
            }
        }

        private int GetScrollBarDivisionSize()
        {
            return (scrollBarRunner.Height - scrollBar.bounds.Height) / Math.Max(1, Items.Count - NUM_VIEWABLE);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if(direction > 0)
            {
                TryToMoveUpOneItem();
            }
            else if(direction < 0)
            {
                TryMoveDownOneItem();
            }
            Game1.playSound("shiny4");
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            for (int i = 0; i < NUM_VIEWABLE; ++i)
            {
                if (currentItemIndex >= 0 && currentItemIndex + i < Items.Count && Items[currentItemIndex + i].bounds.Contains(x - ViewableItems[i].bounds.X, y - ViewableItems[i].bounds.Y))
                {
                    Game1.SetFreeCursorDrag();
                    break;
                }

            }
           
            if (scrollBarRunner.Contains(x, y))
                Game1.SetFreeCursorDrag();
            if (GameMenu.forcePreventClose)
                return;
            upArrow.tryHover(x, y, 0.1f);
            downArrow.tryHover(x, y, 0.1f);
            scrollBar.tryHover(x, y, 0.1f);
            for (int i = 0; i < NUM_VIEWABLE && currentItemIndex + i < Items.Count; ++i)
            {
                if (Items[currentItemIndex + i].HoverTextBounds.Contains(x, y))
                {
                    HoverText = Items[currentItemIndex + i].HoverText;
                    return;
                }
            }
        }

        private string TabNameToString()
        {
            switch (Name)
            {
                case TabName.SpecificSeasonTab:
                    return Utilities.GetTranslation("SEASON_SPECIFIC_HEADER");
                case TabName.SpecificCCSeasonTab:
                    return Utilities.GetTranslation("SEASON_SPECIFIC_CC_HEADER");
                case TabName.CommonCCTab:
                    return Utilities.GetTranslation("COMMON_CC_HEADER");
                case TabName.MerchantTab:
                    return Utilities.GetTranslation("MERCHANT_HEADER");
                case TabName.CookedItemsTab:
                    return Utilities.GetTranslation("COOKED_ITEMS_HEADER");
                default:
                    //Should never get here
                    return "Oopsies";
            }
        }
    }
}
