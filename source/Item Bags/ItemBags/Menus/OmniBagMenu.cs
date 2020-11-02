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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;
using ItemBags.Persistence;
using Microsoft.Xna.Framework.Input;

namespace ItemBags.Menus
{
    public class OmniBagMenu : IBagMenuContent, IGamepadControllable
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
                HoveredItem = GetHoveredBag();
            }
            else
            {
                HoveredItem = null;
            }
        }
        #endregion Lookup Anything Compatibility

        public ItemBagMenu IBM { get; }
        public ItemBag Bag { get { return OmniBag; } }
        public OmniBag OmniBag { get; }
        private void Bag_ContentsChanged(object sender, EventArgs e) { UpdateActualContents(); }
        public bool IsJojaMember { get; }

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

                if (RelativeSlotBounds != null)
                {
                    this.SlotBounds = new ReadOnlyCollection<Rectangle>(RelativeSlotBounds.Select(x => x.GetOffseted(TopLeftScreenPosition)).ToList());
                }
                else
                {
                    this.SlotBounds = null;
                }
            }
        }

        /// <summary>The bounds of each item slot, relative to <see cref="TopLeftScreenPosition"/>. Use <see cref="SlotBounds"/> when rendering to screen space.</summary>
        public ReadOnlyCollection<Rectangle> RelativeSlotBounds { get; private set; }
        /// <summary>The bounds of each item slot</summary>
        public ReadOnlyCollection<Rectangle> SlotBounds { get; private set; }

        public ReadOnlyCollection<ItemBag> Placeholders { get; }
        public List<ItemBag> ActualContents { get; }

        public OmniBagMenu(ItemBagMenu IBM, OmniBag Bag, int Columns, int SlotSize, bool ShowLockedSlots, int Padding)
        {
            this.IBM = IBM;
            this.OmniBag = Bag;
            Bag.OnContentsChanged += Bag_ContentsChanged;

            this.Padding = Padding;

            this.ColumnCount = Columns;
            this.OriginalSlotSize = SlotSize;
            this.SlotSize = SlotSize;
            this.ShowLockedSlots = ShowLockedSlots;

            //  Create a placeholder item for every kind of bag the OmniBag can store
            List<ItemBag> Temp = new List<ItemBag>();
            if (BundleBag.ValidSizes.Any(x => x <= this.Bag.Size))
            {
                ContainerSize PlaceholderSize = BundleBag.ValidSizes.OrderByDescending(x => x).First(x => x <= this.Bag.Size);
                Temp.Add(new BundleBag(PlaceholderSize, false));
            }
            Temp.Add(new Rucksack(Bag.Size, false));
            foreach (BagType BagType in ItemBagsMod.BagConfig.BagTypes)
            {
                if (BagType.SizeSettings.Any(x => x.Size <= this.Bag.Size))
                {
                    ContainerSize PlaceholderSize = BagType.SizeSettings.Select(x => x.Size).OrderByDescending(x => x).First(x => x <= this.Bag.Size);
                    Temp.Add(new BoundedBag(BagType, PlaceholderSize, false));
                }
            }
            this.Placeholders = new ReadOnlyCollection<ItemBag>(Temp);

            this.ActualContents = new List<ItemBag>();
            for (int i = 0; i < Temp.Count; i++)
                ActualContents.Add(null);
            UpdateActualContents();

            SetTopLeft(Point.Zero, false);
            InitializeLayout(1);
        }

        public void UpdateActualContents()
        {
            HashSet<string> ContainedTypeIds = new HashSet<string>(OmniBag.NestedBags.Select(x => x.GetTypeId()));

            for (int i = 0; i < Placeholders.Count; i++)
            {
                string TypeId = Placeholders[i].GetTypeId();
                if (ContainedTypeIds.Contains(TypeId))
                    ActualContents[i] = OmniBag.NestedBags.First(x => x.GetTypeId() == TypeId);
                else
                    ActualContents[i] = null;
            }
        }

        #region Input Handling
        private Rectangle? HoveredSlot = null;

        #region Mouse Handling
        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            if (Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
            {
                Rectangle? PreviouslyHovered = HoveredSlot;

                this.HoveredSlot = null;
                if (SlotBounds != null)
                {
                    foreach (Rectangle Rect in SlotBounds)
                    {
                        if (Rect.Contains(e.NewPosition.ScreenPixels.AsPoint()))
                        {
                            this.HoveredSlot = Rect;
                            this.IsNavigatingWithGamepad = false;
                            break;
                        }
                    }
                }
            }

            UpdateHoveredItem(e);
        }

        public void OnMouseButtonPressed(ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft)
            {
                HandlePrimaryAction(GetHoveredBag());
            }

            if (e.Button == SButton.MouseRight)
            {
                HandleSecondaryAction(GetHoveredBag());
            }
        }

        public void OnMouseButtonReleased(ButtonReleasedEventArgs e)
        {

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
        }

        public Dictionary<NavigationDirection, IGamepadControllable> MenuNeighbors { get; private set; } = new Dictionary<NavigationDirection, IGamepadControllable>();
        public bool TryGetMenuNeighbor(NavigationDirection Direction, out IGamepadControllable Neighbor)
        {
            return MenuNeighbors.TryGetValue(Direction, out Neighbor);
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            return GamepadControls.TryGetSlotNeighbor(SlotBounds, ItemSlot, ColumnCount, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
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
                HoveredSlot = SlotBounds.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
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
                    HandlePrimaryAction(GetHoveredBag());
                }
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.SecondaryAction))
                {
                    HandleSecondaryAction(GetHoveredBag());
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

                }
            }
        }
        #endregion Gamepad support

        private void HandlePrimaryAction(Item TargetItem)
        {
            if (TargetItem is ItemBag TargetBag)
                OmniBag.MoveFromBag(TargetBag, true, IBM.InventorySource, IBM.ActualInventoryCapacity, true);
        }

        private void HandleSecondaryAction(Item TargetItem)
        {
            if (TargetItem is ItemBag TargetBag)
            {
                //IClickableMenu PreviousMenu = this.Bag.PreviousMenu;
                //this.Bag.CloseContents(false, false);
                //TargetBag.OpenContents(InventorySource, ActualInventoryCapacity, PreviousMenu);
                TargetBag.OpenContents(IBM.InventorySource, IBM.ActualInventoryCapacity, this.Bag.ContentsMenu);

                this.HoveredSlot = null;
            }
        }

        #endregion Input Handling

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

            if (e.IsMultipleOf(GamepadControls.Current.NavigationRepeatFrequency) && IsGamepadFocused && IsNavigatingWithGamepad)
            {
                if (!GamepadControls.HandleNavigationButtons(this, null, HoveredSlot))
                    this.IsGamepadFocused = false;
            }
        }

        public void OnClose()
        {
            Bag.OnContentsChanged -= Bag_ContentsChanged;
        }

        public bool CanResize { get; } = true;

        public void InitializeLayout(int ResizeIteration)
        {
            if (OmniBag == null)
                return;

            if (ResizeIteration > 1)
                this.SlotSize = Math.Min(OriginalSlotSize, Math.Max(32, OriginalSlotSize - (ResizeIteration - 1) * 8));

            HoveredSlot = null;

            List<Rectangle> SlotBounds = new List<Rectangle>();

            int CurrentRow = 0;
            int CurrentColumn = 0;

            int TotalSlots = (((Placeholders.Count - 1) / ColumnCount) + 1) * ColumnCount; // make it a perfect square. EX: if 12 columns, and 18 total slots, increase to next multiple of 12... 24
            for (int i = 0; i < TotalSlots; i++)
            {
                if (CurrentColumn == ColumnCount)
                {
                    CurrentRow++;
                    CurrentColumn = 0;
                }

                int X = CurrentColumn * SlotSize;
                int Y = CurrentRow * SlotSize;
                SlotBounds.Add(new Rectangle(Padding + X, Padding + Y, SlotSize, SlotSize));

                CurrentColumn++;
            }

            RelativeSlotBounds = new ReadOnlyCollection<Rectangle>(SlotBounds);

            int TotalWidth = ColumnCount * SlotSize + Padding * 2;
            int TotalHeight = (CurrentRow + 1) * SlotSize + Padding * 2;

            this.RelativeBounds = new Rectangle(0, 0, TotalWidth, TotalHeight);
        }

        public void Draw(SpriteBatch b)
        {
            //b.Draw(TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color.Cyan), Bounds, Color.White);

            //  Draw the backgrounds of each slot
            for (int i = 0; i < SlotBounds.Count; i++)
            {
                if (i < Placeholders.Count)
                    b.Draw(Game1.menuTexture, SlotBounds[i], new Rectangle(128, 128, 64, 64), Color.White);
                else if (ShowLockedSlots)
                    b.Draw(Game1.menuTexture, SlotBounds[i], new Rectangle(64, 896, 64, 64), Color.White);
            }

            //  Draw the items of each slot
            for (int i = 0; i < SlotBounds.Count; i++)
            {
                if (i < Placeholders.Count)
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

                    float IconScale = IsHovered ? 1.25f : 1.0f;
                    if (ActualContents[i] != null)
                    {
                        ItemBag CurrentItem = ActualContents[i];
                        DrawHelpers.DrawItem(b, Destination, CurrentItem, CurrentItem.Stack > 0, true, IconScale, 1f, Color.White, CurrentItem.Stack >= Bag.MaxStackSize ? Color.Red : Color.White);
                    }
                    else
                    {
                        ItemBag CurrentItem = Placeholders[i];
                        DrawHelpers.DrawItem(b, Destination, CurrentItem, CurrentItem.Stack > 0, true, IconScale, 0.35f, Color.White * 0.3f, CurrentItem.Stack >= Bag.MaxStackSize ? Color.Red : Color.White);
                    }
                }
            }
        }

        public void DrawToolTips(SpriteBatch b)
        {
            //  Draw tooltips on the hovered item inside the bag
            if (HoveredSlot.HasValue)
            {
                ItemBag HoveredBag = GetHoveredBag();
                if (HoveredBag == null)
                {
                    int Index = SlotBounds.IndexOf(HoveredSlot.Value);
                    if (Index >= 0 && Index < Placeholders.Count)
                    {
                        HoveredBag = Placeholders[Index];
                    }
                }

                if (HoveredBag != null)
                {
                    Rectangle Location;
                    if (IsNavigatingWithGamepad)
                        Location = HoveredSlot.Value; //new Rectangle(HoveredSlot.Value.Right, HoveredSlot.Value.Bottom, 1, 1);
                    else
                        Location = new Rectangle(Game1.getMouseX() - 8, Game1.getMouseY() + 36, 8 + 36, 1);

                    //if (HoveredBag is Rucksack RS)
                    //{
                    //    int XPos = Location.Right;
                    //    int YPos = Location.Bottom;
                    //    RS.drawTooltip(b, ref XPos, ref YPos, Game1.smallFont, 1f, RS.Description);
                    //}
                    //else
                    //{
                    //    DrawHelpers.DrawToolTipInfo(b, Location, HoveredBag, true, true, true, true, true, Bag.MaxStackSize);
                    //}

                    DrawHelpers.DrawToolTipInfo(b, Location, HoveredBag, true, true, true, true, true, true, Bag.MaxStackSize);
                }
            }
        }

        internal ItemBag GetHoveredBag()
        {
            if (HoveredSlot.HasValue)
            {
                int Index = SlotBounds.IndexOf(HoveredSlot.Value);
                if (Index >= 0 && Index < Placeholders.Count)
                {
                    return ActualContents[Index];
                    //string TypeId = Placeholders[Index].GetTypeId();
                    //return OmniBag.NestedBags.FirstOrDefault(x => x.GetTypeId() == TypeId);
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
