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
    public class TabBar : Widget
    {
        public string[] Labels;

        private int _focusedIndex;
        public int FocusedIndex {
            get {
                return _focusedIndex;
            }
            set {
                _focusedIndex = Math.Max(0, Math.Min(value, Labels.Length-1));
            }
        }

        public TabBar(Rectangle bounds, string[] labels, IClickableMenu parent)
            : base(parent, bounds)
        {
            this.Labels = labels;
        }

        public override void draw(SpriteBatch b)
        {
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x, y+this.Bounds.Height-3, 21, 2));
            int xoff = 20;
            for (int i = 0; i < Labels.Length; ++i) {
                if (i == FocusedIndex) {
                    xoff += drawFocusedTab(b, Labels[i], xoff);
                }
                else {
                    xoff += drawUnfocusedTab(b, Labels[i], xoff);
                }
            }
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x+xoff-1, y+this.Bounds.Height-3, this.Bounds.Width-xoff+1, 2));
        }

        public int drawFocusedTab(SpriteBatch b, string text, int xoff)
        {
            int textWidth = (int)Game1.smallFont.MeasureString(text).X;
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X + xoff;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(48, 268, 4, 36),
                    destinationRectangle: new Rectangle(x, y+1, 2, this.Bounds.Height-3));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x+1, y, textWidth+5, 2));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(48, 268, 4, 36),
                    destinationRectangle: new Rectangle(x+textWidth+5, y+1, 2, this.Bounds.Height-3));
            Utility.drawTextWithShadow(b, text, Game1.smallFont,
                    new Vector2(x+4, y+2), Game1.textColor);
            return textWidth + 7;
        }

        public int drawUnfocusedTab(SpriteBatch b, string text, int xoff)
        {
            int textWidth = (int)Game1.smallFont.MeasureString(text).X;
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X + xoff;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;

            Color textAlpha = Game1.textColor;
            textAlpha.A /= 2;
            Utility.drawTextWithShadow(b, text, Game1.smallFont,
                    new Vector2(x+4, y+6), textAlpha);

            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(64, 324, 4, 52),
                    destinationRectangle: new Rectangle(x, y+5, 2, this.Bounds.Height-7));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(68, 376, 52, 4),
                    destinationRectangle: new Rectangle(x+1, y+4, textWidth+5, 2));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(64, 324, 4, 52),
                    destinationRectangle: new Rectangle(x+textWidth+5, y+5, 2, this.Bounds.Height-7));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x-1, y+this.Bounds.Height-3, textWidth+9, 2));
            return textWidth + 7;
        }

        public override void click(int x, int y, bool playSound = true)
        {
            int start = 20;
            for (int i = 0; i < Labels.Length; ++i) {
                int dx = (int)Game1.smallFont.MeasureString(Labels[i]).X + 7;
                if (x > start && x < start + dx) {
                    this.FocusedIndex = i;
                    if (this.parent is ShaderMenu m) {
                        m.onChildChange(this);
                    }
                    break;
                }
                start += dx;
            }
        }
    }
}
