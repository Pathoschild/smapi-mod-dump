/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// // Copyright 2022-2023 Jamie Taylor
using System;
using GMCMOptions.Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace GMCMOptions.Framework {
    public class ImagePickerOption {
        public enum ArrowLocation {
            Top = -1,
            Sides = 0,
            Bottom = 1
        }
        public enum LabelLocation {
            Top = -1,
            None = 0,
            Bottom = 1
        }

        // constants controlling rendering layout
        const int margin = 5;
        /// <summary>left arrow location in mouseCursors</summary>
        public static readonly Rectangle LeftArrow = new Rectangle(8, 268, 44, 40);
        /// <summary>right arrow location in mouseCursors</summary>
        public static readonly Rectangle RightArrow = new Rectangle(12, 204, 44, 40);



        // saved values from the constructor
        readonly Func<uint> GetValue;
        readonly Action<uint> SetValue;
        readonly Func<uint> GetMaxValue;
        readonly Func<int> MaxImageHeight;
        readonly Func<int> MaxImageWidth;
        readonly Action<uint, SpriteBatch, Vector2> DrawImage;
        readonly Func<uint, String?>? Label;
        readonly Func<uint, String?>? ImageTooltipTitle;
        readonly Func<uint, String?>? ImageTooltipText;
        readonly ArrowLocation arrowLocation;
        readonly LabelLocation labelLocation;
        readonly Action<uint>? onValueChange;

        // UI widgets
        readonly IconButton leftButton;
        readonly IconButton rightButton;
        readonly int arrowButtonHeight;

        // Our current value
        private uint _currentValue;
        private uint currentValue {
            get => _currentValue;
            set {
                if (_currentValue != value) {
                    onValueChange?.Invoke(value);
                }
                _currentValue = value;
            }
        }

        /// <summary>
        /// Create a new image picker option.  They underlying value is the <c>uint</c> that is the index
        /// passed to the <paramref name="drawImage"/> and <paramref name="label"/> functions.
        /// </summary>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="getMaxValue">
        ///   The maximum value this option can have, and thus the maximum value that will be passed to
        ///   <paramref name="drawImage"/> and <paramref name="label"/>.  Note that this is a function, so
        ///   theoretically the number of options does not have to be fixed.  Should this function return a
        ///   value greater than the option's current value then the option's current value will be clamped.
        ///   In common usage, this parameter should be a function that returns one less than the number
        ///   of images.
        /// </param>
        /// <param name="maxImageHeight">
        ///   A function that returns the maximum image height.  Used to center
        ///   arrows vertically in the <c cref="ImageOptionArrowLocation.Sides">Sides</c> arrow placement option.
        /// </param>
        /// <param name="maxImageWidth">
        ///   A function that returns the maximum image width.  This is used to place the arrows and label.
        /// </param>
        /// <param name="drawImage">A function which draws the image for the given index at the given location</param>
        /// <param name="label">A function to return the string to display given the image index, or <c>null</c> to disable that display.</param>
        /// <param name="imageTooltipTitle">
        ///   A function to return the string to use as the tooltip title when hovering over the image itself.
        ///   A <c>null</c> value or returning a <c>null</c> string will fall back to the value returned by
        ///   <paramref name="label"/>, or the empty string if that is null.  Whether the tooltip is displayed
        ///   is controlled by the <paramref name="imageTooltipText"/> parameter.
        /// </param>
        /// <param name="imageTooltipText">
        ///   A function to return the string to use as the tooltip text when hovering over the image itself.
        ///   A <c>null</c> value or returning a <c>null</c> string disables the tooltip.
        /// </param>
        /// <param name="arrowLocation">Where to draw the arrows in relation to the image.</param>
        /// <param name="labelLocation">Where to draw the label in relation to the image.</param>
        /// <param name="onValueChange">An action to invoke whenever the (current, unsaved) value changes</param>
        public ImagePickerOption(Func<uint> getValue,
                                 Action<uint> setValue,
                                 Func<uint> getMaxValue,
                                 Func<int> maxImageHeight,
                                 Func<int> maxImageWidth,
                                 Action<uint, SpriteBatch, Vector2> drawImage,
                                 Func<uint, String?>? label = null,
                                 Func<uint, String?>? imageTooltipTitle = null,
                                 Func<uint, String?>? imageTooltipText = null,
                                 ArrowLocation arrowLocation = ArrowLocation.Top,
                                 LabelLocation labelLocation = LabelLocation.Top,
                                 Action<uint>? onValueChange = null) {
            GetValue = getValue;
            SetValue = setValue;
            GetMaxValue = getMaxValue;
            MaxImageHeight = maxImageHeight;
            MaxImageWidth = maxImageWidth;
            DrawImage = drawImage;
            Label = label;
            ImageTooltipTitle = imageTooltipTitle;
            ImageTooltipText = imageTooltipText;
            this.arrowLocation = arrowLocation;
            this.labelLocation = label is not null ? labelLocation : LabelLocation.None;
            this.onValueChange = onValueChange;
            leftButton = new IconButton(Game1.mouseCursors, LeftArrow, "", LeftButtonClicked, false);
            rightButton = new IconButton(Game1.mouseCursors, RightArrow, "", RightButtonClicked, false);
            arrowButtonHeight = Math.Max(leftButton.Height, rightButton.Height);
        }

        /// <summary>
        /// Invoke the <c>setValue</c> callback passed to the constructor with this option's current state.
        /// </summary>
        public void SaveChanges() {
            SetValue(currentValue);
        }

        /// <summary>
        /// Reset this option's current state by fetching the current value from the <c>getValue</c> argument passed
        /// to the constructor.
        /// </summary>
        public void Reset() {
            currentValue = GetValue();
        }

        /// <summary>
        /// Return the maxiumum height that this option can occupy.
        /// </summary>
        /// <returns>Height in pixels</returns>
        public int Height() {
            int textHeight = 0;
            if (Label is not null) {
                textHeight = (int)Game1.smallFont.MeasureString("XXX").Y;
            }
            int top = Math.Max(arrowLocation == ArrowLocation.Top ? arrowButtonHeight : 0, labelLocation == LabelLocation.Top ? textHeight : 0);
            int bottom = Math.Max(arrowLocation == ArrowLocation.Bottom ? arrowButtonHeight : 0, labelLocation == LabelLocation.Bottom ? textHeight : 0);
            return MaxImageHeight() + (top == 0 ? 0 : top + margin) + (bottom == 0 ? 0 : bottom + margin);
        }

        private void LeftButtonClicked(IconButton button) {
            if (currentValue > 0) currentValue--;
        }
        private void RightButtonClicked(IconButton button) {
            if (currentValue < GetMaxValue()) currentValue++;
        }

        private void DrawArrows(SpriteBatch b, int top, int leftLeft, int rightLeft) {
            if (currentValue > 0) {
                leftButton.Draw(b, leftLeft, top);
            }
            if (currentValue < GetMaxValue()) {
                rightButton.Draw(b, rightLeft, top);
            }
        }
        private void DrawLabel(SpriteBatch b, int top, int left, int totalWidth, float textWidth, String label) {
            float leftPos = left + (totalWidth - textWidth) / 2;
            b.DrawString(Game1.smallFont, label, new Vector2(leftPos, top), Color.Black);
        }
        /// <summary>
        /// Draw this Option at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, Vector2 pos) {
            // if the set of choices has changed, clamp the current value
            currentValue = Math.Min(currentValue, GetMaxValue());
            int maxImageHeight = MaxImageHeight(); // don't compute multiple times per draw
            int maxImageWidth = MaxImageWidth(); // don't compute multiple times per draw
            int top = (int)pos.Y;
            int left = (int)pos.X;
            int imageLeft = arrowLocation == ArrowLocation.Sides ? left + leftButton.Width + margin : left;
            int rightArrowLeft = arrowLocation == ArrowLocation.Sides ? imageLeft + maxImageWidth + margin : left + Math.Max(maxImageWidth - rightButton.Width, leftButton.Width + margin);
            int totalWidth = rightArrowLeft + rightButton.Width - left;
            Vector2 textSize = Vector2.Zero;
            String label = "";
            if (Label is not null) {
                label = Label(currentValue) ?? "";
                textSize = Game1.smallFont.MeasureString(label);
            }
            // top row
            int maybeTopIncr = 0;
            if (arrowLocation == ArrowLocation.Top) {
                DrawArrows(b, top, left, rightArrowLeft);
                maybeTopIncr = arrowButtonHeight;
            }
            if (labelLocation == LabelLocation.Top) {
                DrawLabel(b, top, left, totalWidth, textSize.X, label);
                maybeTopIncr = Math.Max(maybeTopIncr, (int)textSize.Y);
            }
            top += (maybeTopIncr == 0 ? 0 : maybeTopIncr + margin);
            // middle row
            DrawImage(currentValue, b, new Vector2(imageLeft, top));
            bool isMouseOverImage = IsMouseOverRect(new Rectangle(imageLeft, top, maxImageWidth, maxImageHeight));
            if (arrowLocation == ArrowLocation.Sides) {
                DrawArrows(b, top + (maxImageHeight - arrowButtonHeight) / 2, left, rightArrowLeft);
            }
            top += maxImageHeight + margin;
            // bottom row
            if (arrowLocation == ArrowLocation.Bottom) {
                DrawArrows(b, top, left, rightArrowLeft);
            }
            if (labelLocation == LabelLocation.Bottom) {
                DrawLabel(b, top, left, totalWidth, textSize.X, label);
            }
            if (isMouseOverImage) {
                string? text = ImageTooltipText?.Invoke(currentValue);
                if (text is not null) {
                    string title = ImageTooltipTitle?.Invoke(currentValue) ?? label;
                    TooltipHelper.Title = title;
                    TooltipHelper.BodyText = text;
                }
            }
        }

        private bool IsMouseOverRect(Rectangle r) {
            // not sure whether this is necessary, but since I don't have an Android version to test...
            int mouseX;
            int mouseY;
            if (Constants.TargetPlatform == GamePlatform.Android) {
                mouseX = Game1.getMouseX();
                mouseY = Game1.getMouseY();
            } else {
                mouseX = Game1.getOldMouseX();
                mouseY = Game1.getOldMouseY();
            }
            return r.Contains(mouseX, mouseY);
        }

    }
}
