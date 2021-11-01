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
    using Microsoft.Xna.Framework;
    using Models;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;

    /// <inheritdoc cref="BaseService" />
    internal class RenderingActiveMenuService : BaseService, IEventHandlerService<EventHandler<RenderingActiveMenuEventArgs>>
    {
        private readonly PerScreen<ItemGrabMenuEventArgs> _menu = new();

        private RenderingActiveMenuService(ServiceManager serviceManager)
            : base("RenderingActiveMenu")
        {
            // Dependencies
            this.AddDependency<ItemGrabMenuChangedService>(service => (service as ItemGrabMenuChangedService)?.AddHandler(this.OnItemGrabMenuChanged));

            // Events
            serviceManager.Helper.Events.Display.RenderingActiveMenu += this.OnRenderingActiveMenu;
        }

        /// <inheritdoc />
        public void AddHandler(EventHandler<RenderingActiveMenuEventArgs> handler)
        {
            this.RenderingActiveMenu += handler;
        }

        /// <inheritdoc />
        public void RemoveHandler(EventHandler<RenderingActiveMenuEventArgs> handler)
        {
            this.RenderingActiveMenu -= handler;
        }

        private event EventHandler<RenderingActiveMenuEventArgs> RenderingActiveMenu;

        private void OnItemGrabMenuChanged(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is null)
            {
                this._menu.Value = null;
                return;
            }

            this._menu.Value = e;
        }

        [EventPriority(EventPriority.High)]
        private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId)
            {
                return;
            }

            // Draw background
            e.SpriteBatch.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            // Draw rendered items above background
            this.RenderingActiveMenu?.Invoke(this, e);
        }
    }
}