using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace DeliveryService.Framework
{
    internal class ScrollBar
    {
        private readonly IModEvents Events;
        private readonly IInputHelper InputHelper;
        internal readonly Rectangle Coords;
        private readonly ClickableTextureComponent UpArrow;
        private readonly ClickableTextureComponent DownArrow;
        private readonly ClickableTextureComponent Scrollbar;
        private readonly Rectangle ScrollbarRunner;
        private readonly int NumItems;
        public int ItemsonScreen { get; private set; }
        private bool IsScrolling = false;
        public int CurrentItemIndex { get; private set; }
        public ScrollBar(IModEvents events, IInputHelper inputHelper, Rectangle scroll_area, int item_count, int item_height)
        {
            int width = 11;
            SpriteFont font = Game1.smallFont;
            this.Events = events;
            this.InputHelper = inputHelper;
            this.NumItems = item_count;
            this.Coords = new Rectangle(scroll_area.Right - width * Game1.pixelZoom, scroll_area.Y, width * Game1.pixelZoom, scroll_area.Height);
            this.ItemsonScreen = (int)(this.Coords.Height / item_height);
            this.UpArrow = new ClickableTextureComponent("up-arrow", new Rectangle(this.Coords.X, this.Coords.Y, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            this.DownArrow = new ClickableTextureComponent("down-arrow", new Rectangle(this.Coords.X, this.Coords.Bottom - 12 * Game1.pixelZoom, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            this.Scrollbar = new ClickableTextureComponent("scrollbar", new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            this.ScrollbarRunner = new Rectangle(this.Scrollbar.bounds.X, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, this.Scrollbar.bounds.Width, this.DownArrow.bounds.Y - this.UpArrow.bounds.Bottom - 2 * Game1.pixelZoom);
            this.CurrentItemIndex = 0;
        }
        public void Show()
        {
            this.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Events.Input.ButtonReleased += this.OnButtonReleased;
            this.Events.Input.CursorMoved += this.OnCursorMoved;
            this.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
        }
        public void Hide()
        {
            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
            this.Events.Input.ButtonReleased -= this.OnButtonReleased;
            this.Events.Input.CursorMoved -= this.OnCursorMoved;
            this.Events.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            this.UpArrow.draw(spriteBatch);
            this.DownArrow.draw(spriteBatch);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.ScrollbarRunner.X, this.ScrollbarRunner.Y, this.ScrollbarRunner.Width, this.ScrollbarRunner.Height, Color.White, Game1.pixelZoom, false);
            this.Scrollbar.draw(spriteBatch);
        }
        private void SetScrollBarToCurrentIndex()
        {
            float bottom_item = Math.Max(0, this.NumItems - this.ItemsonScreen);
            int top = this.ScrollbarRunner.Top;
            int bottom = this.ScrollbarRunner.Bottom - this.Scrollbar.bounds.Height;
            this.Scrollbar.bounds.Y = (int)(top + (bottom - top) * this.CurrentItemIndex / bottom_item);
        }

        public void LeftClickHeld(int x, int y)
        {
            if (this.IsScrolling)
            {
                y -= Game1.pixelZoom * 3;
                int min = this.ScrollbarRunner.Top;
                int max = this.ScrollbarRunner.Bottom - this.Scrollbar.bounds.Height;
                y = Math.Min(Math.Max(y, min), max);
                float pos = ((float)y - min) / (max - min);
                int bottom_item = Math.Max(0, this.NumItems - this.ItemsonScreen);
                this.CurrentItemIndex = (int)(bottom_item * pos);
                this.SetScrollBarToCurrentIndex();
            }
        }
        public void ReleaseLeftClick(int x, int y)
        {
            this.IsScrolling = false;
        }

        public bool ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.DownArrow.containsPoint(x, y)){
                
                    this.DownArrowPressed();
                Game1.soundBank.PlayCue("shwip");
                return true;
            }
            if (this.UpArrow.containsPoint(x, y))
            {
                this.UpArrowPressed();
                Game1.soundBank.PlayCue("shwip");
                return true;
            }
            if (this.Scrollbar.containsPoint(x, y))
            {
                this.IsScrolling = true;
                return true;
            }
            if (y >= this.UpArrow.bounds.Top && y <= this.DownArrow.bounds.Bottom && x >= this.UpArrow.bounds.Left && x <= this.UpArrow.bounds.Right)
            {
                this.IsScrolling = true;
                this.LeftClickHeld(x, y);
                this.ReleaseLeftClick(x, y);
            }

            return false;
        }
        void DownArrowPressed()
        {
            if (this.CurrentItemIndex >= Math.Max(0, this.NumItems - this.ItemsonScreen))
                return;
            this.CurrentItemIndex++;
            this.SetScrollBarToCurrentIndex();
        }
        void UpArrowPressed()
        {
            if (this.CurrentItemIndex <= 0)
                return;
            this.CurrentItemIndex--;
            this.SetScrollBarToCurrentIndex();
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft || e.Button.IsUseToolButton())
            {
                if (this.ReceiveLeftClick(Game1.getMouseX(), Game1.getMouseY()))
                    if (this.InputHelper != null)
                        this.InputHelper.Suppress(e.Button);
            }

        }
        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            this.ReleaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
        }
        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            this.LeftClickHeld(Game1.getMouseX(), Game1.getMouseY());
        }
        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {

        }
    }
}