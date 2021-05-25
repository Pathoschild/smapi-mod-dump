/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// clang-format on

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;

using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
  public partial class ChestConfigPanel {
    public class ColourPalette : BasicComponent {
      // Private:
    #region Private

      // Enums:
    #region Enums

      private enum PaintMode {
        Chest,
        Hinges
      }

      private void flipPaintMode() {
        this.paintMode = this.paintMode == PaintMode.Chest ? PaintMode.Hinges : PaintMode.Chest;
        this.revertToOriginalColours();
      }

    #endregion

      // to show/paint on menu
      private ExtendedChest chest;

      private PaintMode paintMode = PaintMode.Chest;

      private Color originalChestColour;
      private Color originalChestHingesColour;

      // Hue Slider:
    #region Hue Slider

      public class HueSlider : IDisposable {
        // Consts:
      #region Consts

        public const  Int32 CONST_WIDTH           = 32;
        private const Int32 CONST_SELECTOR_HEIGHT = 4;

      #endregion

        // Private:
      #region Private

        private readonly Action<Color> onColourHovered;
        private readonly Action<Color> onColourChanged;

        private Bitmap    bitmap;
        private Texture2D texture;
        private Int32     selectorY;
        private Int32     selectorActiveY;
        private Color     selectorColour = Color.Red;

        private Texture2D getOrCreateTexture(GraphicsDevice device) {
          if (this.bitmap is not null && this.texture is not null) return this.texture;

          this.bitmap?.Dispose();
          this.texture?.Dispose();

          var blend = new ColorBlend {
            Colors = new[] {
              System.Drawing.Color.Red,
              System.Drawing.Color.Yellow,
              System.Drawing.Color.Lime,
              System.Drawing.Color.Cyan,
              System.Drawing.Color.Blue,
              System.Drawing.Color.Magenta,
              System.Drawing.Color.Red
            },
            Positions = new[] {
              0f,
              1 / 6f,
              2 / 6f,
              3 / 6f,
              4 / 6f,
              5 / 6f,
              1f
            }
          };

          this.bitmap = new Bitmap(CONST_WIDTH, this.mBounds.Height);

          using (var ms = new MemoryStream())
          using (Graphics g = Graphics.FromImage(this.bitmap))
          using (var gradient =
            new LinearGradientBrush(new RectangleF(0.0f, 0.0f, CONST_WIDTH, this.mBounds.Height), System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, 90.0f) {
              InterpolationColors = blend
            }) {
            g.FillRectangle(gradient, 0f, 0f, CONST_WIDTH, this.mBounds.Height);
            this.bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            this.texture = Texture2D.FromStream(device, ms);
          }

          return this.texture;
        }

        private Color getColourAt(Int32 y) { return this.bitmap.GetPixel(0, Math.Max(0, Math.Min(y, this.mBounds.Height - 1))).AsXNAColor(); }

        private void setSelectorY(Int32 y) {
          if (this.bitmap is null) return;
          if (this.selectorY == y) return;

          this.selectorY      = y;
          this.selectorColour = this.getColourAt(y);
          this.onColourHovered(this.selectorColour);
        }

      #endregion

        // Public:
      #region Public

        public Rectangle mBounds;

        public void Reset() { this.SetSelectorActiveY(0); }

        public void SetSelectorActiveY(Int32 y) {
          this.selectorY = this.selectorActiveY = y;
          if (this.bitmap is null) {
            if (Game1.graphics.GraphicsDevice is null) return;
            this.getOrCreateTexture(Game1.graphics.GraphicsDevice);
          }

          this.selectorColour = this.getColourAt(y);
          this.onColourChanged(this.selectorColour);
        }

        public void OnCursorMoved(Vector2 cursorPosition) {
          if (this.mBounds.Contains(cursorPosition.AsXNAPoint()))
            this.setSelectorY((Int32)cursorPosition.Y - this.mBounds.Y);
          else
            this.setSelectorY(this.selectorActiveY);
        }

        public void OnMouseClick(CustomMenu.MouseStateEx mouseState) {
          if (mouseState.mButton != SButton.MouseLeft) return;
          if (!this.mBounds.Contains(mouseState.mPos.AsXNAPoint())) return;
          this.SetSelectorActiveY((Int32)mouseState.mPos.Y - this.mBounds.Y);
        }

        public void Draw(SpriteBatch spriteBatch) {
          spriteBatch.Draw(this.getOrCreateTexture(spriteBatch.GraphicsDevice), new Vector2(this.mBounds.X, this.mBounds.Y), Color.White);

          // Draw the selector
          spriteBatch.Draw(Game1.uncoloredMenuTexture,
                           new Rectangle(this.mBounds.X, this.mBounds.Y + this.selectorY, CONST_WIDTH, CONST_SELECTOR_HEIGHT),
                           new Rectangle(64, 412, 16, 4),
                           Color.White);
          spriteBatch.Draw(Game1.uncoloredMenuTexture,
                           new Rectangle(this.mBounds.X + 12, this.mBounds.Y + this.selectorY - CONST_SELECTOR_HEIGHT - 4, 20, 12),
                           new Rectangle(152, 712, 20, 12),
                           Color.White,
                           MathHelper.PiOver2,
                           Vector2.Zero,
                           SpriteEffects.FlipVertically,
                           this.mBounds.Y / 10000.0f);
        }

      #endregion

        // Constructors:
      #region Constructors

        public HueSlider(Rectangle bounds, Action<Color> onColourHovered, Action<Color> onColourChanged) {
          this.mBounds         = bounds;
          this.onColourHovered = onColourHovered;
          this.onColourChanged = onColourChanged;
        }

      #endregion

        // IDisposable:
      #region IDisposable

        public void Dispose() {
          this.bitmap?.Dispose();
          this.texture?.Dispose();
          this.bitmap  = null;
          this.texture = null;
        }

      #endregion
      }

      private readonly Dictionary<PaintMode, HueSlider> hueSliders = new();

    #endregion

      // Colour Picker:
    #region Colour Picker

      public class ColourPicker : IDisposable {
        // Consts:
      #region Consts

        public const Int32 CONST_X             = HueSlider.CONST_WIDTH + CONST_BORDER_WIDTH;
        public const Int32 CONST_SELECTOR_SIZE = 16;

      #endregion

        // Private:
      #region Private

        private readonly Action<Color> onColourHovered;
        private readonly Action<Color> onColourChanged;

        private Bitmap    bitmap;
        private Texture2D texture;
        private Boolean   selectorIsVisible = true;
        private Point     selectorPos       = Point.Zero;
        private Point     selectorActivePos = Point.Zero;
        private Color     hueColour         = Color.Red;

        private Texture2D getOrCreateTexture(GraphicsDevice device) {
          if (this.bitmap is not null && this.texture is not null) return this.texture;

          this.bitmap?.Dispose();
          this.texture?.Dispose();

          this.bitmap = new Bitmap(this.mBounds.Width, this.mBounds.Height);

          using (var ms = new MemoryStream())
          using (Graphics g = Graphics.FromImage(this.bitmap))
          using (var white_gradient = new LinearGradientBrush(new RectangleF(0.0f, 0.0f, this.mBounds.Width, this.mBounds.Height),
                                                              System.Drawing.Color.Transparent,
                                                              System.Drawing.Color.White,
                                                              0.0f))
          using (var black_gradient = new LinearGradientBrush(new RectangleF(0.0f, 0.0f, this.mBounds.Width, this.mBounds.Height),
                                                              System.Drawing.Color.Transparent,
                                                              System.Drawing.Color.FromArgb(255, 1, 1, 1),
                                                              90.0f)) {
            g.FillRectangle(new SolidBrush(this.hueColour.AsDotNetColor()), 0.0f, 0.0f, this.mBounds.Width, this.mBounds.Height);
            g.FillRectangle(white_gradient, 0f, 0f, this.mBounds.Width, this.mBounds.Height);
            g.FillRectangle(black_gradient, 0f, 0f, this.mBounds.Width, this.mBounds.Height);
            this.bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            this.texture = Texture2D.FromStream(device, ms);
          }

          return this.texture;
        }

        private Color getColourAt(Point position) {
          return this.bitmap.GetPixel(Math.Max(0, Math.Min(position.X, this.mBounds.Width - 1)), Math.Max(0, Math.Min(position.Y, this.mBounds.Height - 1))).AsXNAColor();
        }

        private void setSelectorPos(Point position) {
          if (this.bitmap is null) return;

          this.selectorPos = new Point(Math.Min(this.mBounds.Width - CONST_SELECTOR_SIZE, Math.Max(0, position.X - 4)),
                                       Math.Min(this.mBounds.Height - CONST_SELECTOR_SIZE, Math.Max(0, position.Y - 4)));
          this.onColourHovered(this.getColourAt(position));
        }

      #endregion

        public Rectangle mBounds;

        // Public:
      #region Public

        public void SetHueColour(Color colour) {
          if (this.hueColour == colour) return;

          this.hueColour = colour;
          this.bitmap?.Dispose();
          this.texture?.Dispose();
          this.bitmap  = null;
          this.texture = null;
        }

        public void Reset() { this.selectorActivePos = this.selectorPos = Point.Zero; }

        public void SetSelectorVisible(Boolean isVisible) { this.selectorIsVisible = isVisible; }

        public void SetSelectorActivePos(Point position) {
          this.selectorActivePos = this.selectorPos = new Point(Math.Min(this.mBounds.Width - CONST_SELECTOR_SIZE, Math.Max(0, position.X - 4)),
                                                                Math.Min(this.mBounds.Height - CONST_SELECTOR_SIZE, Math.Max(0, position.Y - 4)));
          if (this.bitmap is null) {
            if (Game1.graphics.GraphicsDevice is null) return;
            this.getOrCreateTexture(Game1.graphics.GraphicsDevice);
          }

          Color colour = this.getColourAt(position);
          if (colour != Color.Black) this.onColourChanged(colour);
        }

        public void OnCursorMoved(Vector2 cursorPosition) {
          if (this.mBounds.Contains(cursorPosition.AsXNAPoint())) { this.setSelectorPos((cursorPosition - this.mBounds.ExtractXYAsXNAVector2()).AsXNAPoint()); }
          else
            this.selectorPos = this.selectorActivePos;
        }

        public void OnMouseClick(CustomMenu.MouseStateEx mouseState) {
          if (mouseState.mButton != SButton.MouseLeft) return;
          if (!this.mBounds.Contains(mouseState.mPos.AsXNAPoint())) return;
          this.SetSelectorActivePos((mouseState.mPos - this.mBounds.ExtractXYAsXNAVector2()).AsXNAPoint());
        }

        public void Draw(SpriteBatch spriteBatch) {
          spriteBatch.Draw(this.getOrCreateTexture(spriteBatch.GraphicsDevice), this.mBounds.ExtractXYAsXNAVector2(), Color.White);

          // Draw the selector
          if (this.selectorIsVisible) {
            spriteBatch.Draw(Game1.uncoloredMenuTexture,
                             new Rectangle(this.mBounds.X + this.selectorPos.X, this.mBounds.Y + this.selectorPos.Y, CONST_SELECTOR_SIZE, CONST_SELECTOR_SIZE),
                             new Rectangle(128, 128, 64, 64),
                             this.bitmap.GetPixel(this.selectorPos.X, this.selectorPos.Y).ContrastColour().AsXNAColor());
          }
        }

      #endregion

        // Constructors:
      #region Constructors

        public ColourPicker(Rectangle bounds, Action<Color> onColourHovered, Action<Color> onColourChanged) {
          this.mBounds         = bounds;
          this.onColourHovered = onColourHovered;
          this.onColourChanged = onColourChanged;
        }

      #endregion

        // IDisposable:
      #region IDisposable

        public void Dispose() {
          this.bitmap?.Dispose();
          this.texture?.Dispose();
          this.bitmap  = null;
          this.texture = null;
        }

      #endregion
      }

      private readonly Dictionary<PaintMode, ColourPicker> colourPickers = new();

    #endregion

      // Hex Menu
    #region Hex Menu

      private Boolean isShowingHexMenu {
        get => this.hexMenuTextBox?.mIsVisible ?? this.hexMenuOKButton?.mIsVisible ?? this.hexMenuCancelButton?.mIsVisible ?? false;
        set {
          this.hexMenuTextBox?.SetVisible(value);
          this.hexMenuOKButton?.SetEnabled(value);
          this.hexMenuOKButton?.SetVisible(value);
          this.hexMenuCancelButton?.SetEnabled(value);
          this.hexMenuCancelButton?.SetVisible(value);
        }
      }

      private CustomTextBox hexMenuTextBox;
      private CustomButton  hexMenuOKButton;
      private CustomButton  hexMenuCancelButton;

      private void createHexMenu() {
        this.hexMenuTextBox?.Dispose();
        this.hexMenuOKButton?.Dispose();
        this.hexMenuCancelButton?.Dispose();

        this.hexMenuTextBox =
          new CustomTextBox(new Rectangle(this.mBounds.Right - this.mBounds.Width / 2 - CONST_BORDER_WIDTH - 8 - (Int32)Game1.smallFont.MeasureString($"{this.paintMode}: ").X,
                                          this.mBounds.Bottom - 64 - 58,
                                          this.mBounds.Width / 2,
                                          52),
                            this.mHostMenuItem.mColours.mForegroundColour,
                            this.mHostMenuItem.mColours.mBackgroundColour,
                            String.Empty,
                            "#",
                            6,
                            $"{this.paintMode}:",
                            Color.White,
                            CustomTextBox.gAcceptHexOnly);
        Rectangle textbox_rect = this.hexMenuTextBox.mBounds;
        this.hexMenuOKButton     = new CustomButton(new Rectangle(textbox_rect.Right - 52, textbox_rect.Bottom + 8, 48, 48), overlayTexture: TexturePresets.gOKButtonTexture);
        this.hexMenuCancelButton = new CustomButton(new Rectangle(textbox_rect.Right - 104, textbox_rect.Bottom + 8, 48, 48), overlayTexture: TexturePresets.gCancelButtonTexture);
        this.isShowingHexMenu    = false;
      }

      private void showHexMenu() {
        this.createHexMenu();

        Color colour = this.paintMode == PaintMode.Chest ? this.originalChestColour : this.originalChestHingesColour;
        this.hexMenuTextBox.Text = $"{colour.R:X2}{colour.G:X2}{colour.B:X2}";
        this.hexMenuTextBox.SelectMe();
        this.isShowingHexMenu = true;
      }

      private void finalizeHexMenu(Boolean changeColour) {
        if (!changeColour || this.hexMenuTextBox.Text.Length != 6) {
          this.isShowingHexMenu = false;

          return;
        }

        try {
          Color colour = Color.FromNonPremultiplied(Int32.Parse(this.hexMenuTextBox.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                                                    Int32.Parse(this.hexMenuTextBox.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                                                    Int32.Parse(this.hexMenuTextBox.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                                                    255);

          if (this.paintMode == PaintMode.Chest) {
            this.changeChestColour(colour);
            this.setupMenuFromColour(this.chest.mChestColour);
          }
          else
            this.changeHingesColour(colour);
        }
        catch (Exception) { // ignored
        }
        finally { this.isShowingHexMenu = false; }
      }

    #endregion

      // Helper Functions
    #region Helper Functions

      private void changeChestColour(Color newColour, Boolean resetToDefault = false) {
        if (!resetToDefault && newColour == Color.Black) newColour = Color.FromNonPremultiplied(1, 1, 1, 255);

        this.chest.mChestColour                                                   = this.originalChestColour = newColour;
        this.mHostMenuItem.mHostMenu.GetSourceAs<Chest>().playerChoiceColor.Value = newColour;
        this.mHostMenuItem.mHostMenu.mSourceInventoryOptions.mBackgroundColour    = this.chest.GetActualColour();
      }

      private void changeHingesColour(Color newColour, Boolean resetToDefault = false) {
        if (this.originalChestColour == Color.Black) return;
        if (!resetToDefault && newColour == Color.Black) newColour = Color.FromNonPremultiplied(1, 1, 1, 255);

        this.originalChestHingesColour = this.chest.mHingesColour = newColour;
        this.mHostMenuItem.mHostMenu.GetSourceAs<Chest>().SetCustomConfigHingesColour(newColour);
      }

      private void revertToOriginalColours() {
        this.chest.mChestColour                                                = this.originalChestColour;
        this.chest.mHingesColour                                               = this.originalChestHingesColour;
        this.mHostMenuItem.mHostMenu.mSourceInventoryOptions.mBackgroundColour = this.chest.GetActualColour();
      }

      private void resetToDefaultColours() {
        this.resetSelectors();
        this.changeHingesColour(Color.Black, true);
        this.changeChestColour(Color.Black, true);
      }

      private void resetSelectors() {
        foreach (HueSlider slider in this.hueSliders.Values) slider.Reset();
        foreach (ColourPicker picker in this.colourPickers.Values) picker.Reset();
      }

      private void setupMenuFromColour(Color colour) {
        if (colour == Color.Black) {
          this.resetToDefaultColours();
          return;
        }

        System.Drawing.Color fw_colour = colour.AsDotNetColor();
        Int32                max       = Math.Max(fw_colour.R, Math.Max(fw_colour.G, fw_colour.B));
        Int32                min       = Math.Min(fw_colour.R, Math.Min(fw_colour.G, fw_colour.B));

        Single hue = fw_colour.GetHue() / 360.0f;
        Single sat = 1.0f - (max == 0 ? 0 : 1.0f - 1.0f * min / max);
        Single val = 1.0f - max / 255.0f;

        HueSlider    slider = this.hueSliders[this.paintMode];
        ColourPicker picker = this.colourPickers[this.paintMode];

        slider.SetSelectorActiveY((Int32)(slider.mBounds.Height * hue));
        picker.SetSelectorActivePos(new Point((Int32)(picker.mBounds.Width * sat), (Int32)(picker.mBounds.Height * val)));
      }

    #endregion

    #endregion

      // Public:
    #region Public

      // Consts:
    #region Consts

      public const Int32 CONST_BORDER_WIDTH = 16;

    #endregion

      // Overrides:
    #region Overrides

      public override void OnButtonPressed(ButtonPressedEventArgs e) {
        base.OnButtonPressed(e);

        if (this.isShowingHexMenu) this.hexMenuTextBox.OnButtonPressed(e);
      }

      public override void OnButtonReleased(ButtonReleasedEventArgs e) {
        base.OnButtonReleased(e);

        if (!this.mIsVisible || !this.isShowingHexMenu || e.Button != SButton.Enter && e.Button != SButton.Escape) return;

        GlobalVars.gSMAPIHelper.Input.Suppress(e.Button);
        this.finalizeHexMenu(e.Button == SButton.Enter);
      }

      public override void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
        this.mBounds = this.mHostMenuItem.mHostMenu.inventory.GetContentRectangle();

        this.resetSelectors();
        this.revertToOriginalColours();

        foreach (HueSlider slider in this.hueSliders.Values) slider.Dispose();
        foreach (ColourPicker picker in this.colourPickers.Values) picker.Dispose();

        this.hexMenuOKButton?.Dispose();
        this.hexMenuCancelButton?.Dispose();

        this.createHexMenu();
        base.OnGameWindowSizeChanged(oldBounds, newBounds);
      }

      public override void OnCursorMoved(Vector2 cursorPos) {
        base.OnCursorMoved(cursorPos);

        if (!this.mIsVisible || this.isShowingHexMenu) return;

        this.hueSliders[this.paintMode].OnCursorMoved(cursorPos);
        this.colourPickers[this.paintMode].OnCursorMoved(cursorPos);

        Point xna_cursor = cursorPos.AsXNAPoint();
        this.colourPickers[this.paintMode].SetSelectorVisible(!this.hueSliders[this.paintMode].mBounds.Contains(xna_cursor));
        if (!this.colourPickers[this.paintMode].mBounds.Contains(xna_cursor)) {
          this.mHoverText = String.Empty;
          this.revertToOriginalColours();
        }
      }

      public override void OnMouseClick(CustomMenu.MouseStateEx mouseState) {
        if (!this.mIsVisible) return;

        Point abs_coords = mouseState.mPos.AsXNAPoint();

        if (!this.mBounds.Contains(abs_coords)) return;

        if (GlobalVars.gSMAPIHelper.Input.IsDown(SButton.LeftControl) || GlobalVars.gSMAPIHelper.Input.IsDown(SButton.RightControl)) {
          if (mouseState.mButton == SButton.MouseMiddle) {
            Config.Get().mShowColourPaletteHelpTooltip = !Config.Get().mShowColourPaletteHelpTooltip;
            GlobalVars.gSMAPIHelper.WriteConfig(Config.Get());
          }
          else if (mouseState.mButton == SButton.MouseLeft) {
            this.mHoverText = String.Empty;
            this.revertToOriginalColours();
            this.showHexMenu();
          }

          this.OnCursorMoved(mouseState.mPos);
        }
        else {
          if (this.isShowingHexMenu) {
            if (mouseState.mButton != SButton.MouseLeft) return;
            if (this.hexMenuOKButton.mBounds.Contains(abs_coords))
              this.finalizeHexMenu(true);
            else if (this.hexMenuCancelButton.mBounds.Contains(abs_coords)) this.finalizeHexMenu(false);
          }
          else {
            switch (mouseState.mButton) {
              case SButton.MouseMiddle:
                this.resetToDefaultColours();
                break;
              case SButton.MouseRight:
                this.flipPaintMode();
                break;
              case SButton.MouseLeft: {
                this.hueSliders[this.paintMode].OnMouseClick(mouseState);
                this.colourPickers[this.paintMode].OnMouseClick(mouseState);
                break;
              }
            }

            base.OnMouseClick(mouseState);
          }
        }
      }

      public override void Draw(SpriteBatch b) {
        if (!this.mIsVisible) return;

        // Draw hue picker
        this.hueSliders[this.paintMode].Draw(b);

        // Draw border
        b.Draw(Game1.uncoloredMenuTexture,
               new Rectangle(this.hueSliders[this.paintMode].mBounds.Right, this.mBounds.Y - 4, CONST_BORDER_WIDTH, this.mBounds.Height + 8),
               new Rectangle(216, 936, CONST_BORDER_WIDTH, 1),
               this.mHostMenuItem.mColours.mBackgroundColour);

        // Draw colour picker
        this.colourPickers[this.paintMode].Draw(b);

        // Draw border
        b.Draw(Game1.uncoloredMenuTexture,
               new Rectangle(this.colourPickers[this.paintMode].mBounds.Right, this.mBounds.Y - 4, CONST_BORDER_WIDTH, this.mBounds.Height + 8),
               new Rectangle(216, 936, CONST_BORDER_WIDTH, 1),
               this.mHostMenuItem.mColours.mBackgroundColour);

        if (this.isShowingHexMenu) {
          b.Draw(Game1.fadeToBlackRect, new Rectangle(this.mBounds.X, this.mBounds.Y - 4, this.mBounds.Width - CONST_BORDER_WIDTH, this.mBounds.Height + 8), Color.Black * 0.65f);

          this.hexMenuTextBox?.Draw(b, false);
          this.hexMenuOKButton?.draw(b);
          this.hexMenuCancelButton?.draw(b);
        }
        else {
          // draw hover text
          if (this.mCursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.mHoverText)) {
            CustomMenu.DrawHoverText(b,
                                     Game1.smallFont,
                                     this.mHoverText,
                                     backgroundColour: this.mHostMenuItem.mColours.mBackgroundColour,
                                     textColour: this.mHostMenuItem.mColours.mForegroundColour);
          }
        }
      }

    #endregion

    #endregion

      // Constructors:
    #region Constructors

      public ColourPalette(CustomItemGrabMenuItem                hostMenuItem, Rectangle bounds, ExtendedChest chestToPaint, String componentName = "",
                           EventHandler<CustomMenu.MouseStateEx> onMouseClick = null) : base(hostMenuItem, bounds, true, componentName, onMouseClick) {
        this.chest                     = chestToPaint;
        this.originalChestColour       = chestToPaint.mChestColour;
        this.originalChestHingesColour = chestToPaint.mHingesColour;

        this.colourPickers[PaintMode.Chest] = new ColourPicker(new Rectangle(bounds.X + ColourPicker.CONST_X, bounds.Y, bounds.Height, bounds.Height),
                                                               c => {
                                                                 this.chest.mChestColour                                                = c;
                                                                 this.mHostMenuItem.mHostMenu.mSourceInventoryOptions.mBackgroundColour = this.chest.GetActualColour();

                                                                 this.mHoverText = Config.Get().mShowColourPaletteHelpTooltip ?
                                                                   $"Left Click: Change the chest's colour{Environment.NewLine}"
                                                                   + $"Right Click: Change to hinges colouring mode{Environment.NewLine}"
                                                                   + $"Middle Click: Reset all colours{Environment.NewLine}{Environment.NewLine}"
                                                                   + $"Ctrl + Left Click: Enter hex code{Environment.NewLine}"
                                                                   + "Ctrl + Middle Click: Toggle this tooltip" :
                                                                   $"Chest: #{c.R:X2}{c.G:X2}{c.B:X2} {c}";
                                                               },
                                                               c => this.changeChestColour(c));
        this.colourPickers[PaintMode.Hinges] = new ColourPicker(new Rectangle(bounds.X + ColourPicker.CONST_X, bounds.Y, bounds.Height, bounds.Height),
                                                                c => {
                                                                  if (c != Color.Black) this.chest.mHingesColour = c;
                                                                  this.mHoverText = Config.Get().mShowColourPaletteHelpTooltip ?
                                                                    $"Left Click: Change the hinges' colour{Environment.NewLine}"
                                                                    + $"Right Click: Change to chest colouring modes{Environment.NewLine}"
                                                                    + $"Middle Click: Reset all colours{Environment.NewLine}{Environment.NewLine}"
                                                                    + $"Ctrl + Left Click: Enter hex code{Environment.NewLine}"
                                                                    + "Ctrl + Middle Click: Toggle this tooltip" :
                                                                    $"Hinges: #{c.R:X2}{c.G:X2}{c.B:X2} {c}";
                                                                  if (this.chest.mChestColour == Color.Black)
                                                                    this.mHoverText =
                                                                      $"Can't change hinges' colour because the chest isn't colored!{Environment.NewLine}{Environment.NewLine}{this.mHoverText}";
                                                                },
                                                                c => {
                                                                  if (this.chest.mChestColour != Color.Black) this.changeHingesColour(c);
                                                                });

        this.hueSliders[PaintMode.Chest] = new HueSlider(new Rectangle(bounds.X, bounds.Y, HueSlider.CONST_WIDTH, bounds.Height),
                                                         this.colourPickers[PaintMode.Chest].SetHueColour,
                                                         this.colourPickers[PaintMode.Chest].SetHueColour);
        this.hueSliders[PaintMode.Hinges] = new HueSlider(new Rectangle(bounds.X, bounds.Y, HueSlider.CONST_WIDTH, bounds.Height),
                                                          this.colourPickers[PaintMode.Hinges].SetHueColour,
                                                          this.colourPickers[PaintMode.Hinges].SetHueColour);

        this.setupMenuFromColour(this.originalChestColour);
      }

    #endregion

      // IDisposable:
    #region IDisposable

      public override void Dispose() {
        base.Dispose();

        foreach (HueSlider slider in this.hueSliders.Values) slider.Dispose();
        foreach (ColourPicker picker in this.colourPickers.Values) picker.Dispose();
        this.hexMenuOKButton?.Dispose();
        this.hexMenuCancelButton?.Dispose();
      }

    #endregion
    }
  }
}
