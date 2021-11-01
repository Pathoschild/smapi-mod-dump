/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace CommonHarmony.Services
{
    using System;
    using Common.Services;
    using Interfaces;
    using Models;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Menus;

    /// <inheritdoc cref="BaseService" />
    internal class RenderedActiveMenuService : BaseService, IEventHandlerService<EventHandler<RenderedActiveMenuEventArgs>>
    {
        private readonly PerScreen<ItemGrabMenuEventArgs> _menu = new();

        private RenderedActiveMenuService(ServiceManager serviceManager)
            : base("RenderedActiveMenu")
        {
            // Dependencies
            this.AddDependency<ItemGrabMenuChangedService>(service => (service as ItemGrabMenuChangedService)?.AddHandler(this.OnItemGrabMenuChanged));

            // Events
            serviceManager.Helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
        }

        /// <inheritdoc />
        public void AddHandler(EventHandler<RenderedActiveMenuEventArgs> eventHandler)
        {
            this.RenderedActiveMenu += eventHandler;
        }

        /// <inheritdoc />
        public void RemoveHandler(EventHandler<RenderedActiveMenuEventArgs> eventHandler)
        {
            this.RenderedActiveMenu -= eventHandler;
        }

        private event EventHandler<RenderedActiveMenuEventArgs> RenderedActiveMenu;

        private void OnItemGrabMenuChanged(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is null)
            {
                this._menu.Value = null;
                return;
            }

            this._menu.Value = e;
        }

        [EventPriority(EventPriority.Low)]
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || this.RenderedActiveMenu is null)
            {
                return;
            }

            // Draw render items below foreground
            this.RenderedActiveMenu?.Invoke(this, e);

            // Draw foreground
            if (this._menu.Value.ItemGrabMenu.hoverText is not null && (this._menu.Value.ItemGrabMenu.hoveredItem is null or null || this._menu.Value.ItemGrabMenu.ItemsToGrabMenu is null))
            {
                if (this._menu.Value.ItemGrabMenu.hoverAmount > 0)
                {
                    IClickableMenu.drawToolTip(e.SpriteBatch, this._menu.Value.ItemGrabMenu.hoverText, string.Empty, null, true, -1, 0, -1, -1, null, this._menu.Value.ItemGrabMenu.hoverAmount);
                }
                else
                {
                    IClickableMenu.drawHoverText(e.SpriteBatch, this._menu.Value.ItemGrabMenu.hoverText, Game1.smallFont);
                }
            }

            if (this._menu.Value.ItemGrabMenu.hoveredItem is not null)
            {
                IClickableMenu.drawToolTip(e.SpriteBatch, this._menu.Value.ItemGrabMenu.hoveredItem.getDescription(), this._menu.Value.ItemGrabMenu.hoveredItem.DisplayName, this._menu.Value.ItemGrabMenu.hoveredItem, this._menu.Value.ItemGrabMenu.heldItem is not null);
            }
            else if (this._menu.Value.ItemGrabMenu.hoveredItem is not null && this._menu.Value.ItemGrabMenu.ItemsToGrabMenu is not null)
            {
                IClickableMenu.drawToolTip(e.SpriteBatch, this._menu.Value.ItemGrabMenu.ItemsToGrabMenu.descriptionText, this._menu.Value.ItemGrabMenu.ItemsToGrabMenu.descriptionTitle, this._menu.Value.ItemGrabMenu.hoveredItem, this._menu.Value.ItemGrabMenu.heldItem is not null);
            }

            this._menu.Value.ItemGrabMenu.heldItem?.drawInMenu(e.SpriteBatch, new(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            this._menu.Value.ItemGrabMenu.drawMouse(e.SpriteBatch);
        }
    }
}