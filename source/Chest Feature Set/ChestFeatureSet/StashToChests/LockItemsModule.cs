/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.Framework.CFSItem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ChestFeatureSet.StashToChests
{
    public class LockItemsModule : Module
    {
        public LockItemsModule(ModEntry modEntry) : base(modEntry) { }

        private Texture2D LockIcon { get; set; }
        private Rectangle LockIconRectangle { get; set; }

        public CFSItemController CFSItemController { get; private set; }

        public override void Activate()
        {
            this.IsActive = true;

            this.LockIcon = this.ModEntry.Helper.ModContent.Load<Texture2D>("assets/lockIcon.png");
            this.LockIconRectangle = new Rectangle(0, 0, this.LockIcon.Width, this.LockIcon.Height);

            this.CFSItemController = new CFSItemController(this.ModEntry, "LockedItems.json");

            this.Events.Display.MenuChanged += this.OnMenuChanged;
            this.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            this.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        public override void Deactivate()
        {
            this.IsActive = false;

            this.CFSItemController.Deactivate();

            this.Events.Display.MenuChanged -= this.OnMenuChanged;
            this.Events.Display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
            this.Events.Input.ButtonPressed -= this.OnButtonPressed;
        }

        /// <summary>
        /// Update inventoryPage.inventory.allClickableComponents & Rebuild the Items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.NewMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage inventoryPage)
            {
                if (inventoryPage.inventory.allClickableComponents == null)
                    inventoryPage.inventory.populateClickableComponentList();

                this.RebuildItems();
            }
            else if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
            {
                if (itemGrabMenu.inventory.allClickableComponents == null)
                    itemGrabMenu.inventory.populateClickableComponentList();

                this.RebuildItems();
            }
        }

        /// <summary>
        /// Draw the lockIcon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.GetCurrentPage() is InventoryPage inventoryPage)
            {
                var LockedItemsComponentsList = this.GetLockedItemComponent(inventoryPage.inventory);

                this.DrawIcon(e, LockedItemsComponentsList);
            }
            else if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
            {
                var LockedItemsComponentsList = this.GetLockedItemComponent(itemGrabMenu.inventory);

                this.DrawIcon(e, LockedItemsComponentsList);
            }
        }

        /// <summary>
        /// Add or Remove the LockItems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.LockItemKey)
            {
                this.CFSItemController.HandleButtonPress();
            }
            else if (e.Button == this.Config.ResetLockItemKey)
            {
                this.CFSItemController.ClearItems();
                Game1.pauseThenMessage(5, "Clear");
            }
        }

        /// <summary>
        /// Get the LockedItems' Components
        /// </summary>
        /// <param name="InventoryMenu"></param>
        /// <returns></returns>
        private List<ClickableComponent> GetLockedItemComponent(InventoryMenu inventory)
        {
            var components = new List<ClickableComponent>();

            foreach (var item in this.CFSItemController.GetItems())
            {
                var hoveredId = Game1.player.getIndexOfInventoryItem(item);
                var hoveredComponent = inventory.getComponentWithID(hoveredId);
                components.Add(hoveredComponent);
            }

            return components;
        }

        /// <summary>
        /// Draw the lockIcon.
        /// </summary>
        /// <param name="e"></param>
        public void DrawIcon(RenderedActiveMenuEventArgs e, List<ClickableComponent> LockedItemsComponentsList)
        {
            foreach (var component in LockedItemsComponentsList)
            {
                if (component == null)
                    return;

                var pos = new Vector2(component.bounds.X + 64 - 32, component.bounds.Y + 64 - 32);

                e.SpriteBatch.Draw(this.LockIcon, pos, this.LockIconRectangle, Color.White);
            }

            // Let MouseCursor above LockIcon.
            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Rebuild the Items.
        /// </summary>
        private void RebuildItems()
        {
            var itemList = this.CFSItemController.GetItems();
            for (int i = 0; i < itemList.Count(); i++)
            {
                try
                {
                    if (Game1.player.Items.Contains(itemList.ElementAt(i)))
                        continue;

                    this.CFSItemController.RemoveItem(itemList.ElementAt(i));
                }
                catch (Exception error)
                {
                    // While the item is in Items but not in player's inventory, it will be error.
                    //this.Monitor.Log(error.ToString(), StardewModdingAPI.LogLevel.Error);
                    this.CFSItemController.RemoveItem(itemList.ElementAt(i));
                }
            }
        }
    }
}