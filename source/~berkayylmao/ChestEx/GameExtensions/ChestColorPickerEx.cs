/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

/*
   MIT License

   Copyright (c) 2019 Berkay Yigit <berkay2578@gmail.com>
       Copyright holder detail: Nickname(s) used by the copyright holder: 'berkay2578', 'berkayylmao'.

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx {
   public class ChestColorPickerEx : IClickableMenu {
      public Boolean visible;
      public Microsoft.Xna.Framework.Color chestColor;
      public Chest dummyChest;

      private Bitmap canvas;
      private Graphics graphics;
      private LinearGradientBrush spectrumGradient, darkMixGradient, lightMixGradient;
      private Microsoft.Xna.Framework.Graphics.Texture2D canvasTexture;
      private Boolean canvasTextureUpdated;
      private Chest actualChest;

      private void setupBrushes() {
         if (this.canvas != null) {
            this.canvas.Dispose();
            this.canvas = null;
         }
         this.canvas = new Bitmap(this.width, this.height);

         if (this.graphics != null) {
            this.graphics.Dispose();
            this.graphics = null;
         }
         this.graphics = Graphics.FromImage(this.canvas);

         if (this.spectrumGradient != null) {
            this.spectrumGradient.Dispose();
            this.spectrumGradient = null;
         }
         var blend = new ColorBlend {
            Colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue, Color.Indigo, Color.Violet, },
            Positions = new Single[] { 0f, 1 / 7f, 2 / 7f, 3 / 7f, 4 / 7f, 5 / 7f, 6 / 7f, 1f }
         };

         this.spectrumGradient = new LinearGradientBrush(new RectangleF(0f, 0f, this.width, this.height), Color.Transparent, Color.Transparent, 0f) {
            InterpolationColors = blend
         };

         if (this.darkMixGradient != null) {
            this.darkMixGradient.Dispose();
            this.darkMixGradient = null;
         }
         this.darkMixGradient = new LinearGradientBrush(new RectangleF(0f, 0f, this.width, this.height * 0.3f), Color.Black, Color.Transparent, 90f);

         if (this.lightMixGradient != null) {
            this.lightMixGradient.Dispose();
            this.lightMixGradient = null;
         }
         this.lightMixGradient = new LinearGradientBrush(new RectangleF(0f, this.height * 0.7f, this.width, this.height * 0.3f), Color.Transparent, Color.White, 90f);
      }
      private void drawPicker() {
         this.graphics.FillRectangle(this.spectrumGradient, 0f, 0f, this.width, this.height);
         this.graphics.FillRectangle(this.darkMixGradient, 0f, 0f, this.width, this.height * 0.3f);
         this.graphics.FillRectangle(this.lightMixGradient, 0f, (this.height * 0.7f) + 1f /* just some glitch */, this.width, this.height * 0.3f);

         this.canvasTextureUpdated = false;
      }
      private void convertPickerToTexture2D(Microsoft.Xna.Framework.Graphics.GraphicsDevice device) {
         if (this.canvasTexture != null) {
            this.canvasTexture.Dispose();
            this.canvasTexture = null;
         }
         using (MemoryStream s = new MemoryStream()) {
            this.canvas.Save(s, System.Drawing.Imaging.ImageFormat.Png);
            s.Seek(0, SeekOrigin.Begin);
            this.canvasTexture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(device, s);
         }
         this.canvasTextureUpdated = true;

      }

      private Microsoft.Xna.Framework.Color getColorAtPixel(Int32 x, Int32 y) {
         Microsoft.Xna.Framework.Color c = chestColor;
         if (this.canvasTexture == null)
            return c;

         x -= this.xPositionOnScreen;
         y -= this.yPositionOnScreen;
         if ((x >= 0 && x < this.canvasTexture.Width) &&
            (y >= 0 && y < this.canvasTexture.Height)) {
            var winColor = this.canvas.GetPixel(x, y);
            if (winColor.R <= 3 && winColor.G <= 3 && winColor.B <= 3) // block default chest color
               return c;
            else
               return new Microsoft.Xna.Framework.Color(winColor.R, winColor.G, winColor.B);
         }

         return c;
      }
      public override void performHoverAction(Int32 x, Int32 y) {
         if (!this.visible)
            return;

         this.dummyChest.playerChoiceColor.Value = getColorAtPixel(x, y);
         this.dummyChest.resetLidFrame();
      }

      public override void receiveLeftClick(Int32 x, Int32 y, Boolean playSound = true) {
         if (!this.visible)
            return;
         base.receiveLeftClick(x, y, playSound);

         this.actualChest.playerChoiceColor.Value = this.dummyChest.playerChoiceColor.Value = chestColor = getColorAtPixel(x, y);
         this.dummyChest.resetLidFrame();
      }

      public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds) {
         base.gameWindowSizeChanged(oldBounds, newBounds);

         this.setupBrushes();
         this.drawPicker();
      }

      public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch b) {
         if (this.visible) {
            if (!this.canvasTextureUpdated)
               this.convertPickerToTexture2D(b.GraphicsDevice);

            Game1.drawDialogueBox(
               this.xPositionOnScreen - 128,
               this.yPositionOnScreen - 96,
               64 + 64,
               92 + 128,
               false, true, null, false, true, 60, 50, 40);

            this.dummyChest.draw(b,
               this.xPositionOnScreen - 128 - 6 + IClickableMenu.borderWidth,
               this.yPositionOnScreen - 4 + IClickableMenu.borderWidth,
               1f, true);

            Game1.drawDialogueBox(
               this.xPositionOnScreen - 32,
               this.yPositionOnScreen - 96,
               this.width + 64,
               this.height + 128,
               false, true, null, false, true, 60, 50, 40);

            b.Draw(this.canvasTexture, new Microsoft.Xna.Framework.Vector2(this.xPositionOnScreen, this.yPositionOnScreen), Microsoft.Xna.Framework.Color.White);
         }
      }

      public ChestColorPickerEx(Int32 x, Int32 y, Int32 width, Int32 height, Chest actualChest) {
         this.visible = Game1.player.showChestColorPicker;

         this.xPositionOnScreen = x;
         this.yPositionOnScreen = y;
         this.width = width;
         this.height = height;

         this.actualChest = actualChest;
         this.dummyChest = new Chest(true);
         this.dummyChest.playerChoiceColor.Value = this.chestColor = actualChest.playerChoiceColor.Value;
         this.dummyChest.resetLidFrame();

         this.setupBrushes();
         this.drawPicker();
      }
   }
}
