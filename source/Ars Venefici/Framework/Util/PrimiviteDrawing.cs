/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Util
{
    static public class PrimiviteDrawing
    {
        static public void DrawRectangle(Texture2D whitePixel, SpriteBatch batch, Rectangle area, int width, Color color)
        {
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, area.Width, width), color);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y, width, area.Height), color);
            batch.Draw(whitePixel, new Rectangle(area.X + area.Width - width, area.Y, width, area.Height), color);
            batch.Draw(whitePixel, new Rectangle(area.X, area.Y + area.Height - width, area.Width, width), color);
        }

        static public void DrawRectangle(Texture2D whitePixel, SpriteBatch batch, Rectangle area)
        {
            DrawRectangle(whitePixel, batch, area, 1, Color.White);
        }

        public static void DrawCircle(Texture2D whitePixel, SpriteBatch spritbatch, Vector2 center, float radius, Color color, int lineWidth = 2, int segments = 16)
        {
            Vector2[] vertex = new Vector2[segments];

            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                vertex[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;
            }

            DrawPolygon(whitePixel, spritbatch, vertex, segments, color, lineWidth);
        }

        public static void DrawPolygon(Texture2D whitePixel, SpriteBatch spriteBatch, Vector2[] vertex, int count, Color color, int lineWidth)
        {
            if (count > 0)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    DrawLineSegment(whitePixel, spriteBatch, vertex[i], vertex[i + 1], color, lineWidth);
                }
                DrawLineSegment(whitePixel, spriteBatch, vertex[count - 1], vertex[0], color, lineWidth);
            }
        }

        public static void DrawLineSegment(Texture2D whitePixel, SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, int lineWidth)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            spriteBatch.Draw(whitePixel, point1, null, color,
            angle, Vector2.Zero, new Vector2(length, lineWidth),
            SpriteEffects.None, 0f);
        }
    }
}
