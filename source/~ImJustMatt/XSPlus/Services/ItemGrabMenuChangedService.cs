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
    using Models;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley.Menus;
    using StardewValley.Objects;

    /// <inheritdoc/>
    internal class ItemGrabMenuChangedService : IEventHandlerService<EventHandler<ItemGrabMenuEventArgs>>
    {
        private readonly PerScreen<IClickableMenu> _menu = new();
        private readonly PerScreen<bool> _attached = new();
        private readonly PerScreen<int> _screenId = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemGrabMenuChangedService"/> class.
        /// </summary>
        /// <param name="displayEvents">Events related to UI and drawing to the screen.</param>
        public ItemGrabMenuChangedService(IDisplayEvents displayEvents)
        {
            displayEvents.MenuChanged += this.OnMenuChanged;
        }

        private event EventHandler<ItemGrabMenuEventArgs>? ItemGrabMenuChanged;

        /// <inheritdoc/>
        public void AddHandler(EventHandler<ItemGrabMenuEventArgs> eventHandler)
        {
            this.ItemGrabMenuChanged += eventHandler;
        }

        /// <inheritdoc/>
        public void RemoveHandler(EventHandler<ItemGrabMenuEventArgs> eventHandler)
        {
            this.ItemGrabMenuChanged -= eventHandler;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (ReferenceEquals(e.NewMenu, this._menu.Value))
            {
                return;
            }

            this._menu.Value = e.NewMenu;
            if (e.NewMenu is not ItemGrabMenu { shippingBin: false, context: Chest { playerChest: { Value: true } } chest } itemGrabMenu)
            {
                this._attached.Value = false;
                this._screenId.Value = -1;
                this.InvokeAll(null, null);
                return;
            }

            this._attached.Value = true;
            this._screenId.Value = Context.ScreenId;
            this.InvokeAll(itemGrabMenu, chest);
        }

        private void InvokeAll(ItemGrabMenu? itemGrabMenu, Chest? chest)
        {
            var eventArgs = new ItemGrabMenuEventArgs(itemGrabMenu, chest);
            this.ItemGrabMenuChanged?.Invoke(this, eventArgs);
        }
    }
}