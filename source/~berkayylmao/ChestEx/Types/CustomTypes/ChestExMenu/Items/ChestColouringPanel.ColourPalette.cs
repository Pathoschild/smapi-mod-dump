/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
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

using System;
using System.IO;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
   public partial class ChestColouringPanel {
      public class ColourPalette : BaseTypes.ICustomItemGrabMenuItem.BasicComponent {
         // Private:
         #region Private

         // for (Drawing).GetPixel(x,y)
         private System.Drawing.Bitmap _colourPalette;
         // to show/paint on menu
         private Chest _liveDummyChest;

         private Color _originalColour;

         #endregion

         // Protected:
         #region Protected

         // Overrides:
         #region Overrides

         protected override Texture2D GetTexture(GraphicsDevice device) {
            if (_colourPalette is null || this.texture is null) {
               // dispose of old values
               _colourPalette?.Dispose();
               this.texture?.Dispose();

               // save current bounds
               var bounds = this.Bounds;

               // colour palette
               var blend = new System.Drawing.Drawing2D.ColorBlend
               {
                  Colors = new System.Drawing.Color[] { System.Drawing.Color.Red, System.Drawing.Color.Orange, System.Drawing.Color.Yellow, System.Drawing.Color.Lime, System.Drawing.Color.Cyan, System.Drawing.Color.Blue, System.Drawing.Color.Indigo, System.Drawing.Color.Violet, System.Drawing.Color.HotPink },
                  Positions = new Single[] { 0f, 1 / 8f, 2 / 8f, 3 / 8f, 4 / 8f, 5 / 8f, 6 / 8f, 7 / 8f, 1f }
               };

               // prep palette
               _colourPalette = new System.Drawing.Bitmap(bounds.Width, bounds.Height);

               // prep temporary palette graphics items
               using var graphics = System.Drawing.Graphics.FromImage(_colourPalette);
               using var gradientColourPalette = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.RectangleF(0f, 0f, bounds.Width, bounds.Height), System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, 0f) { InterpolationColors = blend };
               using var gradientFadeToBlack = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.RectangleF(0f, 0f, bounds.Width, bounds.Height * 0.4f), System.Drawing.Color.Black, System.Drawing.Color.Transparent, 90f);
               using var gradientFadeToWhite = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.RectangleF(0f, bounds.Height * 0.6f, bounds.Width, bounds.Height * 0.4f), System.Drawing.Color.Transparent, System.Drawing.Color.White, 90f);

               // draw palette
               graphics.FillRectangle(gradientColourPalette, 0f, 0f, bounds.Width, bounds.Height);
               graphics.FillRectangle(gradientFadeToBlack, 0f, 0f, bounds.Width, bounds.Height * 0.4f);
               graphics.FillRectangle(gradientFadeToWhite, 0f, (bounds.Height * 0.6f), bounds.Width, bounds.Height * 0.4f);

               // export to MemoryStream
               using var s = new MemoryStream();
               _colourPalette.Save(s, System.Drawing.Imaging.ImageFormat.Png);
               s.Seek(0, SeekOrigin.Begin);

               // draw to Texture2D
               this.texture = Texture2D.FromStream(device, s);
            }

            return this.texture;
         }

         #endregion

         #endregion

         // Public:
         #region Public

         public Color GetColourAt(Int32 x, Int32 y) {
            if (_colourPalette is null)
               return _originalColour;

            var bounds = this.Bounds;
            var colour = _originalColour;

            x -= bounds.X;
            y -= bounds.Y;
            if ((x >= 0 && x < _colourPalette.Width) &&
                (y >= 0 && y < _colourPalette.Height)) {
               var colourAtPixel = _colourPalette.GetPixel(x, y);

               colour = new Color(colourAtPixel.R, colourAtPixel.G, colourAtPixel.B);
            }

            return colour;
         }

         // Overrides:
         #region Overrides

         public override void Draw(SpriteBatch b) {
            if (!this.IsVisible)
               return;

            if (this.GetTexture(b.GraphicsDevice) is Texture2D texture)
               b.Draw(texture, this.Bounds, this.textureTintColourCurrent);

            if (this.cursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.hoverText))
               ICustomMenu.DrawHoverText(b, StardewValley.Game1.smallFont, this.hoverText, (8, 8, 8, 8), this.HostMenuItem.Colours.BackgroundColour, this.HostMenuItem.Colours.ForegroundColour);
         }

         public override void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
            // calc new bounds
            this.Bounds = this.HostMenuItem.HostMenu.ItemsToGrabMenu.GetBounds();

            // dispose of old palette
            _colourPalette?.Dispose();

            base.OnGameWindowSizeChanged(oldBounds, newBounds);
         }

         public override void OnMouseClick(ICustomMenu.MouseStateEx mouseState) {
            var pos_as_point = mouseState.Pos.AsXNAPoint();
            _originalColour = _liveDummyChest.playerChoiceColor.Value = this.GetColourAt(pos_as_point.X, pos_as_point.Y);

            base.OnMouseClick(mouseState);
         }

         public override void OnCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) {
            base.OnCursorMoved(e);

            if (!this.IsVisible)
               return;

            var coords = new Point(Convert.ToInt32(e.NewPosition.ScreenPixels.X), Convert.ToInt32(e.NewPosition.ScreenPixels.Y));

            // check if within bounds
            if (!this.Bounds.Contains(coords)) {
               this.hoverText = "";
               _liveDummyChest.playerChoiceColor.Value = _originalColour;
               return;
            }

            var new_colour = this.GetColourAt(coords.X, coords.Y);

            // change dummy chest colour
            _liveDummyChest.playerChoiceColor.Value = new_colour;

            // set hover text
            this.hoverText = $"#{new_colour.R:X2}{new_colour.G:X2}{new_colour.B:X2} {new_colour.ToString()}";
         }

         #endregion

         #endregion

         // Constructors:
         #region Constructors

         public ColourPalette(BaseTypes.ICustomItemGrabMenuItem hostMenuItem, Rectangle bounds, Chest chestToPaint, String componentName = "", EventHandler<ICustomMenu.MouseStateEx> onMouseClick = null) : base(hostMenuItem, bounds, false, componentName, onMouseClick) {
            _originalColour = chestToPaint.playerChoiceColor.Value;
            _liveDummyChest = chestToPaint;
         }

         #endregion

         // IDisposable:
         #region IDisposable

         public override void Dispose() {
            // dispose of palette
            _colourPalette?.Dispose();

            base.Dispose();
         }

         #endregion
      }
   }
}
