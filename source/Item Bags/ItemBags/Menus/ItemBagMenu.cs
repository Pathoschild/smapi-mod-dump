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
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;
using static ItemBags.Helpers.DrawHelpers;
using ItemBags.Persistence;
using Microsoft.Xna.Framework.Input;

namespace ItemBags.Menus
{
    public class ItemBagMenu : IClickableMenu, IGamepadControllable
    {
        #region Lookup Anything Compatibility
        /// <summary>
        /// Warning - do not remove/rename this field. It is used via reflection by Lookup Anything mod.<para/>
        /// See also: <see cref="https://github.com/Pathoschild/StardewMods/tree/develop/LookupAnything#extensibility-for-modders"/>
        /// </summary>
        public Item HoveredItem = null;
        protected void UpdateHoveredItem(CursorMovedEventArgs e)
        {
            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                HoveredItem = CustomizeIconMenu.HoveredObject as Item;
            }
            else if (InventoryMenu.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
            {
                HoveredItem = InventoryMenu.HoveredItem;
            }
            else if (Content.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
            {
                HoveredItem = Content.HoveredItem;
            }
            else
            {
                HoveredItem = null;
            }
        }
        #endregion Lookup Anything Compatibility

        /// <summary>The number of frames to wait before repeatedly transferring items while the mouse right button is held</summary>
        public const int TransferRepeatFrequency = 4;

        public ItemBag Bag { get; }
        public IList<Item> InventorySource { get; }
        public int ActualInventoryCapacity { get; }

        public Color BorderColor { get; }
        public Color BackgroundColor { get; }

        protected Texture2D White { get { return TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color.White); } }

        private BagInventoryMenu InventoryMenu { get; }

        private IBagMenuContent _Content;
        public IBagMenuContent Content
        {
            get { return _Content; }
            set
            {
                if (Content != value)
                {
                    _Content = value;
                    InitializeLayout();

                    if (Content == null)
                    {
                        InventoryMenu.MenuNeighbors.Remove(NavigationDirection.Up);
                    }
                    else
                    {
                        InventoryMenu.MenuNeighbors[NavigationDirection.Up] = Content;
                        Content.MenuNeighbors[NavigationDirection.Down] = InventoryMenu;
                        //Content.MenuNeighbors[NavigationDirection.Left] = this;
                        //Content.MenuNeighbors[NavigationDirection.Right] = this;
                    }
                }
            }
        }

        #region Sidebar
        private bool IsLeftSidebarVisible { get; }
        private bool IsRightSidebarVisible { get; }

        private enum SidebarButton
        {
            DepositAll,
            WithdrawAll,
            Autoloot,
            HelpInfo,
            CustomizeIcon
        }
        private SidebarButton? HoveredButton { get; set; } = null;
        public bool IsHoveringAutofillButton { get { return HoveredButton.HasValue && HoveredButton.Value == SidebarButton.Autoloot; } }

        private Rectangle DepositAllBounds { get; set; }
        private Rectangle WithdrawAllBounds { get; set; }
        private Rectangle AutolootBounds { get; set; }
        private Rectangle HelpInfoBounds { get; set; }
        private Rectangle CustomizeIconBounds { get; set; }
        
        private List<Rectangle> LeftSidebarButtonBounds { get { return new List<Rectangle>() { DepositAllBounds, WithdrawAllBounds, AutolootBounds }; } }
        private List<Rectangle> RightSidebarButtonBounds { get { return new List<Rectangle>() { HelpInfoBounds, CustomizeIconBounds }; } }

        private Rectangle? HoveredButtonBounds
        {
            get
            {
                if (HoveredButton.HasValue)
                {
                    if (HoveredButton.Value == SidebarButton.DepositAll)
                        return DepositAllBounds;
                    else if (HoveredButton.Value == SidebarButton.WithdrawAll)
                        return WithdrawAllBounds;
                    else if (HoveredButton.Value == SidebarButton.Autoloot)
                        return AutolootBounds;
                    else if (HoveredButton.Value == SidebarButton.HelpInfo)
                        return HelpInfoBounds;
                    else if (HoveredButton.Value == SidebarButton.CustomizeIcon)
                        return CustomizeIconBounds;
                    else
                        throw new NotImplementedException();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                    HoveredButton = null;
                else if (value == DepositAllBounds)
                    HoveredButton = SidebarButton.DepositAll;
                else if (value == WithdrawAllBounds)
                    HoveredButton = SidebarButton.WithdrawAll;
                else if (value == AutolootBounds)
                    HoveredButton = SidebarButton.Autoloot;
                else if (value == HelpInfoBounds)
                    HoveredButton = SidebarButton.HelpInfo;
                else if (value == CustomizeIconBounds)
                    HoveredButton = SidebarButton.CustomizeIcon;
                else
                    throw new NotImplementedException();
            }
        }
        #endregion Sidebar

        public CustomizeIconMenu CustomizeIconMenu { get; private set; }
        /// <summary>True if this <see cref="ItemBagMenu"/> is displaying a blocking child menu on top of it. Subclasses of <see cref="ItemBagMenu"/> 
        /// will not be able to handle user input when <see cref="IsShowingModalMenu"/> is true. All input will be directed to the Modal dialog.</summary>
        public bool IsShowingModalMenu { get { return CustomizeIconMenu != null; } }

        public void CloseModalMenu()
        {
            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                CustomizeIconMenu.OnClose();
                HoveredButton = null;
                CustomizeIconMenu = null;
            }
        }

        /// <param name="InventorySource">Typically this is <see cref="Game1.player.Items"/> if this menu should display the player's inventory.</param>
        /// <param name="ActualCapacity">The maximum # of items that can be stored in the Source list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        /// <param name="InventoryColumns">The number of columns to use when rendering the user's inventory at the bottom-half of the menu. Recommended = 12 to mimic the default inventory of the main GameMenu</param>
        /// <param name="InventorySlotSize">The size, in pixels, to use when rendering each slot of the user's inventory at the bottom-half of the menu. Recommended = <see cref="BagInventoryMenu.DefaultInventoryIconSize"/></param>
        public ItemBagMenu(ItemBag Bag, IList<Item> InventorySource, int ActualCapacity, int InventoryColumns, int InventorySlotSize = BagInventoryMenu.DefaultInventoryIconSize)
            : base(1, 1, 1, 1, true)
        {
            this.BorderColor = new Color(220, 123, 5, 255);
            this.BackgroundColor = new Color(255, 201, 121);

            this.Bag = Bag;
            this.IsLeftSidebarVisible = Bag is BoundedBag || Bag is Rucksack;
            this.IsRightSidebarVisible = true;

            this.InventorySource = InventorySource;
            this.ActualInventoryCapacity = Math.Max(ActualCapacity, InventorySource.Count);
            this.InventoryMenu = new BagInventoryMenu(this, Bag, InventorySource, ActualCapacity, InventoryColumns, InventorySlotSize);
            InventoryMenu.MenuNeighbors[NavigationDirection.Left] = this;
            InventoryMenu.MenuNeighbors[NavigationDirection.Right] = this;

            this.exitFunction += () => { Bag.CloseContents(); };
        }

        public ItemBagMenu(ItemBag Bag, IList<Item> InventorySource, int ActualCapacity, BagMenuOptions Options)
            : this(Bag, InventorySource, ActualCapacity, Options.InventoryColumns, Options.InventorySlotSize) { }

        internal void OnWindowSizeChanged()
        {
            InitializeLayout();
            if (IsShowingModalMenu && CustomizeIconMenu != null)
                CustomizeIconMenu.InitializeLayout(1);
        }

        private const int InventoryMargin = 12;
        public const int ContentsMargin = 12;

        public const int ButtonLeftTopMargin = 4;
        public const int ButtonBottomMargin = 6;
        public static int ButtonSize { get { return 32; } } //{ get { return Constants.TargetPlatform == GamePlatform.Android ? 48 : 32; } }

        private static double DrawingScaleFactor { get { return 1.0 / Game1.options.uiScale * 1.0 * Game1.options.zoomLevel; } }

        private int ResizeIteration { get; set; } = 0;
        private void InitializeLayout()
        {
            ResizeIteration = 0;
            int PreviousWidth = -1;
            int PreviousHeight = -1;

            bool AttemptResize;
            do
            {
                ResizeIteration++;

                int SidebarWidth = 0;
                int SidebarHeight = 0;
                if (IsLeftSidebarVisible || IsRightSidebarVisible)
                {
                    SidebarWidth = ButtonSize + ButtonLeftTopMargin * 2;

                    int LeftButtons = LeftSidebarButtonBounds.Count();
                    int LeftHeight = InventoryMargin + ButtonLeftTopMargin + LeftButtons * ButtonSize + (LeftButtons - 1) * ButtonBottomMargin + ButtonLeftTopMargin + InventoryMargin;
                    int RightButtons = RightSidebarButtonBounds.Count();
                    int RightHeight = InventoryMargin + ButtonLeftTopMargin + RightButtons * ButtonSize + (RightButtons - 1) * ButtonBottomMargin + ButtonLeftTopMargin + InventoryMargin;
                    SidebarHeight = Math.Max(IsLeftSidebarVisible ? LeftHeight : 0, IsRightSidebarVisible ? RightHeight : 0);
                }

                InventoryMenu.InitializeLayout(ResizeIteration);
                Content.InitializeLayout(ResizeIteration);

                //  Compute size of menu
                int InventoryWidth = InventoryMenu.RelativeBounds.Width + InventoryMargin * 2 + SidebarWidth * 2;
                int ContentsWidth = Content.RelativeBounds.Width + ContentsMargin * 2;
                width = Math.Max(InventoryWidth, ContentsWidth);
                bool IsWidthBoundToContents = ContentsWidth > InventoryWidth;
                height = Math.Max(InventoryMenu.RelativeBounds.Height + InventoryMargin * 2, SidebarHeight) + Math.Max(Content.RelativeBounds.Height + ContentsMargin * 2, 0);
                xPositionOnScreen = (int)((Game1.viewport.Size.Width * DrawingScaleFactor - width) / 2);
                yPositionOnScreen = (int)((Game1.viewport.Size.Height * DrawingScaleFactor - height) / 2);

                //  Check if menu fits on screen
                bool IsMenuTooWide = width > Game1.viewport.Size.Width * DrawingScaleFactor;
                bool IsMenuTooTall = height > Game1.viewport.Size.Height * DrawingScaleFactor;
                bool FitsOnScreen = !IsMenuTooWide && !IsMenuTooTall;
                bool DidSizeChange = width != PreviousWidth || height != PreviousHeight;
                PreviousWidth = width;
                PreviousHeight = height;

                AttemptResize = !FitsOnScreen && ResizeIteration < 5 && Content.CanResize && DidSizeChange && (IsWidthBoundToContents || IsMenuTooTall);
            } while (AttemptResize);

            //  Set position of inventory and contents
            InventoryMenu.SetTopLeft(new Point(
                xPositionOnScreen + (width - InventoryMenu.RelativeBounds.Width) / 2,
                yPositionOnScreen + Content.RelativeBounds.Height + ContentsMargin * 2 + InventoryMargin)
            );
            Content.SetTopLeft(new Point(
                xPositionOnScreen + (width - Content.RelativeBounds.Width) / 2,
                //BagInfo.Bounds.Right - ContentsMargin + (width - BagInfo.Bounds.Width - ContentsMargin - GetRelativeContentBounds().Width) / 2,
                yPositionOnScreen + ContentsMargin));

            //  Set bounds of sidebar buttons
            if (IsLeftSidebarVisible)
            {
                DepositAllBounds = new Rectangle(xPositionOnScreen + ContentsMargin + ButtonLeftTopMargin, InventoryMenu.Bounds.Top + ButtonLeftTopMargin, ButtonSize, ButtonSize);
                WithdrawAllBounds = new Rectangle(xPositionOnScreen + ContentsMargin + ButtonLeftTopMargin, InventoryMenu.Bounds.Top + ButtonLeftTopMargin + ButtonSize + ButtonBottomMargin, ButtonSize, ButtonSize);
                AutolootBounds = new Rectangle(xPositionOnScreen + ContentsMargin + ButtonLeftTopMargin, InventoryMenu.Bounds.Top + ButtonLeftTopMargin + ButtonSize * 2 + ButtonBottomMargin * 2, ButtonSize, ButtonSize);
            }
            if (IsRightSidebarVisible)
            {
                HelpInfoBounds = new Rectangle(xPositionOnScreen + width - ContentsMargin - ButtonLeftTopMargin - ButtonSize, InventoryMenu.Bounds.Top + ButtonLeftTopMargin, ButtonSize, ButtonSize);
                CustomizeIconBounds = new Rectangle(xPositionOnScreen + width - ContentsMargin - ButtonLeftTopMargin - ButtonSize, InventoryMenu.Bounds.Top + ButtonLeftTopMargin + ButtonSize + ButtonBottomMargin, ButtonSize, ButtonSize);
            }

            //  Set bounds of close button
            Point CloseButtonOffset = Constants.TargetPlatform == GamePlatform.Android ? new Point(24, -24) : new Point(16, -16);
            upperRightCloseButton.bounds.X = xPositionOnScreen + width - upperRightCloseButton.bounds.Width + CloseButtonOffset.X;
            upperRightCloseButton.bounds.Y = yPositionOnScreen + CloseButtonOffset.Y;
        }

        #region Disable Close Button Focusing
        //  Override these function to prevent the game from auto-selecting the close button in the top-right any time a gamepad key is pressed
        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            //base.automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            //base.setCurrentlySnappedComponentTo(id);
        }

        public override void applyMovementKey(int direction)
        {
            //base.applyMovementKey(direction);
        }
        #endregion Disable Close Button Focusing

        /*//  Doesn't work - this is a bug within the game, not my code. Pressing the 'B' button on my Xbox One controller is sending Keys.E to receiveKeyPress. 
        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.gamepadControls || (key.ToSButton().TryGetController(out Buttons b) && GamepadControls.IsMatch(b, GamepadControls.CloseBag)))
                return; // Let this button press be handled by our custom OnGamepadButtonsPressed logic
            else
                base.receiveKeyPress(key);
        }*/

        #region Input Handling

        #region Mouse Handling
        public void OnMouseMoved(CursorMovedEventArgs e)
        {
            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                CustomizeIconMenu.OnMouseMoved(e);
            }
            else
            {
                if (InventoryMenu.Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || InventoryMenu.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
                {
                    InventoryMenu.OnMouseMoved(e);
                }

                if (Content.Bounds.Contains(e.OldPosition.ScreenPixels.AsPoint()) || Content.Bounds.Contains(e.NewPosition.ScreenPixels.AsPoint()))
                {
                    Content.OnMouseMoved(e);
                }

                if (IsLeftSidebarVisible || IsRightSidebarVisible)
                {
                    Point OldPos = e.OldPosition.ScreenPixels.AsPoint();
                    Point NewPos = e.NewPosition.ScreenPixels.AsPoint();

                    if (LeftSidebarButtonBounds.Any(x => x.Contains(OldPos) || x.Contains(NewPos)) ||
                        RightSidebarButtonBounds.Any(x => x.Contains(OldPos) || x.Contains(NewPos)))
                    {
                        if (IsLeftSidebarVisible && DepositAllBounds.Contains(NewPos))
                            this.HoveredButton = SidebarButton.DepositAll;
                        else if (IsLeftSidebarVisible && WithdrawAllBounds.Contains(NewPos))
                            this.HoveredButton = SidebarButton.WithdrawAll;
                        else if (IsLeftSidebarVisible && AutolootBounds.Contains(NewPos))
                            this.HoveredButton = SidebarButton.Autoloot;
                        else if (IsRightSidebarVisible && HelpInfoBounds.Contains(NewPos))
                            this.HoveredButton = SidebarButton.HelpInfo;
                        else if (IsRightSidebarVisible && !(Bag is BundleBag) && CustomizeIconBounds.Contains(NewPos))
                            this.HoveredButton = SidebarButton.CustomizeIcon;
                        else
                            this.HoveredButton = null;

                        if (HoveredButton != null)
                            this.IsNavigatingWithGamepad = false;
                    }
                }
            }

            UpdateHoveredItem(e);
        }

        public void OnMouseButtonPressed(ButtonPressedEventArgs e)
        {
            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                CustomizeIconMenu.OnMouseButtonPressed(e);
            }
            else
            {
                InventoryMenu.OnMouseButtonPressed(e);
                Content.OnMouseButtonPressed(e);

                if (e.Button == SButton.MouseLeft)
                    HandlePrimaryAction();
                if (e.Button == SButton.MouseRight)
                    HandleSecondaryAction();
            }
        }

        public void OnMouseButtonReleased(ButtonReleasedEventArgs e)
        {
            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                CustomizeIconMenu.OnMouseButtonReleased(e);
            }
            else
            {
                InventoryMenu.OnMouseButtonReleased(e);
                Content.OnMouseButtonReleased(e);
            }
        }
        #endregion Mouse Handling

        public bool IsTransferMultipleModifierHeld { get; private set; }
        public bool IsTransferHalfModifierHeld { get; private set; }

        public void OnModifierKeyPressed(ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.LeftShift || e.Button == SButton.RightShift)
            {
                IsTransferMultipleModifierHeld = true;
            }
            if (e.Button == SButton.LeftControl || e.Button == SButton.RightControl)
            {
                IsTransferHalfModifierHeld = true;
            }
        }

        public void OnModifierKeyReleased(ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.LeftShift || e.Button == SButton.RightShift)
            {
                IsTransferMultipleModifierHeld = false;
            }
            if (e.Button == SButton.LeftControl || e.Button == SButton.RightControl)
            {
                IsTransferHalfModifierHeld = false;
            }
        }

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
            IsNavigatingWithGamepad = true;
        }

        public void LostGamepadFocus()
        {
            HoveredButton = null;
        }

        public Dictionary<NavigationDirection, IGamepadControllable> MenuNeighbors { get; private set; } = new Dictionary<NavigationDirection, IGamepadControllable>();
        public bool TryGetMenuNeighbor(NavigationDirection Direction, out IGamepadControllable Neighbor)
        {
            if (HoveredButton.HasValue)
            {
                if (IsLeftSidebarVisible && LeftSidebarButtonBounds.Contains(HoveredButtonBounds.Value) && Direction == NavigationDirection.Right)
                {
                    Neighbor = InventoryMenu;
                    return true;
                }
                else if (IsRightSidebarVisible && RightSidebarButtonBounds.Contains(HoveredButtonBounds.Value) && Direction == NavigationDirection.Left)
                {
                    Neighbor = InventoryMenu;
                    return true;
                }
            }

            if (Direction == NavigationDirection.Up)
            {
                Neighbor = Content;
                return true;
            }

            Neighbor = null;
            return false;
        }

        public bool TryGetSlotNeighbor(Rectangle? ItemSlot, NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping, out Rectangle? Neighbor)
        {
            if (!ItemSlot.HasValue)
            {
                Neighbor = null;
                return false;
            }
            else if (IsLeftSidebarVisible && LeftSidebarButtonBounds.Contains(ItemSlot.Value))
            {
                return GamepadControls.TryGetSlotNeighbor(LeftSidebarButtonBounds, ItemSlot, 1, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
            }
            else if (IsRightSidebarVisible && RightSidebarButtonBounds.Contains(ItemSlot.Value))
            {
                return GamepadControls.TryGetSlotNeighbor(RightSidebarButtonBounds, ItemSlot, 1, Direction, HorizontalWrapping, VerticalWrapping, out Neighbor);
            }
            else
            {
                Neighbor = null;
                return false;
            }
        }

        public bool TryNavigate(NavigationDirection Direction, NavigationWrappingMode HorizontalWrapping, NavigationWrappingMode VerticalWrapping)
        {
            if (IsGamepadFocused && HoveredButton == null)
            {
                if (IsLeftSidebarVisible)
                    HoveredButtonBounds = LeftSidebarButtonBounds.First();
                else if (IsRightSidebarVisible)
                    HoveredButtonBounds = RightSidebarButtonBounds.First();
                else
                    HoveredButtonBounds = null;
                IsNavigatingWithGamepad = HoveredButton != null;
                return HoveredButton != null;
            }
            else if (TryGetSlotNeighbor(HoveredButtonBounds, Direction, HorizontalWrapping, VerticalWrapping, out Rectangle? Neighbor))
            {
                HoveredButtonBounds = Neighbor.Value;
                IsNavigatingWithGamepad = true;
                return true;
            }
            else
                return false;
        }

        public bool TryNavigateEnter(NavigationDirection StartingSide, Rectangle? ClosestTo)
        {
            if (IsLeftSidebarVisible && StartingSide != NavigationDirection.Left)
            {
                IsGamepadFocused = true;
                IsNavigatingWithGamepad = true;
                if (ClosestTo.HasValue)
                    HoveredButtonBounds = LeftSidebarButtonBounds.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
                else
                    HoveredButtonBounds = LeftSidebarButtonBounds.First();
                return true;
            }
            else if (IsRightSidebarVisible)
            {
                IsGamepadFocused = true;
                IsNavigatingWithGamepad = true;
                if (ClosestTo.HasValue)
                    HoveredButtonBounds = RightSidebarButtonBounds.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
                else
                    HoveredButtonBounds = RightSidebarButtonBounds.First();
                return true;
            }
            else if (IsLeftSidebarVisible)
            {
                IsGamepadFocused = true;
                IsNavigatingWithGamepad = true;
                if (ClosestTo.HasValue)
                    HoveredButtonBounds = LeftSidebarButtonBounds.OrderBy(x => x.SquaredDistanceBetweenCenters(ClosestTo.Value)).First();
                else
                    HoveredButtonBounds = LeftSidebarButtonBounds.First();
                return true;
            }
            else
            {
                HoveredButtonBounds = null;
                return false;
            }
        }

        public bool IsNavigatingWithGamepad { get; private set; }

        public void OnGamepadButtonsPressed(Buttons GamepadButtons)
        {
            //  Handle closing the menu
            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.CloseBag))
            {
                if (IsShowingModalMenu && CustomizeIconBounds != null)
                    CloseModalMenu();
                else
                    Bag.CloseContents();
                return;
            }

            //  Handle modifier buttons
            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.TransferMultipleModifier))
            {
                IsTransferMultipleModifierHeld = true;
            }
            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.TransferHalfModifier))
            {
                IsTransferHalfModifierHeld = true;
            }

            if (!IsGamepadFocused && !InventoryMenu.IsGamepadFocused && !Content.IsGamepadFocused)
                InventoryMenu.IsGamepadFocused = true;

            if (IsGamepadFocused && !RecentlyGainedFocus)
            {
                if (!GamepadControls.HandleNavigationButtons(this, GamepadButtons, HoveredButtonBounds))
                    this.IsGamepadFocused = false;

                //  Handle action buttons
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.PrimaryAction))
                {
                    HandlePrimaryAction();
                }
                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.SecondaryAction))
                {
                    HandleSecondaryAction();
                }
            }

            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                //TODO
            }

            InventoryMenu.OnGamepadButtonsPressed(GamepadButtons);
            Content.OnGamepadButtonsPressed(GamepadButtons);
        }

        public void OnGamepadButtonsReleased(Buttons GamepadButtons)
        {
            //  Handle modifier buttons
            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.TransferMultipleModifier))
            {
                IsTransferMultipleModifierHeld = false;
            }
            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.TransferHalfModifier))
            {
                IsTransferHalfModifierHeld = false;
            }

            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                //TODO
            }

            if (IsGamepadFocused && !RecentlyGainedFocus)
            {

            }

            InventoryMenu.OnGamepadButtonsReleased(GamepadButtons);
            Content.OnGamepadButtonsReleased(GamepadButtons);
        }
        #endregion Gamepad support

        private void HandlePrimaryAction()
        {
            if ((IsLeftSidebarVisible || IsRightSidebarVisible) && HoveredButton.HasValue)
            {
                if (IsLeftSidebarVisible && HoveredButton.Value == SidebarButton.DepositAll)
                {
                    List<Object> ToDeposit = InventorySource.Where(x => x != null && x is Object Obj && Bag.IsValidBagObject(Obj)).Cast<Object>().ToList();
                    Bag.MoveToBag(ToDeposit, ToDeposit.Select(x => x.Stack).ToList(), out int TotalMovedQty, true, InventorySource);
                }
                else if (IsLeftSidebarVisible && HoveredButton.Value == SidebarButton.WithdrawAll)
                {
                    List<Object> ToWithdraw = Bag.Contents.Where(x => x != null).ToList();
                    Bag.MoveFromBag(ToWithdraw, ToWithdraw.Select(x => x.Stack).ToList(), out int TotalMovedQty, true, InventorySource, ActualInventoryCapacity);
                }
                else if (IsLeftSidebarVisible && HoveredButton.Value == SidebarButton.Autoloot)
                {
                    if (Bag is BoundedBag BB)
                        BB.Autofill = !BB.Autofill;
                    else if (Bag is Rucksack RS)
                        RS.CycleAutofill();
                }
                else if (IsRightSidebarVisible && HoveredButton.Value == SidebarButton.HelpInfo)
                {

                }
                else if (IsRightSidebarVisible && HoveredButton.Value == SidebarButton.CustomizeIcon && Bag.CanCustomizeIcon())
                {
                    ItemBag Copy;
                    if (Bag is BoundedBag BB)
                    {
                        if (BB is BundleBag)
                            Copy = new BundleBag(Bag.Size, false);
                        else
                            Copy = new BoundedBag(BB.TypeInfo, Bag.Size, false);
                    }
                    else if (Bag is Rucksack RS)
                    {
                        Copy = new Rucksack(Bag.Size, false);
                    }
                    else if (Bag is OmniBag OB)
                    {
                        Copy = new OmniBag(Bag.Size);
                    }
                    else
                        throw new NotImplementedException(string.Format("Unexpected Bag Type while creating CustomizeIconMenu: {0}", Bag.GetType().ToString()));

                    CustomizeIconMenu = new CustomizeIconMenu(this, this.Bag, Copy, 24);
                }
            }
        }

        private void HandleSecondaryAction()
        {

        }
        #endregion Input Handling

        public void Update(UpdateTickedEventArgs e)
        {
            RecentlyGainedFocus = false;

            if (IsShowingModalMenu && CustomizeIconMenu != null)
            {
                CustomizeIconMenu.Update(e);
            }
            else
            {
                if (e.IsMultipleOf(GamepadControls.Current.NavigationRepeatFrequency) && IsGamepadFocused && IsNavigatingWithGamepad)
                {
                    if (!GamepadControls.HandleNavigationButtons(this, null, HoveredButtonBounds))
                        this.IsGamepadFocused = false;
                }

                InventoryMenu.Update(e);
                Content.Update(e);
            }
        }

        public sealed override void draw(SpriteBatch b)
        {
            try
            {
                DrawBox(b, xPositionOnScreen, yPositionOnScreen, width, height);
                int SeparatorHeight = 24;
                DrawHorizontalSeparator(b, xPositionOnScreen, InventoryMenu.TopLeftScreenPosition.Y - InventoryMargin - SeparatorHeight / 2, width, SeparatorHeight);
                InventoryMenu.Draw(b);

                if (IsLeftSidebarVisible || IsRightSidebarVisible)
                {
                    if (IsLeftSidebarVisible)
                    {
                        //  Draw the deposit/withdraw-all buttons
                        Rectangle ArrowUpIconSourceRect = new Rectangle(421, 459, 12, 12);
                        Rectangle ArrowDownIconSourceRect = new Rectangle(421, 472, 12, 12);
                        int ArrowSize = (int)(ArrowUpIconSourceRect.Width * 1.5 / 32.0 * DepositAllBounds.Width);
                        b.Draw(Game1.menuTexture, DepositAllBounds, new Rectangle(128, 128, 64, 64), Color.White);
                        b.Draw(Game1.mouseCursors, new Rectangle(DepositAllBounds.X + (DepositAllBounds.Width - ArrowSize) / 2, DepositAllBounds.Y + (DepositAllBounds.Height - ArrowSize) / 2, ArrowSize, ArrowSize), ArrowUpIconSourceRect, Color.White);
                        b.Draw(Game1.menuTexture, WithdrawAllBounds, new Rectangle(128, 128, 64, 64), Color.White);
                        b.Draw(Game1.mouseCursors, new Rectangle(WithdrawAllBounds.X + (WithdrawAllBounds.Width - ArrowSize) / 2, WithdrawAllBounds.Y + (WithdrawAllBounds.Height - ArrowSize) / 2, ArrowSize, ArrowSize), ArrowDownIconSourceRect, Color.White);

                        //  Draw the autofill togglebutton
                        Rectangle HandIconSourceRect = new Rectangle(32, 0, 10, 10);
                        int HandIconSize = (int)(HandIconSourceRect.Width * 2.0 / 32.0 * AutolootBounds.Width);
                        b.Draw(Game1.menuTexture, AutolootBounds, new Rectangle(128, 128, 64, 64), Color.White);
                        b.Draw(Game1.mouseCursors, new Rectangle(AutolootBounds.X + (AutolootBounds.Width - HandIconSize) / 2, AutolootBounds.Y + (AutolootBounds.Height - HandIconSize) / 2, HandIconSize, HandIconSize), HandIconSourceRect, Color.White);

                        if (Bag is BoundedBag BB)
                        {
                            if (!BB.Autofill)
                            {
                                Rectangle DisabledIconSourceRect = new Rectangle(322, 498, 12, 12);
                                int DisabledIconSize = (int)(DisabledIconSourceRect.Width * 1.5 / 32.0 * AutolootBounds.Width);
                                Rectangle Destination = new Rectangle(AutolootBounds.Right - DisabledIconSize - 2, AutolootBounds.Bottom - DisabledIconSize - 2, DisabledIconSize, DisabledIconSize);
                                b.Draw(Game1.mouseCursors, Destination, DisabledIconSourceRect, Color.White);
                            }
                        }
                        else if (Bag is Rucksack RS)
                        {
                            if (!RS.Autofill)
                            {
                                Rectangle DisabledIconSourceRect = new Rectangle(322, 498, 12, 12);
                                int DisabledIconSize = (int)(DisabledIconSourceRect.Width * 1.5 / 32.0 * AutolootBounds.Width);
                                Rectangle Destination = new Rectangle(AutolootBounds.Right - DisabledIconSize - 2, AutolootBounds.Bottom - DisabledIconSize - 2, DisabledIconSize, DisabledIconSize);
                                b.Draw(Game1.mouseCursors, Destination, DisabledIconSourceRect, Color.White);
                            }
                            else
                            {
                                if (RS.AutofillPriority == AutofillPriority.Low)
                                {
                                    Rectangle LowPriorityIconSourceRect = new Rectangle(421, 472, 12, 12);
                                    int LowPriorityIconSize = (int)(LowPriorityIconSourceRect.Width * 1.0 / 32.0 * AutolootBounds.Width);
                                    Rectangle Destination = new Rectangle(AutolootBounds.Right - LowPriorityIconSize - 2, AutolootBounds.Bottom - LowPriorityIconSize - 2, LowPriorityIconSize, LowPriorityIconSize);
                                    b.Draw(Game1.mouseCursors, Destination, LowPriorityIconSourceRect, Color.White);
                                }
                                else if (RS.AutofillPriority == AutofillPriority.High)
                                {
                                    Rectangle HighPriorityIconSourceRect = new Rectangle(421, 459, 12, 12);
                                    int HighPriorityIconSize = (int)(HighPriorityIconSourceRect.Width * 1.0 / 32.0 * AutolootBounds.Width);
                                    Rectangle Destination = new Rectangle(AutolootBounds.Right - HighPriorityIconSize - 2, AutolootBounds.Bottom - HighPriorityIconSize - 2, HighPriorityIconSize, HighPriorityIconSize);
                                    b.Draw(Game1.mouseCursors, Destination, HighPriorityIconSourceRect, Color.White);
                                }
                            }
                        }
                    }

                    if (IsRightSidebarVisible)
                    {
                        //  Draw the help button
                        Rectangle HelpIconSourceRect = new Rectangle(176, 425, 9, 12);
                        int HelpIconWidth = (int)(HelpIconSourceRect.Width * 1.5 / 32.0 * HelpInfoBounds.Width);
                        int HelpIconHeight = (int)(HelpIconSourceRect.Height * 1.5 / 32.0 * HelpInfoBounds.Height);
                        b.Draw(Game1.menuTexture, HelpInfoBounds, new Rectangle(128, 128, 64, 64), Color.White);
                        b.Draw(Game1.mouseCursors, new Rectangle(HelpInfoBounds.X + (HelpInfoBounds.Width - HelpIconWidth) / 2, HelpInfoBounds.Y + (HelpInfoBounds.Height - HelpIconHeight) / 2, HelpIconWidth, HelpIconHeight), HelpIconSourceRect, Color.White);

                        if (Bag.CanCustomizeIcon())
                        {
                            //  Draw the customize icon button
                            Rectangle CustomizeSourceRect = new Rectangle(121, 471, 12, 12);
                            int CustomizeIconWidth = CustomizeIconBounds.Width;
                            int CustomizeIconHeight = CustomizeIconBounds.Height;
                            b.Draw(Game1.mouseCursors, new Rectangle(CustomizeIconBounds.X + (CustomizeIconBounds.Width - CustomizeIconWidth) / 2, CustomizeIconBounds.Y + (CustomizeIconBounds.Height - CustomizeIconHeight) / 2,
                                CustomizeIconWidth, CustomizeIconHeight), CustomizeSourceRect, Color.White);
                            b.Draw(Game1.menuTexture, CustomizeIconBounds, new Rectangle(128, 128, 64, 64), Color.White);
                        }
                    }

                    //  Draw a yellow border around the hovered sidebar button
                    if (HoveredButton.HasValue)
                    {
                        Rectangle HoveredBounds = HoveredButtonBounds.Value;
                        Color HighlightColor = Color.Yellow;
                        Texture2D Highlight = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, HighlightColor);
                        b.Draw(Highlight, HoveredBounds, Color.White * 0.25f);
                        int BorderThickness = HoveredBounds.Width / 16;
                        DrawBorder(b, HoveredBounds, BorderThickness, HighlightColor);
                    }
                }

                Content.Draw(b);

                if (IsShowingModalMenu && CustomizeIconMenu != null)
                {
                    CustomizeIconMenu.Draw(b);
                }
                else
                {
                    InventoryMenu.DrawToolTips(b);
                    Content.DrawToolTips(b);

                    //  Draw tooltips on the sidebar buttons
                    if ((IsLeftSidebarVisible || IsRightSidebarVisible) && HoveredButton.HasValue)
                    {
                        if (IsLeftSidebarVisible)
                        {
                            string ButtonToolTip = "";
                            if (HoveredButton.Value == SidebarButton.DepositAll)
                                ButtonToolTip = ItemBagsMod.Translate("DepositAllToolTip");
                            else if (HoveredButton.Value == SidebarButton.WithdrawAll)
                                ButtonToolTip = ItemBagsMod.Translate("WithdrawAllToolTip");
                            else if (HoveredButton.Value == SidebarButton.Autoloot)
                            {
                                if (Bag is BoundedBag BB)
                                    ButtonToolTip = ItemBagsMod.Translate(BB.Autofill ? "AutofillOnToolTip" : "AutofillOffToolTip");
                                else if (Bag is Rucksack RS)
                                {
                                    string TranslationKey;
                                    if (RS.Autofill)
                                    {
                                        if (RS.AutofillPriority == AutofillPriority.Low)
                                            TranslationKey = "RucksackAutofillLowPriorityToolTip";
                                        else if (RS.AutofillPriority == AutofillPriority.High)
                                            TranslationKey = "RucksackAutofillHighPriorityToolTip";
                                        else
                                            throw new NotImplementedException(string.Format("Unrecognized Rucksack AutofillPriority: {0}", RS.AutofillPriority.ToString()));
                                    }
                                    else
                                        TranslationKey = "RucksackAutofillOffToolTip";
                                    ButtonToolTip = ItemBagsMod.Translate(TranslationKey);
                                }
                            }

                            if (!string.IsNullOrEmpty(ButtonToolTip))
                            {
                                int Margin = 16;
                                Vector2 ToolTipSize = Game1.smallFont.MeasureString(ButtonToolTip);
                                DrawBox(b, HoveredButtonBounds.Value.Right, HoveredButtonBounds.Value.Top, (int)(ToolTipSize.X + Margin * 2), (int)(ToolTipSize.Y + Margin * 2));
                                b.DrawString(Game1.smallFont, ButtonToolTip, new Vector2(HoveredButtonBounds.Value.Right + Margin, HoveredButtonBounds.Value.Top + Margin), Color.Black);
                            }
                        }

                        if (IsRightSidebarVisible)
                        {
                            string ButtonToolTip = "";
                            if (HoveredButton.Value == SidebarButton.HelpInfo)
                                ButtonToolTip = ItemBagsMod.Translate("HelpInfoToolTip");
                            else if (HoveredButton.Value == SidebarButton.CustomizeIcon)
                                ButtonToolTip = ItemBagsMod.Translate("CustomizeIconToolTip");

                            if (!string.IsNullOrEmpty(ButtonToolTip))
                            {
                                int Margin = 16;
                                Vector2 ToolTipSize = Game1.smallFont.MeasureString(ButtonToolTip);
                                DrawBox(b, HoveredButtonBounds.Value.Left - (int)(ToolTipSize.X + Margin * 2), HoveredButtonBounds.Value.Top, (int)(ToolTipSize.X + Margin * 2), (int)(ToolTipSize.Y + Margin * 2));
                                b.DrawString(Game1.smallFont, ButtonToolTip, new Vector2(HoveredButtonBounds.Value.Left - Margin - ToolTipSize.X, HoveredButtonBounds.Value.Top + Margin), Color.Black);
                            }
                        }
                    }
                }

                upperRightCloseButton.draw(b);

                if (!Game1.options.hardwareCursor && !Game1.options.gamepadControls)
                {
                    drawMouse(b);
                }
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Unhandled error in ItemBagMenu.Draw: {0}", ex.Message), LogLevel.Error);
            }
        }

        internal void OnClose()
        {
            InventoryMenu.OnClose();
            Content.OnClose();
        }
    }
}
