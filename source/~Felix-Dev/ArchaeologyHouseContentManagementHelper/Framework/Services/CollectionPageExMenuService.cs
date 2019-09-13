using FelixDev.StardewMods.Common.Helpers.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework.Services
{
    /// <summary>
    /// This class is responsible for injecting the extended collections page into the game.
    /// </summary>
    internal class CollectionPageExMenuService
    {
        private readonly IMonitor monitor;
        private readonly IModEvents events;

        private bool running;

        private bool ignoreMenuChanged;
        private bool switchBackToCollectionsMenu;

        private IClickableMenu savedGameMenu;

        /// <summary>
        /// The index of the collections-page tab in the game menu.
        /// </summary>
        private int collectionsPageTabIndex;

        public CollectionPageExMenuService()
        {
            monitor = ModEntry.CommonServices.Monitor;
            events = ModEntry.CommonServices.Events;

            collectionsPageTabIndex = -1;

            running = false;
        }

        public void Start()
        {
            if (running)
            {
                monitor.Log("[CollectionPageExMenuService] is already running!", LogLevel.Info);
                return;
            }

            running = true;

            events.Display.MenuChanged += OnMenuChanged;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[CollectionPageExMenuService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            events.Display.MenuChanged -= OnMenuChanged;

            running = false;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // menu closed
            if (e.NewMenu == null)
            {
                ignoreMenuChanged = false;

                if (e.OldMenu is LetterViewerMenu && switchBackToCollectionsMenu)
                {
                    ignoreMenuChanged = true;
                    Game1.activeClickableMenu = savedGameMenu;
                }

                switchBackToCollectionsMenu = false;
                return;
            }

            // menu changed or opened

            if (e.NewMenu is GameMenu gameMenu && !ignoreMenuChanged)
            {
                List<IClickableMenu> pages = ModEntry.CommonServices.ReflectionHelper.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();

                if (collectionsPageTabIndex == -1)
                {
                    collectionsPageTabIndex = pages.Replace(tab => tab is CollectionsPage,
                        new CollectionsPageEx(gameMenu.xPositionOnScreen, gameMenu.yPositionOnScreen, gameMenu.width - 64 - 16, gameMenu.height));
                }
                else
                {
                    pages[collectionsPageTabIndex] = new CollectionsPageEx(gameMenu.xPositionOnScreen, gameMenu.yPositionOnScreen, gameMenu.width - 64 - 16, gameMenu.height);
                }
            }

            else if (e.NewMenu is LetterViewerMenu && e.OldMenu is GameMenu gameMenu2)
            {
                switchBackToCollectionsMenu = true;
                savedGameMenu = gameMenu2;
            }

            ignoreMenuChanged = false;

        }
    }
}
