using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using SB_VerticalToolMenu.Framework;

namespace SB_VerticalToolMenu
{
    internal class ModEntry : Mod
    {
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;
        private VerticalToolBar verticalToolbar;
        private bool isInitiated, modOverride;
        private int currentToolIndex;
        private int scrolling;
        private int triggerPolling = 300;
        private int released = 0;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
            helper.Events.Input.MouseWheelScrolled += onMouseWheelScrolled;
            helper.Events.Input.ButtonPressed += onButtonPressed;
            helper.Events.Input.ButtonReleased += onButtonReleased;
            helper.Events.Display.MenuChanged += onMenuChanged;

            isInitiated = false;
            modOverride = false;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!isInitiated)
                return;

            // check input modifier
            var input = this.Helper.Input;
            modOverride = false;
            if (!Game1.player.UsingTool && input.IsDown(Config.Controls.HoldToActivateSlotKeys))
            {
                if (input.IsDown(Config.Controls.ChooseSlot1))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[0].name);
                else if (input.IsDown(Config.Controls.ChooseSlot2))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[1].name);
                else if (input.IsDown(Config.Controls.ChooseSlot3))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[2].name);
                else if (input.IsDown(Config.Controls.ChooseSlot4))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[3].name);
                else if (input.IsDown(Config.Controls.ChooseSlot5))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[4].name);

                modOverride = true;
            }

            // check current tool
            if (verticalToolbar.numToolsinToolbar > 0 && Game1.player.CurrentToolIndex != currentToolIndex)
            {
                if (modOverride || (triggerPolling < 300))
                {
                    Game1.player.CurrentToolIndex = currentToolIndex;
                    modOverride = false;
                }
            }

            // check polling
            if (verticalToolbar.numToolsinToolbar > 0)
            {
                if (scrolling != 0)
                {
                    if (!input.IsDown(this.Config.Controls.ScrollLeft) && !input.IsDown(this.Config.Controls.ScrollRight))
                    {
                        scrolling = 0;
                        return;
                    }
                    Game1.player.CurrentToolIndex = currentToolIndex;
                    int elapsedGameTime = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    this.triggerPolling -= elapsedGameTime;
                    if(this.triggerPolling <= 0 && !modOverride)
                    {
                        Game1.player.CurrentToolIndex = currentToolIndex;
                        this.triggerPolling = 100;
                        checkHoveredItem(scrolling);
                    }
                }
                else if (released < 300)
                {
                    Game1.player.CurrentToolIndex = currentToolIndex;
                    int polling = this.released;
                    int elapsedGameTime = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    this.released = polling + elapsedGameTime;
                    if (released > 300 && !modOverride)
                    {
                        Game1.player.CurrentToolIndex = currentToolIndex;
                        released = 300;
                    }

                }
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!isInitiated)
                return;

            // set scrolling
            if(verticalToolbar.numToolsinToolbar > 0 && (e.Button == this.Config.Controls.ScrollLeft || e.Button == this.Config.Controls.ScrollRight))
            {
                Game1.player.CurrentToolIndex = currentToolIndex;
                int num = e.Button == this.Config.Controls.ScrollLeft ? -1 : 1;
                checkHoveredItem(num);
                scrolling = num;
            }
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!isInitiated)
                return;

            if (verticalToolbar.numToolsinToolbar > 0 && (e.Button == this.Config.Controls.ScrollLeft || e.Button == this.Config.Controls.ScrollRight))
            {
                Game1.player.CurrentToolIndex = currentToolIndex;
                scrolling = 0;
                released = 0;
                triggerPolling = 300;
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu menu && menu.currentTab == GameMenu.inventoryTab)
            {
                List<IClickableMenu> pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                pages.RemoveAt(0);
                pages.Insert(0, new ModInventoryPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height));
            }
        }

        private void checkHoveredItem(int num)
        {
            if ( !(!Game1.player.UsingTool && !Game1.dialogueUp && ((Game1.pickingTool || Game1.player.CanMove) && (!Game1.player.areAllItemsNull() && !Game1.eventUp))) ) return;
                if (Game1.options.invertScrollDirection)
                num *= -1;

            while (true)
            {
                currentToolIndex += num;
                if (num < 0)
                {
                    if (currentToolIndex < 0)
                    {
                        currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[verticalToolbar.numToolsinToolbar - 1].name);
                    }
                    else if (currentToolIndex > 11 && currentToolIndex < Convert.ToInt32(verticalToolbar.buttons[0].name))
                    {
                        currentToolIndex = 11;
                    }

                }
                else if (num > 0)
                {
                    if (currentToolIndex > Convert.ToInt32(verticalToolbar.buttons[verticalToolbar.numToolsinToolbar - 1].name))
                    {
                        currentToolIndex = 0;
                    }
                    else if (currentToolIndex > 11 && currentToolIndex < Convert.ToInt32(verticalToolbar.buttons[0].name))
                    {
                        currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[0].name);
                    }
                }

                if (Game1.player.Items[currentToolIndex] != null)
                    break;
            }
            modOverride = true;
        }

        /// <summary>Raised after the player scrolls the mouse wheel.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            if (!isInitiated)
                return;

            if (verticalToolbar.numToolsinToolbar > 0)
                checkHoveredItem(e.Delta > 0 ? 1 : -1);
        }

        private Toolbar getToolbar()
        {
            return Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            verticalToolbar = new VerticalToolBar(getToolbar().xPositionOnScreen - (VerticalToolBar.getInitialWidth() / 2), Game1.viewport.Height - VerticalToolBar.getInitialHeight());
            Game1.onScreenMenus.Add(verticalToolbar);

            currentToolIndex = Game1.player.CurrentToolIndex;
            isInitiated = true;
        }
    }
}