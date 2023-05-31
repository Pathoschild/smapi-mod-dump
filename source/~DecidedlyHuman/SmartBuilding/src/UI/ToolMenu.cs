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
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SmartBuilding.UI
{
    public class ToolMenu : IClickableMenu
    {
        private readonly ModState modState;
        private readonly List<ToolButton> toolButtons;
        private readonly Texture2D windowSkin;
        private int currentMouseX = 0;
        private int currentMouseY = 0;

        // Debug gubbins.
        private string debugString;

        // Input state gubbins
        private bool leftMouseDown;

        private Logger logger;
        private int previousMouseX;
        private int previousMouseY;
        private Texture2D toolButtonSpritesheet;
        private bool windowBeingDragged;

        public ToolMenu(Logger l, Texture2D buttonSpritesheet, List<ToolButton> buttons, ModState modState)
        {
            int startingXPos = (int)MathF.Round(100 * Game1.options.uiScale);
            int startingYPos = (int)MathF.Round(100 * Game1.options.uiScale);
            int startingWidth = 100;
            int startingHeight = 0;
            this.modState = modState;
            this.toolButtonSpritesheet = buttonSpritesheet;
            this.windowSkin = this.windowSkin;
            this.toolButtons = buttons;

            // startingHeight = 64 * (toolButtons.Count + 1)

            // startingHeight += 4 * 4;

            // First, we increment our height by 64 for every button, unless it's a layer button.
            foreach (var button in this.toolButtons)
                if (button.Type != ButtonType.Layer)
                    startingHeight += 64;

            // Then, we add 8 per button to allow 8 pixels of spacing between buttons.
            startingHeight += this.toolButtons.Count * 8;

            this.initialize(startingXPos, startingYPos, startingWidth, startingHeight);
            this.logger = l;
        }

        public bool Enabled { get; set; } = false;

        public override void draw(SpriteBatch b)
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            if (this.modState.ActiveTool != ButtonId.None)
                if (this.modState.ActiveTool == ButtonId.Erase)
                {
                    DecidedlyShared.Ui.Utils.DrawBox(
                        b,
                        Game1.menuTexture,
                        new Rectangle(0, 256, 60, 60),
                        this.xPositionOnScreen + 64, this.yPositionOnScreen,
                        this.width + 32 + 8,
                        64 * 4 + 8 * 8 + 64,
                        16, 12, 16, 12
                    );

                    DecidedlyShared.Ui.Utils.DrawBox(
                        b,
                        Game1.menuTexture,
                        new Rectangle(0, 256, 60, 60),
                        this.xPositionOnScreen + 64, this.yPositionOnScreen,
                        this.width + 32 + 8,
                        64 * 4 + 8 * 8 + 64,
                        16, 12, 16, 12
                    );
                }

            DecidedlyShared.Ui.Utils.DrawBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                this.xPositionOnScreen, this.yPositionOnScreen,
                this.width, this.height,
                16, 12, 16, 12
            );

            foreach (var button in this.toolButtons) button.Draw(b);

            foreach (var button in this.toolButtons)
                if (button.IsHovered)
                    // b.DrawString(Game1.smallFont, button.ButtonTooltip, new Vector2(Game1.getMouseX() + 79, Game1.getMouseY() + 1), Color.Black);
                    Utility.drawTextWithColoredShadow(b, button.ButtonTooltip, Game1.dialogueFont,
                        new Vector2(Game1.getMouseX() + 78, Game1.getMouseY()),
                        Color.WhiteSmoke, new Color(Color.Black, 0.75f));
            //drawToolTip(b, button.ButtonTooltip, "Title", null, new Vector2(Game1.getMouseX() + 78, Game1.getMouseY()), Color.White);
            this.drawMouse(b);
            base.draw(b);
        }

        public override void update(GameTime time)
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            // MouseState mouseState = Game1.input.GetMouseState();
            // int currentMouseX = mouseState.X;
            // int currentMouseY = mouseState.Y;
            //
            // currentMouseX = (int)MathF.Floor(currentMouseX / Game1.options.uiScale);
            // currentMouseY = (int)MathF.Floor(currentMouseY / Game1.options.uiScale);
            //
            // DoWindowDrag(currentMouseX, currentMouseY);
            this.UpdateComponents();
        }

        private void DoWindowDrag(int x, int y)
        {
            if (!this.Enabled || !this.windowBeingDragged)
                return;
        }

        private void UpdateComponents()
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            var startingBounds = new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16, 64, 64);

            foreach (var button in this.toolButtons)
            {
                button.Component.bounds = startingBounds;

                startingBounds.Y += 64 + 8;
                // startingBounds.Y += 74;
            }

            foreach (var button in this.toolButtons)
                if (button.Type == ButtonType.Layer)
                {
                    button.Component.bounds = new Rectangle(button.Component.bounds.X + 64 + 32,
                        button.Component.bounds.Y - 430,
                        button.Component.bounds.Width,
                        button.Component.bounds.Height);

                    if (button.LayerToTarget == TileFeature.Furniture)
                        button.Component.bounds.Height = 128;
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
            if (!this.Enabled)
                return;

            // This is where we'll loop through all of our buttons, and perform actions appropriately.
            foreach (var button in this.toolButtons)
                if (button.Component.containsPoint(x, y))
                {
                    if (button.Type == ButtonType.Layer)
                    {
                        if (this.modState.ActiveTool != ButtonId.None)
                            if (this.modState.ActiveTool == ButtonId.Erase)
                                button.ButtonAction();
                    }
                    else
                    {
                        this.modState.SelectedLayer = TileFeature.None;
                        button.ButtonAction();
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
            this.LockWithinBounds(ref this.xPositionOnScreen, ref this.yPositionOnScreen);
        }

        public void MiddleMouseReleased(int x, int y)
        {
            if (!this.Enabled)
                return;

            this.windowBeingDragged = false;
        }

        public void MiddleMouseHeld(int x, int y)
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            // This is where we'll handle moving the UI. It doesn't matter which element the cursor is over.
            if (this.isWithinBounds(x, y) || this.windowBeingDragged)
            {
                this.windowBeingDragged = true;

                var newBounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

                int xDelta = x - this.previousMouseX;
                int yDelta = y - this.previousMouseY;

                this.xPositionOnScreen += xDelta;
                this.yPositionOnScreen += yDelta;

                // THIS IS THE GOOD ONE.
                // xPositionOnScreen = x - newBounds.Width / 2;
                // yPositionOnScreen = (int)Math.Round(y - newBounds.Width * 0.5f);

                //SnapToPixels();
                this.LockWithinBounds(ref this.xPositionOnScreen, ref this.yPositionOnScreen);

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
            if (x + this.width + 110 > Game1.uiViewport.Width)
                x = Game1.uiViewport.Width - this.width - 110;
            if (y + this.height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - this.height;
        }

        //private int stringThing = "YOU NEED TO ADD A LAYER BUTTON FOR DRAWN TILES SO THE ERASE TOOL CAN REMOVE THOSE, WHILE NOT REMOVING ANYTHING IN THE WORLD.";

        public void SetCursorHoverState(int x, int y)
        {
            this.modState.BlockMouseInteractions = this.isWithinBounds(x, y);
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

            foreach (var button in this.toolButtons)
                if (button.IsHovered)
                    return true;

            return isInMainWindowBounds;
        }

        public void DoHover(int x, int y)
        {
            // If the menu isn't enabled, just return.
            if (!this.Enabled)
                return;

            //logger.Log($"DoHover coords: {x}x{y}");

            foreach (var button in this.toolButtons)
                if (button.Component.containsPoint(x, y))
                {
                    // If it's a layer button, we only want to do anything if erase is the currently selected tool.
                    if (button.Type == ButtonType.Layer)
                    {
                        if (this.modState.ActiveTool != ButtonId.None && this.modState.ActiveTool == ButtonId.Erase)
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

            this.previousMouseX = x;
            this.previousMouseY = y;
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
