using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Reflection;

namespace SB_VerticalToolMenu
{

    public class ModEntry : Mod
    {
        VerticalToolBar verticalToolbar;
        bool isInitiated, modOverride;
        int currentToolIndex;
        int scrolling;
        int triggerPolling = 300;
        int released = 0;
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += initializeMod;
            GameEvents.UpdateTick += checkCurrentTool;
            GameEvents.UpdateTick += checkPolling;
            ControlEvents.KeyboardChanged += chooseToolKey;
            ControlEvents.MouseChanged += checkHoveredItemMouse;
            ControlEvents.ControllerTriggerPressed += setScrolling;
            ControlEvents.ControllerTriggerReleased += unsetScrolling;
            MenuEvents.MenuChanged += hijackInventoryPage;

            isInitiated = false;
            modOverride = false;
        }

        private void checkPolling(object sender, EventArgs e)
        {
            if (isInitiated && verticalToolbar.numToolsinToolbar > 0)
                if (scrolling != 0)
                {
                    if ( GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.LeftTrigger) && GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.RightTrigger)) 
                    {
                        scrolling = 0;
                        return;
                    }
                    Game1.player.CurrentToolIndex = currentToolIndex;
                    int triggerPolling = this.triggerPolling;
                    int elapasedGameTime1 = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    this.triggerPolling = triggerPolling - elapasedGameTime1;
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

        private void setScrolling(object sender, EventArgsControllerTriggerPressed e)
        {
            if (!isInitiated) return;
            if(verticalToolbar.numToolsinToolbar > 0 && (e.PlayerIndex == PlayerIndex.One && e.ButtonPressed == Buttons.LeftTrigger || e.ButtonPressed == Buttons.RightTrigger))
            {
                Game1.player.CurrentToolIndex = currentToolIndex;
                int num = e.ButtonPressed == Buttons.LeftTrigger ? -1 : 1;
                checkHoveredItem(num);
                scrolling = num;
            }
        }

        private void unsetScrolling(object sender, EventArgsControllerTriggerReleased e)
        {
            if (verticalToolbar.numToolsinToolbar > 0 && (e.PlayerIndex == PlayerIndex.One && e.ButtonReleased == Buttons.LeftTrigger || e.ButtonReleased == Buttons.RightTrigger))
            {
                Game1.player.CurrentToolIndex = currentToolIndex;
                scrolling = 0;
                released = 0;
                triggerPolling = 300;
            }
        }

        private void hijackInventoryPage(object sender, EventArgsClickableMenuChanged e)
        {
            if (Game1.activeClickableMenu is GameMenu)
            {
                GameMenu menu = (GameMenu)Game1.activeClickableMenu;
                if (menu.currentTab == GameMenu.inventoryTab)
                {
                    List<IClickableMenu> pages = (List<IClickableMenu>)typeof(GameMenu).GetField("pages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(menu);
                    pages.RemoveAt(0);
                    pages.Insert(0, new InventoryPage(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height));
                }
            }
        }

        private void checkCurrentTool(object sender, EventArgs e)
        {
            if (!isInitiated) return;
            if (verticalToolbar.numToolsinToolbar > 0 && Game1.player.CurrentToolIndex != currentToolIndex)
            {
                if (modOverride || (triggerPolling < 300))
                {
                    Game1.player.CurrentToolIndex = currentToolIndex;
                    modOverride = false;
                }
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

                if (Game1.player.items[currentToolIndex] != null)
                    break;
            }
            modOverride = true;
        }

        private void checkHoveredItemMouse(object sender, EventArgsMouseStateChanged e)
        {
            if (!isInitiated) return;
            if (verticalToolbar.numToolsinToolbar > 0 && Mouse.GetState().ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue)
            {
                int num = Mouse.GetState().ScrollWheelValue > Game1.oldMouseState.ScrollWheelValue ? -1 : 1;
                checkHoveredItem(num);
            }
        }

        private void chooseToolKey(object sender, EventArgsKeyboardStateChanged e)
        {
            if (!Game1.player.UsingTool && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                if (e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[0].name);
                else if (e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[1].name);
                else if (e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[2].name);
                else if (e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[3].name);
                else if (e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && e.NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad5))
                    currentToolIndex = Convert.ToInt32(verticalToolbar.buttons[4].name);

                modOverride = true;
            }
        }

        private Toolbar getToolbar()
        {
            for (int index = 0; index < Game1.onScreenMenus.Count; ++index)
            {
                if (Game1.onScreenMenus[index] is Toolbar)
                {
                    return Game1.onScreenMenus[index] as Toolbar;
                }
            }

            return null;
        }

        private void initializeMod(object sender, EventArgs e)
        {
            verticalToolbar = new VerticalToolBar(getToolbar().xPositionOnScreen - (VerticalToolBar.getInitialWidth() / 2), Game1.viewport.Height - VerticalToolBar.getInitialHeight());
            Game1.onScreenMenus.Add(verticalToolbar);

            currentToolIndex = Game1.player.CurrentToolIndex;
            isInitiated = true;
        }
    }
}