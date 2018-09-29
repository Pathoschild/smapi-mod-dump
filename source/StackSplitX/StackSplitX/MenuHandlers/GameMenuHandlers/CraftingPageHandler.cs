using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StackSplitX.MenuHandlers
{
    public class CraftingPageHandler : GameMenuPageHandler<CraftingPage>
    {
        /// <summary>Used to store the click location so it's the same after the user submits the split menu even if they moved their mouse.</summary>
        private Point ClickItemLocation;

        /// <summary>Used to track if the inventory or crafting menu recieved input last.</summary>
        private bool WasInventoryClicked = false;

        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public CraftingPageHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
        }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        public override void InitInventory()
        {
            // We need to do this explicitly because the crafting page uses a different variable name for hover item.
            var inventoryMenu = this.MenuPage.GetType().GetField("inventory").GetValue(this.MenuPage) as InventoryMenu;
            var hoveredItemField = Helper.Reflection.GetField<Item>(this.MenuPage, "hoverItem");
            var heldItemField = Helper.Reflection.GetField<Item>(this.MenuPage, "heldItem");

            this.Inventory.Init(inventoryMenu, heldItemField, hoveredItemField);
        }

        /// <summary>Tells the handler that the inventory was shift-clicked.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public override EInputHandled InventoryClicked(out int stackAmount)
        {
            this.WasInventoryClicked = true;
            return base.InventoryClicked(out stackAmount);
        }

        /// <summary>Tells the handler that the interface recieved the hotkey input.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public override EInputHandled OpenSplitMenu(out int stackAmount)
        {
            stackAmount = 1; // Craft 1 by default
            this.WasInventoryClicked = false;

            var hoverRecipe = Helper.Reflection.GetField<CraftingRecipe>(this.MenuPage, "hoverRecipe").GetValue();
            var hoveredItem = hoverRecipe?.createItem();
            var heldItem = Helper.Reflection.GetField<Item>(this.MenuPage, "heldItem").GetValue();
            var cooking = Helper.Reflection.GetField<bool>(this.MenuPage, "cooking").GetValue();

            // If we're holding an item already then it must stack with the item we want to craft.
            if (hoveredItem == null || (heldItem != null && heldItem.Name != hoveredItem.Name))
                return EInputHandled.NotHandled;

            // Only allow items that can actually stack
            var extraIems = cooking ? Utility.getHomeOfFarmer(Game1.player).fridge.items : null;
            if (!hoveredItem.canStackWith(hoveredItem) || !hoverRecipe.doesFarmerHaveIngredientsInInventory(extraIems))
                return EInputHandled.NotHandled;

            this.ClickItemLocation = new Point(Game1.getOldMouseX(), Game1.getOldMouseY());
            return EInputHandled.Consumed;
        }

        /// <summary>Lets the handler run the logic for doing the split after the amount has been input.</summary>
        /// <param name="amount">The stack size the user requested.</param>
        public override void OnStackAmountEntered(int amount)
        {
            // Run regular inventory logic if that's what was clicked.
            if (this.WasInventoryClicked)
            {
                base.OnStackAmountEntered(amount);
                return;
            }

            // TODO: check the max amonut able to be crafted to avoid unnecessary iterations
            for (int i = 0; i < amount; ++i)
            {
                this.MenuPage.receiveRightClick(this.ClickItemLocation.X, this.ClickItemLocation.Y);
            }
        }
    }
}
