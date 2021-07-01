/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace StackSplitRedux.MenuHandlers
    {
    public class InventoryHandler
        {
        /// <summary>If the handler has been initialized yet by calling Init.</summary>
        public bool Initialized => this.NativeInventoryMenu != null;

        /// <summary>Convenience for grabbing native inventory buttons.</summary>
        private List<ClickableComponent> Inventory => this.NativeInventoryMenu.inventory;

        /// <summary>Convenience for grabbing native inventory items.</summary>
        private IList<Item> InventoryItems => this.NativeInventoryMenu.actualInventory;

        /// <summary>Native inventory.</summary>
        private InventoryMenu NativeInventoryMenu;

        /// <summary>Inventory interface bounds.</summary>
        private Rectangle Bounds;

        /// <summary>Where the user clicked so moving </summary>
        private int SelectedItemPosition_X;
        private int SelectedItemPosition_Y;

        /// <summary>The hovered item field owned by the parent menu that contains the inventory.</summary>
        private IReflectedField<Item> HoveredItemField;

        /// <summary>Currently hovered item in the inventory.</summary>
        private Item HoveredItem;

        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public InventoryHandler() {
            }

        /// <summary>This must be called everytime the inventory is opened/resized.</summary>
        /// <param name="inventoryMenu">Native inventory.</param>
        public void Init(InventoryMenu inventoryMenu, IReflectedField<Item> hoveredItemField) {
            Debug.Assert(inventoryMenu is not null);
            this.NativeInventoryMenu = inventoryMenu;
            this.HoveredItemField = hoveredItemField;

            // Create the bounds around the inventory
            var first = this.Inventory[0].bounds;
            var last = this.Inventory.Last().bounds;
            this.Bounds = new Rectangle(
                first.X,
                first.Y,
                last.X + last.Width - first.X,
                last.Y + last.Height - first.Y);
            }

        /// <summary>Broad phase check to see if the inventory interface was clicked.</summary>
        /// <param name="mousePos">Mouse position.</param>
        public bool WasClicked(Point mousePos) {
            Debug.Assert(this.Initialized);
            return this.Bounds.Contains(mousePos);
            }

        /// <summary>Broad phase check to see if the inventory interface was clicked.</summary>
        /// <param name="mouseX">Mouse X position.</param>
        /// <param name="mouseY">Mouse Y position.</param>
        public bool WasClicked(int mouseX, int mouseY) {
            Debug.Assert(this.Initialized);
            return this.Bounds.Contains(mouseX, mouseY);
            }

        /// <summary>Stores the data needed to be able to split an item stack. This must be called before CanSplitSelectedItem and SplitSelectedItem.</summary>
        /// <param name="mouseX">Mouse x position.</param>
        /// <param name="mouseY">Mouse y position.</param>
        public void SelectItem(int mouseX, int mouseY) {
            Debug.Assert(this.Initialized);

            this.SelectedItemPosition_X = mouseX;
            this.SelectedItemPosition_Y = mouseY;
            this.HoveredItem = this.HoveredItemField.GetValue();
            }

        /// <summary>Checks if the selected item can be split. SelectItem must be called first.</summary>
        public bool CanSplitSelectedItem() {
            Debug.Assert(this.Initialized);

            var hoveredItem = this.HoveredItem;
            var heldItem = Game1.player.CursorSlotItem;

            if (hoveredItem is null) return false;
            if (hoveredItem.Stack <= 1) return false;
            if (heldItem is null) return true;
            return hoveredItem.canStackWith(heldItem) && heldItem.Stack < heldItem.maximumStackSize();
            }

        /// <summary>Updates the stack values of the hovered and held item.</summary>
        /// <param name="stackAmount">The amount to be added to the held amount.</param>
        public void SplitSelectedItem(int stackAmount) {
            Debug.Assert(this.HoveredItemField != null);

            var hoveredItem = this.HoveredItem;
            var hoveredItemCount = this.HoveredItem.Stack;  // Grab & hold the value
            int maxStack = hoveredItem.maximumStackSize();

            var heldItem = Game1.player.CursorSlotItem;
            var heldItemCount = heldItem?.Stack ?? 0;  // Grab & hold the value

            // Run native click code to get the selected item
            // This is why we need to grab & hold the values above: rightClick immediately decreases HoveredItem.Stack and increases heldItem.Stack
            heldItem = this.NativeInventoryMenu.rightClick(this.SelectedItemPosition_X, this.SelectedItemPosition_Y, heldItem);
            Debug.Assert(heldItem != null);

            // Clamp the amount to the total number of items
            stackAmount = Math.Min(Math.Max(0, stackAmount), hoveredItemCount);
            // If we couldn't grab all that we wanted then only subtract the amount we were able to grab
            if ((heldItemCount + stackAmount) > maxStack)
                stackAmount = maxStack - heldItemCount;
            heldItemCount += stackAmount;

            // Perform the reduction
            hoveredItemCount -= stackAmount;
            if (hoveredItemCount <= 0)
                // Remove the item from the inventory if it's now all being held.
                RemoveItemFromInventory(hoveredItem);
            else
                // Commit the manipulated hovered value, overwriting changes by rightClick() above
                this.HoveredItem.Stack = hoveredItemCount;

            // Commit the new heldItemCount, overwriting changes by rightClick() above
            heldItem.Stack = heldItemCount;

            // Update the native fields
            Game1.player.CursorSlotItem = heldItem;

            // Null it out now that we're done with this operation
            this.HoveredItem = null;
            }

        /// <summary>Runs the default shift+right-click behavior on the selected item.</summary>
        public void CancelSplit() {
            if (this.Initialized && this.HoveredItem != null) {
                // Split with the default amount to simulate the default behaviour
                SplitSelectedItem(GetDefaultSplitStackAmount());

                // Null it out now that we're done with this operation
                this.HoveredItem = null;
                }
            }

        /// <summary>Gets the stack amount you would usually have when shift+right-clicking.</summary>
        public int GetDefaultSplitStackAmount() {
            // +1 before /2 will round UP the result, the intention of original code
            return (this.HoveredItem.Stack + 1) / 2;
            }

        /// <summary>Removes an item from the native inventory</summary>
        /// <param name="item">The item to remove.</param>
        private void RemoveItemFromInventory(Item item) {
            var index = this.InventoryItems.IndexOf(item);
            if (index >= 0 && index < this.InventoryItems.Count) {
                this.InventoryItems[index] = null;
                }
            }
        }
    }
