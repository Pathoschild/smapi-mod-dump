/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BetterPanning
{
    public static class DrawingExtensions
    {
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float depth = 0F, float shadowDepth = 0.005F)
        {
            batch.DrawStringWithShadow(font, text, position, color, Color.Black, Vector2.One, depth, shadowDepth);
        }

        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="shadowColor">The color of the shadow.</param>
        /// <param name="scale">The amount to scale the size of the string by.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, Color shadowColor, Vector2 scale, float depth, float shadowDepth)
        {
            batch.DrawString(font, text, position + Vector2.One * Game1.pixelZoom * scale / 2f, shadowColor, 0F, Vector2.Zero, scale, SpriteEffects.None, shadowDepth);
            batch.DrawString(font, text, position, color, 0F, Vector2.Zero, scale, SpriteEffects.None, depth);
        }
    }
}
