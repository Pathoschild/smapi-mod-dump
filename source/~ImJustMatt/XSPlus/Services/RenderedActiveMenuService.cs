/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSPlus.Services
{
    using System;
    using Interfaces;
    using Microsoft.Xna.Framework;
    using Models;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Menus;

    /// <inheritdoc />
    internal class RenderedActiveMenuService : IEventHandlerService<EventHandler<RenderedActiveMenuEventArgs>>
    {
        private readonly IDisplayEvents _displayEvents;
        private readonly PerScreen<int> _screenId = new();
        private readonly PerScreen<bool> _attached = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderedActiveMenuService"/> class.
        /// </summary>
        /// <param name="displayEvents">Events related to UI and drawing to the screen.</param>
        /// <param name="itemGrabMenuChangedService">Event for when the active menu switches from/to an ItemGrabMenu with a chest.</param>
        public RenderedActiveMenuService(IDisplayEvents displayEvents, ItemGrabMenuChangedService itemGrabMenuChangedService)
        {
            this._displayEvents = displayEvents;
            itemGrabMenuChangedService.AddHandler(this.OnItemGrabMenuChangedEvent);
        }

        private event EventHandler<RenderedActiveMenuEventArgs>? RenderedActiveMenu;

        /// <inheritdoc/>
        public void AddHandler(EventHandler<RenderedActiveMenuEventArgs> eventHandler)
        {
            this.RenderedActiveMenu += eventHandler;
        }

        /// <inheritdoc/>
        public void RemoveHandler(EventHandler<RenderedActiveMenuEventArgs> eventHandler)
        {
            this.RenderedActiveMenu -= eventHandler;
        }

        private void OnItemGrabMenuChangedEvent(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is not null && !this._attached.Value)
            {
                this._displayEvents.RenderedActiveMenu += this.OnRenderedActiveMenu;
                this._screenId.Value = Context.ScreenId;
                return;
            }

            if (e.ItemGrabMenu is null)
            {
                this._displayEvents.RenderedActiveMenu -= this.OnRenderedActiveMenu;
                this._screenId.Value = -1;
                this._attached.Value = false;
            }
        }

        [EventPriority(EventPriority.Low)]
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (this._screenId.Value != Context.ScreenId || this.RenderedActiveMenu is null || Game1.activeClickableMenu is not ItemGrabMenu itemGrabMenu)
            {
                return;
            }

            // Draw render items below foreground
            this.RenderedActiveMenu?.Invoke(this, e);

            // Draw foreground
            if (itemGrabMenu.hoverText is not null && (itemGrabMenu.hoveredItem is null or null || itemGrabMenu.ItemsToGrabMenu is null))
            {
                if (itemGrabMenu.hoverAmount > 0)
                {
                    IClickableMenu.drawToolTip(e.SpriteBatch, itemGrabMenu.hoverText, string.Empty, null, heldItem: true, -1, 0, -1, -1, null, itemGrabMenu.hoverAmount);
                }
                else
                {
                    IClickableMenu.drawHoverText(e.SpriteBatch, itemGrabMenu.hoverText, Game1.smallFont);
                }
            }

            if (itemGrabMenu.hoveredItem is not null)
            {
                IClickableMenu.drawToolTip(e.SpriteBatch, itemGrabMenu.hoveredItem.getDescription(), itemGrabMenu.hoveredItem.DisplayName, itemGrabMenu.hoveredItem, itemGrabMenu.heldItem is not null);
            }
            else if (itemGrabMenu.hoveredItem is not null && itemGrabMenu.ItemsToGrabMenu is not null)
            {
                IClickableMenu.drawToolTip(e.SpriteBatch, itemGrabMenu.ItemsToGrabMenu.descriptionText, itemGrabMenu.ItemsToGrabMenu.descriptionTitle, itemGrabMenu.hoveredItem, itemGrabMenu.heldItem is not null);
            }

            itemGrabMenu.heldItem?.drawInMenu(e.SpriteBatch, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            itemGrabMenu.drawMouse(e.SpriteBatch);
        }
    }
}