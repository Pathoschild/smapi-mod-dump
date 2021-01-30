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
    public class UngroupedLayoutOptions : IBagMenuContent
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

        public ItemBagMenu IBM { get { return Menu?.IBM; } }

        public int Padding { get; } = 8;

        private int OriginalSlotSize { get; }
        /// <summary>The size, in pixels, to use when rendering an item slot. Recommended = <see cref="BagInventoryMenu.DefaultInventoryIconSize"/></summary>
        public int SlotSize { get; private set; }
        /// <summary>The number of columns to display in each row</summary>
        public int ColumnCount { get; }
        /// <summary>If true, then the grid will always be displayed as a perfect square, even if some of the slots in the bottom-right cannot store anything.<para/>
        /// For example, if the bag can only store 8 items and is set to 5 columns, then the grid will display as 2 rows, 5 columns = 10 slots. The bottom-right 2 slots will be rendered as an empty slot.</summary>
        public bool ShowLockedSlots { get; }

        /// <summary>Determines which indices will start a new row afterwards. Default = Empty list.<para/>
        /// For example, if you have 12 columns, and 21 items, and a linebreak at index = 16, then the grid will display as:<para/>
        /// Row 1 = 12 items. Row 2 = 5 items ((16+1)-12=5 leftover, note that the index=16 means up to AND INCLUDING the item at index=16 (so 17 items) gets displayed before a forced linebreak). Then a linebreak of empty space. Row 3 = 4 items</summary>
        public ReadOnlyCollection<int> LineBreaks { get; }
        /// <summary>Only relevant if <see cref="LineBreaks"/> is a non-empty list. Determines the verical spacing, in pixels, to use for each new line.</summary>
        public ReadOnlyCollection<int> LineBreakHeights { get; }

        /// <summary>The bounds of each item slot, relative to <see cref="TopLeftScreenPosition"/>. Use <see cref="SlotBounds"/> when rendering to screen space.</summary>
        public ReadOnlyCollection<Rectangle> RelativeSlotBounds { get; private set; }
        /// <summary>The bounds of each item slot</summary>
        public ReadOnlyCollection<Rectangle> SlotBounds { get; private set; }
        /// <summary>The bounds of each null item slot (as in, one that doesn't hold any items, rather than one that holds items but quantity = 0), relative to <see cref="TopLeftScreenPosition"/>. Use <see cref="LockedSlotBounds"/> when rendering to screen space.</summary>
        public ReadOnlyCollection<Rectangle> RelativeLockedSlotBounds { get; private set; }
        /// <summary>The bounds of each null item slot (as in, one that doesn't hold any items, rather than one that holds items but quantity = 0)</summary>
        public ReadOnlyCollection<Rectangle> LockedSlotBounds { get; private set; }
        public event EventHandler<ItemSlotRenderedEventArgs> OnItemSlotRendered;

        private Point _TopLeftScreenPosition;
        public Point TopLeftScreenPosition {
            get { return _TopLeftScreenPosition; }
            private set { SetTopLeft(value, true); }
        }

        public void SetTopLeft(Point Point) { SetTopLeft(Point, true); }
        internal void SetTopLeft(Point NewValue, bool CheckIfChanged)
        {
            if (!CheckIfChanged || TopLeftScreenPosition != NewValue)
            {
                _TopLeftScreenPosition = NewValue;

                if (RelativeSlotBounds != null)
                {
                    this.SlotBounds = new ReadOnlyCollection<Rectangle>(RelativeSlotBounds.Select(x => x.GetOffseted(TopLeftScreenPosition)).ToList());
                    this.LockedSlotBounds = new ReadOnlyCollection<Rectangle>(RelativeLockedSlotBounds.Select(x => x.GetOffseted(TopLeftScreenPosition)).ToList());
                }
                else
                {
                    this.SlotBounds = null;
                    this.LockedSlotBounds = null;
                }

                this.Bounds = RelativeBounds.GetOffseted(TopLeftScreenPosition);
            }
        }

        /// <summary>The bounds of this menu's content, relative to <see cref="TopLeftScreenPosition"/></summary>
        public Rectangle RelativeBounds { get; private set; }
        public Rectangle Bounds { get; private set; }

        public ReadOnlyCollection<Object> PlaceholderItems { get; private set; }
        public bool IsEmptyMenu { get { return PlaceholderItems == null || !PlaceholderItems.Any(); } }

        public UngroupedLayoutOptions(int ColumnCount, bool ShowLockedSlots, IList<int> LineBreaks, IList<int> LineBreakHeights, int SlotSize = BagInventoryMenu.DefaultInventoryIconSize)
        {
            this.ColumnCount = ColumnCount;
            this.ShowLockedSlots = ShowLockedSlots;
            this.LineBreaks = new ReadOnlyCollection<int>(LineBreaks);
            this.LineBreakHeights = new ReadOnlyCollection<int>(LineBreakHeights);
            this.SlotSize = SlotSize;
            this.OriginalSlotSize = SlotSize;
        }

        public UngroupedLayoutOptions(BagMenuOptions.UngroupedLayout ULO)
            : this(ULO.Columns, true, ULO.LineBreakIndices, ULO.LineBreakHeights, ULO.SlotSize) { }

        #region Input Handling
        public Rectangle? HoveredSlot { get; private set; } = null;

        private DateTime? SecondaryActionButtonPressedTime = null;
        private bool IsSecondaryActionButtonHeld { get { return SecondaryActionButtonPressedTime.HasValue; } }
        private Rectangle? SecondaryActionButtonPressedLocation = null;

        #region Mouse Handling
        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            Rectangle? PreviouslyHovered = HoveredSlot;

            this.HoveredSlot = null;
            if (SlotBounds != null)
            {
                foreach (Rectangle Rect in SlotBounds)
                {
                    if (Rect.Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
                    {
                        if (PreviouslyHovered.HasValue && Rect != PreviouslyHovered.Value)
                            SecondaryActionButtonPressedLocation = null;
                        this.HoveredSlot = Rect;
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
                HandlePrimaryAction(GetHoveredItem());
            }

            if (e.Button == SButton.MouseRight)
            {
                HandleSecondaryAction(GetHoveredItem());
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
            HoveredSlot = SlotBounds.First();
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
            if (MenuNeighbors.TryGetValue(Direction, out Neighbor))
                return true;
            else if (Menu != null)
                return Menu.MenuNeighbors.TryGetValue(Direction, out Neighbor);
            else
                return false;
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            List<Rectangle> AllSlots = SlotBounds.Union(LockedSlotBounds).OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
            return GamepadControls.TryGetSlotNeighbor(AllSlots, ItemSlot, ColumnCount, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            if (IsGamepadFocused && HoveredSlot == null)
            {
                HoveredSlot = SlotBounds.First();
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
                List<Rectangle> AllSlots = SlotBounds.Union(LockedSlotBounds).OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
                HoveredSlot = AllSlots.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
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
            if (TargetItem is Object PressedObject)
            {
                int Qty = ItemBag.GetQuantityToTransfer(ItemBag.InputTransferAction.PrimaryActionButtonPressed, PressedObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                Bag.MoveFromBag(PressedObject, Qty, out int MovedQty, true, Menu.IBM.InventorySource, Menu.IBM.ActualInventoryCapacity);
            }
        }

        private void HandleSecondaryAction(Item TargetItem)
        {
            if (TargetItem is Object PressedObject)
            {
                ItemBag.InputTransferAction TransferAction = IsSecondaryActionButtonHeld ? ItemBag.InputTransferAction.SecondaryActionButtonHeld : ItemBag.InputTransferAction.SecondaryActionButtonPressed;
                int Qty = ItemBag.GetQuantityToTransfer(TransferAction, PressedObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                Bag.MoveFromBag(PressedObject, Qty, out int MovedQty, true, Menu.IBM.InventorySource, Menu.IBM.ActualInventoryCapacity);
            }
        }
        #endregion Input Handling

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

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

        #region Parent Menu
        private BoundedBagMenu _Menu;
        public BoundedBagMenu Menu
        {
            get { return _Menu; }
            private set
            {
                if (Menu != value)
                {
                    _Menu = value;
                    BoundedBag = Menu?.BoundedBag;
                }
            }
        }

        private BoundedBag _BoundedBag;
        public BoundedBag BoundedBag
        {
            get { return _BoundedBag; }
            private set
            {
                if (BoundedBag != value)
                {
                    if (BoundedBag != null)
                        BoundedBag.OnContentsChanged -= Bag_ContentsChanged;
                    _BoundedBag = value;
                    if (BoundedBag != null)
                        BoundedBag.OnContentsChanged += Bag_ContentsChanged;
                }
            }
        }

        private void Bag_ContentsChanged(object sender, EventArgs e) { UpdateQuantities(); }
        public ItemBag Bag { get { return BoundedBag; } }

        public void SetParent(BoundedBagMenu Menu)
        {
            this.Menu = Menu;
            if (Menu == null)
            {
                this.PlaceholderItems = new ReadOnlyCollection<Object>(new List<Object>());
            }
            else
            {
                List<AllowedObject> UngroupedObjects = BoundedBag.AllowedObjects.Where(x => !Menu.GroupByQuality || !x.HasQualities).ToList();

                List<Object> Placeholders = new List<Object>();
                foreach (AllowedObject Item in UngroupedObjects)
                {
                    if (!Item.HasQualities || Item.IsBigCraftable)
                    {
                        Object Obj = Item.IsBigCraftable ?
                            new Object(Vector2.Zero, Item.Id, false) :
                            new Object(Item.Id, 0, false, -1, 0);
                        Placeholders.Add(Obj);
                    }
                    else
                        Placeholders.AddRange(Item.Qualities.Select(x => new Object(Item.Id, 0, false, -1, (int)x)));
                }
                this.PlaceholderItems = new ReadOnlyCollection<Object>(Placeholders);

                UpdateQuantities();

                SetTopLeft(Point.Zero, false);
            }
        }

        private void UpdateQuantities()
        {
            //  Initialize all quantities back to zero
            foreach (Object Placeholder in PlaceholderItems)
                Placeholder.Stack = 0;

            //  Set quantities of the placeholder items to match the corresponding amount of the item currently stored in the bag
            foreach (Object Item in Bag.Contents)
            {
                Object Placeholder = PlaceholderItems.FirstOrDefault(x => ItemBag.AreItemsEquivalent(x, Item, false));
                if (Placeholder != null)
                    ItemBag.ForceSetQuantity(Placeholder, Item.Stack);
            }
        }
        #endregion Parent Menu

        public bool CanResize { get; } = true;

        public void InitializeLayout(int ResizeIteration)
        {
            if (ResizeIteration > 1)
                this.SlotSize = Math.Min(OriginalSlotSize, Math.Max(24, OriginalSlotSize - (ResizeIteration - 1) * 8));

            HoveredSlot = null;
            SecondaryActionButtonPressedTime = null;
            SecondaryActionButtonPressedLocation = null;

            List<Rectangle> SlotBounds = new List<Rectangle>();
            List<Rectangle> LockedSlotBounds = new List<Rectangle>();

            int CurrentIndex = 0;
            int CurrentRow = 0;
            int CurrentColumn = 0;
            int CurrentLine = 0;
            int TotalLineBreakHeight = 0;

            List<AllowedObject> UngroupedObjects = BoundedBag.AllowedObjects.Where(x => !Menu.GroupByQuality || !x.HasQualities).ToList();
            foreach (AllowedObject Obj in UngroupedObjects)
            {
                foreach (int Quality in Enumerable.Range(0, Obj.HasQualities ? Obj.Qualities.Count : 1))
                {
                    if (CurrentColumn == ColumnCount)
                    {
                        CurrentRow++;
                        CurrentColumn = 0;
                    }

                    int X = CurrentColumn * SlotSize;
                    int Y = CurrentRow * SlotSize + TotalLineBreakHeight;
                    SlotBounds.Add(new Rectangle(X + Padding, Y + Padding, SlotSize, SlotSize));

                    if (LineBreaks.Contains(CurrentIndex))
                    {
                        if (CurrentLine < LineBreakHeights.Count)
                            TotalLineBreakHeight += LineBreakHeights[CurrentLine % LineBreakHeights.Count];
                        CurrentLine++;
                        CurrentRow++;

                        if (ShowLockedSlots)
                        {
                            //  Fill in the rest of this row with locked slots
                            int LockedSlotColumn = CurrentColumn + 1;
                            while (LockedSlotColumn < ColumnCount)
                            {
                                LockedSlotBounds.Add(new Rectangle(LockedSlotColumn * SlotSize + Padding, Y + Padding, SlotSize, SlotSize));
                                LockedSlotColumn++;
                            }
                        }

                        CurrentColumn = 0;
                    }
                    else
                    {
                        CurrentColumn++;
                    }

                    CurrentIndex++;
                }
            }

            if (ShowLockedSlots)
            {
                //  Fill in the rest of this row with locked slots
                int Y = CurrentRow * SlotSize + TotalLineBreakHeight;
                int LockedSlotColumn = CurrentColumn;
                while (LockedSlotColumn < ColumnCount)
                {
                    LockedSlotBounds.Add(new Rectangle(LockedSlotColumn * SlotSize + Padding, Y + Padding, SlotSize, SlotSize));
                    LockedSlotColumn++;
                }
            }

            RelativeSlotBounds = new ReadOnlyCollection<Rectangle>(SlotBounds);
            RelativeLockedSlotBounds = new ReadOnlyCollection<Rectangle>(LockedSlotBounds);

            this.RelativeBounds = new Rectangle(0, 0, ColumnCount * SlotSize + Padding * 2, (CurrentRow + 1) * SlotSize + TotalLineBreakHeight + Padding * 2);
        }

        public void Draw(SpriteBatch b)
        {
            if (IsEmptyMenu)
                return;

            //b.Draw(TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color.Cyan), Bounds, Color.White);

            //  Draw the backgrounds of each slot
            foreach (Rectangle LockedSlot in LockedSlotBounds)
            {
                b.Draw(Game1.menuTexture, LockedSlot, new Rectangle(64, 896, 64, 64), Color.White);
            }
            foreach (Rectangle UnlockedSlot in SlotBounds)
            {
                b.Draw(Game1.menuTexture, UnlockedSlot, new Rectangle(128, 128, 64, 64), Color.White);
            }

            //  Draw the items of each slot
            for (int i = 0; i < SlotBounds.Count; i++)
            {
                Rectangle Destination = SlotBounds[i];

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

                Object CurrentItem = PlaceholderItems[i];

                float IconScale = IsHovered ? 1.25f : 1.0f;
                Color Overlay = CurrentItem.Stack == 0 ? Color.White * 0.30f : Color.White;
                DrawHelpers.DrawItem(b, Destination, CurrentItem, CurrentItem.Stack > 0, true, IconScale, 1.0f, Overlay, CurrentItem.Stack >= Bag.MaxStackSize ? Color.Red : Color.White);

                OnItemSlotRendered?.Invoke(this, new ItemSlotRenderedEventArgs(b, Destination, CurrentItem, IsHovered));
            }

            foreach (Rectangle LockedSlot in LockedSlotBounds)
            {
                if (HoveredSlot == LockedSlot)
                {
                    Color HighlightColor = Color.Yellow;
                    Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                    b.Draw(Highlight, LockedSlot, Color.White * 0.25f);

                    int BorderThickness = LockedSlot.Width / 16;
                    DrawHelpers.DrawBorder(b, LockedSlot, BorderThickness, HighlightColor);
                }
            }
        }

        public void DrawToolTips(SpriteBatch b)
        {
            if (IsEmptyMenu)
                return;

            if (HoveredSlot.HasValue)
            {
                Object HoveredItem = GetHoveredItem();
                if (HoveredItem != null)
                {
                    Rectangle Location;
                    if (IsNavigatingWithGamepad)
                        Location = HoveredSlot.Value; //new Rectangle(HoveredSlot.Value.Right, HoveredSlot.Value.Bottom, 1, 1);
                    else
                        Location = new Rectangle(Game1.getMouseX() - 8, Game1.getMouseY() + 36, 8 + 36, 1);
                    DrawHelpers.DrawToolTipInfo(b, Location, HoveredItem, true, true, true, true, true, true, Bag.MaxStackSize);
                }
            }
        }

        internal Object GetHoveredItem()
        {
            if (HoveredSlot.HasValue)
            {
                int Index = SlotBounds.IndexOf(HoveredSlot.Value);
                if (Index >= 0 && Index < PlaceholderItems.Count)
                    return PlaceholderItems[Index];
                else
                    return null;
            }
            else
                return null;
        }

        public void OnClose()
        {
            SetParent(null);
        }
    }
}
