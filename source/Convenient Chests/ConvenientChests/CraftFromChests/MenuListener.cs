using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests.CraftFromChests {
    public class MenuListener {
        public static readonly int CraftingMenuTab = Constants.TargetPlatform == GamePlatform.Android ? 3 : GameMenu.craftingTab;
        
        private readonly IModEvents Events;

        public event EventHandler GameMenuShown;
        public event EventHandler GameMenuClosed;
        public event EventHandler CraftingMenuShown;
        public event EventHandler CraftingMenuClosed;

        public MenuListener(IModEvents events) {
            this.Events = events;
        }

        public void RegisterEvents() {
            ModEntry.Log("Register");
            this.Events.Display.MenuChanged += OnMenuChanged;
        }

        public void UnregisterEvents() {
            ModEntry.Log("UnRegister");
            this.Events.Display.MenuChanged -= OnMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (e.NewMenu == e.OldMenu)
                return;

            switch (e.OldMenu) {
                case GameMenu _:
                    GameMenuClosed?.Invoke(sender, e);
                    UnregisterTabEvent();
                    break;

                case CraftingPage _:
                    if (e.NewMenu is CraftingPage)
                        break;

                    CraftingMenuClosed?.Invoke(sender, e);
                    break;
            }

            switch (e.NewMenu) {
                case GameMenu _:
                    GameMenuShown?.Invoke(sender, e);
                    this.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
                    break;

                case CraftingPage _:
                case object m when m.GetType().ToString() == "CookingSkill.NewCraftingPage":
                    CraftingMenuShown?.Invoke(sender, e);
                    break;
            }
        }

        private int _previousTab = -1;

        public event EventHandler GameMenuTabChanged;

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e) {
            switch (Game1.activeClickableMenu) {
                case TitleMenu _:
                    // Quit to title
                    // -> unregister silently
                    UnregisterTabEvent();
                    return;

                case GameMenu gameMenu when gameMenu.currentTab == _previousTab:
                    // Nothing changed
                    return;

                case GameMenu gameMenu:
                    // Tab changed!
                    GameMenuTabChanged?.Invoke(null, EventArgs.Empty);

                    if (_previousTab == CraftingMenuTab)
                        CraftingMenuClosed?.Invoke(sender, EventArgs.Empty);

                    else if (gameMenu.currentTab == CraftingMenuTab)
                        CraftingMenuShown?.Invoke(sender, EventArgs.Empty);

                    _previousTab = gameMenu.currentTab;
                    break;

                default:
                    // How did we get here?
                    ModEntry.StaticMonitor.Log($"Unexpected menu: {Game1.activeClickableMenu?.GetType().ToString() ?? "null"}");
                    UnregisterTabEvent();
                    return;
            }
        }

        private void UnregisterTabEvent() {
            this.Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
            _previousTab                           =  -1;
        }
    }
}