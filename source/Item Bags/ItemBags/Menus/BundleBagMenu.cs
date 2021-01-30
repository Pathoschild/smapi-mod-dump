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
using ItemBags.Community_Center;
using ItemBags.Helpers;
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
    public class BundleBagMenu : IBagMenuContent
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
        public ItemBag Bag { get { return BundleBag; } }
        public BundleBag BundleBag { get; }
        public bool IsJojaMember { get; }
        private void Bag_ContentsChanged(object sender, EventArgs e) { UpdateQuantities(); }

        public int Padding { get; }

        private int OriginalSlotSize { get; }
        /// <summary>The size, in pixels, to use when rendering an item slot. Recommended = <see cref="BagInventoryMenu.DefaultInventoryIconSize"/></summary>
        public int SlotSize { get; private set; }
        /// <summary>The number of columns to display in each row</summary>
        public int ColumnCount { get; }
        /// <summary>If true, then the grid will always be displayed as a perfect square, even if some of the slots in the bottom-right cannot store anything.<para/>
        /// For example, if the bag can only store 8 items and is set to 5 columns, then the grid will display as 2 rows, 5 columns = 10 slots. The bottom-right 2 slots will be rendered as an empty slot.</summary>
        public bool ShowLockedSlots { get; }

        /// <summary>The bounds of this menu's content, relative to <see cref="TopLeftScreenPosition"/></summary>
        public Rectangle RelativeBounds { get; private set; }
        public Rectangle Bounds { get { return RelativeBounds.GetOffseted(TopLeftScreenPosition); } }

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
            }
        }

        public BundleBagMenu(ItemBagMenu IBM, BundleBag Bag, int Columns, int SlotSize, bool ShowLockedSlots, int Padding)
        {
            this.IBM = IBM;
            this.BundleBag = Bag;
            this.IsJojaMember = CommunityCenterBundles.Instance.IsJojaMember;
            Bag.OnContentsChanged += Bag_ContentsChanged;

            this.Padding = Padding;

            this.ColumnCount = Columns;
            this.OriginalSlotSize = SlotSize;
            this.SlotSize = SlotSize;
            this.ShowLockedSlots = ShowLockedSlots;

            this.ItemPlaceholders = new Dictionary<BundleItem, Object>();
            CommunityCenterBundles.Instance.IterateAllBundleItems(x =>
            {
                if (!BundleBag.InvalidRooms[BundleBag.Size].Contains(x.Task.Room.Name))
                {
                    if (!x.IsCompleted)
                    {
                        ItemPlaceholders.Add(x, x.ToObject());
                    }
                }
            });

            UpdateQuantities();
            SetTopLeft(Point.Zero, false);
            InitializeLayout(1);
        }

        public void UpdateQuantities()
        {
            foreach (Object Item in ItemPlaceholders.Values)
                Item.Stack = 0;

            foreach (Object BagItem in Bag.Contents)
            {
                //  Find all placeholders requiring this item Id/Quality
                List<KeyValuePair<BundleItem, Object>> Placeholders = ItemPlaceholders.Where(x => ItemBag.AreItemsEquivalent(x.Value, BagItem, false))
                    .OrderByDescending(x => x.Key.IsRequired).ToList();

                //  Distribute the Stack of this item to the placeholders, up to each placeholder's required quantity.
                //  EX: Suppose you have 10 Parsnips in the bag, and Task#1 requires 3 Parsnip, Task#2 Requires 12 Parsnip.
                //  Put 3/10 in Task#1, then 7/10 in Task#2. Thus Task#1 is now fulfilled, and Task#2 is at 7/12.
                int RemainingQuantity = BagItem.Stack;
                int CurrentIndex = 0;
                while (RemainingQuantity > 0 && CurrentIndex < Placeholders.Count)
                {
                    int RequiredQuantity = Placeholders[CurrentIndex].Key.Quantity;
                    Object Placeholder = Placeholders[CurrentIndex].Value;

                    int Quantity = Math.Max(0, Math.Min(RemainingQuantity, RequiredQuantity));
                    Placeholder.Stack = Quantity;
                    RemainingQuantity -= Quantity;

                    if (Quantity > 0 && BagItem.Category == Object.artisanGoodsCategory)
                        Placeholder.Price = BagItem.Price;

                    CurrentIndex++;
                }
            }
        }

        #region Input Handling
        private BundleTask HoveredBundleTask = null;
        private BundleItem HoveredBundleItem = null;

        private DateTime? SecondaryActionButtonPressedTime = null;
        private bool IsSecondaryActionButtonHeld { get { return SecondaryActionButtonPressedTime.HasValue; } }
        private BundleItem SecondaryActionButtonPressedItem = null;

        #region Mouse Handling
        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            if (!IsJojaMember)
            {
                if (Bounds.Contains(e.OldPosition.LegacyScreenPixels().AsPoint()) || Bounds.Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
                {
                    BundleItem PreviouslyHovered = HoveredBundleItem;

                    this.HoveredBundleItem = null;
                    if (ItemSlotPositions != null)
                    {
                        foreach (KeyValuePair<BundleItem, Rectangle> KVP in ItemSlotPositions)
                        {
                            Rectangle Rect = KVP.Value;
                            if (Rect.GetOffseted(TopLeftScreenPosition).Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
                            {
                                if (PreviouslyHovered != null && PreviouslyHovered != KVP.Key)
                                    SecondaryActionButtonPressedItem = null;
                                this.HoveredBundleItem = KVP.Key;
                                this.IsNavigatingWithGamepad = false;
                                break;
                            }
                        }
                    }

                    this.HoveredBundleTask = null;
                    if (TaskHeaderPositions != null)
                    {
                        foreach (KeyValuePair<BundleTask, Rectangle> KVP in TaskHeaderPositions)
                        {
                            Rectangle Rect = KVP.Value;
                            if (Rect.GetOffseted(TopLeftScreenPosition).Contains(e.NewPosition.LegacyScreenPixels().AsPoint()))
                            {
                                this.HoveredBundleTask = KVP.Key;
                                this.IsNavigatingWithGamepad = false;
                                break;
                            }
                        }
                    }
                }
            }

            UpdateHoveredItem(e);
        }

        public void OnMouseButtonPressed(ButtonPressedEventArgs e)
        {
            if (!IsJojaMember)
            {
                if (e.Button == SButton.MouseLeft)
                {
                    HandlePrimaryAction(GetHoveredItem());
                }

                if (e.Button == SButton.MouseRight)
                {
                    HandleSecondaryAction(GetHoveredItem());
                    SecondaryActionButtonPressedItem = HoveredBundleItem;
                    SecondaryActionButtonPressedTime = DateTime.Now;
                }
            }
        }

        public void OnMouseButtonReleased(ButtonReleasedEventArgs e)
        {
            if (!IsJojaMember)
            {
                if (e.Button == SButton.MouseRight)
                {
                    SecondaryActionButtonPressedTime = null;
                    SecondaryActionButtonPressedItem = null;
                }
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
            //HoveredBundleItem = ItemSlotPositions.First().Key;
            //HoveredBundleTask = null;
            HoveredBundleItem = null;
            HoveredBundleTask = TaskHeaderPositions.First().Key;
            IsNavigatingWithGamepad = true;
        }

        public void LostGamepadFocus()
        {
            HoveredBundleItem = null;
            HoveredBundleTask = null;
            SecondaryActionButtonPressedItem = null;
            SecondaryActionButtonPressedTime = null;
        }

        public Dictionary<NavigationDirection, IGamepadControllable> MenuNeighbors { get; private set; } = new Dictionary<NavigationDirection, IGamepadControllable>();
        public bool TryGetMenuNeighbor(NavigationDirection Direction, out IGamepadControllable Neighbor)
        {
            return MenuNeighbors.TryGetValue(Direction, out Neighbor);
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            List<Rectangle> AllSlots = TaskHeaderPositions.Select(x => x.Value).Union(ItemSlotPositions.Select(x => x.Value)).Union(LockedSlotPositions)
                .OrderBy(x => x.Top).ThenBy(x => x.Left).ToList();
            return GamepadControls.TryGetSlotNeighbor(AllSlots, ItemSlot, ColumnCount, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
        }

        /// <param name="RelativeToScreen">If false, the returned rectangle will use <see cref="TopLeftScreenPosition"/> as the Origin.</param>
        private Rectangle? GetHoveredSlot(bool RelativeToScreen)
        {
            if (HoveredBundleItem != null)
            {
                Rectangle Result = ItemSlotPositions[HoveredBundleItem];
                if (RelativeToScreen)
                    Result = Result.GetOffseted(TopLeftScreenPosition);
                return Result;
            }
            else if (HoveredBundleTask != null)
            {
                Rectangle Result = TaskHeaderPositions[HoveredBundleTask];
                if (RelativeToScreen)
                    Result = Result.GetOffseted(TopLeftScreenPosition);
                return Result;
            }
            else
                return null;
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            Rectangle? HoveredSlot = GetHoveredSlot(false);
            if (IsGamepadFocused && HoveredSlot == null)
            {
                HoveredBundleItem = ItemSlotPositions.First().Key;
                IsNavigatingWithGamepad = HoveredBundleItem != null;
                return HoveredBundleItem != null;
            }
            else if (TryGetSlotNeighbor(HoveredSlot, Direction, HorizontalWrapping, VerticalWrapping, out Rectangle? Neighbor))
            {
                foreach (KeyValuePair<BundleItem, Rectangle> KVP in ItemSlotPositions)
                {
                    if (KVP.Value == Neighbor.Value)
                    {
                        HoveredBundleItem = KVP.Key;
                        HoveredBundleTask = null;
                        IsNavigatingWithGamepad = true;
                        return true;
                    }
                }

                foreach (KeyValuePair<BundleTask, Rectangle> KVP in TaskHeaderPositions)
                {
                    if (KVP.Value == Neighbor.Value)
                    {
                        HoveredBundleTask = KVP.Key;
                        HoveredBundleItem = null;
                        IsNavigatingWithGamepad = true;
                        return true;
                    }
                }

                HoveredBundleItem = null;
                HoveredBundleTask = null;

                return false;
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
                List<Rectangle> AllSlots = TaskHeaderPositions.Select(x => x.Value).Union(ItemSlotPositions.Select(x => x.Value)).ToList();
                Rectangle ToSelect = AllSlots.OrderBy(x => x.GetOffseted(TopLeftScreenPosition).SquaredDistanceBetweenCenters(ClosestTo.Value)).First();

                foreach (KeyValuePair<BundleItem, Rectangle> KVP in ItemSlotPositions)
                {
                    if (KVP.Value == ToSelect)
                    {
                        HoveredBundleItem = KVP.Key;
                        HoveredBundleTask = null;
                        return true;
                    }
                }

                foreach (KeyValuePair<BundleTask, Rectangle> KVP in TaskHeaderPositions)
                {
                    if (KVP.Value == ToSelect)
                    {
                        HoveredBundleTask = KVP.Key;
                        HoveredBundleItem = null;
                        return true;
                    }
                }
            }

            if (StartingSide == NavigationDirection.Right)
            {
                while (TryNavigate(NavigationDirection.Right, NavigationWrappingMode.NoWrap, NavigationWrappingMode.NoWrap)) { }
            }
            if (StartingSide == NavigationDirection.Down)
            {
                while (TryNavigate(NavigationDirection.Down, NavigationWrappingMode.NoWrap, NavigationWrappingMode.NoWrap)) { }
            }

            return true;
        }

        public bool IsNavigatingWithGamepad { get; private set; }

        public void OnGamepadButtonsPressed(Buttons GamepadButtons)
        {
            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                if (!GamepadControls.HandleNavigationButtons(this, GamepadButtons, GetHoveredSlot(true)))
                    this.IsGamepadFocused = false;

                //  Handle action buttons
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.PrimaryAction))
                {
                    HandlePrimaryAction(GetHoveredItem());
                }
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.SecondaryAction))
                {
                    HandleSecondaryAction(GetHoveredItem());
                    SecondaryActionButtonPressedItem = HoveredBundleItem;
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
                    SecondaryActionButtonPressedItem = null;
                    SecondaryActionButtonPressedTime = null;
                }
            }
        }
        #endregion Gamepad support

        private void HandlePrimaryAction(Item TargetItem)
        {
            if (TargetItem is Object TargetObject)
            {
                int Qty = ItemBag.GetQuantityToTransfer(ItemBag.InputTransferAction.PrimaryActionButtonPressed, TargetObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                Bag.MoveFromBag(TargetObject, Qty, out int MovedQty, true, IBM.InventorySource, IBM.ActualInventoryCapacity);
            }
        }

        private void HandleSecondaryAction(Item TargetItem)
        {
            if (TargetItem is Object TargetObject)
            {
                ItemBag.InputTransferAction TransferAction = IsSecondaryActionButtonHeld ? ItemBag.InputTransferAction.SecondaryActionButtonHeld : ItemBag.InputTransferAction.SecondaryActionButtonPressed;
                int Qty = ItemBag.GetQuantityToTransfer(TransferAction, TargetObject, IBM.IsTransferMultipleModifierHeld, IBM.IsTransferHalfModifierHeld);
                Bag.MoveFromBag(TargetObject, Qty, out int MovedQty, true, IBM.InventorySource, IBM.ActualInventoryCapacity);
            }
        }
        #endregion Input Handling

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

            if (!IsJojaMember)
            {
                if (e.IsMultipleOf(ItemBagMenu.TransferRepeatFrequency))
                {
                    if (IsSecondaryActionButtonHeld && HoveredBundleItem != null && SecondaryActionButtonPressedItem != null && HoveredBundleItem == SecondaryActionButtonPressedItem
                        && SecondaryActionButtonPressedTime.HasValue && DateTime.Now.Subtract(SecondaryActionButtonPressedTime.Value).TotalMilliseconds >= 500)
                    {
                        HandleSecondaryAction(GetHoveredItem());
                    }
                }
            }

            if (e.IsMultipleOf(GamepadControls.Current.NavigationRepeatFrequency) && IsGamepadFocused && IsNavigatingWithGamepad)
            {
                if (!GamepadControls.HandleNavigationButtons(this, null, GetHoveredSlot(true)))
                    this.IsGamepadFocused = false;
            }
        }

        public void OnClose()
        {
            Bag.OnContentsChanged -= Bag_ContentsChanged;
        }

        public Dictionary<BundleItem, Object> ItemPlaceholders { get; }

        //  All of the below rectangles are relative to 0,0 NOT TopLeftScreenPosition
        //  Positions of the RoomHeaders (Names of Rooms)
        public Dictionary<BundleRoom, Rectangle> RoomHeaderPositions { get; private set; }
        //  Positions of slots that are associated with a given Room
        public Dictionary<BundleRoom, List<Rectangle>> RoomSlotPositions { get; private set; }
        //  Positions of slots that don't hold items, but are instead just a placeholder prefix for the item slots associated with the BundleTask
        public Dictionary<BundleTask, Rectangle> TaskHeaderPositions { get; private set; }
        //  Positions of slots that are associated with a given BundleTask
        public Dictionary<BundleTask, List<Rectangle>> TaskSlotPositions { get; private set; }
        //  Positions of each item slot
        public OrderedDictionary<BundleItem, Rectangle> ItemSlotPositions { get; private set; }

        private List<Rectangle> LockedSlotPositions { get; set; }

        private SpriteFont RoomHeaderFont { get { return Game1.smallFont; } }
        private const float RoomHeaderScale = 0.8f;

        public bool CanResize { get; } = true;

        public void InitializeLayout(int ResizeIteration)
        {
            if (BundleBag == null)
                return;

            if (ResizeIteration > 1)
                this.SlotSize = Math.Min(OriginalSlotSize, Math.Max(24, OriginalSlotSize - (ResizeIteration - 1) * 8));

            if (IsJojaMember)
            {
                this.RelativeBounds = new Rectangle(0, 0, 640 + Padding * 2, 288 + Padding * 2);
            }
            else
            {
                HoveredBundleTask = null;
                HoveredBundleItem = null;
                SecondaryActionButtonPressedTime = null;
                SecondaryActionButtonPressedItem = null;

                int RoomBottomMargin = 12;
                int RoomRightMargin = 8;

                int StartX = Padding;
                int StartY = Padding;

                CommunityCenterBundles CC = CommunityCenterBundles.Instance;

                this.RoomHeaderPositions = new Dictionary<BundleRoom, Rectangle>();
                this.RoomSlotPositions = new Dictionary<BundleRoom, List<Rectangle>>();
                this.TaskHeaderPositions = new Dictionary<BundleTask, Rectangle>();
                this.TaskSlotPositions = new Dictionary<BundleTask, List<Rectangle>>();
                this.ItemSlotPositions = new OrderedDictionary<BundleItem, Rectangle>();
                this.LockedSlotPositions = new List<Rectangle>();

                int RoomNameWidth = CC.Rooms.Select(x => (int)(RoomHeaderFont.MeasureString(x.DisplayName).X * RoomHeaderScale)).DefaultIfEmpty(100).Max() + 32;

                int CurrentRow = 0;
                int CurrentColumn = 0;
                int RoomIndex = 0;
                foreach (BundleRoom Room in CC.Rooms)
                {
                    if (BundleBag.InvalidRooms[BundleBag.Size].Contains(Room.Name))
                        continue;

                    Rectangle RoomHeader = new Rectangle(StartX, StartY + CurrentRow * SlotSize + RoomIndex * RoomBottomMargin, RoomNameWidth, SlotSize);
                    RoomHeaderPositions.Add(Room, RoomHeader);

                    List<Rectangle> CurrentRoomSlotPositions = new List<Rectangle>();
                    RoomSlotPositions.Add(Room, CurrentRoomSlotPositions);

                    foreach (BundleTask Task in Room.Tasks)
                    {
                        if (CurrentColumn == ColumnCount)
                        {
                            CurrentColumn = 0;
                            CurrentRow++;
                        }

                        Rectangle TaskHeader = new Rectangle(StartX + RoomNameWidth + RoomRightMargin + CurrentColumn * SlotSize, StartY + CurrentRow * SlotSize + RoomIndex * RoomBottomMargin, SlotSize, SlotSize);
                        TaskHeaderPositions.Add(Task, TaskHeader);
                        CurrentRoomSlotPositions.Add(TaskHeader);

                        List<Rectangle> CurrentTaskSlotPositions = new List<Rectangle>();
                        TaskSlotPositions.Add(Task, CurrentTaskSlotPositions);

                        CurrentColumn++;

                        foreach (BundleItem Item in Task.Items)
                        {
                            if (!BundleTask.IsValidItemId(Item.Id))
                                continue;

                            if (CurrentColumn == ColumnCount)
                            {
                                CurrentColumn = 0;
                                CurrentRow++;
                            }

                            Rectangle Slot = new Rectangle(StartX + RoomNameWidth + RoomRightMargin + CurrentColumn * SlotSize, StartY + CurrentRow * SlotSize + RoomIndex * RoomBottomMargin, SlotSize, SlotSize);
                            CurrentRoomSlotPositions.Add(Slot);
                            CurrentTaskSlotPositions.Add(Slot);
                            ItemSlotPositions.Add(Item, Slot);

                            CurrentColumn++;
                        }
                    }

                    if (ShowLockedSlots)
                    {
                        while (CurrentColumn < ColumnCount)
                        {
                            Rectangle LockedSlot = new Rectangle(StartX + RoomNameWidth + RoomRightMargin + CurrentColumn * SlotSize, StartY + CurrentRow * SlotSize + RoomIndex * RoomBottomMargin, SlotSize, SlotSize);
                            LockedSlotPositions.Add(LockedSlot);
                            CurrentColumn++;
                        }
                    }

                    CurrentColumn = 0;
                    CurrentRow++;
                    RoomIndex++;
                }

                this.RelativeBounds = new Rectangle(0, 0, Padding + RoomNameWidth + RoomRightMargin + ColumnCount * SlotSize + Padding,
                    Padding + CurrentRow * SlotSize + Padding + (RoomIndex - 1) * RoomBottomMargin);
            }
        }

        private static Rectangle SlotDarkBackground = new Rectangle(620, 244, 18, 18);
        private static Rectangle SlotMediumBackground = new Rectangle(512, 244, 18, 18);
        private static Rectangle SlotLightBackground = new Rectangle(530, 262, 18, 18);
        private static Rectangle CheckMark = new Rectangle(51, 4, 11, 8);
        private static int CheckMarkScale = 2;

        public void Draw(SpriteBatch b)
        {
            if (IsJojaMember)
            {
                Rectangle BackgroundDestination = new Rectangle(Padding, Padding, RelativeBounds.Width - Padding * 2, RelativeBounds.Height - Padding * 2).GetOffseted(TopLeftScreenPosition);
                b.Draw(TextureHelpers.JojaCDForm, BackgroundDestination, new Rectangle(0, 0, TextureHelpers.JojaCDForm.Width, TextureHelpers.JojaCDForm.Height - 16), Color.White);

                string Text = "You Traitor!";
                SpriteFont Font = Game1.smallFont;
                Vector2 TextSize = Font.MeasureString(Text) * 2;

                int JojaSuxDestinationSize = 128;

                int BoxPadding = 32;
                int BoxWidth = Math.Max((int)TextSize.X, JojaSuxDestinationSize) + BoxPadding * 2;
                int BoxHeight = (int)TextSize.Y + BoxPadding * 2 + JojaSuxDestinationSize - JojaSuxDestinationSize / 8;

                Rectangle BoxDestination = new Rectangle((RelativeBounds.Width - BoxWidth) / 2, (RelativeBounds.Height - BoxHeight) / 2, 
                    BoxWidth, BoxHeight).GetOffseted(TopLeftScreenPosition);
                DrawHelpers.DrawBox(b, BoxDestination);

                Vector2 TextDestination = new Vector2(BoxDestination.X + (BoxDestination.Width - TextSize.X) / 2, BoxDestination.Y + BoxPadding);
                b.DrawString(Font, Text, TextDestination, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

                Rectangle JojaSuxSourcePosition = new Rectangle(258, 640, 32, 32);
                Rectangle JojaSuxDestination = new Rectangle(BoxDestination.X + (BoxDestination.Width - JojaSuxDestinationSize) / 2, BoxDestination.Bottom - BoxPadding - JojaSuxDestinationSize, JojaSuxDestinationSize, JojaSuxDestinationSize);
                b.Draw(Game1.mouseCursors, JojaSuxDestination, JojaSuxSourcePosition, Color.White);
            }
            else
            {
                //  Draw room names
                foreach (KeyValuePair<BundleRoom, Rectangle> RoomHeader in RoomHeaderPositions)
                {
                    bool IsCompleted = RoomHeader.Key.IsCompleted;
                    Rectangle Position = RoomHeader.Value.GetOffseted(TopLeftScreenPosition);
                    DrawHelpers.DrawBox(b, Position);

                    string Text = RoomHeader.Key.DisplayName;
                    Vector2 Size = RoomHeaderFont.MeasureString(Text) * RoomHeaderScale;
                    b.DrawString(RoomHeaderFont, Text, new Vector2(Position.X + (Position.Width - Size.X) / 2, Position.Y + (Position.Height - Size.Y) / 2),
                        IsCompleted ? Color.Green : Color.Black, 0f, Vector2.Zero, RoomHeaderScale, SpriteEffects.None, 1f);

                    if (IsCompleted)
                    {
                        Rectangle CheckMarkDestination = new Rectangle(Position.Right - SlotSize / 6 - CheckMark.Width * CheckMarkScale, Position.Bottom - SlotSize / 6 - CheckMark.Height * CheckMarkScale,
                            CheckMark.Width * CheckMarkScale, CheckMark.Height * CheckMarkScale);
                        b.Draw(TextureHelpers.PlayerStatusList, CheckMarkDestination, CheckMark, Color.White);
                    }
                }

                //  Draw the backgrounds of each slot
                foreach (Rectangle LockedSlot in LockedSlotPositions)
                {
                    b.Draw(Game1.menuTexture, LockedSlot.GetOffseted(TopLeftScreenPosition), new Rectangle(64, 896, 64, 64), Color.White);
                }
                foreach (KeyValuePair<BundleItem, Rectangle> ItemSlot in ItemSlotPositions)
                {
                    Rectangle TexturePosition;
                    if (ItemSlot.Key.IsCompleted)
                    {
                        TexturePosition = SlotDarkBackground;
                    }
                    else
                    {
                        Object Item = ItemPlaceholders[ItemSlot.Key];
                        if (Item.Stack == 0)
                            TexturePosition = SlotLightBackground;
                        else if (Item.Stack == ItemSlot.Key.Quantity)
                            TexturePosition = SlotDarkBackground;
                        else
                            TexturePosition = SlotMediumBackground;
                    }

                    b.Draw(TextureHelpers.JunimoNoteTexture, ItemSlot.Value.GetOffseted(TopLeftScreenPosition), TexturePosition, Color.White);
                }
                foreach (KeyValuePair<BundleTask, Rectangle> TaskHeader in TaskHeaderPositions)
                {
                    Rectangle TexturePosition = SlotMediumBackground;
                    if (TaskHeader.Key.IsCompleted)
                        TexturePosition = SlotDarkBackground;
                    b.Draw(TextureHelpers.JunimoNoteTexture, TaskHeader.Value.GetOffseted(TopLeftScreenPosition), TexturePosition, Color.White);
                }

                //  Draw the Task headers
                foreach (KeyValuePair<BundleTask, Rectangle> TaskHeader in TaskHeaderPositions)
                {
                    bool IsCompleted = TaskHeader.Key.IsCompleted;

                    //  Draw a thin yellow border if mouse is hovering this slot
                    bool IsHovered = TaskHeader.Key == HoveredBundleTask;
                    if (IsHovered)
                    {
                        Rectangle Destination = TaskHeader.Value.GetOffseted(TopLeftScreenPosition);

                        Color HighlightColor = Color.Yellow;
                        Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                        b.Draw(Highlight, Destination, Color.White * 0.25f);

                        int BorderThickness = Destination.Width / 16;
                        DrawHelpers.DrawBorder(b, Destination, BorderThickness, HighlightColor);
                    }

                    Rectangle Slot = TaskHeader.Value;
                    Rectangle ScaledSlot = new Rectangle(Slot.X + Slot.Width / 6, Slot.Y + Slot.Height / 6, Slot.Width - Slot.Width / 3, Slot.Height - Slot.Height / 3);
                    Rectangle SourceIconPosition = IsCompleted ? TaskHeader.Key.SpriteSmallIconOpenedPosition : TaskHeader.Key.SpriteSmallIconClosedPosition;
                    b.Draw(TextureHelpers.JunimoNoteTexture, ScaledSlot.GetOffseted(TopLeftScreenPosition), SourceIconPosition, Color.White);

                    if (IsCompleted)
                    {
                        Rectangle CheckMarkDestination = new Rectangle(Slot.Right - 1 - CheckMark.Width * CheckMarkScale, Slot.Bottom - 1 - CheckMark.Height * CheckMarkScale,
                            CheckMark.Width * CheckMarkScale, CheckMark.Height * CheckMarkScale).GetOffseted(TopLeftScreenPosition);
                        b.Draw(TextureHelpers.PlayerStatusList, CheckMarkDestination, CheckMark, Color.White);
                    }
                }

                //  Draw the items of each slot
                foreach (KeyValuePair<BundleItem, Rectangle> ItemSlot in ItemSlotPositions)
                {
                    Rectangle Destination = ItemSlot.Value.GetOffseted(TopLeftScreenPosition);
                    Object CurrentItem;
                    if (!ItemPlaceholders.TryGetValue(ItemSlot.Key, out CurrentItem))
                        CurrentItem = ItemSlot.Key.ToObject();

                    bool IsCompleted = ItemSlot.Key.IsCompleted;

                    //  Draw a thin yellow border if mouse is hovering this slot
                    bool IsHovered = ItemSlot.Key == HoveredBundleItem;
                    if (IsHovered)
                    {
                        Color HighlightColor = Color.Yellow;
                        Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                        b.Draw(Highlight, Destination, Color.White * 0.25f);

                        int BorderThickness = Destination.Width / 16;
                        DrawHelpers.DrawBorder(b, Destination, BorderThickness, HighlightColor);
                    }

                    float IconScale = IsHovered ? 1.25f : 1.0f;
                    Color Overlay = CurrentItem.Stack == 0 || IsCompleted ? Color.White * 0.30f : Color.White;
                    bool DrawQuantity = CurrentItem.Stack > 0 && !IsCompleted && ItemSlot.Key.Quantity > 1;
                    DrawHelpers.DrawItem(b, Destination, CurrentItem, DrawQuantity, true, IconScale, 1.0f, Overlay,
                        CurrentItem.Stack >= ItemSlot.Key.Quantity ? Color.Green : Color.White);

                    if (IsCompleted)
                    {
                        Rectangle CheckMarkDestination = new Rectangle(Destination.Right - 1 - CheckMark.Width * CheckMarkScale, Destination.Bottom - 1 - CheckMark.Height * CheckMarkScale,
                            CheckMark.Width * CheckMarkScale, CheckMark.Height * CheckMarkScale);
                        b.Draw(TextureHelpers.PlayerStatusList, CheckMarkDestination, CheckMark, Color.White);
                    }
                }
            }
        }

        public void DrawToolTips(SpriteBatch b)
        {
            if (!IsJojaMember)
            {
                if (HoveredBundleItem != null)
                {
                    Object HoveredItem = GetHoveredItem();
                    if (HoveredItem != null)
                    {
                        Rectangle Location;
                        if (IsNavigatingWithGamepad)
                            Location =  ItemSlotPositions[HoveredBundleItem].GetOffseted(TopLeftScreenPosition);
                        else
                            Location = new Rectangle(Game1.getMouseX() - 8, Game1.getMouseY() + 36, 8 + 36, 1);
                        DrawHelpers.DrawToolTipInfo(b, Location, HoveredItem, true, true, true, true, true, true,
                            HoveredBundleItem.Quantity, !HoveredBundleItem.IsCompleted, Color.White);
                    }
                }

                if (HoveredBundleTask != null)
                {
                    Rectangle ToolTipAnchorPoint = TaskHeaderPositions[HoveredBundleTask].GetOffseted(TopLeftScreenPosition);

                    int Padding = 20;
                    int TaskImageSize = 96;
                    int RewardIconSize = 48;
                    int RewardWidth = RewardIconSize + 12 + RewardIconSize;

                    //  Compute header
                    SpriteFont HeaderFont = Game1.dialogueFont;
                    float HeaderScale = 1f;
                    string HeaderText = !string.IsNullOrEmpty(HoveredBundleTask.TranslatedName) ? HoveredBundleTask.TranslatedName : HoveredBundleTask.Name + " Bundle";
                    Vector2 HeaderSize = HeaderFont.MeasureString(HeaderText) * HeaderScale;

                    //  Compute description
                    int ReadyToComplete = 0;
                    foreach (BundleItem BI in HoveredBundleTask.Items)
                    {
                        if (!BI.IsCompleted && ItemPlaceholders.TryGetValue(BI, out Object Placeholder) && Placeholder.Stack >= BI.Quantity)
                        {
                            ReadyToComplete++;
                        }
                    }
                    SpriteFont BodyFont = Game1.smallFont;
                    float BodyScale = 1.0f;
                    string BodyText = HoveredBundleTask.IsCompleted ? ItemBagsMod.Translate("BundleTaskCompletedToolTip") :
                        ItemBagsMod.Translate("BundleTaskIncompleteToolTip", new Dictionary<string, string>()
                        {
                            { "CompletedCount", HoveredBundleTask.Items.Count(x => x.IsCompleted).ToString() },
                            { "TotalCount", HoveredBundleTask.Items.Count.ToString() },
                            { "ReadyToCompleteCount", ReadyToComplete.ToString() },
                            { "RequiredCount", HoveredBundleTask.RequiredItemCount.ToString() }
                        });
                    Vector2 BodySize = BodyFont.MeasureString(BodyText) * BodyScale;

                    int ToolTipWidth = (int)new List<float>() { TaskImageSize, HeaderSize.X, BodySize.X, RewardWidth }.Max() + Padding * 2;
                    int ToolTipHeight = Padding + TaskImageSize + 16 + (int)HeaderSize.Y + 12 + (int)BodySize.Y + 12 + RewardIconSize + Padding;

                    //  Ensure tooltip is fully visible on screen
                    Rectangle Position = new Rectangle(ToolTipAnchorPoint.Right, ToolTipAnchorPoint.Top, ToolTipWidth, ToolTipHeight);
                    if (Position.Right > Game1.viewport.Size.Width)
                        Position = new Rectangle(ToolTipAnchorPoint.Left - ToolTipWidth, Position.Top, Position.Width, Position.Height);
                    if (Position.Bottom > Game1.viewport.Size.Height)
                        Position = new Rectangle(Position.X, ToolTipAnchorPoint.Bottom - ToolTipHeight, Position.Width, Position.Height);

                    DrawHelpers.DrawBox(b, Position);

                    int CurrentY = Position.Y + Padding;

                    //  Draw image associated with this task
                    Rectangle TaskImagePosition = new Rectangle(Position.X + (Position.Width - TaskImageSize) / 2, CurrentY, TaskImageSize, TaskImageSize);
                    DrawHelpers.DrawBorder(b, new Rectangle(TaskImagePosition.Left - 4, TaskImagePosition.Top - 4, TaskImagePosition.Width + 8, TaskImagePosition.Height + 8), 4, Color.Black);
                    b.Draw(HoveredBundleTask.ActualLargeIconTexture, TaskImagePosition, HoveredBundleTask.ActualLargeIconPosition, Color.White);
                    CurrentY += TaskImageSize + 16;

                    //  Draw header text (Task's name)
                    Vector2 HeaderPosition = new Vector2(Position.X + (Position.Width - HeaderSize.X) / 2, CurrentY);
                    b.DrawString(HeaderFont, HeaderText, HeaderPosition, Color.Black, 0f, Vector2.Zero, HeaderScale, SpriteEffects.None, 1f);
                    CurrentY += (int)(HeaderSize.Y + 12);

                    //  Draw description text
                    Vector2 BodyPosition = new Vector2(Position.X + (Position.Width - BodySize.X) / 2, CurrentY);
                    b.DrawString(BodyFont, BodyText, BodyPosition, Color.SlateGray, 0f, Vector2.Zero, BodyScale, SpriteEffects.None, 1f);
                    CurrentY += (int)(BodySize.Y + 12);

                    //  Draw reward item
                    Rectangle RewardPosition = new Rectangle(Position.X + (Position.Width - RewardWidth) / 2, CurrentY, RewardWidth, RewardIconSize);
                    b.Draw(TextureHelpers.JunimoNoteTexture, new Rectangle(RewardPosition.X, RewardPosition.Y, RewardIconSize, RewardIconSize), new Rectangle(548, 264, 18, 18), Color.White);
                    if (HoveredBundleTask.Reward != null)
                        DrawHelpers.DrawItem(b, new Rectangle(RewardPosition.Right - RewardIconSize, RewardPosition.Y, RewardIconSize, RewardIconSize), HoveredBundleTask.Reward.ToItem(), true, true, 1f, 1f, Color.White, Color.White);
                }
            }
        }

        internal Object GetHoveredItem()
        {
            if (IsJojaMember)
            {
                return null;
            }
            else
            {
                if (HoveredBundleItem != null)
                {
                    if (ItemPlaceholders.TryGetValue(HoveredBundleItem, out Object IncompleteItem))
                        return IncompleteItem;
                    else
                        return HoveredBundleItem.ToObject();
                }
                else
                    return null;
            }
        }
    }
}
