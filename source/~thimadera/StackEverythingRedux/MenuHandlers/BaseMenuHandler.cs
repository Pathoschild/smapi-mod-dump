/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StackEverythingRedux.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace StackEverythingRedux.MenuHandlers
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
        public BaseMenuHandler()
        {
            Log.TraceIfD($"[{nameof(BaseMenuHandler<TMenuType>)}] Instantiated with TMenuType = {typeof(TMenuType)}");
        }

        ~BaseMenuHandler()
        {
            Log.TraceIfD($"[{nameof(BaseMenuHandler<TMenuType>)}] Finalized for TMenuType = {typeof(TMenuType)}");
        }

        /// <summary>Checks if the menu this handler wraps is open.</summary>
        /// <returns>True if it is open, false otherwise.</returns>
        public virtual bool IsOpen()
        {
            return IsMenuOpen;
        }

        /// <summary>Checks the menu is the correct type.</summary>
        public bool IsCorrectMenuType(IClickableMenu menu)
        {
            return menu is TMenuType;
        }

        /// <summary>Notifies the handler that its native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public virtual bool Open(IClickableMenu menu)
        {
            Debug.Assert(IsCorrectMenuType(menu));
            NativeMenu = menu as TMenuType;
            IsMenuOpen = true;

            if (HasInventory)
            {
                InitInventory();
            }

            return true;
        }

        /// <summary>Notifies the handler that its native menu was closed.</summary>
        public virtual void Close()
        {
            IsMenuOpen = false;
            SplitMenu = null;
        }

        /// <summary>Runs on tick for handling things like highlighting text.</summary>
        public virtual void Update()
        {
            if (Game1.mouseClickPolling < GetRightClickPollingInterval())
            {
                SplitMenu?.Update();
            }
            else if (SplitMenu != null)
            {
                // Close the menu if the interval is reached as the player likely wants its regular behavior
                _ = CancelMove();
            }
        }

        /// <summary>Tells the handler to close the split menu.</summary>
        public virtual void CloseSplitMenu()
        {
            SplitMenu?.Close();
            SplitMenu = null;
        }

        /// <summary>Draws the split menu.</summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SplitMenu?.Draw(spriteBatch);
        }

        /// <summary>Try to perform hover action on split menu.</summary>
        public virtual void PerformHoverAction(int x, int y)
        {
            SplitMenu?.PerformHoverAction(x, y);
        }

        /// <summary>Handle user input.</summary>
        /// <param name="button">The pressed button.</param>
        public EInputHandled HandleInput(SButton button)
        {
            string pfx = $"[{nameof(BaseMenuHandler<TMenuType>)}.{nameof(HandleInput)}]";

            // Was right click pressed
            if (button == SButton.MouseRight)
            {
                // Invoke split menu if the modifier key was also down
                if (IsModifierKeyDown() && CanOpenSplitMenu())
                {
                    // Cancel the current operation
                    if (SplitMenu != null)
                    {
                        // TODO: return this value if it's consumed?
                        _ = CancelMove();
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
                    ClickItemLocation = new Point(Game1.getOldMouseX(true), Game1.getOldMouseY(true));
                    Log.TraceIfD($"{pfx}.ClickItemLocation = {ClickItemLocation}");

                    // Notify the handler the inventory was clicked.
                    if (HasInventory)
                    {
                        if (!InvHandler.Initialized)
                        {
                            Log.Trace($"{pfx} Handler has inventory but inventory isn't initialized.");
                        }
                        else
                        {
                            if (InvHandler.WasClicked(Game1.getMouseX(true), Game1.getMouseY(true)))
                            {
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
            else if (button == SButton.MouseLeft)
            {
                int mX = Game1.getMouseX(true);
                int mY = Game1.getMouseY(true);
                // If the player clicks within the bounds of the tooltip then forward the input to that. 
                // Otherwise they're clicking elsewhere and we should close the tooltip.
                if (SplitMenu != null && SplitMenu.ContainsPoint(mX, mY))
                {
                    SplitMenu.ReceiveLeftClick(mX, mY);
                    return EInputHandled.Consumed;
                }

                EInputHandled handled = HandleLeftClick();
                if (handled == EInputHandled.NotHandled && SplitMenu != null)
                {
                    // Lost focus; cancel the move (run default behavior)
                    return CancelMove();
                }
                return handled;
            }
            else if (ShouldConsumeKeyboardInput(button))
            {
                return EInputHandled.Handled;
            }

            return EInputHandled.NotHandled;
        }


        /// <summary>Allows derived classes to handle left clicks when they are not focused on the split menu.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled HandleLeftClick()
        {
            return EInputHandled.NotHandled;
        }

        /// <summary>Whether we should consume the input, preventing it from reaching the game.</summary>
        /// <param name="keyPressed">The key that was pressed.</param>
        /// <returns>True if it should be consumed, false otherwise.</returns>
        protected virtual bool ShouldConsumeKeyboardInput(SButton keyPressed)
        {
            if (keyPressed == SButton.Enter && SplitMenu is not null)
            {
                SplitMenu.Submit();
            }
            return SplitMenu != null;
        }

        /// <summary>How long the right click has to be held for before the receiveRIghtClick gets called rapidly (See Game1.Update)</summary>
        /// <returns>The polling interval.</returns>
        protected virtual float GetRightClickPollingInterval()
        {
            return RIGHT_CLICK_POLLING_INTVL;
        }

        /// <summary>Allows derived handlers to provide additional checks before opening the split menu.</summary>
        /// <returns>True if it can be opened.</returns>
        protected virtual bool CanOpenSplitMenu()
        {
            return true;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected abstract EInputHandled OpenSplitMenu();

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled InventoryClicked()
        {
            Debug.Assert(HasInventory);
            return OpenSplitMenu();
        }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected virtual EInputHandled CancelMove()
        {
            CloseSplitMenu();

            if (HasInventory && InvHandler.Initialized)
            {
                InvHandler.CancelSplit();
            }

            return EInputHandled.NotHandled;
        }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected virtual void OnStackAmountReceived(string s)
        {
            CloseSplitMenu();
        }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        protected virtual void InitInventory()
        {
            if (!HasInventory)
            {
                return;
            }

            try
            {
                // Have to use Reflection here because IClickableMenu does not define .inventory nor .hoveredItem;
                // rather, they are defined per-class by subclasses that implement IClickableMenu.
                // Our wrappers are designed so that if .HasInventory == true, then the native classes DO have .inventory & .hoveredItem
                // (known by inspecting them class defs in the game code).
                // Since there is no Interface defined in the game code that guarantees the existence of .inventory & .hoveredItem,
                // we either have to cast them explicitly, or just use Reflection to retrieve.
                InventoryMenu inventoryMenu = StackEverythingRedux.Reflection.GetField<IClickableMenu>(NativeMenu, "inventory").GetValue() as InventoryMenu;
                IReflectedField<Item> hoveredItemField = StackEverythingRedux.Reflection.GetField<Item>(NativeMenu, "hoveredItem");

                InvHandler.Init(inventoryMenu, hoveredItemField);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(BaseMenuHandler<TMenuType>)}.{nameof(InitInventory)}] Failed to initialize the inventory handler. Exception:\n{e}");
            }
        }

        protected bool IsModifierKeyDown()
        {
            return StaticConfig.ModifierKeys.Any(StackEverythingRedux.Input.IsDown);//return Mod.Input.IsDown(SButton.LeftAlt) || Mod.Input.IsDown(SButton.LeftShift);
        }
    }
}
