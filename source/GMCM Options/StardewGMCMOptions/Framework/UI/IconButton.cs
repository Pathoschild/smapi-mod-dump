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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace GMCMOptions.Framework.UI {
    /// <summary>
    ///   An <c>IconButton</c> is a widget rendered as a "button" consisting of an icon and some text.  The widget's
    ///   value is a boolean specifying whether the button is "selected" or not.  Selected buttons render with a
    ///   different background color.  The selection status is not automatically updated, and should be set as
    ///   desired in the <c>onClick</c> callback supplied in the constructor.
    /// </summary>
    public class IconButton : ClickAndDragTracking {
        /// <summary>
        /// A function that toggles the selected state of the given button.  It may be convienient to use this
        /// function as the <c>onClick</c> argument to the constructor.
        /// </summary>
        /// <param name="btn">The button to toggle</param>
        public static void ToggleSelected(IconButton btn) {
            btn.Selected = !btn.Selected;
        }

        // constants controlling rendering layout
        const int margin = 5;
        const int textOffsetX = 5;

        // saved values of the constructor args
        private readonly Texture2D icon;
        private readonly Rectangle iconRect;
        private readonly String text;
        private readonly Action<IconButton> onClick;
        private readonly float fontScale;

        // additional values computed and cached for convenience in the constructor
        private readonly Color selectedBackgroundColor;
        private readonly int textOffsetY;
        readonly int width;
        readonly int height;

        public override int Width => width;
        public override int Height => height;

        /// <summary>Whether this widget is currently in the "selected" state or not</summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Construct a new icon button widget.
        /// </summary>
        /// <param name="icon">The icon to render</param>
        /// <param name="text">The text to render</param>
        /// <param name="onClick">A callback to invoke when the mouse is pressed inside the bounds of the widget</param>
        /// <param name="selected">Whether the widget is initially selected</param>
        /// <param name="fontScale">The scale to apply when drawing the text</param>
        /// <param name="selectedBackgroundColor">The background color to use when the widget is selected.  Defaults
        ///   to a mostly-transparent black.</param>
        public IconButton(Texture2D icon, Rectangle? sourceRect = null, String text = "", Action<IconButton> onClick = null, bool selected = true,
            float fontScale = 0.5f, Color? selectedBackgroundColor = null) {
            this.icon = icon;
            this.iconRect = sourceRect ?? new Rectangle(0, 0, icon.Width, icon.Height);
            this.text = text;
            this.onClick = onClick;
            this.Selected = selected;
            this.fontScale = fontScale;
            this.selectedBackgroundColor = selectedBackgroundColor ?? new Color(0, 0, 0, 24);
            Vector2 textSize = Game1.tinyFont.MeasureString(text) * fontScale;
            width = margin * 2 + iconRect.Width + (textSize.X == 0 ? 0 : textOffsetX) + (int)textSize.X;
            height = margin * 2 + Math.Max(iconRect.Height, (int)textSize.Y);
            textOffsetY = iconRect.Height > textSize.Y ? (iconRect.Height - (int)textSize.Y) / 2 : 0;
        }

        protected override void OnClick(int mouseX, int mouseY, int drawX, int drawY) {
            onClick?.Invoke(this);
        }

        /// <summary>
        /// Draw this widget at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, int posX, int posY) {
            UpdateMouseState(posX, posY);
            if (Selected) {
                b.Draw(ColorUtil.Pixel, new Rectangle(posX, posY, Width, Height), selectedBackgroundColor);
            }
            b.Draw(icon, new Vector2(posX + margin, posY + margin), iconRect, Color.White);
            Utility.drawBoldText(b, text, Game1.tinyFont, new Vector2(posX + margin + iconRect.Width + textOffsetX, posY + margin + textOffsetY), Color.Black, fontScale);

        }
    }
}
