/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DecidedlyShared.Logging;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SmartBuilding.UI
{
    public class ToolMenu : IClickableMenu
    {
        private List<ToolButton> toolButtons;
        private Texture2D toolButtonSpritesheet;
        private Texture2D windowSkin;
        private bool enabled = false;
        private ModState modState;

        private int currentMouseX = 0;
        private int currentMouseY = 0;
        private int previousMouseX = 0;
        private int previousMouseY = 0;

        private Logger logger;

        // Input state gubbins
        private bool leftMouseDown;
        private bool windowBeingDragged;

        // Debug gubbins.
        private string debugString;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public ToolMenu(Logger l, Texture2D buttonSpritesheet, List<ToolButton> buttons, ModState modState)
        {
            int startingXPos = (int)MathF.Round(100 * Game1.options.uiScale);
            int startingYPos = (int)MathF.Round(100 * Game1.options.uiScale);
            int startingWidth = 100;
            int startingHeight = 0;
            this.modState = modState;
            toolButtonSpritesheet = buttonSpritesheet;
            this.windowSkin = windowSkin;
            toolButtons = buttons;

            // startingHeight = 64 * (toolButtons.Count + 1)

            // startingHeight += 4 * 4;

            // First, we increment our height by 64 for every button, unless it's a layer button.
            foreach (ToolButton button in toolButtons)
            {
                if (button.Type != ButtonType.Layer)
                    startingHeight += 64;
            }
            
            // Then, we add 8 per button to allow 8 pixels of spacing between buttons.
            startingHeight += toolButtons.Count * 8;

            base.initialize(startingXPos, startingYPos, startingWidth, startingHeight);
            logger = l;
        }

        public override void draw(SpriteBatch b)
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;

            if (modState.ActiveTool != ButtonId.None)
            {
                if (modState.ActiveTool == ButtonId.Erase)
                {
                    drawTextureBox(
                        b,
                        Game1.menuTexture,
                        new Rectangle(0, 256, 60, 60),
                        xPositionOnScreen + 64,
                        yPositionOnScreen,
                        width + 32 + 8,
                        64 * 4 + 8 * 8 + 64,
                        Color.White,
                        1f,
                        true
                    );
                }
            }
            
            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                1f,
                true
            );
            
            // drawTextureBox(
            //     b,
            //     xPositionOnScreen,
            //     yPositionOnScreen,
            //     width,
            //     height,
            //     Color.White
            // );

            foreach (ToolButton button in toolButtons)
            {
                button.Draw(b);
            }
            
            foreach (ToolButton button in toolButtons)
            {
                if (button.IsHovered)
                {
                    // b.DrawString(Game1.smallFont, button.ButtonTooltip, new Vector2(Game1.getMouseX() + 79, Game1.getMouseY() + 1), Color.Black);
                    Utility.drawTextWithColoredShadow(b, button.ButtonTooltip, Game1.dialogueFont, new Vector2(Game1.getMouseX() + 78, Game1.getMouseY()), Color.WhiteSmoke, new Color(Color.Black, 0.75f));
                    //drawToolTip(b, button.ButtonTooltip, "Title", null, new Vector2(Game1.getMouseX() + 78, Game1.getMouseY()), Color.White);
                }
            }

            drawMouse(b);
            base.draw(b);
        }

        public override void update(GameTime time)
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;
            
            // MouseState mouseState = Game1.input.GetMouseState();
            // int currentMouseX = mouseState.X;
            // int currentMouseY = mouseState.Y;
            //
            // currentMouseX = (int)MathF.Floor(currentMouseX / Game1.options.uiScale);
            // currentMouseY = (int)MathF.Floor(currentMouseY / Game1.options.uiScale);
            //
            // DoWindowDrag(currentMouseX, currentMouseY);
            UpdateComponents();
        }

        private void DoWindowDrag(int x, int y)
        {
            if (!enabled || !windowBeingDragged)
                return;
            
            
        }

        private void UpdateComponents()
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;

            Rectangle startingBounds = new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16, 64, 64);

            foreach (ToolButton button in toolButtons)
            {
                button.Component.bounds = startingBounds;

                startingBounds.Y += 64 + 8;
                // startingBounds.Y += 74;
            }

            foreach (ToolButton button in toolButtons)
            {
                if (button.Type == ButtonType.Layer)
                {
                    button.Component.bounds = new Rectangle(button.Component.bounds.X + 64 + 32, button.Component.bounds.Y - 430, button.Component.bounds.Width, button.Component.bounds.Height);
                    
                    if (button.LayerToTarget == TileFeature.Furniture)
                        button.Component.bounds.Height = 128;
                }
            }

            // foreach (ToolButton button in toolButtons)
            // {
            //     // if (button.Type == ButtonType.Tool)
            //     // {
            //     //     if (ModState.ActiveTool.HasValue)
            //     //     {
            //     //         if (button.Id.Equals(ModState.ActiveTool))
            //     //             button.CurrentOverlayColour = Color.Red;
            //     //         else
            //     //             button.CurrentOverlayColour = Color.White;
            //     //     }
            //     //     else
            //     //         button.CurrentOverlayColour = Color.White;
            //     // }
            // }
        }

        public void receiveLeftClickOutOfBounds(int x, int y)
        {
            // // If the menu isn't enabled, just return.
            // if (!enabled)
            //     return;
            //
            // // This is where we'll look through all of our buttons, and perform actions appropriately.
            // foreach (ToolButton button in toolButtons)
            // {
            //     if (button.Component.containsPoint(x, y))
            //     {
            //         if (button.Type == ButtonType.Layer)
            //         {
            //             if (ModState.ActiveTool.HasValue)
            //             {
            //                 if (ModState.ActiveTool.Value == ButtonId.Erase)
            //                     button.ButtonAction();
            //             }
            //         }
            //     }
            // }
            //
            // // This is where we'll loop through all of our buttons, and perform actions appropriately.
            // foreach (ToolButton button in toolButtons)
            // {
            //     if (button.Component.containsPoint(x, y))
            //     {
            //         if (button.Type != ButtonType.Layer)
            //         {
            //             ModState.SelectedLayer = null;
            //             button.ButtonAction();
            //         }
            //         
            //         // if (button.Type == ButtonType.Layer)
            //         // {
            //         //     if (ModState.ActiveTool.HasValue)
            //         //     {
            //         //         if (ModState.ActiveTool.Value == ButtonId.Erase)
            //         //             button.ButtonAction();
            //         //     }
            //         // }
            //         // else
            //         // {
            //         //     ModState.SelectedLayer = null;
            //         //     button.ButtonAction();
            //         // }
            //         // // First, we check to see if a button is a too
            //         // if (button.Type == ButtonType.Tool)
            //         //     ModState.ActiveTool = button.Id;
            //         //
            //         // if (button.Type == ButtonType.Function)
            //         // {
            //         //     if (button.Id == ButtonId.ConfirmBuild)
            //         //         confirmBuild();
            //         //     
            //         //     if (button.Id == ButtonId.ClearBuild)
            //         //         clearBuild();
            //         // }
            //     }
            // }
            
        }

        public void ReceiveLeftClick(int x, int y)
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;
            
            // This is where we'll loop through all of our buttons, and perform actions appropriately.
            foreach (ToolButton button in toolButtons)
            {
                if (button.Component.containsPoint(x, y))
                {
                    if (button.Type == ButtonType.Layer)
                    {
                        if (modState.ActiveTool != ButtonId.None)
                        {
                            if (modState.ActiveTool == ButtonId.Erase)
                                button.ButtonAction();
                        }
                    }
                    else
                    {
                        modState.SelectedLayer = TileFeature.None;
                        button.ButtonAction();
                    }
                }
            }
            
            // // This is where we'll loop through all of our buttons, and perform actions appropriately.
            // foreach (ToolButton button in toolButtons)
            // {
            //     if (button.Component.containsPoint(x, y))
            //     {
            //         if (button.Type != ButtonType.Layer)
            //         {
            //             ModState.SelectedLayer = null;
            //             button.ButtonAction();
            //         }
            //         
            //         // if (button.Type == ButtonType.Layer)
            //         // {
            //         //     if (ModState.ActiveTool.HasValue)
            //         //     {
            //         //         if (ModState.ActiveTool.Value == ButtonId.Erase)
            //         //             button.ButtonAction();
            //         //     }
            //         // }
            //         // else
            //         // {
            //         //     ModState.SelectedLayer = null;
            //         //     button.ButtonAction();
            //         // }
            //         // // First, we check to see if a button is a too
            //         // if (button.Type == ButtonType.Tool)
            //         //     ModState.ActiveTool = button.Id;
            //         //
            //         // if (button.Type == ButtonType.Function)
            //         // {
            //         //     if (button.Id == ButtonId.ConfirmBuild)
            //         //         confirmBuild();
            //         //     
            //         //     if (button.Id == ButtonId.ClearBuild)
            //         //         clearBuild();
            //         // }
            //     }
            // }
        }
        
        // public override void receiveLeftClick(int x, int y, bool playSound = true)
        // {
        //     // If the menu isn't enabled, just return.
        //     if (!enabled)
        //         return;
        //
        //     // This is where we'll loop through all of our buttons, and perform actions appropriately.
        //     foreach (ToolButton button in toolButtons)
        //     {
        //         if (button.Component.containsPoint(x, y))
        //         {
        //             if (button.Type != ButtonType.Layer)
        //             {
        //                 ModState.SelectedLayer = null;
        //                 button.ButtonAction();
        //             }
        //             
        //             // if (button.Type == ButtonType.Layer)
        //             // {
        //             //     if (ModState.ActiveTool.HasValue)
        //             //     {
        //             //         if (ModState.ActiveTool.Value == ButtonId.Erase)
        //             //             button.ButtonAction();
        //             //     }
        //             // }
        //             // else
        //             // {
        //             //     ModState.SelectedLayer = null;
        //             //     button.ButtonAction();
        //             // }
        //             // // First, we check to see if a button is a too
        //             // if (button.Type == ButtonType.Tool)
        //             //     ModState.ActiveTool = button.Id;
        //             //
        //             // if (button.Type == ButtonType.Function)
        //             // {
        //             //     if (button.Id == ButtonId.ConfirmBuild)
        //             //         confirmBuild();
        //             //     
        //             //     if (button.Id == ButtonId.ClearBuild)
        //             //         clearBuild();
        //             // }
        //         }
        //     }
        // }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            LockWithinBounds(ref xPositionOnScreen, ref yPositionOnScreen);
        }

        public void MiddleMouseReleased(int x, int y)
        {
            if (!enabled)
                return;

            windowBeingDragged = false;
        }

        public void MiddleMouseHeld(int x, int y)
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;

            // This is where we'll handle moving the UI. It doesn't matter which element the cursor is over.
            if (this.isWithinBounds(x, y) || windowBeingDragged)
            {
                windowBeingDragged = true;
                
                Rectangle newBounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);

                int xDelta = x - previousMouseX;
                int yDelta = y - previousMouseY;
                
                xPositionOnScreen += xDelta;
                yPositionOnScreen += yDelta;
                
                // THIS IS THE GOOD ONE.
                // xPositionOnScreen = x - newBounds.Width / 2;
                // yPositionOnScreen = (int)Math.Round(y - newBounds.Width * 0.5f);
                
                //SnapToPixels();
                LockWithinBounds(ref xPositionOnScreen, ref yPositionOnScreen);

                // xPositionOnScreen = x - xDelta;
                // yPositionOnScreen = y - yDelta;
                
                // xPositionOnScreen = (int)MathF.Round(xPositionOnScreen * Game1.options.uiScale);
                // yPositionOnScreen = (int)MathF.Round(yPositionOnScreen * Game1.options.uiScale);
            }
        }

        private void LockWithinBounds(ref int x, ref int y)
        {
            // First, we check to see if the window is out of bounds to the left or above.
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            
            // Then we check in the positive (to the right and down).
            if (x + width + 110 > Game1.uiViewport.Width)
                x = Game1.uiViewport.Width - width - 110;
            if (y + height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - height;
        }

        //private int stringThing = "YOU NEED TO ADD A LAYER BUTTON FOR DRAWN TILES SO THE ERASE TOOL CAN REMOVE THOSE, WHILE NOT REMOVING ANYTHING IN THE WORLD.";

        public void SetCursorHoverState(int x, int y)
        {
            modState.BlockMouseInteractions = isWithinBounds(x, y);
        }

        public override bool isWithinBounds(int x, int y)
        {
            bool isInMainWindowBounds = base.isWithinBounds(x, y);

            // if (!isInBounds)
            // {
            //     foreach (ToolButton button in toolButtons)
            //     {
            //         button.IsHovered = false;
            //         button.CurrentOverlayColour = Color.White;
            //     }
            // }

            foreach (ToolButton button in toolButtons)
            {
                if (button.IsHovered)
                    return true;
            }

            return isInMainWindowBounds;
        }

        public void DoHover(int x, int y)
        {
            // If the menu isn't enabled, just return.
            if (!enabled)
                return;
            
            //logger.Log($"DoHover coords: {x}x{y}");

            foreach (ToolButton button in toolButtons)
            {
                if (button.Component.containsPoint(x, y))
                {
                    // If it's a layer button, we only want to do anything if erase is the currently selected tool.
                    if (button.Type == ButtonType.Layer)
                    {
                        if (modState.ActiveTool != ButtonId.None && modState.ActiveTool == ButtonId.Erase)
                        {
                            // logger.Log($"Button {button.Id} hovered.");
                            button.CurrentOverlayColour = Color.Gray;
                            button.IsHovered = true;
                        }
                    }
                    else
                    {
                        // logger.Log($"Button {button.Id} hovered.");
                        button.CurrentOverlayColour = Color.Gray;
                        button.IsHovered = true;
                    }
                }
                else
                {
                    button.CurrentOverlayColour = Color.White;
                    button.IsHovered = false;
                }
            }
            
            previousMouseX = x;
            previousMouseY = y;
        }

        public override void performHoverAction(int x, int y)
        {
            //logger.Log($"performHoverAction coords: {x}x{y}");
            // // If the menu isn't enabled, just return.
            // if (!enabled)
            //     return;
            //
            // foreach (ToolButton button in toolButtons)
            // {
            //     if (button.Component.containsPoint(x, y))
            //     {
            //         button.CurrentOverlayColour = Color.Gray;
            //         button.IsHovered = true;
            //     }
            //     else
            //     {
            //         button.CurrentOverlayColour = Color.White;
            //         button.IsHovered = false;
            //     }
            // }
        }
    }
}