using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewValleyEsp.Menu
{
    class Scrollbar : IClickableMenu
    {
        private readonly ClickableTextureComponent bar;
        private readonly ClickableTextureComponent upArrow;
        private readonly ClickableTextureComponent downArrow;
        private Rectangle runner;

        public int Pages { get; set; }
        public int Position { get; set; } = 0;
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Scrollbar(int x, int y, int h, int pages)
        {
            upArrow = new ClickableTextureComponent(new Rectangle(x + 16, y, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
            downArrow = new ClickableTextureComponent(new Rectangle(x + 16, y + h, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
            bar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40),
                Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
            Top = upArrow.bounds.Y + upArrow.bounds.Height + 4;
            Bottom = h - Top + 8;
            Left = bar.bounds.X;
            Right = Left + bar.bounds.Width;
            runner = new Rectangle(Left, Top, bar.bounds.Width, Bottom);
            Pages = pages;
        }

        public void SetBarAt(int i)
        {
            if (i >= Pages)
                bar.bounds.Y = runner.Bottom - bar.bounds.Height;
            else if (i <= 0)
                bar.bounds.Y = runner.Top;
            else
                bar.bounds.Y = (int)(runner.Top + (1f * i / Pages) * (runner.Height - bar.bounds.Height));
        }

        public override void draw(SpriteBatch b)
        {
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), runner.X, runner.Y, runner.Width, runner.Height, Color.White, 4f, false);
            upArrow.draw(b);
            downArrow.draw(b);
            if (Pages > 1)
                bar.draw(b);
        }
    }
}
