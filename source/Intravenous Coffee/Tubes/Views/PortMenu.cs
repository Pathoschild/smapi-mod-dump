using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pathoschild.Stardew.Common;
using StardewValley;
using StardewValley.Menus;

// TODO:
// - request/provide toggle? or 2 add buttons
// - request amount
// - scrolling

namespace Tubes
{
    internal delegate void PortDeleteFilter(PortFilter filter);

    internal enum PortFilterType
    {
        REQUESTS, PROVIDES
    }

    internal class PortFilterComponent
    {
        // toggle button for request/provide
        internal readonly PortFilter Filter;
        internal readonly DropdownComponent Dropdown;
        internal readonly ButtonComponent DeleteButton;
        internal ButtonComponent RequestAllToggle;
        internal SliderComponent RequestAmountSlider;
        internal bool RequestAmountChanged = false;

        public int Width { get; private set; }
        public int Height { get; private set; }

        internal PortFilterComponent(PortFilter filter, PortFilterType type, PortDeleteFilter onDeleted)
        {
            this.Filter = filter;

            int selected = 0;
            if (ItemCategories.NumToName.TryGetValue(this.Filter.Category, out string category))
                selected = ItemCategories.Names.IndexOf(category);

            this.Dropdown = new DropdownComponent(ItemCategories.Names, "", 300) { visible = true, SelectionIndex = selected };
            this.Dropdown.DropDownOptionSelected += DropDownOptionSelected;

            this.DeleteButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.Clear, 2, true) { visible = true, HoverText = "Delete" };
            this.DeleteButton.ButtonPressed += () => onDeleted(Filter);

            if (type == PortFilterType.REQUESTS)
                BuildSlider();
        }

        private void BuildSlider()
        {
            if (Filter.RequestAmount == int.MaxValue) {
                this.RequestAllToggle = new ButtonComponent("Until full", Sprites.Icons.Sheet, Sprites.Icons.Set, 2, true) { visible = true, HoverText = "Click to request a specific amount" };
                this.RequestAllToggle.ButtonPressed += () => { Filter.RequestAmount = 1; BuildSlider(); };
                this.RequestAmountSlider = null;
            } else {
                int min = Math.Max(0, 100 * (int)((Filter.RequestAmount - 1) / 100));
                this.RequestAmountSlider = new SliderComponent("Amount", Math.Max(1, min), min + 101, 1, Filter.RequestAmount, true, RequestAmountSlider?.X ?? 0, RequestAmountSlider?.Y ?? 0) { visible = true };
                this.RequestAmountSlider.SliderValueChanged += (v) => { Filter.RequestAmount = (int)v; RequestAmountChanged = true; };
                this.RequestAmountChanged = false;
                this.RequestAllToggle = null;
            }
            UpdateLayout(Dropdown.X, Dropdown.Y, Width, Height);
        }

        internal void DropDownOptionSelected(int selected)
        {
            string category = ItemCategories.Names[selected];
            this.Filter.Category = ItemCategories.NameToNum[category];
        }

        public bool receiveLeftClick(int x, int y, bool playSound = true)
        {
            Dropdown.receiveLeftClick(x, y, playSound);
            RequestAmountSlider?.receiveLeftClick(x, y, playSound);
            RequestAllToggle?.receiveLeftClick(x, y, playSound);
            if (DeleteButton.containsPoint(x, y)) {
                DeleteButton.receiveLeftClick(x, y, playSound);
                return true;
            }
            return false;
        }

        public void leftClickHeld(int x, int y)
        {
            Dropdown.leftClickHeld(x, y);
            RequestAmountSlider?.leftClickHeld(x, y);
        }

        public void releaseLeftClick(int x, int y)
        {
            Dropdown.releaseLeftClick(x, y);
            RequestAmountSlider?.releaseLeftClick(x, y);
            if (RequestAmountChanged)
                BuildSlider();
        }

        public void performHoverAction(int x, int y)
        {
            RequestAllToggle?.performHoverAction(x, y);
            DeleteButton.performHoverAction(x, y);
        }

        public void draw(SpriteBatch b) {
            Dropdown.draw(Dropdown.IsActiveComponent() ? Game1.spriteBatch : b);
            RequestAllToggle?.draw(b);
            RequestAmountSlider?.draw(b);
            DeleteButton.draw(b);
        }

        public void UpdateLayout(int x, int y, int width, int height)
        {
            int margin = 24;

            int xpos = x;
            Dropdown.updateLocation(xpos, y, 300);
            xpos += Dropdown.Width + margin;

            int yMid;
            if (RequestAmountSlider != null) {
                yMid = Math.Max(0, (Dropdown.Height - RequestAmountSlider.Height) / 2);
                RequestAmountSlider.updateLocation(xpos, y + yMid);
                xpos += RequestAmountSlider.Width + margin;
            } else if (RequestAllToggle != null) {
                yMid = Math.Max(0, (Dropdown.Height - RequestAllToggle.Height) / 2);
                RequestAllToggle.updateLocation(xpos, y + yMid);
                xpos += RequestAllToggle.Width + margin;
            }

            yMid = Math.Max(0, (Dropdown.Height - DeleteButton.Height) / 2);
            DeleteButton.updateLocation(x + width - DeleteButton.Width, y + yMid);

            Width = width;
            Height = Dropdown.Height;
        }
    }

    internal class PortFiltersPage
    {
        internal List<PortFilter> Filters;
        internal List<PortFilterComponent> Components = new List<PortFilterComponent>();
        internal Action OnChanged;
        internal PortFilterType Type;

        internal PortFiltersPage(List<PortFilter> filters, Action onChanged, PortFilterType type)
        {
            this.Filters = filters;
            this.OnChanged = onChanged;
            this.Type = type;
            foreach (var filter in Filters)
                Components.Add(new PortFilterComponent(filter, Type, DeleteFilter));
        }

        internal void AddFilter()
        {
            PortFilter filter = new PortFilter();
            Filters.Add(filter);
            Components.Add(new PortFilterComponent(filter, Type, DeleteFilter));
            OnChanged();
        }

        internal void DeleteFilter(PortFilter filter)
        {
            int index = Filters.IndexOf(filter);
            Filters.RemoveAt(index);
            Components.RemoveAt(index);
            OnChanged();
            Game1.playSound("hammer");
        }
    }

    internal class PortMenu : IClickableMenu
    {
        private static readonly Rectangle kMenuTextureSourceRect = new Rectangle(0, 256, 60, 60);
        private const int kScrollAmount = 120;

        private readonly ButtonComponent RequestsTabButton;
        private readonly ButtonComponent ProvidesTabButton;
        private readonly ButtonComponent ScrollUpButton;
        private readonly ButtonComponent ScrollDownButton;
        private readonly ButtonComponent AddButton;

        private PortFilterType CurrentTab = PortFilterType.REQUESTS;

        private PortFiltersPage RequestsPage;
        private PortFiltersPage ProvidesPage;
        private PortFiltersPage Page { get => CurrentTab == PortFilterType.REQUESTS ? RequestsPage : ProvidesPage; }
        private List<PortFilterComponent> Filters { get => Page.Components; }

        private int ScrollOffset;
        private int ScrollMax;

        public PortMenu(List<PortFilter> requests, List<PortFilter> provides)
        {
            this.RequestsPage = new PortFiltersPage(requests, UpdateLayout, PortFilterType.REQUESTS);
            this.ProvidesPage = new PortFiltersPage(provides, UpdateLayout, PortFilterType.PROVIDES);

            this.RequestsTabButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.DownArrow, 1, true) { visible = true, HoverText = "Requests" };
            this.RequestsTabButton.ButtonPressed += () => { CurrentTab = PortFilterType.REQUESTS; ScrollMax = 0;  UpdateLayout(); };
            this.ProvidesTabButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.UpArrow, 1, true) { visible = true, HoverText = "Provides" };
            this.ProvidesTabButton.ButtonPressed += () => { CurrentTab = PortFilterType.PROVIDES; ScrollMax = 0; UpdateLayout(); };
            this.AddButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.GreenPlus, 3, true) { visible = true };
            this.AddButton.ButtonPressed += () => { Page.AddFilter(); };

            this.ScrollUpButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.UpArrow, 1, true) { visible = true };
            this.ScrollUpButton.ButtonPressed += this.ScrollUp;
            this.ScrollDownButton = new ButtonComponent("", Sprites.Icons.Sheet, Sprites.Icons.DownArrow, 1, true) { visible = true };
            this.ScrollDownButton.ButtonPressed += this.ScrollDown;

            this.UpdateLayout();
        }

        public override bool isWithinBounds(int x, int y)
        {
            return new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height).Expand(this.ScrollUpButton.Width + 30).Contains(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.isWithinBounds(x, y)) {
                foreach (var filter in this.Filters)
                    filter.Dropdown.releaseLeftClick(x, y);
                this.exitThisMenu();
                return;
            }

            foreach (var filter in this.Filters) {
                if (filter.receiveLeftClick(x, y, playSound))
                    return;  // stop processing when a filter is clicked, because it may modify Filters.
            }

            ScrollUpButton.receiveLeftClick(x, y, playSound);
            ScrollDownButton.receiveLeftClick(x, y, playSound);
            RequestsTabButton.receiveLeftClick(x, y, playSound);
            ProvidesTabButton.receiveLeftClick(x, y, playSound);
            AddButton.receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            foreach (var filter in this.Filters)
                filter.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            foreach (var filter in this.Filters)
                filter.releaseLeftClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            foreach (var filter in this.Filters)
                filter.performHoverAction(x, y);
            AddButton.performHoverAction(x, y);
            ProvidesTabButton.performHoverAction(x, y);
            RequestsTabButton.performHoverAction(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public override void receiveScrollWheelAction(int direction)
        {
            // positive direction is UP, positive ScrollOffset is DOWN.
            ScrollOffset -= direction;
            UpdateLayout();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            UpdateLayout();
        }

        public override void receiveGamePadButton(Buttons button)
        {
            switch (button) {
                // left click
                case Buttons.A:
                    Point p = Game1.getMousePosition();
                    this.receiveLeftClick(p.X, p.Y);
                    break;

                // exit
                case Buttons.B:
                    this.exitThisMenu();
                    break;

                // scroll up
                case Buttons.RightThumbstickUp:
                    ScrollUp();
                    break;

                // scroll down
                case Buttons.RightThumbstickDown:
                    ScrollDown();
                    break;
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            TubesMod._monitor.InterceptErrors("drawing the port filter menu", () => {
                int x = this.xPositionOnScreen;
                int y = this.yPositionOnScreen;
                const int gutter = 15;
                int contentWidth = this.width - gutter * 2;
                int contentHeight = this.height - gutter * 2;

                // draw background
                // (This uses a separate sprite batch because it needs to be drawn before the
                // foreground batch, and we can't use the foreground batch because the background is
                // outside the clipping area.)
                using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                    backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                    IClickableMenu.drawTextureBox(backgroundBatch, Game1.menuTexture, kMenuTextureSourceRect, this.xPositionOnScreen, this.yPositionOnScreen, width, height, Color.White);
                    RequestsTabButton.draw(backgroundBatch, x, y - RequestsTabButton.Height);
                    ProvidesTabButton.draw(backgroundBatch, x + RequestsTabButton.Width + 16, y - RequestsTabButton.Height);
                    ScrollUpButton.draw(backgroundBatch, x + width + gutter, y);
                    ScrollDownButton.draw(backgroundBatch, x + width + gutter, y + height - ScrollDownButton.Height);
                    backgroundBatch.End();
                }

                // draw foreground
                // (This uses a separate sprite batch to set a clipping area for scrolling.)
                using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                    // begin draw
                    GraphicsDevice device = Game1.graphics.GraphicsDevice;
                    device.ScissorRectangle = new Rectangle(x + gutter, y + gutter, contentWidth, contentHeight);
                    contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                    this.AddButton.draw(contentBatch);

                    foreach (var filter in this.Filters.Reverse<PortFilterComponent>())
                        filter.draw(contentBatch);

                    contentBatch.End();
                }

                // draw cursor
                this.drawMouse(Game1.spriteBatch);
            }, (Exception ex) => {
                TubesMod._monitor.InterceptErrors("handling an error in PortMenu.draw", () => this.exitThisMenu());
            });
        }

        private void UpdateLayout()
        {
            this.width = Math.Min(Game1.tileSize * 16, Game1.viewport.Width);
            this.height = Math.Min(Game1.tileSize * 10, Game1.viewport.Height);

            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;

            ScrollOffset = Math.Max(0, ScrollOffset); // don't scroll past top
            ScrollOffset = Math.Min(ScrollMax, ScrollOffset); // don't scroll past bottom

            int margin = 24;
            int x = this.xPositionOnScreen + margin;
            int y = this.yPositionOnScreen + margin - ScrollOffset;
            int contentHeight = this.height - margin * 2;
            int contentTop = y;

            foreach (PortFilterComponent filter in this.Filters) {
                filter.UpdateLayout(x, y, width - margin * 2, height - margin * 2);
                y += filter.Height + margin;
            }

            AddButton.updateLocation(x, y);
            AddButton.HoverText = CurrentTab == PortFilterType.REQUESTS ? "New Request" : "New Provider";
            y += AddButton.Height + margin;

            ScrollMax = Math.Max(0, y - contentTop - contentHeight);
        }

        private void ScrollUp()
        {
            receiveScrollWheelAction(kScrollAmount);
        }

        private void ScrollDown()
        {
            receiveScrollWheelAction(-kScrollAmount);
        }

    }
}