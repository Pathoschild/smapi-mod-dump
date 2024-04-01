/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ChestFeatureSet.Framework.CFSItem
{
    public class CFSItemController
    {
        private readonly IModEvents Events;

        private CFSItemManager CFSItemManager { get; set; }

        private readonly HashSet<Item> Items;
        private Item HoveredItem { get; set; }

        /// <summary>
        /// New ItemController
        /// </summary>
        public CFSItemController(ModEntry modEntry, string saveFileName)
        {
            this.Events = modEntry.Helper.Events;

            this.CFSItemManager = new CFSItemManager(this, modEntry, saveFileName);

            this.Events.GameLoop.DayStarted += this.CFSItemManager.OnDayStarted;
            this.Events.GameLoop.Saving += this.CFSItemManager.OnSaving;

            this.Items = new HashSet<Item>();
            this.HoveredItem = null;
        }

        public void Deactivate()
        {
            this.Events.GameLoop.DayStarted -= this.CFSItemManager.OnDayStarted;
            this.Events.GameLoop.Saving -= this.CFSItemManager.OnSaving;
        }

        /// <summary>
        /// Gets the item that is currently hovered or null if no item is hovered.
        /// </summary>
        /// <returns>The item that is hovered or null</returns>
        public Item GetHoveredItem()
        {
            if (Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage inventoryPage)
                return inventoryPage.hoveredItem;
            else if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
                return itemGrabMenu.hoveredItem;

            return null;
        }

        /// <summary>
        /// Handle the button pressed event.
        /// </summary>
        /// <param name="cursor">The current cursor position.</param>
        public void HandleButtonPress()
        {
            this.UpdateHoveredItem();

            if (this.HoveredItem == null)
                return;

            if (this.Items.Contains(this.HoveredItem))
                this.Items.Remove(this.HoveredItem);
            else
                this.Items.Add(this.HoveredItem);
        }

        /// <summary>
        /// Returns the items currently selected
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Item> GetItems()
        {
            return this.Items;
        }

        /// <summary>
        /// Update the items
        /// </summary>
        /// <param name="items"></param>
        public void SetItems(IEnumerable<Item> items)
        {
            this.ClearItems();
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }

        /// <summary>
        /// Remove the item.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(Item item)
        {
            if (item != null)
                this.Items.Remove(item);
        }

        /// <summary>
        /// Add the item.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            if (item != null)
                this.Items.Add(item);
        }

        /// <summary>
        /// Clears the list of items.
        /// </summary>
        public void ClearItems()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Update the HoveredItem.
        /// </summary>
        public void UpdateHoveredItem()
        {
            this.HoveredItem = this.GetHoveredItem();
        }
    }
}
