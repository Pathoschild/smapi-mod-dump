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
    public class GroupedLayoutOptions : IBagMenuContent
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

        public enum ColumnType
        {
            RegularQuality = 0,
            SilverQuality = 1,
            GoldQuality = 2,
            IridiumQuality = 3,
            RowValue = 4
        }

        public static ColumnType ConvertObjectQualityToColumnType(ObjectQuality Quality)
        {
            if (Quality == ObjectQuality.Regular)
                return ColumnType.RegularQuality;
            else if (Quality == ObjectQuality.Silver)
                return ColumnType.SilverQuality;
            else if (Quality == ObjectQuality.Gold)
                return ColumnType.GoldQuality;
            else if (Quality == ObjectQuality.Iridium)
                return ColumnType.IridiumQuality;
            else
                throw new NotImplementedException(string.Format("Unrecognized ObjectQuality: {0}", Quality.ToString()));
        }

        public static ObjectQuality ConvertColumnTypeToObjectQuality(ColumnType Column)
        {
            if (Column == ColumnType.RegularQuality)
                return ObjectQuality.Regular;
            else if (Column == ColumnType.SilverQuality)
                return ObjectQuality.Silver;
            else if (Column == ColumnType.GoldQuality)
                return ObjectQuality.Gold;
            else if (Column == ColumnType.IridiumQuality)
                return ObjectQuality.Iridium;
            else if (Column == ColumnType.RowValue)
                throw new InvalidOperationException("Cannot convert from ColumnType.RowValue to ObjectQuality");
            else
                throw new NotImplementedException(string.Format("Unrecognized ColumnType: {0}", Column.ToString()));
        }

        public ItemBagMenu IBM { get { return Menu?.IBM; } }

        public int Padding { get; } = 8;

        private int OriginalSlotSize { get; }
        /// <summary>The size, in pixels, to use when rendering an item slot. Recommended = <see cref="BagInventoryMenu.DefaultInventoryIconSize"/></summary>
        public int SlotSize { get; private set; }
        /// <summary>Determines how many groups to show in each row. Recommended = 3.<para/>
        /// A group will typically be 4 columns (all 4 qualities), and possibly a value column, so the total number of columns in a row is usually either <see cref="GroupsPerRow"/>*4 or <see cref="GroupsPerRow"/>*5</summary>
        public int GroupsPerRow { get; }
        /// <summary>If true, each distinct Item group will have an extra column, that shows the summed value of the items in that group</summary>
        public bool ShowValueColumn { get; }

        /// <summary>Key = Item Id, then Item Quality, Value = The bounds of that item's slot, relative to <see cref="TopLeftScreenPosition"/>. Use <see cref="SlotBounds"/> when rendering to screen space.</summary>
        public OrderedDictionary<string, Dictionary<ColumnType, Rectangle>> RelativeSlotBounds { get; private set; }
        /// <summary>Key = Item Id, then Item Quality, Value = The bounds of that item's slot</summary>
        public OrderedDictionary<string, Dictionary<ColumnType, Rectangle>> SlotBounds { get; private set; }
        public event EventHandler<ItemSlotRenderedEventArgs> OnItemSlotRendered;

        public Dictionary<string, Dictionary<ObjectQuality, Object>> Placeholders { get; private set; }

        public ReadOnlyCollection<Rectangle> RelativeColumnHeaderBounds { get; private set; }
        public ReadOnlyCollection<Rectangle> ColumnHeaderBounds { get; private set; }

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
                    //  Shift every Rectangle over by the new TopLeft point
                    OrderedDictionary<string, Dictionary<ColumnType, Rectangle>> Temp = new OrderedDictionary<string, Dictionary<ColumnType, Rectangle>>();
                    foreach (var KVP in RelativeSlotBounds)
                    {
                        string ItemId = KVP.Key;
                        Dictionary<ColumnType, Rectangle> RelativePositions = KVP.Value;
                        Dictionary<ColumnType, Rectangle> TranslatedPositions = new Dictionary<ColumnType, Rectangle>();
                        foreach (var KVP2 in RelativePositions)
                        {
                            ColumnType Type = KVP2.Key;
                            Rectangle Relative = KVP2.Value;
                            TranslatedPositions.Add(Type, Relative.GetOffseted(TopLeftScreenPosition));
                        }
                        Temp.Add(ItemId, TranslatedPositions);
                    }
                    this.SlotBounds = Temp;

                    this.ColumnHeaderBounds = new ReadOnlyCollection<Rectangle>(RelativeColumnHeaderBounds.Select(x => x.GetOffseted(TopLeftScreenPosition)).ToList());
                }
                else
                {
                    this.SlotBounds = null;
                    this.ColumnHeaderBounds = null;
                }

                this.Bounds = RelativeBounds.GetOffseted(TopLeftScreenPosition);
            }
        }

        /// <summary>The bounds of this menu's content, relative to <see cref="TopLeftScreenPosition"/></summary>
        public Rectangle RelativeBounds { get; private set; }
        public Rectangle Bounds { get; private set; }

        public ReadOnlyCollection<AllowedObject> GroupedObjects { get; private set; }
        public bool IsEmptyMenu { get { return GroupedObjects == null || !GroupedObjects.Any(); } }

        private int TotalSlots { get; set; }
        public int RowCount { get; private set; }
        public int ColumnsPerGroup { get; }

        public const int HorizontalMarginBetweenGroups = 8;

        public GroupedLayoutOptions(int GroupsPerRow, bool ShowValueColumn, int SlotSize = BagInventoryMenu.DefaultInventoryIconSize)
        {
            this.GroupsPerRow = GroupsPerRow;
            this.ShowValueColumn = ShowValueColumn;
            this.OriginalSlotSize = SlotSize;
            this.SlotSize = SlotSize;

            this.ColumnsPerGroup = Enum.GetValues(typeof(ObjectQuality)).Cast<ObjectQuality>().Count();
            if (ShowValueColumn)
                ColumnsPerGroup++;
        }

        public GroupedLayoutOptions(BagMenuOptions.GroupedLayout GLO)
            : this(GLO.GroupsPerRow, GLO.ShowValueColumn, GLO.SlotSize) { }

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
                foreach (Rectangle Rect in SlotBounds.SelectMany(x => x.Value.Values))
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
                Object PressedObject = GetHoveredItem();
                HandlePrimaryAction(PressedObject);
            }

            if (e.Button == SButton.MouseRight)
            {
                Object PressedObject = GetHoveredItem();
                HandleSecondaryAction(PressedObject);

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
            HoveredSlot = SlotBounds.First().Value[ColumnType.RegularQuality];
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
            List<Rectangle> AllSlots = SlotBounds.Select(x => x.Value).SelectMany(x => x.OrderBy(y => y.Key).Select(z => z.Value)).ToList();
            return GamepadControls.TryGetSlotNeighbor(AllSlots, ItemSlot, GroupsPerRow * ColumnsPerGroup, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            if (IsGamepadFocused && HoveredSlot == null)
            {
                HoveredSlot = SlotBounds.First().Value[ColumnType.RegularQuality];
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
                List<Rectangle> AllSlots = SlotBounds.Select(x => x.Value).SelectMany(x => x.OrderBy(y => y.Key).Select(z => z.Value)).ToList();
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
                this.GroupedObjects = new ReadOnlyCollection<AllowedObject>(new List<AllowedObject>());
                this.Placeholders = new Dictionary<string, Dictionary<ObjectQuality, Object>>();
            }
            else
            {
                this.GroupedObjects = new ReadOnlyCollection<AllowedObject>(BoundedBag.AllowedObjects.Where(x => Menu.GroupByQuality && x.HasQualities && !x.IsBigCraftable).ToList());

                int TotalFilledSlots = GroupedObjects.Count * ColumnsPerGroup;
                this.TotalSlots = ((GroupedObjects.Count - 1) / GroupsPerRow + 1) * GroupsPerRow * ColumnsPerGroup; // Force grid to perfect square, for ex if there were 13 items and 12 columns, we'd want 2x12=24 slots
                this.RowCount = (GroupedObjects.Count - 1) / GroupsPerRow + 1;

                List<ObjectQuality> Qualities = Enum.GetValues(typeof(ObjectQuality)).Cast<ObjectQuality>().ToList();

                //  Create an item with quantity=0 for each Item that the Bag is capable of storing
                this.Placeholders = new Dictionary<string, Dictionary<ObjectQuality, Object>>();
                foreach (AllowedObject Item in GroupedObjects)
                {
                    if (Item.IsBigCraftable)
                        throw new InvalidOperationException(string.Format("BigCraftable Items are not valid for GroupedLayouts. Bag = {0}, ItemId = {1}", Menu.BoundedBag.DisplayName, Item.Id));

                    Dictionary<ObjectQuality, Object> Group = new Dictionary<ObjectQuality, Object>();

                    foreach (ObjectQuality Quality in Qualities)
                    {
                        Group.Add(Quality, new Object(Item.Id, 0, false, -1, (int)Quality));
                    }

                    Placeholders.Add(Item.Id, Group);
                }

                UpdateQuantities();

                SetTopLeft(Point.Zero, false);
            }
        }

        private void UpdateQuantities()
        {
            //  Initialize all quantities back to zero
            foreach (Object Placeholder in Placeholders.SelectMany(x => x.Value.Values))
                Placeholder.Stack = 0;

            //  Set quantities of the placeholder items to match the corresponding amount of the item currently stored in the bag
            foreach (Object Item in Bag.Contents)
            {
                if (Placeholders.TryGetValue(Item.ItemId, out Dictionary<ObjectQuality, Object> Group))
                {
                    ObjectQuality Quality = (ObjectQuality)Item.Quality;
                    if (Group.TryGetValue(Quality, out Object Placeholder))
                    {
                        ItemBag.ForceSetQuantity(Placeholder, Item.Stack);

                        if (Placeholder.Price != Item.Price)
                        {
#if DEBUG
                            string WarningMsg = string.Format("Warning - GroupedLayout placeholder item '{0}' does not have a matching price to the corresponding item in the bag."
                                + " Placeholder.Price={1}, BagItem.Price={2}", Placeholder.DisplayName, Placeholder.Price, Item.Price);
                            ItemBagsMod.ModInstance.Monitor.Log(WarningMsg, LogLevel.Warn);
#endif
                            Placeholder.Price = Item.Price;
                        }
                    }
                }
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

            int ColumnHeaderHeight = SlotSize;
            int GroupWidth = ColumnsPerGroup * SlotSize;

            int RequiredWidth = GroupsPerRow * ColumnsPerGroup * SlotSize + Padding * 2 + (GroupsPerRow - 1) * HorizontalMarginBetweenGroups;
            int RequiredHeight = RowCount * SlotSize + Padding * 2 + ColumnHeaderHeight;
            this.RelativeBounds = new Rectangle(0, 0, RequiredWidth, RequiredHeight);

            //  Create the cells within each row/column
            OrderedDictionary<string, Dictionary<ColumnType, Rectangle>> SlotBounds = new OrderedDictionary<string, Dictionary<ColumnType, Rectangle>>();
            for (int i = 0; i < GroupedObjects.Count; i++)
            {
                AllowedObject Group = GroupedObjects[i];

                int Row = i / GroupsPerRow;
                int Column = i - Row * GroupsPerRow;

                int GroupStartX = Column * GroupWidth + Padding + (i % GroupsPerRow) * HorizontalMarginBetweenGroups;

                Dictionary<ColumnType, Rectangle> GroupSlots = new Dictionary<ColumnType, Rectangle>();
                for (int j = 0; j < ColumnsPerGroup; j++)
                {
                    ColumnType ColumnType = (ColumnType)j;
                    int X = GroupStartX + j * SlotSize;
                    Rectangle Slot = new Rectangle(X, Row * SlotSize + Padding + ColumnHeaderHeight, SlotSize, SlotSize);
                    GroupSlots.Add(ColumnType, Slot);
                }

                SlotBounds.Add(Group.Id, GroupSlots);
            }
            RelativeSlotBounds = SlotBounds;

            //  Create column headers
            List<Rectangle> ColumnHeaderBounds = new List<Rectangle>();
            for (int i = 0; i < GroupsPerRow; i++)
            {
                int GroupStartX = i * GroupWidth + Padding + i * HorizontalMarginBetweenGroups;

                for (int j = 0; j < ColumnsPerGroup; j++)
                {
                    ColumnHeaderBounds.Add(new Rectangle(GroupStartX + j * SlotSize, Padding, ColumnHeaderHeight, ColumnHeaderHeight));
                }
            }
            RelativeColumnHeaderBounds = new ReadOnlyCollection<Rectangle>(ColumnHeaderBounds);
        }

        private Texture2D GoldIconTexture { get { return TextureHelpers.EmojiSpritesheet; } }
        private Rectangle GoldIconSourceRect { get { return new Rectangle(117, 18, 9, 9); } }

        public void Draw(SpriteBatch b)
        {
            if (IsEmptyMenu)
                return;

            //b.Draw(TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color.Orange), Bounds, Color.White);

            //  Draw column headers
            for (int i = 0; i < ColumnHeaderBounds.Count; i++)
            {
                Rectangle Destination = ColumnHeaderBounds[i];

                b.Draw(Game1.menuTexture, Destination, new Rectangle(64, 896, 64, 64), Color.White);

                Rectangle IconDestination = new Rectangle(Destination.X + Destination.Width / 4, Destination.Y + Destination.Height / 4, Destination.Width / 2, Destination.Height / 2);

                ColumnType Type = (ColumnType)(i % ColumnsPerGroup);

                if (Type == ColumnType.RowValue)
                {
                    //Could also use Game1.mouseCursors with SourceSprite = new Rectangle(280, 411, 16, 16);
                    b.Draw(GoldIconTexture, IconDestination, GoldIconSourceRect, Color.White);
                }
                else
                {
                    Rectangle SourceRect = ItemBag.QualityIconTexturePositions[ConvertColumnTypeToObjectQuality(Type)];
                    b.Draw(Game1.mouseCursors, IconDestination, SourceRect, Color.White);
                }
            }

            //  Draw cells
            foreach (var KVP in SlotBounds)
            {
                string ItemId = KVP.Key;

                foreach (var KVP2 in KVP.Value)
                {
                    ColumnType ColumnType = KVP2.Key;
                    Rectangle Destination = KVP2.Value;
                    b.Draw(Game1.menuTexture, Destination, new Rectangle(128, 128, 64, 64), Color.White);

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

                    if (ColumnType != ColumnType.RowValue)
                    {
                        ObjectQuality Quality = ConvertColumnTypeToObjectQuality(ColumnType);
                        Object CurrentItem = Placeholders[ItemId][Quality];

                        float IconScale = IsHovered ? 1.25f : 1.0f;
                        Color Overlay = CurrentItem.Stack == 0 ? Color.White * 0.30f : Color.White;
                        DrawHelpers.DrawItem(b, Destination, CurrentItem, CurrentItem.Stack > 0, true, IconScale, 1.0f, Overlay, CurrentItem.Stack >= Bag.MaxStackSize ? Color.Red : Color.White);

                        OnItemSlotRendered?.Invoke(this, new ItemSlotRenderedEventArgs(b, Destination, CurrentItem, IsHovered));
                    }
                    else
                    {
                        //  Sum up the value of all different qualities of this item
                        int SummedValue = Placeholders[ItemId].Values.Sum(x => x.Stack * ItemBag.GetSingleItemPrice(x));
                        int NumDigits = DrawHelpers.GetNumDigits(SummedValue);

                        //  Compute width/height of the number
                        float ValueScale;
                        int ValueWidth, ValueHeight, CurrentIteration = 0;
                        do
                        {
                            ValueScale = (2.7f - CurrentIteration * 0.1f) * Destination.Width / (float)BagInventoryMenu.DefaultInventoryIconSize;
                            ValueWidth = (int)DrawHelpers.MeasureNumber(SummedValue, ValueScale);
                            ValueHeight = (int)(DrawHelpers.TinyDigitBaseHeight * ValueScale);
                            CurrentIteration++;
                        } while (ValueWidth > Destination.Width * 1.04); // * 1.04 to let the value extend very slightly outside the bounds of the slot

                        //  Draw the number in the center of the slot
                        Vector2 TopLeftPosition = new Vector2(Destination.X + (Destination.Width - ValueWidth) / 2 + 1, Destination.Y + (Destination.Height - ValueHeight) / 2);
                        Color ValueColor = GetValueColor(SummedValue);
                        Utility.drawTinyDigits(SummedValue, b, TopLeftPosition, ValueScale, 0f, ValueColor);
                    }
                }
            }
        }

        private static Color GetValueColor(int Value)
        {
            return Value < 2000 ? Color.White : Value < 10000 ? Color.Yellow : Value < 50000 ? Color.Orange : Color.Green;
        }

        public void DrawToolTips(SpriteBatch b)
        {
            if (IsEmptyMenu)
                return;

            //  Draw tooltip over hovered item
            if (HoveredSlot.HasValue)
            {
                //  Get the hovered Item Id and ColumnType
                string ItemId = null;
                ColumnType? Column = null;
                foreach (var KVP in SlotBounds)
                {
                    foreach (var KVP2 in KVP.Value)
                    {
                        if (KVP2.Value == HoveredSlot.Value)
                        {
                            ItemId = KVP.Key;
                            Column = KVP2.Key;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(ItemId) && Column.HasValue)
                        break;
                }

                if (!string.IsNullOrEmpty(ItemId) && Column.HasValue)
                {
                    if (Column == ColumnType.RowValue)
                    {
                        List<Object> Items = Placeholders[ItemId].Values.ToList();
                        List<int> Quantities = Items.Select(x => x.Stack).ToList();
                        List<int> SingleValues = Items.Select(x => ItemBag.GetSingleItemPrice(x)).ToList();
                        List<int> MultipliedValues = Items.Select(x => x.Stack * ItemBag.GetSingleItemPrice(x)).ToList();

                        //  Compute how many digits each column needs so that we can align each number properly by making each number take up the maximum size of other numbers in the same column
                        int SingleValueColumnDigits = SingleValues.Max(x => DrawHelpers.GetNumDigits(x));
                        int QuantityColumnDigits = Quantities.Max(x => DrawHelpers.GetNumDigits(x));
                        int MultipliedValueColumnDigits = MultipliedValues.Max(x => DrawHelpers.GetNumDigits(x));
                        float DigitScale = 3.0f;
                        float SingleValueColumnWidth = SingleValueColumnDigits * DigitScale * DrawHelpers.TinyDigitBaseWidth;
                        float QuantityColumnWidth = QuantityColumnDigits * DigitScale * DrawHelpers.TinyDigitBaseWidth;
                        float MultipliedValueColumnWidth = MultipliedValueColumnDigits * DigitScale * DrawHelpers.TinyDigitBaseWidth;

                        //  Compute how big the tooltip needs to be
                        int Margin = 32;
                        int LineHeight = 28;
                        int HorizontalSeparatorHeight = 6;
                        int SeparatorMargin = 4;
                        float PlusCharacterWidth = Game1.tinyFont.MeasureString("+").X;
                        float MultiplyCharacterWidth = Game1.tinyFont.MeasureString("*").X;
                        float EqualsCharacterWidth = Game1.tinyFont.MeasureString("=").X;
                        float SpaceWidth = 7f;
                        int TotalWidth = (int)(Margin + PlusCharacterWidth + SpaceWidth + SingleValueColumnWidth + SpaceWidth + MultiplyCharacterWidth + SpaceWidth + QuantityColumnWidth + SpaceWidth + EqualsCharacterWidth + SpaceWidth + MultipliedValueColumnWidth + Margin);
                        int TotalHeight = (int)(Margin + Items.Count * LineHeight + SeparatorMargin + HorizontalSeparatorHeight + SeparatorMargin + LineHeight + Margin);

                        //  Ensure tooltip is fully visible on screen
                        Rectangle ToolTipLocation = new Rectangle(HoveredSlot.Value.Right, HoveredSlot.Value.Top, TotalWidth, TotalHeight);
                        if (ToolTipLocation.Right > Game1.viewport.Size.Width)
                            ToolTipLocation = new Rectangle(HoveredSlot.Value.Left - TotalWidth, HoveredSlot.Value.Top, TotalWidth, TotalHeight);

                        //  Draw background
                        DrawHelpers.DrawBox(b, ToolTipLocation);

                        //  Draw each row of values
                        int ShadowOffset = 2;
                        int CurrentYPosition = ToolTipLocation.Y + Margin;
                        for (int i = 0; i < Items.Count; i++)
                        {
                            float CurrentXPosition = ToolTipLocation.X + Margin;
                            if (i != 0)
                                DrawHelpers.DrawStringWithShadow(b, Game1.tinyFont, "+", CurrentXPosition, CurrentYPosition, Color.White, Color.Black, ShadowOffset, ShadowOffset);
                            CurrentXPosition += PlusCharacterWidth + SpaceWidth;
                            Utility.drawTinyDigits(SingleValues[i], b, new Vector2(CurrentXPosition + SingleValueColumnWidth - DrawHelpers.MeasureNumber(SingleValues[i], DigitScale), CurrentYPosition), DigitScale, 1.0f, Color.White);
                            CurrentXPosition += SingleValueColumnWidth + SpaceWidth;
                            DrawHelpers.DrawStringWithShadow(b, Game1.tinyFont, "*", CurrentXPosition, CurrentYPosition + LineHeight / 4, Color.White, Color.Black, ShadowOffset, ShadowOffset);
                            CurrentXPosition += MultiplyCharacterWidth + SpaceWidth;
                            Utility.drawTinyDigits(Quantities[i], b, new Vector2(CurrentXPosition + QuantityColumnWidth - DrawHelpers.MeasureNumber(Quantities[i], DigitScale), CurrentYPosition), DigitScale, 1.0f, Color.White);
                            CurrentXPosition += QuantityColumnWidth + SpaceWidth;
                            DrawHelpers.DrawStringWithShadow(b, Game1.tinyFont, "=", CurrentXPosition, CurrentYPosition - LineHeight / 6, Color.White, Color.Black, ShadowOffset, ShadowOffset);
                            CurrentXPosition += EqualsCharacterWidth + SpaceWidth;
                            Utility.drawTinyDigits(MultipliedValues[i], b, new Vector2(CurrentXPosition + MultipliedValueColumnWidth - DrawHelpers.MeasureNumber(MultipliedValues[i], DigitScale), CurrentYPosition), DigitScale, 1.0f, Color.White);

                            CurrentYPosition += LineHeight;
                        }

                        //  Draw separator
                        CurrentYPosition += SeparatorMargin;
                        DrawHelpers.DrawHorizontalSeparator(b, ToolTipLocation.X + Margin, CurrentYPosition, TotalWidth - Margin * 2, HorizontalSeparatorHeight);
                        CurrentYPosition += HorizontalSeparatorHeight + SeparatorMargin;

                        //  Draw total value
                        int SummedValue = MultipliedValues.Sum();
                        float SummedValueWidth = DrawHelpers.MeasureNumber(SummedValue, DigitScale) + GoldIconSourceRect.Width * 2f + 8;
                        Vector2 SummedValuePosition = new Vector2(ToolTipLocation.X + ((ToolTipLocation.Width - SummedValueWidth) / 2), CurrentYPosition + 6);
                        Rectangle IconDestination = new Rectangle((int)SummedValuePosition.X, (int)(SummedValuePosition.Y + (DrawHelpers.TinyDigitBaseHeight * DigitScale - GoldIconSourceRect.Height * 2) / 2), GoldIconSourceRect.Width * 2, GoldIconSourceRect.Height * 2);
                        b.Draw(GoldIconTexture, IconDestination, GoldIconSourceRect, Color.White);
                        Utility.drawTinyDigits(SummedValue, b, new Vector2(SummedValuePosition.X + GoldIconSourceRect.Width * 2f + 8, SummedValuePosition.Y), DigitScale, 1.0f, GetValueColor(SummedValue));
                    }
                    else
                    {
                        Object HoveredItem = Placeholders[ItemId][ConvertColumnTypeToObjectQuality(Column.Value)];
                        Rectangle Location;
                        if (IsNavigatingWithGamepad)
                            Location = HoveredSlot.Value; //new Rectangle(HoveredSlot.Value.Right, HoveredSlot.Value.Bottom, 1, 1);
                        else
                            Location = new Rectangle(Game1.getMouseX() - 8, Game1.getMouseY() + 36, 8 + 36, 1);
                        DrawHelpers.DrawToolTipInfo(b, Location, HoveredItem, true, true, true, true, true, true, Bag.MaxStackSize);
                    }
                }
            }
        }

        internal Object GetHoveredItem()
        {
            if (HoveredSlot.HasValue)
            {
                //  Get the hovered Item Id and ColumnType
                string ItemId = null;
                ColumnType? Column = null;
                foreach (var KVP in SlotBounds)
                {
                    foreach (var KVP2 in KVP.Value)
                    {
                        if (KVP2.Value == HoveredSlot.Value)
                        {
                            ItemId = KVP.Key;
                            Column = KVP2.Key;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(ItemId) && Column.HasValue)
                        break;
                }

                if (!string.IsNullOrEmpty(ItemId) && Column.HasValue && Column != ColumnType.RowValue)
                {
                    Object HoveredItem = Placeholders[ItemId][ConvertColumnTypeToObjectQuality(Column.Value)];
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
            SetParent(null);
        }
    }

}
