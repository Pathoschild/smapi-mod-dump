/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jn84/QualitySmash
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace QualitySmash
{
    public class ModEntry : Mod
    {
        internal enum SmashType
        {
            Color,
            Quality
        }

        private QualitySmashHandler handlerUiButtons;
        private SingleSmashHandler handlerKeybinds;
        private ModConfig config;

        internal IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<ModConfig>();

            var buttonColor = helper.Content.Load<Texture2D>("assets/buttonColor.png");
            var buttonQuality = helper.Content.Load<Texture2D>("assets/buttonQuality.png");

            this.helper = helper;
            this.handlerUiButtons = new QualitySmashHandler(this, this.config, buttonColor, buttonQuality);
            this.handlerKeybinds = new SingleSmashHandler(this, this.config, buttonColor, buttonQuality);

            AddEvents(helper);

        }

        private void AddEvents(IModHelper helper)
        {
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
            helper.Events.Input.CursorMoved += OnCursorMoved;
        }

        /// <summary>
        /// Gets the ItemGrabMenu if it's from a fridge or chest
        /// </summary>
        /// <returns>The ItemGrabMenu</returns>
        internal MenuWithInventory GetValidButtonSmashMenu()
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is MenuWithInventory)
            {
                var menu = Game1.activeClickableMenu as MenuWithInventory;
                if (menu is ItemGrabMenu grabMenu)
                {
                    // Exclude mini shipping bin, shipping bin, fishing chests, etc
                    if (grabMenu.ItemsToGrabMenu.capacity <= 9 || grabMenu.source == 2 || grabMenu.source == 3)
                        return null;
                    return grabMenu;
                }
            }
            return null;
        }

        internal IClickableMenu GetValidKeybindSmashMenu()
        {
            // InventoryMenu or MenuWithInventory.. Use ItemGrabMenu?
            if (Game1.activeClickableMenu != null &&
                (Game1.activeClickableMenu is ItemGrabMenu ||
                 Game1.activeClickableMenu is GameMenu)) 
                return Game1.activeClickableMenu;
            return null;
        }

        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            UpdateHoverText();
        }

        private void UpdateHoverText()
        {
            var scaledMousePos = Game1.getMousePosition(true);

            if (config.EnableUISmashButtons)
                handlerUiButtons.TryHover(scaledMousePos.X, scaledMousePos.Y);
            if (config.EnableSingleItemSmashKeybinds)
                handlerKeybinds.TryHover(scaledMousePos.X, scaledMousePos.Y);
        }

        /// <summary>
        /// Begins a check of whether a mouse click or button press was on a Smash button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == config.ColorSmashKeybind || e.Button == config.QualitySmashKeybind)
            {
                UpdateHoverText();
                return;
            }

            if (e.Button != SButton.MouseLeft && e.Button != SButton.ControllerA)
                return;

            if (config.EnableUISmashButtons && GetValidButtonSmashMenu() != null)
            {
                handlerUiButtons.HandleClick(e);
            }

            if (config.EnableSingleItemSmashKeybinds && GetValidKeybindSmashMenu() != null)
            {
                if (helper.Input.IsDown(config.ColorSmashKeybind) ||
                    helper.Input.IsDown(config.QualitySmashKeybind))
                {
                    handlerKeybinds.HandleClick(e);
                    helper.Input.Suppress(SButton.MouseLeft);
                    helper.Input.Suppress(SButton.ControllerA);
                }
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == config.ColorSmashKeybind || e.Button == config.QualitySmashKeybind)
            {
                UpdateHoverText();
                return;
            }
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (GetValidButtonSmashMenu() is ItemGrabMenu grabmenu)
                if (config.EnableUISmashButtons)
                {
                    handlerUiButtons.DrawButtons();
                    handlerUiButtons.PopulateIds(grabmenu);
                }

            if (GetValidKeybindSmashMenu() != null)
                if (config.EnableSingleItemSmashKeybinds)
                    handlerKeybinds.DrawHoverText();
        }
    }
}