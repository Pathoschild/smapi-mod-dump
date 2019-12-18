using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModSettingsTab.Framework
{
    public static class Helper
    {
        public static void DrawTextureBox(
            SpriteBatch b,
            Texture2D texture,
            Rectangle sourceRect,
            int x,
            int y,
            int width,
            int height,
            Color color,
            float scale = 1f,
            bool drawShadow = true,
            float layerDepth = 0.76f)
        {
            var num = sourceRect.Width / 3;
            if (drawShadow)
            {
                b.Draw(texture,
                    new Vector2(x + width - (int) ( num *  scale) - 8, y + 8),
                    new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Vector2(x - 8, y + height - (int) ( num *  scale) + 8),
                    new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Vector2(x + width - (int) ( num *  scale) - 8,
                        y + height - (int) ( num *  scale) + 8),
                    new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Rectangle(x + (int) ( num *  scale) - 8, y + 8,
                        width - (int) ( num *  scale) * 2, (int) ( num *  scale)),
                    new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), Color.Black * 0.4f, 0.0f,
                    Vector2.Zero, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Rectangle(x + (int) ( num *  scale) - 8,
                        y + height - (int) ( num *  scale) + 8,
                        width - (int) ( num *  scale) * 2, (int) ( num *  scale)),
                    new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Rectangle(x - 8, y + (int) ( num *  scale) + 8,
                        (int) ( num *  scale), height - (int) ( num *  scale) * 2),
                    new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f, 0.0f,
                    Vector2.Zero, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Rectangle(x + width - (int) ( num *  scale) - 8,
                        y + (int) ( num *  scale) + 8, (int) ( num *  scale),
                        height - (int) ( num *  scale) * 2),
                    new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth-0.0001f);
                b.Draw(texture,
                    new Rectangle((int) ( num *  scale / 2.0) + x - 8,
                        (int) ( num *  scale / 2.0) + y + 8,
                        width - (int) ( num *  scale), height - (int) ( num *  scale)),
                    new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, SpriteEffects.None, layerDepth-0.0001f);
            }

            b.Draw(texture,
                new Rectangle((int) ( num *  scale) + x, (int) ( num *  scale) + y,
                    width - (int) ( num *  scale * 2.0),
                    height - (int) ( num *  scale * 2.0)),
                new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2(x, y),
                new Rectangle(sourceRect.X, sourceRect.Y, num, num), color, 0.0f, Vector2.Zero, scale,
                SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2(x + width - (int) ( num *  scale), y),
                new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture, new Vector2(x, y + height - (int) ( num *  scale)),
                new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture,
                new Vector2(x + width - (int) ( num *  scale),
                    y + height - (int) ( num *  scale)),
                new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture,
                new Rectangle(x + (int) ( num *  scale), y,
                    width - (int) ( num *  scale) * 2, (int) ( num *  scale)),
                new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), color, 0.0f, Vector2.Zero,
                SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x + (int) ( num *  scale),
                    y + height - (int) ( num *  scale),
                    width - (int) ( num *  scale) * 2, (int) ( num *  scale)),
                new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x, y + (int) ( num *  scale), (int) ( num *  scale),
                    height - (int) ( num *  scale) * 2),
                new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), color, 0.0f, Vector2.Zero,
                SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x + width - (int) ( num *  scale),
                    y + (int) (num * scale), (int) ( num * scale),
                    height - (int) ( num *  scale) * 2),
                new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
        }
    }
}