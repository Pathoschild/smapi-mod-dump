/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2020 Jamie Taylor
// Portions Copyright 2016–2019 Pathoschild and other contributors, see NOTICE for license
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ToDew {
    /// <summary>
    /// Encapsulates the UI for the to-do list.
    /// </summary>
    public class ToDoMenu : IClickableMenu, IDisposable {
        /// <summary>
        /// Rendering for an individual row in the to-do list.
        /// </summary>
        private class MenuItem : ClickableComponent {
            const int MinMenuItemHeight = 40;
            const int rightMarginReserve = 40;
            const int borderWidth = 2;
            const int leftMarginReserve = 70;
            const int topPadding = 5;

            internal readonly ToDoList.ListItem todoItem;
            internal readonly int myIndex;
            internal readonly int totalItemCount;
            public MenuItem(ToDoList.ListItem todoItem, int index, int totalItemCount)
                : base(Rectangle.Empty, "ToDo: " + todoItem.Text) {
                this.todoItem = todoItem;
                this.myIndex = index;
                this.totalItemCount = totalItemCount;
            }

            public bool IsFirstItem { get => myIndex == 0; }
            public bool IsLastItem { get => myIndex == totalItemCount - 1; }

            public static readonly Rectangle smallUpArrow = new Rectangle(420, 459, 12, 12);
            public static readonly Rectangle smallDownArrow = new Rectangle(420, 472, 12, 12);

            private int lastX = 0;
            private int lastY = 0;
            private int lastWidth = 0;

            private Rectangle upArrowBounds;
            private Rectangle downArrowBounds;
            private Rectangle insideBorderArea;
            // computes all of the bounds for this object and sub-objects except total Height
            private void ComputeBounds(int positionX, int positionY, int width) {
                this.bounds.X = positionX;
                this.bounds.Y = positionY;
                this.bounds.Width = width;
                this.insideBorderArea = new Rectangle(positionX + borderWidth * 2, positionY + borderWidth, width - borderWidth * 4, 0);
                if (IsFirstItem) {
                    upArrowBounds = Rectangle.Empty;
                } else {
                    upArrowBounds = new Rectangle(this.bounds.X + this.bounds.Width - rightMarginReserve + 8, this.bounds.Y + 8, smallUpArrow.Width, smallUpArrow.Height);
                }
                if (IsLastItem) {
                    downArrowBounds = Rectangle.Empty;
                } else {
                    downArrowBounds = new Rectangle(this.bounds.X + this.bounds.Width - rightMarginReserve + 8, this.bounds.Y + 10 + smallUpArrow.Height, smallDownArrow.Width, smallDownArrow.Height);
                }
            }
            /// <summary>Draw the ToDo List item to the screen.</summary>
            /// <param name="spriteBatch">The sprite batch being drawn.</param>
            /// <param name="positionX">The X position at which to draw the item.</param>
            /// <param name="positionY">The Y position at which to draw the item.</param>
            /// <param name="width">The width to draw.</param>
            /// <param name="highlight">Whether to highlight the search result.</param>
            public Vector2 Draw(SpriteBatch spriteBatch, int positionX, int positionY, int width, int mouseX, int MouseY) {
                if (positionX != lastX || positionY != lastY || width != lastWidth) {
                    // update bounds
                    ComputeBounds(positionX, positionY, width);
                }
                Color highlightBorderColor = Color.Black;
                bool mouseInButton = false;

                // draw
                var textSize = spriteBatch.DrawTextBlock(Game1.smallFont, todoItem.Text, new Vector2(this.bounds.X, this.bounds.Y) + new Vector2(leftMarginReserve, topPadding), this.bounds.Width - leftMarginReserve - rightMarginReserve); // text
                this.bounds.Height = Math.Max(MinMenuItemHeight, topPadding + (int)textSize.Y);
                this.insideBorderArea.Height = this.bounds.Height - borderWidth * 2;
                spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(this.bounds.Width, borderWidth), Color.Black); // border
                if (!IsFirstItem) {
                    bool highlight = upArrowBounds.Contains(mouseX, MouseY);
                    spriteBatch.DrawSprite(Game1.mouseCursors, smallUpArrow, upArrowBounds.X, upArrowBounds.Y, null, highlight ? 1.2f : 1.0f);
                    mouseInButton |= highlight;
                }
                if (!IsLastItem) {
                    bool highlight = downArrowBounds.Contains(mouseX, MouseY);
                    spriteBatch.DrawSprite(Game1.mouseCursors, smallDownArrow, downArrowBounds.X, downArrowBounds.Y, null, highlight ? 1.2f : 1.0f);
                    mouseInButton |= highlight;
                }
                if (/* !mouseInButton &&*/ this.containsPoint(mouseX, MouseY)) {
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + borderWidth, new Vector2(this.bounds.Width, borderWidth), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + this.bounds.Height - borderWidth, new Vector2(this.bounds.Width, borderWidth), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(borderWidth * 2, this.bounds.Height), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X + this.bounds.Width - borderWidth * 2, this.bounds.Y, new Vector2(borderWidth * 2, this.bounds.Height), highlightBorderColor);
                    if (IsLastItem) {
                        // there is no item below us to draw a border, so let's do it ourselves so the highlight box doesn't look weird
                        spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + this.bounds.Height, new Vector2(this.bounds.Width, borderWidth), Color.Black); // border
                        this.bounds.Height += borderWidth;
                    }
                }

                // return size
                return new Vector2(this.bounds.Width, this.bounds.Height);
            }

            // presumes we aren't sent this message unless the mouse is actually within our bounds
            public void receiveClick(int mouseX, int mouseY, ToDoList theList) {
                if (upArrowBounds.Contains(mouseX, mouseY)) {
                    theList.MoveItemUp(todoItem.Id);
                    Game1.playSound("shwip");
                    return;
                }
                if (downArrowBounds.Contains(mouseX, mouseY)) {
                    theList.MoveItemDown(todoItem.Id);
                    Game1.playSound("shwip");
                    return;
                }
                if (insideBorderArea.Contains(mouseX, mouseY)) {
                    theList.DeleteItem(todoItem.Id);
                    Game1.playSound("trashcan");
                    return;
                }
            }
        }

        private readonly ModEntry theMod;
        private readonly ToDoList theList;
        private List<MenuItem> menuItemList;

        private readonly TextBox Textbox;
        /// <summary>The maximum pixels to scroll.</summary>
        private int MaxScroll;
        /// <summary>The number of pixels to scroll.</summary>
        private int CurrentScroll;
        /// <summary>Force the CurrentScroll to the bottom (MaxScroll) after rendering all items</summary>
        /// Set after adding an item because adding is asynchronous for farmhands.  Cleared on other actions.
        private bool forceScrollToBottom = false;
        /// <summary>The area where list items are rendered (used to filter mouse clicks)</summary>
        private Rectangle contentArea = Rectangle.Empty;

        public ToDoMenu(ModEntry theMod, ToDoList theList) {
            this.theMod = theMod;
            this.theList = theList;

            // update size
            this.width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
            this.height = Math.Min((int)((float)Sprites.Letter.Sprite.Height / Sprites.Letter.Sprite.Width * this.width), Game1.viewport.Height);

            // update position
            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;

            // create the text box
            this.Textbox = new TextBox(Sprites.Textbox.Sheet, null, Game1.smallFont, Color.Black);
            this.Textbox.TitleText = "Add to-do item";
            this.Textbox.Selected = true;

            // initialize the list UI and callback
            theList.OnChanged += OnListChanged;
            syncMenuItemList();
        }

        private void syncMenuItemList() {
            var items = theList.Items;
            var itemCount = items.Count;
            menuItemList = new List<MenuItem>(itemCount);
            for (int i = 0; i < itemCount; i++) {
                menuItemList.Add(new MenuItem(items[i], i, itemCount));
            }
        }

        public override void draw(SpriteBatch b) {
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            const int gutter = 15;
            float leftOffset = gutter;
            float topOffset = gutter;
            float contentWidth = this.width - gutter * 2;
            float contentHeight = this.height - gutter * 2;
            float bodyWidth = this.width - leftOffset - gutter;


            // get font
            SpriteFont font = Game1.smallFont;
            float spaceWidth = CommonHelper.GetSpaceWidth(font);

            // draw background and header
            // (This uses a separate sprite batch because it needs to be drawn before the
            // foreground batch, and we can't use the foreground batch because the background is
            // outside the clipping area.)
            using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                float scale = this.width / (float)Sprites.Letter.Sprite.Width;
                backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                backgroundBatch.Draw(Sprites.Letter.Sheet, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen),
                    Sprites.Letter.Sprite, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                Vector2 titleSize = backgroundBatch.DrawTextBlock(font, "To-Dew List", new Vector2(x + leftOffset, y + topOffset), bodyWidth, bold: true);
                Vector2 farmNameSize = backgroundBatch.DrawTextBlock(font, "for " + Game1.player.farmName + " Farm", new Vector2(x + leftOffset + titleSize.X + spaceWidth, y + topOffset), bodyWidth);
                topOffset += Math.Max(titleSize.Y, farmNameSize.Y);

                this.Textbox.X = x + (int)leftOffset;
                this.Textbox.Y = y + (int)topOffset;
                this.Textbox.Width = (int)bodyWidth;
                this.Textbox.Draw(backgroundBatch);
                topOffset += this.Textbox.Height;

                backgroundBatch.End();
            }

            topOffset += gutter;
            int headerHeight = (int)topOffset;

            // draw foreground
            // (This uses a separate sprite batch to set a clipping area for scrolling.)
            using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                GraphicsDevice device = Game1.graphics.GraphicsDevice;
                Rectangle prevScissorRectangle = device.ScissorRectangle;
                try {
                    // begin draw
                    device.ScissorRectangle = new Rectangle(x + gutter, y + headerHeight, (int)contentWidth, (int)contentHeight - headerHeight);
                    contentArea = device.ScissorRectangle;
                    contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                    //// scroll view
                    this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                    this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                    topOffset -= this.CurrentScroll; // scrolled down == move text up

                    // draw fields
                    {
                        int mouseX = Game1.getMouseX();
                        int mouseY = Game1.getMouseY();
                        foreach (MenuItem item in this.menuItemList) {
                            var objSize = item.Draw(contentBatch, x + (int)leftOffset, y + (int)topOffset, (int)bodyWidth, mouseX, mouseY);
                            topOffset += objSize.Y;
                        }
                    }

                    // update max scroll
                    this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));
                    if (forceScrollToBottom) {
                        this.CurrentScroll = this.MaxScroll;
                    }

                    // draw scroll icons
                    if (this.MaxScroll > 0 && this.CurrentScroll > 0)
                        contentBatch.DrawSprite(CommonSprites.Icons.Sheet, CommonSprites.Icons.UpArrow, x + gutter, y + contentHeight - CommonSprites.Icons.DownArrow.Height - gutter - CommonSprites.Icons.UpArrow.Height);
                    if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
                        contentBatch.DrawSprite(CommonSprites.Icons.Sheet, CommonSprites.Icons.DownArrow, x + gutter, y + contentHeight - CommonSprites.Icons.DownArrow.Height);

                    // end draw
                    contentBatch.End();
                } finally {
                    device.ScissorRectangle = prevScissorRectangle;
                }


                this.drawMouse(Game1.spriteBatch);
            }
        }

        /// <summary>The method invoked when the player presses an input button.</summary>
        /// <param name="key">The pressed input.</param>
        public override void receiveKeyPress(Keys key) {
            // deliberately avoid calling base, which may let another key close the menu
            if (key.Equals(Keys.Escape) || this.theMod.config.secondaryCloseButton.Equals(key.ToSButton()))
                this.exitThisMenu();

            if (key.Equals(Keys.Enter)) {
                this.theList.AddItem(this.Textbox.Text);
                this.Textbox.Text = "";
                this.forceScrollToBottom = true;
                //this.MaxScroll += MenuItem.MenuItemHeight;
                //this.CurrentScroll = this.MaxScroll;
                Game1.playSound("coin");
            }
        }

        public override void receiveGamePadButton(Buttons b) {
            if (this.theMod.config.secondaryCloseButton.Equals(b.ToSButton())) {
                this.exitThisMenu();
            } else {
                base.receiveGamePadButton(b);
            }
        }

        /// <summary>The method invoked when the player scrolls the mouse wheel on the lookup UI.</summary>
        /// <param name="direction">The scroll direction.</param>
        public override void receiveScrollWheelAction(int direction) {
            this.forceScrollToBottom = false;
            this.CurrentScroll -= direction; // down direction == increased scroll
        }

        /// <summary>The method invoked when the player left-clicks on the menu UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            this.forceScrollToBottom = false;
            if (!contentArea.Contains(x, y)) return;
            foreach (MenuItem match in this.menuItemList) {
                if (match.containsPoint(x, y)) {
                    match.receiveClick(x, y, theList);
                    return;
                }
            }
        }

        /// <summary>The method invoked when the player right-clicks on the menu UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.forceScrollToBottom = false;
            if (!contentArea.Contains(x, y)) return;
            foreach (MenuItem match in this.menuItemList) {
                if (match.containsPoint(x, y)) {
                    this.Textbox.Text = match.todoItem.Text;
                    return;
                }
            }
        }

        private void OnListChanged(object sender, List<ToDoList.ListItem> e) {
            syncMenuItemList();
        }

        public void Dispose() {
            this.theList.OnChanged -= OnListChanged;
        }
    }

    // Based on https://github.com/Pathoschild/StardewMods/blob/develop/LookupAnything/Components/Sprites.cs
    /// <summary>Simplifies access to the game's sprite sheets.</summary>
    /// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
    internal static class Sprites {
        /*********
        ** Accessors
        *********/
        /// <summary>Sprites used to draw a letter.</summary>
        public static class Letter {
            /// <summary>The sprite sheet containing the letter sprites.</summary>
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");

            /// <summary>The letter background (including edges and corners).</summary>
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);

            /// <summary>The notebook paper letter background (including edges and corners).</summary>
            public static readonly Rectangle NotebookSprite = new Rectangle(320, 0, 320, 180);
        }

        /// <summary>Sprites used to draw a textbox.</summary>
        public static class Textbox {
            /// <summary>The sprite sheet containing the textbox sprites.</summary>
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        }
    }
}
