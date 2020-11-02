/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Helpers;
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ItemBags.Bags.BoundedBag;
using Object = StardewValley.Object;

namespace ItemBags.Menus
{
    public class ItemSlotRenderedEventArgs : EventArgs
    {
        public SpriteBatch SB { get; }
        public Rectangle Slot { get; }
        public Object Item { get; }
        public bool IsHovered { get; }

        public ItemSlotRenderedEventArgs(SpriteBatch SB, Rectangle Slot, Object Item, bool IsHovered)
        {
            this.SB = SB;
            this.Slot = Slot;
            this.Item = Item;
            this.IsHovered = IsHovered;
        }
    }

    public class BoundedBagMenu : IBagMenuContent
    {
        #region Lookup Anything Compatibility
        /// <summary>
        /// Warning - do not remove/rename this field. It is used via reflection by Lookup Anything mod.<para/>
        /// See also: <see cref="https://github.com/Pathoschild/StardewMods/tree/develop/LookupAnything#extensibility-for-modders"/>
        /// </summary>
        public Item HoveredItem { get; private set; }
        public void UpdateHoveredItem(CursorMovedEventArgs e)
        {
            if (Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
            {
                if (!GroupedOptions.IsEmptyMenu && GroupedOptions.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
                {
                    HoveredItem = GroupedOptions.GetHoveredItem();
                }
                else if (!UngroupedOptions.IsEmptyMenu && UngroupedOptions.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
                {
                    HoveredItem = UngroupedOptions.GetHoveredItem();
                }
                else
                {
                    HoveredItem = null;
                }
            }
            else
            {
                HoveredItem = null;
            }
        }
        #endregion Lookup Anything Compatibility

        public ItemBagMenu IBM { get; }
        public ItemBag Bag { get { return BoundedBag; } }
        public BoundedBag BoundedBag { get; }

        public int Padding { get; }

        /// <summary>If true, each distinct Item Id that can have multiple qualities will be displayed adjacent to each other in a group of 4 columns, where the columns in are the different qualities (Regular, Silver, Gold, Iridium)</summary>
        public bool GroupByQuality { get; }
        /// <summary>Only relevant if <see cref="GroupByQuality"/> = true. Layout options to use for Items that are grouped by quality.</summary>
        public GroupedLayoutOptions GroupedOptions { get; }

        /// <summary>Layout options to use for Items that are not grouped by quality. See also <see cref="GroupByQuality"/></summary>
        public UngroupedLayoutOptions UngroupedOptions { get; }

        /// <summary>The bounds of this menu's content, relative to <see cref="TopLeftScreenPosition"/></summary>
        public Rectangle RelativeBounds { get; private set; }
        public Rectangle Bounds { get; private set; }

        private Point _TopLeftScreenPosition;
        public Point TopLeftScreenPosition {
            get { return _TopLeftScreenPosition; }
            private set { SetTopLeft(value, true); }
        }

        public void SetTopLeft(Point Point) { SetTopLeft(Point, true); }
        private void SetTopLeft(Point NewValue, bool CheckIfChanged = true)
        {
            if (!CheckIfChanged || TopLeftScreenPosition != NewValue)
            {
                Point Previous = TopLeftScreenPosition;
                _TopLeftScreenPosition = NewValue;

                if (GroupedOptions != null && UngroupedOptions != null)
                {
                    Point Offset = new Point(TopLeftScreenPosition.X - Previous.X, TopLeftScreenPosition.Y - Previous.Y);

                    Point GroupedTopLeft = new Point(GroupedOptions.TopLeftScreenPosition.X + Offset.X, GroupedOptions.TopLeftScreenPosition.Y + Offset.Y);
                    this.GroupedOptions.SetTopLeft(GroupedTopLeft, CheckIfChanged);
                    Point UngroupedTopLeft = new Point(UngroupedOptions.TopLeftScreenPosition.X + Offset.X, UngroupedOptions.TopLeftScreenPosition.Y + Offset.Y);
                    this.UngroupedOptions.SetTopLeft(UngroupedTopLeft, CheckIfChanged);

                    if (HorizontalSeparatorPosition.HasValue)
                        HorizontalSeparatorPosition = HorizontalSeparatorPosition.Value.GetOffseted(Offset);

                    this.Bounds = new Rectangle(TopLeftScreenPosition.X, TopLeftScreenPosition.Y, RelativeBounds.Width, RelativeBounds.Height);
                }
            }
        }

        public BoundedBagMenu(ItemBagMenu IBM, BoundedBag Bag, bool GroupContentsByQuality, GroupedLayoutOptions GroupedLayout, UngroupedLayoutOptions UngroupedLayout, int Padding)
        {
            this.IBM = IBM;
            this.BoundedBag = Bag;
            this.Padding = Padding;

            this.GroupByQuality = GroupContentsByQuality;
            this.GroupedOptions = GroupedLayout;
            this.GroupedOptions.SetParent(this);
            this.GroupedOptions.OnItemSlotRendered += OnItemSlotRendered;
            this.UngroupedOptions = UngroupedLayout;
            this.UngroupedOptions.SetParent(this);
            this.UngroupedOptions.OnItemSlotRendered += OnItemSlotRendered;

            if (!GroupedOptions.IsEmptyMenu && !UngroupedOptions.IsEmptyMenu)
            {
                GroupedOptions.MenuNeighbors[NavigationDirection.Down] = UngroupedOptions;
                UngroupedOptions.MenuNeighbors[NavigationDirection.Up] = GroupedOptions;
            }

            SetTopLeft(Point.Zero, false);
            InitializeLayout(1);
        }

        public BoundedBagMenu(ItemBagMenu IBM, BoundedBag Bag, BagMenuOptions Opts, int Padding)
            : this(IBM, Bag, Opts.GroupByQuality, new GroupedLayoutOptions(Opts.GroupedLayoutOptions), new UngroupedLayoutOptions(Opts.UngroupedLayoutOptions), Padding) { }

        #region Input Handling

        #region Mouse Handling
        private Point CurrentMousePosition = new Point(0, 0);

        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            CurrentMousePosition = e.NewPosition.ScreenPixels.AsPoint();

            if (Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
            {
                if (!GroupedOptions.IsEmptyMenu && (GroupedOptions.Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || GroupedOptions.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint())))
                {
                    GroupedOptions.OnMouseMoved(e);
                }

                if (!UngroupedOptions.IsEmptyMenu && (UngroupedOptions.Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || UngroupedOptions.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint())))
                {
                    UngroupedOptions.OnMouseMoved(e);
                }
            }

            UpdateHoveredItem(e);
        }

        public void OnMouseButtonPressed(ButtonPressedEventArgs e)
        {
            bool Handled = false;
            if (e.Button == SButton.MouseLeft)
                Handled = TryHandlePrimaryAction(true);

            if (!Handled)
            {
                if (!GroupedOptions.IsEmptyMenu)
                    GroupedOptions.OnMouseButtonPressed(e);
                if (!UngroupedOptions.IsEmptyMenu)
                    UngroupedOptions.OnMouseButtonPressed(e);
            }
        }

        public void OnMouseButtonReleased(ButtonReleasedEventArgs e)
        {
            if (!GroupedOptions.IsEmptyMenu)
                GroupedOptions.OnMouseButtonReleased(e);
            if (!UngroupedOptions.IsEmptyMenu)
                UngroupedOptions.OnMouseButtonReleased(e);
        }
        #endregion Mouse Handling

        #region Gamepad support
        public bool RecentlyGainedFocus { get; private set; } = false;

        public bool IsGamepadFocused
        {
            get
            {
                return (GroupedOptions != null && !GroupedOptions.IsEmptyMenu && GroupedOptions.IsGamepadFocused) || 
                    (UngroupedOptions != null && !UngroupedOptions.IsEmptyMenu && UngroupedOptions.IsGamepadFocused);
            }
        }

        public void GainedGamepadFocus()
        {
            RecentlyGainedFocus = true;
        }

        public void LostGamepadFocus()
        {

        }

        public Dictionary<NavigationDirection, IGamepadControllable> MenuNeighbors { get; private set; } = new Dictionary<NavigationDirection, IGamepadControllable>();
        public bool TryGetMenuNeighbor(NavigationDirection Direction, out IGamepadControllable Neighbor)
        {
            return MenuNeighbors.TryGetValue(Direction, out Neighbor);
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            Neighbor = null;
            return false;
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            return false;
        }

        public bool TryNavigateEnter(NavigationDirection StartingSide, Rectangle? ClosestTo)
        {
            if (!GroupedOptions.IsEmptyMenu && !UngroupedOptions.IsEmptyMenu)
            {
                if (StartingSide == NavigationDirection.Down)
                    return UngroupedOptions.TryNavigateEnter(StartingSide, ClosestTo);
                else if (StartingSide == NavigationDirection.Up)
                    return GroupedOptions.TryNavigateEnter(StartingSide, ClosestTo);
            }

            if (!GroupedOptions.IsEmptyMenu)
                return GroupedOptions.TryNavigateEnter(StartingSide, ClosestTo);
            else if (!UngroupedOptions.IsEmptyMenu)
                return UngroupedOptions.TryNavigateEnter(StartingSide, ClosestTo);
            else
                return false;
        }

        public bool IsNavigatingWithGamepad { get; private set; }

        public void OnGamepadButtonsPressed(Buttons GamepadButtons)
        {
            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.BoundedBagToggleAutofill))
                    TryHandlePrimaryAction(false);

                if (!GroupedOptions.IsEmptyMenu)
                    GroupedOptions.OnGamepadButtonsPressed(GamepadButtons);
                if (!UngroupedOptions.IsEmptyMenu)
                    UngroupedOptions.OnGamepadButtonsPressed(GamepadButtons);
            }
        }

        public void OnGamepadButtonsReleased(Buttons GamepadButtons)
        {
            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                if (!GroupedOptions.IsEmptyMenu)
                    GroupedOptions.OnGamepadButtonsReleased(GamepadButtons);
                if (!UngroupedOptions.IsEmptyMenu)
                    UngroupedOptions.OnGamepadButtonsReleased(GamepadButtons);
            }
        }
        #endregion Gamepad support

        private bool TryHandlePrimaryAction(bool IsMouseInput)
        {
            if (BoundedBag.Autofill && !IBM.IsTransferMultipleModifierHeld && !IBM.IsTransferHalfModifierHeld)
            {
                if (!GroupedOptions.IsEmptyMenu && GroupedOptions.HoveredSlot.HasValue)
                {
                    if (!IsMouseInput || GetAutofillToggleClickableRegion(GroupedOptions.HoveredSlot.Value).Contains(CurrentMousePosition))
                    {
                        BoundedBag.ToggleItemAutofill(GroupedOptions.GetHoveredItem());
                        return true;
                    }
                }

                if (!UngroupedOptions.IsEmptyMenu && UngroupedOptions.HoveredSlot.HasValue)
                {
                    if (!IsMouseInput || GetAutofillToggleClickableRegion(UngroupedOptions.HoveredSlot.Value).Contains(CurrentMousePosition))
                    {
                        BoundedBag.ToggleItemAutofill(UngroupedOptions.GetHoveredItem());
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion Input Handling

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

            if (!GroupedOptions.IsEmptyMenu)
                GroupedOptions.Update(e);
            if (!UngroupedOptions.IsEmptyMenu)
                UngroupedOptions.Update(e);
        }

        public void OnClose()
        {
            GroupedOptions.OnItemSlotRendered -= OnItemSlotRendered;
            GroupedOptions?.OnClose();
            UngroupedOptions.OnItemSlotRendered -= OnItemSlotRendered;
            UngroupedOptions?.OnClose();
        }

        protected Rectangle? HorizontalSeparatorPosition { get; private set; }

        public bool CanResize { get; } = true;

        public void InitializeLayout(int ResizeIteration)
        {
            if (GroupedOptions == null || UngroupedOptions == null)
                return;

            GroupedOptions.InitializeLayout(ResizeIteration);
            UngroupedOptions.InitializeLayout(ResizeIteration);

            int RequiredWidth;
            int RequiredHeight;
            if (UngroupedOptions.IsEmptyMenu)
            {
                RequiredWidth = GroupedOptions.RelativeBounds.Width + Padding * 2;
                RequiredHeight = GroupedOptions.RelativeBounds.Height + Padding * 2;

                Point GroupedOptionsPos = new Point(TopLeftScreenPosition.X + Padding, TopLeftScreenPosition.Y + Padding);
                GroupedOptions.SetTopLeft(GroupedOptionsPos);
                Point UngroupedOptionsPos = new Point(0, 0);
                UngroupedOptions.SetTopLeft(UngroupedOptionsPos);
                HorizontalSeparatorPosition = null;
            }
            else if (GroupedOptions.IsEmptyMenu)
            {
                RequiredWidth = UngroupedOptions.RelativeBounds.Width + Padding * 2;
                RequiredHeight = UngroupedOptions.RelativeBounds.Height + Padding * 2;

                Point GroupedOptionsPos = new Point(0, 0);
                GroupedOptions.SetTopLeft(GroupedOptionsPos);
                Point UngroupedOptionsPos = new Point(TopLeftScreenPosition.X + Padding, TopLeftScreenPosition.Y + Padding);
                UngroupedOptions.SetTopLeft(UngroupedOptionsPos);
                HorizontalSeparatorPosition = null;
            }
            else
            {
                int SeparatorHeight = 12;
                RequiredWidth = Math.Max(GroupedOptions.RelativeBounds.Width, UngroupedOptions.RelativeBounds.Width) + Padding * 2;
                RequiredHeight = Padding + GroupedOptions.RelativeBounds.Height + SeparatorHeight + UngroupedOptions.RelativeBounds.Height + Padding;

                Point GroupedOptionsPos = new Point(TopLeftScreenPosition.X + (RequiredWidth - GroupedOptions.RelativeBounds.Width) / 2, TopLeftScreenPosition.Y + Padding);
                GroupedOptions.SetTopLeft(GroupedOptionsPos);
                Point UngroupedOptionsPos = new Point(TopLeftScreenPosition.X + (RequiredWidth - UngroupedOptions.RelativeBounds.Width) / 2, TopLeftScreenPosition.Y + Padding + GroupedOptions.RelativeBounds.Height + SeparatorHeight);
                UngroupedOptions.SetTopLeft(UngroupedOptionsPos);

                //  Add a horizontal separator
                int SeparatorXPosition = TopLeftScreenPosition.X + Padding;
                int SeparatorYPosition = TopLeftScreenPosition.Y + Padding + GroupedOptions.RelativeBounds.Height;
                int SeparatorWidth = Math.Max(GroupedOptions.RelativeBounds.Width, UngroupedOptions.RelativeBounds.Width);
                HorizontalSeparatorPosition = new Rectangle(SeparatorXPosition, SeparatorYPosition, SeparatorWidth, SeparatorHeight);
            }

            this.RelativeBounds = new Rectangle(0, 0, RequiredWidth, RequiredHeight);
        }

        public void Draw(SpriteBatch b)
        {
            //b.Draw(TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color.Red), ContentsBounds, Color.White);

            if (HorizontalSeparatorPosition.HasValue)
                DrawHelpers.DrawHorizontalSeparator(b, HorizontalSeparatorPosition.Value);
            GroupedOptions.Draw(b);
            UngroupedOptions.Draw(b);
        }

        public void DrawToolTips(SpriteBatch b)
        {
            GroupedOptions.DrawToolTips(b);
            UngroupedOptions.DrawToolTips(b);
        }

        private void OnItemSlotRendered(object sender, ItemSlotRenderedEventArgs e)
        {
            //  Draw a toggle to enable/disable autofilling this item
            if (BoundedBag.Autofill && (e.IsHovered || IBM.IsHoveringAutofillButton))
            {
                Rectangle AutofillDestination;
                float Transparency;
                if (IBM.IsHoveringAutofillButton)
                {
                    double PercentSize = 0.75;
                    int Width = (int)(e.Slot.Width * PercentSize);
                    int Height = (int)(e.Slot.Height * PercentSize);

                    AutofillDestination = new Rectangle(e.Slot.Center.X - Width / 2, e.Slot.Center.Y - Height / 2, Width, Height);
                    Transparency = 1.0f;
                }
                else
                {
                    AutofillDestination = GetAutofillToggleDrawPosition(e.Slot);
                    Transparency = GetAutofillToggleClickableRegion(e.Slot).Contains(CurrentMousePosition) ? 1.0f : 0.75f;
                }

                Rectangle HandIconSourceRect = new Rectangle(32, 0, 10, 10);
                int HandIconSize = (int)(HandIconSourceRect.Width * 2.0 / 32.0 * AutofillDestination.Width);
                //b.Draw(Game1.menuTexture, AutofillDestination, new Rectangle(128, 128, 64, 64), Color.White);
                e.SB.Draw(Game1.mouseCursors, new Rectangle(AutofillDestination.X + (AutofillDestination.Width - HandIconSize) / 2, AutofillDestination.Y + (AutofillDestination.Height - HandIconSize) / 2, HandIconSize, HandIconSize), HandIconSourceRect, Color.White * Transparency);

                if (!BoundedBag.CanAutofillWithItem(e.Item))
                {
                    Rectangle DisabledIconSourceRect = new Rectangle(322, 498, 12, 12);
                    int DisabledIconSize = (int)(DisabledIconSourceRect.Width * 1.5 / 32.0 * AutofillDestination.Width);
                    Rectangle DisabledIconDestination = new Rectangle(AutofillDestination.Right - DisabledIconSize - 2, AutofillDestination.Bottom - DisabledIconSize - 2, DisabledIconSize, DisabledIconSize);
                    e.SB.Draw(Game1.mouseCursors, DisabledIconDestination, DisabledIconSourceRect, Color.White * Transparency);
                }
            }
        }

        private Rectangle GetAutofillToggleDrawPosition(Rectangle ItemSlot)
        {
            return new Rectangle(ItemSlot.X, ItemSlot.Y, ItemSlot.Width / 2, ItemSlot.Height / 2);
        }

        private Rectangle GetAutofillToggleClickableRegion(Rectangle ItemSlot)
        {
            Rectangle DrawPosition = GetAutofillToggleDrawPosition(ItemSlot);
            double PaddingPercent = 0.25;
            int ClickableWidth = (int)(DrawPosition.Width * (1.0 - PaddingPercent * 2));
            int ClickableHeight = (int)(DrawPosition.Height * (1.0 - PaddingPercent * 2));
            return new Rectangle(DrawPosition.Center.X - ClickableWidth / 2, DrawPosition.Center.Y - ClickableHeight / 2, ClickableWidth, ClickableHeight);
        }
    }
}
