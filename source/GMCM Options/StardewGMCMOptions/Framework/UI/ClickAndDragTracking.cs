/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2022 Jamie Taylor
﻿using System;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace GMCMOptions.Framework.UI {
    /// <summary>
    /// A base class for widgets that need to track mouse clicks or drags.
    /// The <c cref="UpdateMouseState(int, int)">UpdateMouseState</c> method should be called at regular intervals
    /// (e.g., at the beginning of the widget's <c>Draw</c> method), and may result in one or both of the
    /// <c cref="OnClick(int, int, int, int)">OnClick</c> or <c cref="OnDrag(int, int, int, int)">OnDrag</c> methods
    /// being called.
    /// </summary>
    public abstract class ClickAndDragTracking {
        /// <summary>The width of the widget</summary>
        public abstract int Width { get; }
        /// <summary>The height of the widget</summary>
        public abstract int Height { get; }

        /// <summary>
        ///   Called when the mouse is pressed while inside the bounds of the widget
        /// </summary>
        protected virtual void OnClick(int mouseX, int mouseY, int drawX, int drawY) { }
        /// <summary>
        ///   Called when the mouse is pressed while inside the bounds of the widget, and with each
        ///   subsequent (distinct) position of the mouse until it is released.
        /// </summary>
        protected virtual void OnDrag(int mouseX, int mouseY, int drawX, int drawY) { }

        public ClickAndDragTracking() {
        }

        private ButtonState lastButtonState = ButtonState.Released;
        private int lastMouseX = 0;
        private int lastMouseY = 0;
        private bool dragging = false;

        /// <summary>
        ///   Update the mouse state, detecting click and drag behavior.  This method should be called at regular
        ///   intervals (e.g., at the beginning of the widget's <c>Draw</c> method), and may result in one or both of the
        ///   <c cref="OnClick(int, int, int, int)">OnClick</c> or <c cref="OnDrag(int, int, int, int)">OnDrag</c> methods
        ///   being called.
        /// </summary>
        /// <param name="drawX">The X coordinate of the position on the screen where the widget is drawn</param>
        /// <param name="drawY">The Y coordinate of the position on the screen where the widget is drawn</param>
        protected void UpdateMouseState(int drawX, int drawY) {
            ButtonState buttonState = Mouse.GetState().LeftButton;
            if (buttonState == ButtonState.Pressed) {
                int mouseX = Game1.getMouseX();
                int mouseY = Game1.getMouseY();
                if (drawX <= mouseX && mouseX <= drawX + Width
                    && drawY <= mouseY && mouseY <= drawY + Height
                    && lastButtonState == ButtonState.Released) {
                    dragging = true;
                    lastMouseX = lastMouseY = 0;
                    OnClick(mouseX, mouseY, drawX, drawY);
                }
                if (dragging) {
                    if (lastMouseX != mouseX || lastMouseY != mouseY) {
                        lastMouseX = mouseX;
                        lastMouseY = mouseY;
                        OnDrag(mouseX, mouseY, drawX, drawY);
                    }
                }
            } else {
                dragging = false;
            }
            lastButtonState = buttonState;
        }
    }
}
