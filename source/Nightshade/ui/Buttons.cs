/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ichortower.ui
{
    public class TextButton : Widget
    {
        public static int yPadding = 4;
        public static int xPadding = 10;

        public string Text;
        public string Sound = "drumkit6";
        public Action ClickDelegate = null;

        public TextButton(IClickableMenu parent, int x, int y,
                string text, string hoverText = null, Action onClick = null)
            : base(parent)
        {
            Bounds = new Rectangle(x, y,
                    (int)Game1.smallFont.MeasureString(text).X + 2*xPadding,
                    Game1.smallFont.LineSpacing + 2*yPadding);
            Text = text;
            HoverText = hoverText;
            ClickDelegate = onClick;
        }

        public TextButton(IClickableMenu parent, Rectangle bounds,
                string text, string hoverText = null, Action onClick = null)
            : base(parent, bounds)
        {
            ClickDelegate = onClick;
        }

        public override void click(int x, int y, bool playSound = true)
        {
            if (playSound) {
                Game1.playSound(Sound);
            }
            ClickDelegate?.Invoke();
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            ButtonShared.drawFrame(this, b, screenb);
            b.DrawString(Game1.smallFont, this.Text,
                    new Vector2(screenb.X+xPadding, screenb.Y+yPadding), Game1.textColor);
        }
    }

    public class IconButton : Widget
    {
        public static int defaultWidth = 30;
        public static int defaultHeight = 30;
        public static int iconXY = 22;
        public static Texture2D IconTexture = null;

        public string Sound = "drumkit6";
        public int IconIndex = 0;
        public Action ClickDelegate = null;

        public IconButton(IClickableMenu parent, int x, int y,
                int iconIndex, string hoverText = null, Action onClick = null)
            : this(parent, new Rectangle(x, y, defaultWidth, defaultHeight),
                    iconIndex, hoverText, onClick)
        {
        }

        public IconButton(IClickableMenu parent, Rectangle bounds,
                int iconIndex, string hoverText = null, Action onClick = null)
            : base(parent, bounds)
        {
            IconIndex = iconIndex;
            HoverText = hoverText;
            ClickDelegate = onClick;
            IconTexture = ShaderMenu.IconTexture;
        }

        public override void click(int x, int y, bool playSound = true)
        {
            if (playSound) {
                Game1.playSound(Sound);
            }
            ClickDelegate?.Invoke();
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            ButtonShared.drawFrame(this, b, screenb);
            int offset = (screenb.Width - iconXY) / 2;
            b.Draw(IconTexture, color: Game1.textColor,
                    sourceRectangle: new Rectangle(IconIndex*iconXY, 0, iconXY, iconXY),
                    destinationRectangle: new Rectangle(screenb.X+offset, screenb.Y+offset, iconXY, iconXY));
        }

    }

    public class ButtonShared
    {
        public static void drawFrame(Widget w, SpriteBatch b, Rectangle bounds)
        {
            int boxX = w.InHoverState && !w.InActiveState ? 267 : 256;
            Rectangle[] sources = Widget.nineslice(new Rectangle(boxX, 256, 10, 10), 2, 2);
            Rectangle[] dests = Widget.nineslice(bounds, 4, 4);
            for (int i = 0; i < sources.Length; ++i) {
                b.Draw(Game1.mouseCursors, color: Color.White,
                        sourceRectangle: sources[i],
                        destinationRectangle: dests[i]);
            }
        }
    }
}
