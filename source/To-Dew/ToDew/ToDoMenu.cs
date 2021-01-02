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
            internal const int MenuItemHeight = 60;
            internal readonly ToDoList.ListItem todoItem;
            public MenuItem(ToDoList.ListItem todoItem)
                : base(Rectangle.Empty, "ToDo: " + todoItem.Text) {
                this.todoItem = todoItem;
            }
            /// <summary>Draw the ToDo List item to the screen.</summary>
            /// <param name="spriteBatch">The sprite batch being drawn.</param>
            /// <param name="positionX">The X position at which to draw the item.</param>
            /// <param name="positionY">The Y position at which to draw the item.</param>
            /// <param name="width">The width to draw.</param>
            /// <param name="highlight">Whether to highlight the search result.</param>
            public Vector2 Draw(SpriteBatch spriteBatch, int positionX, int positionY, int width, bool highlight = false) {
                // update bounds
                this.bounds.X = positionX;
                this.bounds.Y = positionY;
                this.bounds.Width = width;
                this.bounds.Height = MenuItemHeight;
                const int borderWidth = 2;
                int leftMarginReserve = 70;
                int topPadding = this.bounds.Height / 2;

                // draw
                if (highlight)
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(this.bounds.Width, this.bounds.Height), Color.Beige);
                spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(this.bounds.Width, borderWidth), Color.Black); // border
                spriteBatch.DrawTextBlock(Game1.smallFont, todoItem.Text, new Vector2(this.bounds.X, this.bounds.Y) + new Vector2(leftMarginReserve, topPadding), this.bounds.Width - leftMarginReserve); // text

                // return size
                return new Vector2(this.bounds.Width, this.bounds.Height);
            }
        }

        private readonly IMonitor Monitor;
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

        public ToDoMenu(IMonitor monitor, ToDoList theList) {
            this.Monitor = monitor;
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
            menuItemList = new List<MenuItem>(theList.Items.Count);
            foreach (ToDoList.ListItem item in theList.Items) {
                menuItemList.Add(new MenuItem(item));
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
            float wrapWidth = this.width - leftOffset - gutter;


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

                Vector2 titleSize = backgroundBatch.DrawTextBlock(font, "To-Dew List", new Vector2(x + leftOffset, y + topOffset), wrapWidth, bold: true);
                Vector2 farmNameSize = backgroundBatch.DrawTextBlock(font, "for " + Game1.player.farmName + " Farm", new Vector2(x + leftOffset + titleSize.X + spaceWidth, y + topOffset), wrapWidth);
                topOffset += Math.Max(titleSize.Y, farmNameSize.Y);

                this.Textbox.X = x + (int)leftOffset;
                this.Textbox.Y = y + (int)topOffset;
                this.Textbox.Width = (int)wrapWidth;
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
                            bool isHighlighted = item.containsPoint(mouseX, mouseY);
                            var objSize = item.Draw(contentBatch, x + (int)leftOffset, y + (int)topOffset, (int)wrapWidth, isHighlighted);
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
            if (key.Equals(Keys.Escape))
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
            foreach (MenuItem match in this.menuItemList) {
                if (match.containsPoint(x, y)) {
                    this.theList.DeleteItem(match.todoItem.Id);
                    Game1.playSound("trashcan");
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
