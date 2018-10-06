using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
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
        private IMonitor monitor;
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

            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[LostBookFoundDialogService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;

            running = false;
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            ignoreMenuChanged = false;

            if (e.PriorMenu is LetterViewerMenu && switchBackToCollectionsMenu)
            {
                ignoreMenuChanged = true;
                Game1.activeClickableMenu = savedGameMenu;
            }

            switchBackToCollectionsMenu = false;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
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

            else if (e.NewMenu is LetterViewerMenu && e.PriorMenu is GameMenu gameMenu2)
            {
                switchBackToCollectionsMenu = true;
                savedGameMenu = gameMenu2;
            }

            ignoreMenuChanged = false;
        }
    }
}
