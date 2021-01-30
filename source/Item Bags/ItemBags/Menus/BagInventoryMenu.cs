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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Object = StardewValley.Object;

namespace ItemBags.Menus
{
    public class BagInventoryMenu : IBagMenuContent, IGamepadControllable
    {
        #region Lookup Anything Compatibility
        /// <summary>
        /// Warning - do not remove/rename this field. It is used via reflection by Lookup Anything mod.<para/>
        /// See also: <see cref="https://github.com/Pathoschild/StardewMods/tree/develop/LookupAnything#extensibility-for-modders"/>
        /// </summary>
        public Item HoveredItem { get; private set; }
        public void UpdateHoveredItem(CursorMovedEventArgs e)
        {
            if (Bounds.Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
            {
                HoveredItem = GetHoveredItem();
            }
            else
            {
                HoveredItem = null;
            }
        }
        #endregion Lookup Anything Compatibility

        public ItemBagMenu IBM { get; }
        public ItemBag Bag { get; }

        public int Padding { get; }

        public IList<Item> Source { get; }
        public int ActualCapacity { get; }

        public const int DefaultInventoryIconSize = 64;
        /// <summary>The number of columns to use when rendering the user's inventory at the bottom-half of the menu. Recommended = 12 to mimic the default inventory of the main GameMenu</summary>
        public int InventoryColumns { get; }
        /// <summary>The size, in pixels, to use when rendering each slot of the user's inventory at the bottom-half of the menu. Recommended = <see cref="DefaultInventoryIconSize"/></summary>
        public int InventorySlotSize { get; }

        /// <summary>The bounds of each inventory slot, relative to <see cref="TopLeftScreenPosition"/>. Use <see cref="InventorySlotBounds"/> when rendering to screen space.</summary>
        public ReadOnlyCollection<Rectangle> RelativeInventorySlotBounds { get; private set; }
        /// <summary>The bounds of each inventory slot</summary>
        public ReadOnlyCollection<Rectangle> InventorySlotBounds { get; private set; }

        private int TotalInventorySlots { get; set; }
        private int UnlockedInventorySlots { get; set; }
        private int LockedInventorySlots { get { return TotalInventorySlots - UnlockedInventorySlots; } }
        private bool IsLockedInventorySlot(int Index) { return Index >= UnlockedInventorySlots; }

        private Point _TopLeftScreenPosition;
        public Point TopLeftScreenPosition {
            get { return _TopLeftScreenPosition; }
            private set { SetTopLeft(value, true); }
        }

        public void SetTopLeft(Point NewValue) { SetTopLeft(NewValue, true); }
        private void SetTopLeft(Point NewValue, bool CheckIfChanged = true)
        {
            if (!CheckIfChanged || TopLeftScreenPosition != NewValue)
            {
                _TopLeftScreenPosition = NewValue;

                if (RelativeInventorySlotBounds != null)
                {
                    List<Rectangle> TranslatedSlots = new List<Rectangle>();
                    foreach (Rectangle Relative in RelativeInventorySlotBounds)
                    {
                        Rectangle Translated = Relative.GetOffseted(TopLeftScreenPosition);
                        TranslatedSlots.Add(Translated);
                    }
                    this.InventorySlotBounds = new ReadOnlyCollection<Rectangle>(TranslatedSlots);
                }
                else
                {
                    this.InventorySlotBounds = null;
                }

                this.Bounds = RelativeBounds.GetOffseted(TopLeftScreenPosition);
            }
        }

        /// <summary>The bounds of this menu's content, relative to <see cref="TopLeftScreenPosition"/></summary>
        public Rectangle RelativeBounds { get; private set; }
        public Rectangle Bounds { get; private set; }

        /// <param name="Source">Typically this is <see cref="Game1.player.Items"/> if this menu should display the player's inventory.</param>
        /// <param name="ActualCapacity">If non-null, allows you to override the maximum # of items that can be stored in the Source list</param>
        /// <param name="InventoryColumns">The number of columns to use when rendering the user's inventory at the bottom-half of the menu. Recommended = 12 to mimic the default inventory of the main GameMenu</param>
        /// <param name="InventorySlotSize">The size, in pixels, to use when rendering each slot of the user's inventory at the bottom-half of the menu. Recommended = <see cref="DefaultInventoryIconSize"/></param>
        public BagInventoryMenu(ItemBagMenu IBM, ItemBag Bag, IList<Item> Source, int? ActualCapacity, int InventoryColumns, int InventorySlotSize = DefaultInventoryIconSize)
        {
            this.IBM = IBM;
            this.Bag = Bag;
            this.Source = Source;
            this.ActualCapacity = ActualCapacity.HasValue ? Math.Max(ActualCapacity.Value, Source.Count) : Source.Count;
            this.InventoryColumns = InventoryColumns;
            this.InventorySlotSize = InventorySlotSize;
            SetTopLeft(Point.Zero, false);
            InitializeLayout(1);
        }

        #region Input Handling
        private Rectangle? HoveredSlot = null;

        private DateTime? SecondaryActionButtonPressedTime = null;
        private bool IsSecondaryActionButtonHeld { get { return SecondaryActionButtonPressedTime.HasValue; } }
        private Rectangle? SecondaryActionButtonPressedLocation = null;

        #region Mouse Handling
        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            Rectangle? PreviouslyHovered = HoveredSlot;

            this.HoveredSlot = null;
            if (InventorySlotBounds != null)
            {
                foreach (Rectangle Rect in InventorySlotBounds)
                {
                    if (Rect.Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
                    {
                        if (PreviouslyHovered.HasValue && Rect != PreviouslyHovered.Value)
                            SecondaryActionButtonPressedLocation = null;
                        this.HoveredSlot = Rect;
                        IsNavigatingWithGamepad = false;
                        break;
                    }
                }
            }

            UpdateHoveredItem(e);
        }

        public void OnMouseButtonPressed(ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft)
            {
                Item PressedItem = GetHoveredItem();
                HandlePrimaryAction(PressedItem);
            }
            else if (e.Button == SButton.MouseRight)
            {
                Item PressedItem = GetHoveredItem();
                HandleSecondaryAction(PressedItem);

                SecondaryActionButtonPressedLocation = HoveredSlot;
                SecondaryActionButtonPressedTime = DateTime.Now;
            }
        }

        public void OnMouseButtonReleased(ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.MouseRight)
            {
                SecondaryActionButtonPressedTime = null;
                SecondaryActionButtonPressedLocation = null;
            }
        }
        #endregion Mouse Handling

        #region Gamepad support
        public bool RecentlyGainedFocus { get; private set; }

        private bool _IsGamepadFocused;
        public bool IsGamepadFocused
        {
            get { return _IsGamepadFocused; }
            set
            {
                if (IsGamepadFocused != value)
                {
                    _IsGamepadFocused = value;
                    if (IsGamepadFocused)
                        GainedGamepadFocus();
                    else
                        LostGamepadFocus();
                }
            }
        }

        public void GainedGamepadFocus()
        {
            RecentlyGainedFocus = true;
            HoveredSlot = InventorySlotBounds.First();
            IsNavigatingWithGamepad = true;
        }

        public void LostGamepadFocus()
        {
            HoveredSlot = null;
            SecondaryActionButtonPressedLocation = null;
            SecondaryActionButtonPressedTime = null;
        }

        public Dictionary<NavigationDirection, IGamepadControllable> MenuNeighbors { get; private set; } = new Dictionary<NavigationDirection, IGamepadControllable>();
        public bool TryGetMenuNeighbor(NavigationDirection Direction, out IGamepadControllable Neighbor)
        {
            return MenuNeighbors.TryGetValue(Direction, out Neighbor);
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            return GamepadControls.TryGetSlotNeighbor(InventorySlotBounds, ItemSlot, InventoryColumns, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            if (IsGamepadFocused && HoveredSlot == null)
            {
                HoveredSlot = InventorySlotBounds.FirstOrDefault();
                IsNavigatingWithGamepad = HoveredSlot != null;
                return HoveredSlot != null;
            }
            else if (TryGetSlotNeighbor(HoveredSlot, Direction, HorizontalWrapping, VerticalWrapping, out Rectangle? Neighbor))
            {
                HoveredSlot = Neighbor.Value;
                IsNavigatingWithGamepad = true;
                return true;
            }
            else
                return false;
        }

        public bool TryNavigateEnter(NavigationDirection StartingSide, Rectangle? ClosestTo)
        {
            IsGamepadFocused = true;
            IsNavigatingWithGamepad = true;

            if (ClosestTo.HasValue)
            {
                HoveredSlot = InventorySlotBounds.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
            }
            else
            {
                if (StartingSide == NavigationDirection.Right)
                {
                    while (TryNavigate(NavigationDirection.Right, NavigationWrappingMode.NoWrap, NavigationWrappingMode.NoWrap)) { }
                }
                if (StartingSide == NavigationDirection.Down)
                {
                    while (TryNavigate(NavigationDirection.Down, NavigationWrappingMode.NoWrap, NavigationWrappingMode.NoWrap)) { }
                }
            }

            return true;
        }

        public bool IsNavigatingWithGamepad { get; private set; }

        public void OnGamepadButtonsPressed(Buttons GamepadButtons)
        {
            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                if (!GamepadControls.HandleNavigationButtons(this, GamepadButtons, HoveredSlot))
                    this.IsGamepadFocused = false;

                //  Handle action buttons
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.PrimaryAction))
                {
                    HandlePrimaryAction(GetHoveredItem());
                }
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.SecondaryAction))
                {
                    HandleSecondaryAction(GetHoveredItem());
                    SecondaryActionButtonPressedLocation = HoveredSlot;
                    SecondaryActionButtonPressedTime = DateTime.Now;
                }
            }
        }

        public void OnGamepadButtonsReleased(Buttons GamepadButtons)
        {
            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                //  Handle action buttons
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.SecondaryAction))
                {
                    SecondaryActionButtonPressedLocation = null;
                    SecondaryActionButtonPressedTime = null;
                }
            }
        }
        #endregion Gamepad support

        private void HandlePrimaryAction(Item TargetItem)
        {
            if (TargetItem != null)
            {
                //  Click another bag to move it into an opened OmniBag
                if (this.Bag is OmniBag OB && TargetItem is ItemBag IB && IB != this.Bag)
                {
                    OB.MoveToBag(IB, true, Source, true);
                }
                //  I'm not aware of a way to right-click items on Android, so simulate a right-click if tapping a bag,
                //  so that the clicked bag will be closed (if currently-opened), or opened (if not currently-opened)
                else if (Constants.TargetPlatform == GamePlatform.Android && TargetItem is ItemBag TargetBag)
                {
                    HandleSecondaryAction(TargetItem);
                }
                //  Transfer the object into the bag
                else if (TargetItem is Object PressedObject && Bag.IsValidBagObject(PressedObject))
                {
                    int Qty = ItemBag.GetQuantityToTransfer(ItemBag.InputTransferAction.PrimaryActionButtonPressed, PressedObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                    Bag.MoveToBag(PressedObject, Qty, out int MovedQty, true, Source);
                }
            }
        }

        private void HandleSecondaryAction(Item TargetItem)
        {
            if (TargetItem != null)
            {
                if (TargetItem is ItemBag IB)
                {
                    //  Click current bag to close it
                    if (IB == this.Bag)
                    {
                        //IB.CloseContents();
                        //  Rather than immediately closing the menu, queue it up to be closed on the next game update.
                        //  When a bag menu is closed, the previous menu is restored. If we don't queue up the bag close,
                        //  then the game will process this current right-click event on the previous menu after it's restored.
                        //  By handling the right-click on 2 different menus, it can cause an unintended action on the restored menu.
                        //  For example, if the restored menu is a chest interface, the mouse cursor could coincidentally be hovering an item
                        //  in the chest so this current right-click action would close the bag, then transfer the hovered chest item, rather than just closing the bag.
                        QueueCloseBag = true;
                    }
                    else
                    {
                        if (this.Bag is OmniBag OB)
                        {
                            IClickableMenu PreviousMenu = this.Bag.PreviousMenu;
                            this.Bag.CloseContents(false, false);
                            IB.OpenContents(Source, ActualCapacity, PreviousMenu);
                        }
                        else
                        {
                            IClickableMenu PreviousMenu = this.Bag.PreviousMenu;
                            if (PreviousMenu is ItemBagMenu IBM && IBM.Content is OmniBagMenu OBM)
                                PreviousMenu = OBM.OmniBag.PreviousMenu;
                            this.Bag.CloseContents(false, false);
                            IB.OpenContents(Source, ActualCapacity, PreviousMenu);
                        }
                    }
                }
                //  Transfer the object into the bag
                else if (TargetItem is Object PressedObject && Bag.IsValidBagObject(PressedObject))
                {
                    ItemBag.InputTransferAction TransferAction = IsSecondaryActionButtonHeld ? ItemBag.InputTransferAction.SecondaryActionButtonHeld : ItemBag.InputTransferAction.SecondaryActionButtonPressed;
                    int Qty = ItemBag.GetQuantityToTransfer(TransferAction, PressedObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                    Bag.MoveToBag(PressedObject, Qty, out int MovedQty, true, Source);
                }
            }
        }
        #endregion Input Handling

        private bool QueueCloseBag = false;

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

            if (QueueCloseBag)
            {
                QueueCloseBag = false;
                this.Bag.CloseContents();
                return;
            }

            if (e.IsMultipleOf(ItemBagMenu.TransferRepeatFrequency) && HoveredSlot.HasValue)
            {
                if (IsSecondaryActionButtonHeld && SecondaryActionButtonPressedLocation.HasValue && HoveredSlot.Value == SecondaryActionButtonPressedLocation.Value
                    && SecondaryActionButtonPressedTime.HasValue && DateTime.Now.Subtract(SecondaryActionButtonPressedTime.Value).TotalMilliseconds >= 500)
                {
                    HandleSecondaryAction(GetHoveredItem());
                }
            }

            if (e.IsMultipleOf(GamepadControls.Current.NavigationRepeatFrequency) && IsGamepadFocused && IsNavigatingWithGamepad)
            {
                if (!GamepadControls.HandleNavigationButtons(this, null, HoveredSlot))
                    this.IsGamepadFocused = false;
            }
        }

        public bool CanResize { get; } = false;

        public void InitializeLayout(int ResizeIteration)
        {
            HoveredSlot = null;
            SecondaryActionButtonPressedTime = null;
            SecondaryActionButtonPressedLocation = null;

            //  Compute size of inventory
            int InventoryMargin = 16; // Empty space around the inventory slots

            this.TotalInventorySlots = ActualCapacity;
            this.UnlockedInventorySlots = ActualCapacity;
            int InventoryRows = (TotalInventorySlots - 1) / InventoryColumns + 1;

            int LockedSlotsSeparatorHeight = 16;
            bool HasLockedSlots = LockedInventorySlots > 0;
            bool ShowLockedSlotsSeparator = HasLockedSlots && LockedInventorySlots % InventoryColumns == 0;

            int RequiredInventoryWidth = InventoryColumns * InventorySlotSize + InventoryMargin * 2;
            int RequiredInventoryHeight = InventoryRows * InventorySlotSize + InventoryMargin * 2;
            if (ShowLockedSlotsSeparator)
                RequiredInventoryHeight += LockedSlotsSeparatorHeight;

            this.RelativeBounds = new Rectangle(0, 0, RequiredInventoryWidth, RequiredInventoryHeight);

            //  Set bounds of inventory
            List<Rectangle> InvSlotBounds = new List<Rectangle>();
            for (int i = 0; i < TotalInventorySlots; i++)
            {
                int Row = i / InventoryColumns;
                int Column = i - Row * InventoryColumns;

                int X = InventoryMargin + Column * InventorySlotSize;
                int Y = InventoryMargin + Row * InventorySlotSize;

                bool IsBelowLockedSlotsSeparator = ShowLockedSlotsSeparator && i >= UnlockedInventorySlots;
                if (IsBelowLockedSlotsSeparator)
                    Y += LockedSlotsSeparatorHeight;

                InvSlotBounds.Add(new Rectangle(X, Y, InventorySlotSize, InventorySlotSize));
            }
            RelativeInventorySlotBounds = new ReadOnlyCollection<Rectangle>(InvSlotBounds);
        }

        public void Draw(SpriteBatch b)
        {
            //  Draw the background textures of each inventory slot
            for (int i = 0; i < InventorySlotBounds.Count; i++)
            {
                Rectangle Destination = InventorySlotBounds[i];
                if (IsLockedInventorySlot(i))
                {
                    b.Draw(Game1.menuTexture, Destination, new Rectangle(64, 896, 64, 64), Color.White);
                }
                else
                {
                    b.Draw(Game1.menuTexture, Destination, new Rectangle(128, 128, 64, 64), Color.White);
                }
            }

            for (int i = 0; i < InventorySlotBounds.Count; i++)
            {
                Rectangle Destination = InventorySlotBounds[i];

                Item CurrentItem = null;
                if (!IsLockedInventorySlot(i) && i < Source.Count)
                    CurrentItem = Source[i];
                bool IsValidBagItem = (Bag is OmniBag OB && CurrentItem is ItemBag IB && OB.IsValidBag(IB)) || Bag.IsValidBagItem(CurrentItem);

                //  Draw a transparent black or white overlay if the item is valid for the bag or not
                Color Overlay = IsValidBagItem || CurrentItem == this.Bag ? Color.White : Color.Black;
                float OverlayTransparency = IsValidBagItem || CurrentItem == this.Bag ? 0.15f : 0.35f;
                b.Draw(TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Overlay), Destination, Color.White * OverlayTransparency);

                if (CurrentItem != null)
                {
                    //  Draw a red outline on the bag that's currently open
                    if (CurrentItem == this.Bag)
                    {
                        b.Draw(Game1.menuTexture, Destination, new Rectangle(0, 896, 64, 64), Color.White);
                    }

                    //  Draw a thin yellow border if mouse is hovering this slot
                    bool IsHovered = Destination == HoveredSlot;
                    if (IsHovered)
                    {
                        Color HighlightColor = Color.Yellow;
                        Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                        b.Draw(Highlight, Destination, Color.White * 0.25f);

                        int BorderThickness = Destination.Width / 16;
                        DrawHelpers.DrawBorder(b, Destination, BorderThickness, HighlightColor);
                    }

                    //  Draw the item
                    float Scale = IsHovered && IsValidBagItem ? 1.25f : 1.0f;
                    float Transparency = IsValidBagItem || CurrentItem == this.Bag || CurrentItem is ItemBag ? 1.0f : 0.35f;
                    if (InventorySlotSize == DefaultInventoryIconSize)
                    {
                        CurrentItem.drawInMenu(b, new Vector2(Destination.X, Destination.Y), Scale, Transparency, 1f, StackDrawType.Draw_OneInclusive, Color.White, true);
                    }
                    else
                    {
                        DrawHelpers.DrawItem(b, Destination, CurrentItem, true, true, Scale, Transparency, Color.White, Color.White);
                    }
                }
                else if (IsNavigatingWithGamepad && Destination == HoveredSlot)
                {
                    Color HighlightColor = Color.Yellow;
                    Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                    b.Draw(Highlight, Destination, Color.White * 0.25f);

                    int BorderThickness = Destination.Width / 16;
                    DrawHelpers.DrawBorder(b, Destination, BorderThickness, HighlightColor);
                }
            }
        }

        public void DrawToolTips(SpriteBatch b)
        {
            if (HoveredSlot.HasValue)
            {
                Item HoveredItem = GetHoveredItem();
                if (HoveredItem != null && Bag.IsValidBagItem(HoveredItem))
                {
                    Rectangle Location;
                    if (IsNavigatingWithGamepad)
                        Location = HoveredSlot.Value; //new Rectangle(HoveredSlot.Value.Right, HoveredSlot.Value.Bottom, 1, 1);
                    else
                        Location = new Rectangle(Game1.getMouseX() - 8, Game1.getMouseY() + 36, 8 + 36, 1);
                    DrawHelpers.DrawToolTipInfo(b, Location, HoveredItem, true, true, true, true, true, true, null);
                }
            }
        }

        internal Item GetHoveredItem()
        {
            if (HoveredSlot.HasValue)
            {
                int Index = InventorySlotBounds.IndexOf(HoveredSlot.Value);
                if (!IsLockedInventorySlot(Index) && Index < Source.Count)
                {
                    Item HoveredItem = Source[Index];
                    return HoveredItem;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public void OnClose()
        {

        }
    }
}
