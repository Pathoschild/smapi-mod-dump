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
    public class Label : Widget
    {
        public string Text;
        public Widget ActivateWidget;
        public Alignment HAlign = Alignment.Left;
        public Alignment VAlign = Alignment.Center;

        public Label(IClickableMenu parent, Rectangle bounds, string text,
                string hoverText = null, Widget activate = null)
            : base(parent, bounds)
        {
            this.Text = text;
            this.HoverText = hoverText;
            this.ActivateWidget = activate;
            _resize();
        }

        private void _resize()
        {
            Vector2 dimen = Game1.smallFont.MeasureString(Text);
            Bounds.Width = Math.Max(Bounds.Width, (int)dimen.X);
        }

        public override void draw(SpriteBatch b)
        {
            _resize();
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width,
                    this.Bounds.Height);
            int xw = Bounds.Width;
            Vector2 screenpos = new(
                    this.HAlign switch {
                        Alignment.Left => screenb.X,
                        Alignment.Center => screenb.Center.X - xw/2,
                        Alignment.Right => screenb.X + screenb.Width - xw,
                        _ => screenb.X
                    },
                    this.VAlign switch {
                        Alignment.Top => screenb.Y,
                        Alignment.Center => screenb.Center.Y - Game1.smallFont.LineSpacing/2,
                        Alignment.Bottom => screenb.Y + screenb.Height - Game1.smallFont.LineSpacing,
                        _ => screenb.Y
                    }
            );
            Utility.drawTextWithShadow(b, this.Text, Game1.smallFont,
                    screenpos, Game1.textColor);
        }

        public override void click(int x, int y, bool playSound = true)
        {
            this.ActivateWidget?.click(x, y, playSound);
        }
    }

    public enum Alignment {
        Left,
        Center,
        Right,
        Top,
        Bottom
    }
}
