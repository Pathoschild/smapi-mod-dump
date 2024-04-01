/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProductionStats.Common;

internal static class DrawHelper
{
    /// <summary>Get the dimensions of a space character.</summary>
    /// <param name="font">The font to measure.</param>
    public static float GetSpaceWidth(SpriteFont font)
    {
        return CommonHelper.GetSpaceWidth(font);
    }

    /// <summary>Draw a sprite to the screen.</summary>
    /// <param name="spriteBatch">The sprite batch being drawn.</param>
    /// <param name="sheet">The sprite sheet containing the sprite.</param>
    /// <param name="sprite">The sprite coordinates and dimensions in the sprite sheet.</param>
    /// <param name="x">The X-position at which to draw the sprite.</param>
    /// <param name="y">The X-position at which to draw the sprite.</param>
    /// <param name="color">The color to tint the sprite.</param>
    /// <param name="scale">The scale to draw.</param>
    public static void DrawSprite(
        this SpriteBatch spriteBatch,
        Texture2D sheet,
        Rectangle sprite,
        float x,
        float y,
        Color? color = null,
        float scale = 1)
    {
        spriteBatch.Draw(sheet,
                         new Vector2(x, y),
                         sprite,
                         color ?? Color.White,
                         0,
                         Vector2.Zero,
                         scale,
                         SpriteEffects.None,
                         0);
    }
}