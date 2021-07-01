/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;

namespace StackSplitRedux.MenuHandlers
    {
    public abstract class GameMenuPageHandler<TPageType> : IGameMenuPageHandler where TPageType : IClickableMenu
        {
        /// <summary>The inventory handler.</summary>
        protected InventoryHandler InventoryHandler = null;

        /// <summary>Does this menu have an inventory section.</summary>
        protected bool HasInventory { get; set; } = true;

        /// <summary>The native menu that owns all the pages.</summary>
        protected IClickableMenu NativeMenu { get; private set; }

        /// <summary>The native page this handler is for.</summary>
        protected TPageType MenuPage { get; private set; }

        /// <summary>Null constructor that currently only logs instantiation</summary>
        public GameMenuPageHandler() {
            Log.TraceIfD($"[{nameof(GameMenuPageHandler<TPageType>)}] Instantiated with TPageType = {typeof(TPageType)}");
            }

        ~GameMenuPageHandler() {
            Log.TraceIfD($"[{nameof(GameMenuPageHandler<TPageType>)}] Finalized for TPageType = {typeof(TPageType)}");
            }

        /// <summary>Notifies the page handler that its corresponding menu has been opened.</summary>
        /// <param name="menu">The native menu owning all the pages.</param>
        /// <param name="page">The specific page this handler is for.</param>
        /// <param name="inventoryHandler">The inventory handler.</param>
        public virtual void Open(IClickableMenu menu, IClickableMenu page, InventoryHandler inventoryHandler) {
            this.NativeMenu = menu;
            this.MenuPage = page as TPageType;
            this.InventoryHandler = inventoryHandler;

            if (this.HasInventory)
                InitInventory();
            }

        /// <summary>Tell the handler to close.</summary>
        public virtual void Close() {
            this.NativeMenu = null;
            this.MenuPage = null;
            this.InventoryHandler = null;
            }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        public virtual void InitInventory() {
            try {
                // Have to use Reflection here because IClickableMenu does not define .inventory nor .hoveredItem
                // (Subclasses of IClickableMenu that have .HasInventory == true DO define .inventory & .hoveredItem,
                // but they do not define an interface for that, so it's either trying to cast to those subclasses
                // one-by-one, or simply use Reflection to fetch the fields.
                var inventoryMenu = Mod.Reflection.GetField<IClickableMenu>(this.MenuPage, "inventory").GetValue() as InventoryMenu;
                var hoveredItemField = Mod.Reflection.GetField<Item>(this.MenuPage, "hoveredItem");
                Log.TraceIfD(
                    $"[{nameof(GameMenuPageHandler<TPageType>)}.{nameof(InitInventory)}] Initializing InventoryHandler " +
                    $"for menu = {inventoryMenu}, hovered = {hoveredItemField}"
                    );
                this.InventoryHandler.Init(inventoryMenu, hoveredItemField);
                }
            catch (Exception e) {
                Log.Error($"[{nameof(GameMenuPageHandler<TPageType>)}.{nameof(InitInventory)}] Failed to initialize the inventory handler:\n{e}");
                }
            }

        /// <summary>Tells the handler that the inventory was shift-clicked.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public virtual EInputHandled InventoryClicked(out int stackAmount) {
            stackAmount = 0;

            // This logic is the same for all the page handlers so we can do it here.
            this.InventoryHandler.SelectItem(Game1.getMouseX(true), Game1.getMouseY(true));
            if (this.InventoryHandler.CanSplitSelectedItem()) {
                stackAmount = this.InventoryHandler.GetDefaultSplitStackAmount();

                return EInputHandled.Consumed;
                }
            return EInputHandled.NotHandled;
            }

        /// <summary>Tells the handler that the interface recieved the hotkey input.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public virtual EInputHandled OpenSplitMenu(out int stackAmount) {
            stackAmount = 0;
            return EInputHandled.NotHandled;
            }

        /// <summary>Tells the handler to cancel the move/run the default behaviour.</summary>
        /// <returns>If the input invoking the cancel should be consumed or not.</returns>
        public virtual EInputHandled CancelMove() {
            return EInputHandled.NotHandled;
            }

        /// <summary>Lets the handler run the logic for doing the split after the amount has been input.</summary>
        /// <param name="amount">The stack size the user requested.</param>
        public virtual void OnStackAmountEntered(int amount) {
            this.InventoryHandler.SplitSelectedItem(amount);
            }
        }
    }
