/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StackSplitRedux.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Diagnostics;
using System.Linq;

namespace StackSplitRedux.MenuHandlers
    {
    public abstract class BaseMenuHandler<TMenuType>
        : IMenuHandler where TMenuType : IClickableMenu
        {
        private const float RIGHT_CLICK_POLLING_INTVL = 650f;

        /// <summary>The inventory handler.</summary>
        protected readonly InventoryHandler InvHandler = new();

        /// <summary>Split menu we display for the user to input the desired stack size.</summary>
        protected StackSplitMenu SplitMenu;

        /// <summary>Native game menu this handler is for.</summary>
        protected TMenuType NativeMenu { get; private set; }

        /// <summary>Does this menu have an inventory section.</summary>
        protected bool HasInventory { get; set; } = true;

        /// <summary>Where the player clicked when the split menu was opened.</summary>
        protected Point ClickItemLocation { get; private set; }

        /// <summary>Tracks if the menu is currently open.</summary>
        private bool IsMenuOpen = false;


        /// <summary>Null constructor that currently only does logging if DEBUG</summary>
        public BaseMenuHandler() {
            Log.TraceIfD($"[{nameof(BaseMenuHandler<TMenuType>)}] Instantiated with TMenuType = {typeof(TMenuType)}");
            }

        ~BaseMenuHandler() {
            Log.TraceIfD($"[{nameof(BaseMenuHandler<TMenuType>)}] Finalized for TMenuType = {typeof(TMenuType)}");
            }

        /// <summary>Checks if the menu this handler wraps is open.</summary>
        /// <returns>True if it is open, false otherwise.</returns>
        public virtual bool IsOpen() {
            return this.IsMenuOpen;
            }

        /// <summary>Checks the menu is the correct type.</summary>
        public bool IsCorrectMenuType(IClickableMenu menu) {
            return menu is TMenuType;
            }

        /// <summary>Notifies the handler that its native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public virtual void Open(IClickableMenu menu) {
            Debug.Assert(IsCorrectMenuType(menu));
            this.NativeMenu = menu as TMenuType;
            this.IsMenuOpen = true;

            if (this.HasInventory)
                InitInventory();
            }

        /// <summary>Notifies the handler that its native menu was closed.</summary>
        public virtual void Close() {
            this.IsMenuOpen = false;
            this.SplitMenu = null;
            }

        /// <summary>Runs on tick for handling things like highlighting text.</summary>
        public virtual void Update() {
            if (Game1.mouseClickPolling < GetRightClickPollingInterval()) {
                this.SplitMenu?.Update();
                }
            else if (this.SplitMenu != null) {
                // Close the menu if the interval is reached as the player likely wants its regular behavior
                CancelMove();
                }
            }

        /// <summary>Tells the handler to close the split menu.</summary>
        public virtual void CloseSplitMenu() {
            this.SplitMenu?.Close();
            this.SplitMenu = null;
            }

        /// <summary>Draws the split menu.</summary>
        public virtual void Draw(SpriteBatch spriteBatch) {
            this.SplitMenu?.Draw(spriteBatch);
            }

        /// <summary>Handle user input.</summary>
        /// <param name="button">The pressed button.</param>
        public EInputHandled HandleInput(SButton button) {
            string pfx = $"[{nameof(BaseMenuHandler<TMenuType>)}.{nameof(HandleInput)}]";

            // Was right click pressed
            if (button == SButton.MouseRight) {
                // Invoke split menu if the modifier key was also down
                if (IsModifierKeyDown() && CanOpenSplitMenu()) {
                    // Cancel the current operation
                    if (this.SplitMenu != null) {
                        // TODO: return this value if it's consumed?
                        CancelMove();
                        }

                    // Starting SDV 1.5, there are two coordinate systems on-screen: "UI Mode" and "Non UI Mode". This is because now the UI can be zoomed separately
                    // from the game world. As a result, grabbing mouse coordinates may cause problems if UI Zoom is not the same as Gameworld Zoom.
                    // For some reasons, if not explicitly specified with "true" below, get(Old)?Mouse(X|Y) methods here will retrieve the "Non UI Mode" coordinate, passing that to
                    // the native code causing the native code to 'hit' on the wrong item when doing the stack-splitting. As a result, the pointed-to item gets deducted,
                    // but the transferred item will be a different item altogether, and if the item was the last item, it's possible that the transferred item will be
                    // 'null', causing the player to lose the original item.
                    // For more information:
                    //   1) https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.5#UI_scale_changes
                    //   2) https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#UI_scaling

                    // Store where the player clicked to pass to the native code after the split menu has been submitted so it remains the same even if the mouse moved.
                    // Patcher note:
                    this.ClickItemLocation = new Point(Game1.getOldMouseX(true), Game1.getOldMouseY(true));
                    Log.TraceIfD($"{pfx}.ClickItemLocation = {this.ClickItemLocation}");

                    // Notify the handler the inventory was clicked.
                    if (this.HasInventory) {
                        if (!this.InvHandler.Initialized)
                            Log.Trace($"{pfx} Handler has inventory but inventory isn't initialized.");
                        else {
                            if (this.InvHandler.WasClicked(Game1.getMouseX(true), Game1.getMouseY(true))) {
                                Log.TraceIfD($"{pfx} Jumping to InventoryClicked");
                                return InventoryClicked();
                                }
                            }
                        }

                    Log.TraceIfD($"{pfx} Jumping to OpenSplitMenu");
                    return OpenSplitMenu();
                    }
                return EInputHandled.NotHandled;
                }
            else if (button == SButton.MouseLeft) {
                var mX = Game1.getMouseX(true);
                var mY = Game1.getMouseY(true);
                // If the player clicks within the bounds of the tooltip then forward the input to that. 
                // Otherwise they're clicking elsewhere and we should close the tooltip.
                if (this.SplitMenu != null && this.SplitMenu.ContainsPoint(mX, mY)) {
                    this.SplitMenu.ReceiveLeftClick(mX, mY);
                    return EInputHandled.Consumed;
                    }

                var handled = HandleLeftClick();
                if (handled == EInputHandled.NotHandled && this.SplitMenu != null) {
                    // Lost focus; cancel the move (run default behavior)
                    return CancelMove();
                    }
                return handled;
                }
            else if (ShouldConsumeKeyboardInput(button))
                return EInputHandled.Handled;

            return EInputHandled.NotHandled;
            }


        /// <summary>Allows derived classes to handle left clicks when they are not focused on the split menu.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled HandleLeftClick() {
            return EInputHandled.NotHandled;
            }

        /// <summary>Whether we should consume the input, preventing it from reaching the game.</summary>
        /// <param name="keyPressed">The key that was pressed.</param>
        /// <returns>True if it should be consumed, false otherwise.</returns>
        protected virtual bool ShouldConsumeKeyboardInput(SButton keyPressed) {
            return this.SplitMenu != null;
            }

        /// <summary>How long the right click has to be held for before the receiveRIghtClick gets called rapidly (See Game1.Update)</summary>
        /// <returns>The polling interval.</returns>
        protected virtual float GetRightClickPollingInterval() {
            return RIGHT_CLICK_POLLING_INTVL;
            }

        /// <summary>Allows derived handlers to provide additional checks before opening the split menu.</summary>
        /// <returns>True if it can be opened.</returns>
        protected virtual bool CanOpenSplitMenu() {
            return true;
            }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected abstract EInputHandled OpenSplitMenu();

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled InventoryClicked() {
            Debug.Assert(this.HasInventory);
            return OpenSplitMenu();
            }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled CancelMove() {
            CloseSplitMenu();

            if (this.HasInventory && this.InvHandler.Initialized)
                this.InvHandler.CancelSplit();

            return EInputHandled.NotHandled;
            }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected virtual void OnStackAmountReceived(string s) {
            CloseSplitMenu();
            }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        protected virtual void InitInventory() {
            if (!this.HasInventory)
                return;

            try {
                // Have to use Reflection here because IClickableMenu does not define .inventory nor .hoveredItem;
                // rather, they are defined per-class by subclasses that implement IClickableMenu.
                // Our wrappers are designed so that if .HasInventory == true, then the native classes DO have .inventory & .hoveredItem
                // (known by inspecting them class defs in the game code).
                // Since there is no Interface defined in the game code that guarantees the existence of .inventory & .hoveredItem,
                // we either have to cast them explicitly, or just use Reflection to retrieve.
                var inventoryMenu = Mod.Reflection.GetField<IClickableMenu>(this.NativeMenu, "inventory").GetValue() as InventoryMenu;
                var hoveredItemField = Mod.Reflection.GetField<Item>(this.NativeMenu, "hoveredItem");

                this.InvHandler.Init(inventoryMenu, hoveredItemField);
                }
            catch (Exception e) {
                Log.Error($"[{nameof(BaseMenuHandler<TMenuType>)}.{nameof(InitInventory)}] Failed to initialize the inventory handler. Exception:\n{e}");
                }
            }

        protected bool IsModifierKeyDown() {
            return StaticConfig.ModifierKeys.Any(s => Mod.Input.IsDown(s));
            //return Mod.Input.IsDown(SButton.LeftAlt) || Mod.Input.IsDown(SButton.LeftShift);
            }
        }
    }
