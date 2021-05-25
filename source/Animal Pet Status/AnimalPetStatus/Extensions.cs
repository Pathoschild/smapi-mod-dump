/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hakej/Animal-Pet-Status
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalPetStatus
{
    public static class Extensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Vector2 origin)
        {
            spriteBatch.DrawString(font, text, pos, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Color color, Vector2 origin)
        {
            spriteBatch.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position)
        {
            spriteBatch.DrawString(font, text, position, Color.White);
        }
    }
}
