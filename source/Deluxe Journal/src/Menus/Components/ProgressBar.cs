/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public class ProgressBar : ClickableComponent
    {
        public enum TextAlignment
        {
            Center,
            Left,
            Right
        }

        public const int DefaultHeight = 48;

        public static readonly Color DefaultCompleteColor = new Color(38, 192, 32);
        public static readonly Color DefaultProgressColor = new Color(255, 145, 5);

        private readonly Rectangle _colorBarLeftSourceRect;
        private readonly Rectangle _colorBarMiddleSourceRect;
        private readonly Rectangle _colorBarRightSourceRect;
        private readonly Rectangle _fillSourceRect;
        private readonly int _sections;

        public Texture2D texture;
        public Rectangle barLeftSourceRect;
        public Rectangle barMiddleSourceRect;
        public Rectangle barRightSourceRect;
        public Rectangle notchSourceRect;

        private Color _progressColor;
        private Color _completeColor;

        public Vector2 TextMargin { get; set; }

        public TextAlignment AlignText { get; set; }

        public ProgressBar(Rectangle bounds, int sections)
            : this(bounds, sections, DefaultProgressColor, DefaultCompleteColor)
        {
        }

        public ProgressBar(Rectangle bounds, int sections, Color progressColor, Color completeColor)
            : base(bounds, string.Empty)
        {
            _sections = sections;
            _progressColor = progressColor;
            _completeColor = completeColor;
            _colorBarLeftSourceRect = new(0, 16, 2, 8);
            _colorBarMiddleSourceRect = new(2, 16, 36, 8);
            _colorBarRightSourceRect = new(38, 16, 2, 8);
            _fillSourceRect = new(47, 67, 1, 8);

            texture = Game1.mouseCursors2;
            barLeftSourceRect = new(0, 224, 6, 12);
            barMiddleSourceRect = new(6, 224, 35, 12);
            barRightSourceRect = new(41, 224, 6, 12);
            notchSourceRect = new(47, 224, 1, 12);

            TextMargin = new(80, 0);
            AlignText = TextAlignment.Left;
        }

        private static Color DarkenColor(Color color)
        {
            return new Color((int)(color.R * 0.8f) - 40, (int)(color.G * 0.8f) - 40, (int)(color.B * 0.8f) - 40, color.A);
        }

        public void Draw(SpriteBatch b, SpriteFont font, Color textColor, ColorSchema colorSchema, int currentCount, int maxCount)
        {
            float progress = (float)currentCount / maxCount;
            Color barColor = (progress >= 1f) ? _completeColor : _progressColor;

            Draw(b, font, textColor, barColor, colorSchema, currentCount, maxCount);
        }

        public void Draw(SpriteBatch b, SpriteFont font, Color textColor, Color barColor, ColorSchema colorSchema, int currentCount, int maxCount)
        {
            float progress = MathHelper.Clamp((float)currentCount / maxCount, 0, 1f);
            string text = currentCount + "/" + maxCount;
            Vector2 textSize = font.MeasureString(AlignText == TextAlignment.Center ? currentCount + "/" : text);
            int maxTextWidth = (int)Math.Max(font.MeasureString(maxCount + "/" + maxCount).X, textSize.X);
            int alignment = (int)Math.Ceiling(Math.Max(maxTextWidth + 4, TextMargin.X) / 40.0) * 40;
            int sections = (maxCount < _sections) ? maxCount : _sections;

            Rectangle barBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            switch (AlignText)
            {
                case TextAlignment.Left:
                    barBounds.Width -= alignment;
                    break;
                case TextAlignment.Right:
                    barBounds.Width -= alignment;
                    barBounds.X += alignment;
                    break;
            }

            b.Draw(texture, new Rectangle(barBounds.X, barBounds.Y, 24, barBounds.Height), barLeftSourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            b.Draw(texture, new Rectangle(barBounds.X + 24, barBounds.Y, barBounds.Width - 48, barBounds.Height), barMiddleSourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            b.Draw(texture, new Rectangle(barBounds.Right - 24, barBounds.Y, 24, barBounds.Height), barRightSourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

            barBounds.X += 12;
            barBounds.Y += 12;
            barBounds.Width -= 24;
            barBounds.Height -= 24;

            b.Draw(Game1.staminaRect, barBounds, colorSchema.Main);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.X, barBounds.Y, 8, barBounds.Height), _colorBarLeftSourceRect, colorSchema.Shadow);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.X + 8, barBounds.Y, barBounds.Width - 16, barBounds.Height), _colorBarMiddleSourceRect, colorSchema.Shadow);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.Right - 8, barBounds.Y, 8, barBounds.Height), _colorBarRightSourceRect, colorSchema.Shadow);

            for (int i = 1; i < sections; i++)
            {
                b.Draw(texture, new Vector2(barBounds.X + barBounds.Width * ((float)i / sections), (float)barBounds.Y), notchSourceRect, Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
            }

            barBounds.Width = (int)((barBounds.Width - 24) * progress) - 4;
            b.Draw(DeluxeJournalMod.UiTexture, barBounds, _fillSourceRect, barColor, 0, Vector2.Zero, SpriteEffects.None, 0.005f);

            barBounds.X += barBounds.Width;
            barBounds.Width = 4;
            b.Draw(DeluxeJournalMod.UiTexture, barBounds, _fillSourceRect, DarkenColor(barColor), 0, Vector2.Zero, SpriteEffects.None, 0.005f);

            switch (AlignText)
            {
                case TextAlignment.Center:
                    b.DrawString(font, text, new Vector2(bounds.X + (((bounds.Width / 2) - textSize.X) / 4 * 4) + 8, bounds.Y + TextMargin.Y), textColor);
                    break;
                case TextAlignment.Left:
                    Utility.drawTextWithShadow(b, text, font, new Vector2(bounds.Right - textSize.X, bounds.Y + TextMargin.Y), textColor);
                    break;
                case TextAlignment.Right:
                    Utility.drawTextWithShadow(b, text, font, new Vector2(bounds.X, bounds.Y + TextMargin.Y), textColor);
                    break;
            }
        }
    }
}
