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
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ichortower.ui
{
    public class Slider : Widget
    {
        public static int defaultWidth = 201;
        public static int defaultHeight = 20;

        public int[] Range = new int[2] {-100, 100};
        private int _value;
        public int Value
        {
            get => _value;
            set {
                _value = Math.Max(Range[0], Math.Min(value, Range[1]));
            }
        }

        public Func<int, string> ValueDelegate = null;

        public Slider(IClickableMenu parent, int xpos, int ypos,
                string name = "", int initial = 0)
            : this(parent, new Rectangle(xpos, ypos, Slider.defaultWidth, Slider.defaultHeight),
                    name, initial)
        {
        }

        public Slider(IClickableMenu parent, Rectangle bounds,
                string name = "", int initial = 0, int[] range = null)
            : base(parent, bounds, name)
        {
            if (range != null && range.Length > 1) {
                this.Range = new int[2] {range[0], range[1]};
            }
            this.Value = initial;
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            b.Draw(Game1.mouseCursors, color: Color.White,
                    sourceRectangle: new Rectangle(228, 425, 6, 2),
                    destinationRectangle: new Rectangle(screenb.X, screenb.Center.Y - 2, screenb.Width, 2));
            b.Draw(Game1.mouseCursors, color: Color.White,
                    sourceRectangle: new Rectangle(228, 425, 6, 2),
                    destinationRectangle: new Rectangle(screenb.X, screenb.Center.Y, screenb.Width, 2),
                    effects: SpriteEffects.FlipVertically,
                    rotation: 0f, origin: Vector2.Zero, layerDepth: 0f);
            int dist = (int)((float)(this.Value-this.Range[0]) /
                    (float)(this.Range[1]-this.Range[0]) * screenb.Width);
            int boxX = InHoverState ? 267 : 256;
            b.Draw(Game1.mouseCursors, color: Color.White,
                    sourceRectangle: new Rectangle(boxX, 256, 3, 10),
                    destinationRectangle: new Rectangle(screenb.X + dist - 6, screenb.Y, 6, 20));
            b.Draw(Game1.mouseCursors, color: Color.White,
                    sourceRectangle: new Rectangle(boxX+7, 256, 3, 10),
                    destinationRectangle: new Rectangle(screenb.X + dist, screenb.Y, 6, 20));
            string disp = this.ValueDelegate?.Invoke(this.Value) ?? $"{this.Value}";
            Utility.drawTextWithShadow(b, disp, Game1.smallFont,
                    new Vector2(screenb.X + screenb.Width + 4, screenb.Y - 4),
                    Game1.textColor);
        }

        public override void click(int x, int y, bool playSound = true)
        {
            int prev = this.Value;
            this.Value = (int)Utility.Lerp(Range[0], Range[1],
                    (float)(x-Bounds.X) / (float)Bounds.Width);
            if (prev != this.Value && this.parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }

        public override void clickHold(int x, int y)
        {
            this.click(x, y);
        }

        public override void keyPress(Keys key)
        {
            int prev = this.Value;
            if (key == Keys.Right || Game1.options.doesInputListContain(Game1.options.moveRightButton, key)) {
                this.Value += 1;
            }
            else if (key == Keys.Left || Game1.options.doesInputListContain(Game1.options.moveLeftButton, key)) {
                this.Value -= 1;
            }
            if (prev != this.Value && this.parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }

        public override void scrollWheel(int direction)
        {
            int prev = Value;
            Value += Math.Sign(direction);
            if (prev != Value && parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }

        public Func<int, string> FloatRenderer(float denom)
        {
            return (val) => string.Format("{0:0.00}", (float)val/denom);
        }
    }
}
