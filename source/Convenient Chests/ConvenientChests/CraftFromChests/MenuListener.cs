 using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests.CraftFromChests {
    public static class MenuListener {
        public static List<IClickableMenu> GetTabs(this GameMenu m, IReflectionHelper h) => h.GetField<List<IClickableMenu>>(m, "pages").GetValue();

        public static event EventHandler GameMenuShown;
        public static event EventHandler GameMenuClosed;
        public static event EventHandler CraftingMenuShown;
        public static event EventHandler CraftingMenuClosed;

        public static void RegisterEvents() {
            ModEntry.Log("Register");
            MenuEvents.MenuChanged += OnMenuChanged;
            MenuEvents.MenuClosed  += OnMenuClosed;
        }

        public static void UnregisterEvents() {
            ModEntry.Log("UnRegister");
            MenuEvents.MenuChanged -= OnMenuChanged;
            MenuEvents.MenuClosed  -= OnMenuClosed;
        }

        private static void OnMenuChanged(object sender, EventArgsClickableMenuChanged e) {
            if (e.NewMenu == e.PriorMenu)
                return;

            switch (e.PriorMenu) {
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
                    GraphicsEvents.OnPostRenderGuiEvent += OnPostRenderGuiEvent;
                    break;

                case CraftingPage _:
                    CraftingMenuShown?.Invoke(sender, e);
                    break;
            }
        }

        private static void OnMenuClosed(object sender, EventArgsClickableMenuClosed e) {
            switch (e.PriorMenu) {
                case GameMenu _:
                    GameMenuClosed?.Invoke(sender, e);
                    UnregisterTabEvent();
                    break;

                case CraftingPage _:
                    CraftingMenuClosed?.Invoke(sender, e);
                    break;
            }
        }


        private static int _previousTab = -1;

        public static event EventHandler GameMenuTabChanged;

        private static void OnPostRenderGuiEvent(object sender, EventArgs e) {
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

                    if (_previousTab == GameMenu.craftingTab)
                        CraftingMenuClosed?.Invoke(sender, EventArgs.Empty);

                    else if (gameMenu.currentTab == GameMenu.craftingTab)
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

        private static void UnregisterTabEvent() {
            GraphicsEvents.OnPostRenderGuiEvent -= OnPostRenderGuiEvent;
            _previousTab                        =  -1;
        }
    }
}