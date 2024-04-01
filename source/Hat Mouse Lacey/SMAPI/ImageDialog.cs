/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace ichortower_HatMouseLacey
{
    internal class ImageDialog : IClickableMenu
    {
        private static string imageOpenSfx = "breathin";
        private static string captionOpenSfx = "breathin";
        private static string closeSfx = "breathout";
        private static int transitionTime = 180;
        public enum WidgetState {
            NotShown,
            AnimatingIn,
            AwaitingInput,
            AnimatingOut,
        }

        private Texture2D texture;
        private string captionText = null!;
        private Point sourceTile;
        private TemporaryAnimatedSprite closeIcon;
        public WidgetState imageState = WidgetState.AnimatingIn;
        private Rectangle imageTransitionRect = new();
        private Rectangle imageNormalRect = new();
        private int imageTransitionStartTime = -1;
        public WidgetState captionState = WidgetState.NotShown;
        private Rectangle captionTransitionRect = new();
        private Rectangle captionNormalRect = new();
        private int captionTransitionStartTime = -1;

        public ImageDialog(int tileX, int tileY, string texturePath, string messageKey = null)
            : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height)
        {
            sourceTile = new Point(tileX, tileY);
            texture = HML.ModHelper.GameContent.Load<Texture2D>(texturePath);
            if (messageKey != null && messageKey != "") {
                captionText = Game1.content.LoadString(messageKey);
            }
            ScaleToFit();
            Game1.playSound(imageOpenSfx);
        }

        public void ScaleToFit()
        {
            Rectangle bounds = texture.Bounds;
            int scale = 4;
            while (scale > 1 && (bounds.Width * scale > Game1.uiViewport.Width ||
                        bounds.Height * scale > Game1.uiViewport.Height)) {
                --scale;
            }
            int width = bounds.Width * scale;
            int height = bounds.Height * scale;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            imageNormalRect = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            if (captionText != null) {
                width = Math.Min(1240, SpriteText.getWidthOfString(captionText) + 64);
                height = SpriteText.getHeightOfString(captionText, width - 20) + 4;
                pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
                captionNormalRect = new Rectangle((int)pos.X,
                        Game1.uiViewport.Height - height - 64, width, height);
            }
        }

        public void SetupCloseIcon()
        {
            Vector2 iconPosition = new Vector2(
                    captionNormalRect.X + captionNormalRect.Width - 40,
                    captionNormalRect.Y + captionNormalRect.Height - 44);
            closeIcon = new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                    new Rectangle(289, 342, 11, 12), 80f, 11, 999999,
                    iconPosition, flicker: false, flipped: false, 0.89f, 0f,
                    Color.White, 4f, 0f, 0f, 0f, local: true);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (imageState == WidgetState.AnimatingIn ||
                    imageState == WidgetState.AnimatingOut) {
                if (imageTransitionStartTime == -1) {
                    imageTransitionStartTime = (int)time.TotalGameTime.TotalMilliseconds;
                }
                int elapsed = (int)time.TotalGameTime.TotalMilliseconds -
                        imageTransitionStartTime;
                int percent = elapsed * 100 / ImageDialog.transitionTime;
                if (percent >= 100) {
                    imageTransitionStartTime = -1;
                    if (imageState == WidgetState.AnimatingOut) {
                        exitThisMenu(false);
                        imageState = WidgetState.NotShown;
                    }
                    else {
                        imageState = WidgetState.AwaitingInput;
                    }
                }
                // animate from map tile to final location
                Rectangle start = new(sourceTile.X * 64 + 32 - Game1.viewport.X,
                        sourceTile.Y * 64 + 16 - Game1.viewport.Y, 0, 0);
                Rectangle end = imageNormalRect;
                // or, in reverse if animating out
                if (imageState == WidgetState.AnimatingOut) {
                    Rectangle temp = start;
                    start = end;
                    end = temp;
                }
                imageTransitionRect = new Rectangle(
                        start.X + (end.X - start.X) * percent / 100,
                        start.Y + (end.Y - start.Y) * percent / 100,
                        start.Width + (end.Width - start.Width) * percent / 100,
                        start.Height + (end.Height - start.Height) * percent / 100);
            }

            if (captionState == WidgetState.AnimatingIn ||
                    captionState == WidgetState.AnimatingOut) {
                if (captionTransitionStartTime == -1) {
                    captionTransitionStartTime = (int)time.TotalGameTime.TotalMilliseconds;
                }
                int elapsed = (int)time.TotalGameTime.TotalMilliseconds -
                        captionTransitionStartTime;
                int percent = elapsed * 100 / ImageDialog.transitionTime;
                if (percent >= 100) {
                    captionTransitionStartTime = -1;
                    if (captionState == WidgetState.AnimatingOut) {
                        captionState = WidgetState.NotShown;
                    }
                    else {
                        captionState = WidgetState.AwaitingInput;
                        SetupCloseIcon();
                    }
                }
                Rectangle start = new(captionNormalRect.X + captionNormalRect.Width/2,
                        captionNormalRect.Y + captionNormalRect.Height/2, 0, 0);
                Rectangle end = captionNormalRect;
                if (captionState == WidgetState.AnimatingOut) {
                    Rectangle temp = start;
                    start = end;
                    end = temp;
                }
                captionTransitionRect = new Rectangle(
                        start.X + (end.X - start.X) * percent / 100,
                        start.Y + (end.Y - start.Y) * percent / 100,
                        start.Width + (end.Width - start.Width) * percent / 100,
                        start.Height + (end.Height - start.Height) * percent / 100);
            }

            closeIcon?.update(time);
        }

        public override void draw(SpriteBatch b)
        {
            if (imageState == WidgetState.AwaitingInput) {
                b.Draw(texture, imageNormalRect, null, Color.White);
            }
            else if (imageState == WidgetState.AnimatingIn ||
                    imageState == WidgetState.AnimatingOut) {
                b.Draw(texture, imageTransitionRect, null, Color.White);
            }

            if (captionState == WidgetState.AwaitingInput) {
                drawBox(b, captionNormalRect);
                // draw text
                SpriteText.drawString(b, captionText, captionNormalRect.X + 8,
                        captionNormalRect.Y + 8, 999999, captionNormalRect.Width);
                // close icon is a TAS so ignore it
            }
            else if (captionState == WidgetState.AnimatingIn ||
                    captionState == WidgetState.AnimatingOut) {
                drawBox(b, captionTransitionRect);
            }
            closeIcon?.draw(b, localPosition: true);
            base.drawMouse(b);
        }

        public void drawBox(SpriteBatch b, Rectangle rect)
        {
            int x = rect.X;
            int y = rect.Y;
            int w = rect.Width;
            int h = rect.Height;
            if (w == 0 || h == 0) {
                return;
            }
            b.Draw(Game1.mouseCursors, rect,
                    new Rectangle(306, 320, 16, 16), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(x, y-20, w, 24),
                    new Rectangle(275, 313, 1, 6), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(x+12, y+h, w-20, 32),
                    new Rectangle(275, 328, 1, 8), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(x-32, y+24, 32, h-28),
                    new Rectangle(264, 325, 8, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(x+w, y, 28, h),
                    new Rectangle(293, 324, 7, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(x-44, y-28),
                    new Rectangle(261, 311, 14, 13), Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(x+w-8, y-28),
                    new Rectangle(291, 311, 12, 11), Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(x+w-8, y+h-8),
                    new Rectangle(291, 326, 12, 12), Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(x-44, y+h-4),
                    new Rectangle(261, 327, 14, 11), Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ScaleToFit();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (imageState != WidgetState.AwaitingInput) {
                return;
            }
            if (captionState == WidgetState.AwaitingInput) {
                closeIcon = null;
                captionState = WidgetState.NotShown;
                imageState = WidgetState.AnimatingOut;
                imageTransitionRect = imageNormalRect;
                if (playSound) {
                    Game1.playSound(closeSfx);
                }
            }
            else if (captionText != null && captionState == WidgetState.NotShown) {
                captionState = WidgetState.AnimatingIn;
                if (playSound) {
                    Game1.playSound(captionOpenSfx);
                }
            }
            else {
                imageState = WidgetState.AnimatingOut;
                imageTransitionRect = imageNormalRect;
                if (playSound) {
                    Game1.playSound(closeSfx);
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            receiveLeftClick(x, y, playSound);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key != Keys.None &&
                    (Game1.options.doesInputListContain(Game1.options.menuButton, key) ||
                    Game1.options.doesInputListContain(Game1.options.actionButton, key))) {
                receiveLeftClick(0, 0);
            }
        }
    }

}
