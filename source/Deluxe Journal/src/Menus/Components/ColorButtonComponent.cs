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
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public class ColorButtonComponent : ButtonComponent
    {
        private readonly Rectangle _topMaskSource;
        private readonly Rectangle _midMaskSource;
        private readonly Rectangle _botMaskSource;
        private readonly Color _topMaskColor;
        private readonly Color _midMaskColor;
        private readonly Color _botMaskColor;

        public bool Hovering { get; private set; }

        public ColorSchema ColorSchema { get; }

        public ColorButtonComponent(string name, Rectangle bounds, int colorSchemaIndex, bool drawShadow = true)
            : base(name, bounds, colorSchemaIndex, DeluxeJournalMod.UiTexture!, new(64, 64, 12, 12), 4f, drawShadow)
        {
            _topMaskSource = new(48, 72, 8, 8);
            _midMaskSource = new(56, 64, 8, 8);
            _botMaskSource = new(48, 64, 8, 8);

            if (colorSchemaIndex >= 0 && colorSchemaIndex < DeluxeJournalMod.ColorSchemas.Count)
            {
                ColorSchema = DeluxeJournalMod.ColorSchemas[colorSchemaIndex];
            }
            else
            {
                ColorSchema = ColorSchema.ErrorSchema;
            }

            if (ColorSchema.Luminance(ColorSchema.Padding) > ColorSchema.Luminance(ColorSchema.Main))
            {
                _topMaskColor = ColorSchema.Main;
                _midMaskColor = ColorSchema.Header;
                _botMaskColor = ColorSchema.Accent;
            }
            else
            {
                _topMaskColor = ColorSchema.Accent;
                _midMaskColor = ColorSchema.Padding;
                _botMaskColor = ColorSchema.Main;
            }
        }

        public override void tryHover(int x, int y, float maxScaleIncrease = 0)
        {
            Hovering = visible && containsPoint(x, y);
        }

        public override void draw(SpriteBatch b, Color color, float layerDepth, int frameOffset = 0, int xOffset = 0, int yOffset = 0)
        {
            Rectangle inner = new(bounds.X + 8, bounds.Y + 8, 32, 32);

            if (drawShadow)
            {
                Vector2 center = new(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                Vector2 origin = new(sourceRect.Width / 2f, sourceRect.Height / 2f);
                Rectangle source = new(Selected || Hovering ? sourceRect.X + sourceRect.Width : sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);

                Utility.drawWithShadow(b, texture, center, source, Color.White, 0, origin, 4f);
            }
            else
            {
                b.Draw(texture, bounds, sourceRect, Color.White);
            }

            b.Draw(texture, inner, _topMaskSource, _topMaskColor);
            b.Draw(texture, inner, _midMaskSource, _midMaskColor);
            b.Draw(texture, inner, _botMaskSource, _botMaskColor);
        }
    }
}
