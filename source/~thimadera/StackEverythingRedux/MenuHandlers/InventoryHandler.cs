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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace StackEverythingRedux.MenuHandlers
{
    public class InventoryHandler
    {
        /// <summary>If the handler has been initialized yet by calling Init.</summary>
        public bool Initialized => NativeInventoryMenu != null;

        /// <summary>Convenience for grabbing native inventory buttons.</summary>
        private List<ClickableComponent> Inventory => NativeInventoryMenu.inventory;

        /// <summary>Convenience for grabbing native inventory items.</summary>
        private IList<Item> InventoryItems => NativeInventoryMenu.actualInventory;

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
        public InventoryHandler()
        {
        }

        /// <summary>This must be called everytime the inventory is opened/resized.</summary>
        /// <param name="inventoryMenu">Native inventory.</param>
        public void Init(InventoryMenu inventoryMenu, IReflectedField<Item> hoveredItemField)
        {
            Debug.Assert(inventoryMenu is not null);
            NativeInventoryMenu = inventoryMenu;
            HoveredItemField = hoveredItemField;

            // Create the bounds around the inventory
            Rectangle first = Inventory[0].bounds;
            Rectangle last = Inventory.Last().bounds;
            Bounds = new Rectangle(
                first.X,
                first.Y,
                last.X + last.Width - first.X,
                last.Y + last.Height - first.Y);
        }

        /// <summary>Broad phase check to see if the inventory interface was clicked.</summary>
        /// <param name="mousePos">Mouse position.</param>
        public bool WasClicked(Point mousePos)
        {
            Debug.Assert(Initialized);
            return Bounds.Contains(mousePos);
        }

        /// <summary>Broad phase check to see if the inventory interface was clicked.</summary>
        /// <param name="mouseX">Mouse X position.</param>
        /// <param name="mouseY">Mouse Y position.</param>
        public bool WasClicked(int mouseX, int mouseY)
        {
            Debug.Assert(Initialized);
            return Bounds.Contains(mouseX, mouseY);
        }

        /// <summary>Stores the data needed to be able to split an item stack. This must be called before CanSplitSelectedItem and SplitSelectedItem.</summary>
        /// <param name="mouseX">Mouse x position.</param>
        /// <param name="mouseY">Mouse y position.</param>
        public void SelectItem(int mouseX, int mouseY)
        {
            Debug.Assert(Initialized);

            SelectedItemPosition_X = mouseX;
            SelectedItemPosition_Y = mouseY;
            HoveredItem = HoveredItemField.GetValue();
        }

        /// <summary>Checks if the selected item can be split. SelectItem must be called first.</summary>
        public bool CanSplitSelectedItem()
        {
            Debug.Assert(Initialized);

            Item hoveredItem = HoveredItem;
            Item heldItem = Game1.player.CursorSlotItem;

            return hoveredItem is not null
&& hoveredItem.Stack > 1
&& (heldItem is null || (hoveredItem.canStackWith(heldItem) && heldItem.Stack < heldItem.maximumStackSize()));
        }

        /// <summary>Updates the stack values of the hovered and held item.</summary>
        /// <param name="stackAmount">The amount to be added to the held amount.</param>
        public void SplitSelectedItem(int stackAmount)
        {
            Debug.Assert(HoveredItemField != null);

            Item hoveredItem = HoveredItem;
            int hoveredItemCount = HoveredItem.Stack;  // Grab & hold the value
            int maxStack = hoveredItem.maximumStackSize();

            Item heldItem = Game1.player.CursorSlotItem;
            int heldItemCount = heldItem?.Stack ?? 0;  // Grab & hold the value

            // Run native click code to get the selected item
            // This is why we need to grab & hold the values above: rightClick immediately decreases HoveredItem.Stack and increases heldItem.Stack
            heldItem = NativeInventoryMenu.rightClick(SelectedItemPosition_X, SelectedItemPosition_Y, heldItem);
            Debug.Assert(heldItem != null);

            // Clamp the amount to the total number of items
            stackAmount = Math.Min(Math.Max(0, stackAmount), hoveredItemCount);
            // If we couldn't grab all that we wanted then only subtract the amount we were able to grab
            if (heldItemCount + stackAmount > maxStack)
            {
                stackAmount = maxStack - heldItemCount;
            }

            heldItemCount += stackAmount;

            // Perform the reduction
            hoveredItemCount -= stackAmount;
            if (hoveredItemCount <= 0)
            {
                // Remove the item from the inventory if it's now all being held.
                RemoveItemFromInventory(hoveredItem);
            }
            else
            {
                // Commit the manipulated hovered value, overwriting changes by rightClick() above
                HoveredItem.Stack = hoveredItemCount;
            }

            // Commit the new heldItemCount, overwriting changes by rightClick() above
            heldItem.Stack = heldItemCount;

            // Update the native fields
            Game1.player.CursorSlotItem = heldItem;

            // Null it out now that we're done with this operation
            HoveredItem = null;
        }

        /// <summary>Runs the default shift+right-click behavior on the selected item.</summary>
        public void CancelSplit()
        {
            if (Initialized && HoveredItem != null)
            {
                // Split with the default amount to simulate the default behaviour
                SplitSelectedItem(GetDefaultSplitStackAmount());

                // Null it out now that we're done with this operation
                HoveredItem = null;
            }
        }

        /// <summary>Gets the stack amount you would usually have when shift+right-clicking.</summary>
        public int GetDefaultSplitStackAmount()
        {
            // +1 before /2 will round UP the result, the intention of original code
            return (HoveredItem.Stack + 1) / 2;
        }

        /// <summary>Removes an item from the native inventory</summary>
        /// <param name="item">The item to remove.</param>
        private void RemoveItemFromInventory(Item item)
        {
            int index = InventoryItems.IndexOf(item);
            if (index >= 0 && index < InventoryItems.Count)
            {
                InventoryItems[index] = null;
            }
        }
    }
}
