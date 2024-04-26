/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests.CraftFromChests {
    public class CraftingMenuArgs : EventArgs {
        public CraftingPage Page { get; private set; }
        public bool IsCookingPage { get; private set; }

        public CraftingMenuArgs(CraftingPage craftingPage, bool isCookingPage) {
            Page = craftingPage;
            IsCookingPage = isCookingPage;
        }
    }

    public class MenuListener {
        private readonly IModEvents Events;
        public event EventHandler<CraftingMenuArgs> CraftingMenuShown;

        private int PreviousTab = -1;

        public MenuListener(IModEvents events) {
            Events = events;
        }

        public void RegisterEvents() {
            ModEntry.Log("Register");
            Events.Display.MenuChanged += OnMenuChanged;
        }

        public void UnregisterEvents() {
            ModEntry.Log("UnRegister");
            Events.Display.MenuChanged -= OnMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (e.NewMenu == e.OldMenu)
                return;

            switch (e.OldMenu) {
                case GameMenu _:
                    UnregisterTabEvent();
                    break;
            }

            switch (e.NewMenu) {
                case GameMenu _:
                    Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
                    break;

                case object m when m.GetType().ToString() == "CookingSkill.NewCraftingPage":
                    CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(m as CraftingPage, true));
                    break;

                case CraftingPage p:
                    CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(p, p.cooking));
                    break;
            }
        }


        public event EventHandler GameMenuTabChanged;

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e) {
            switch (Game1.activeClickableMenu) {
                case null: // Game Menu closed
                case TitleMenu: // Quit to title
                    UnregisterTabEvent();
                    return;

                case GameMenu gameMenu:
                    if (gameMenu.currentTab == PreviousTab)
                        // Nothing changed
                        return;

                    // Tab changed!
                    GameMenuTabChanged?.Invoke(null, EventArgs.Empty);

                    // check current page
                    var currentPage = gameMenu.GetCurrentPage();
                    if (currentPage is CraftingPage p)
                        CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(p, false));

                    PreviousTab = gameMenu.currentTab;
                    break;

                default:
                    // How did we get here?
                    ModEntry.StaticMonitor.Log($"Unexpected menu: {Game1.activeClickableMenu.GetType()}");
                    UnregisterTabEvent();
                    return;
            }
        }

        private void UnregisterTabEvent() {
            Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
            PreviousTab = -1;
        }
    }
}