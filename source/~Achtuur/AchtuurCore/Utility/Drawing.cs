/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace AchtuurCore.Utility;

public static class Drawing
{
    /// <summary>
    /// <para>A blank pixel which can be colorized and stretched to draw geometric shapes.</para>
    /// 
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/CommonHelper.cs#L27"
    /// </summary>
    private static readonly Lazy<Texture2D> LazyPixel = new(() =>
    {
        Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });
    public static Texture2D Pixel => Drawing.LazyPixel.Value;
    /****
    ** Drawing 
    ****/
    /// <summary>Draw a sprite to the screen. (<see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/CommonHelper.cs#L370"/>)</summary>
    /// <param name="batch">The sprite batch.</param>
    /// <param name="x">The X-position at which to start the line.</param>
    /// <param name="y">The X-position at which to start the line.</param>
    /// <param name="size">The line dimensions.</param>
    /// <param name="color">The color to tint the sprite.</param>
    public static void DrawLine(this SpriteBatch batch, float x, float y, in Vector2 size, in Color? color = null)
    {
        batch.Draw(Drawing.Pixel, new Rectangle((int)x, (int)y, (int)size.X, (int)size.Y), color ?? Color.White);
    }
    public static void DrawLine(this SpriteBatch batch, Vector2 pos, in Vector2 size, in Color? color = null)
    {
        batch.Draw(Drawing.Pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), color ?? Color.White);
    }

    public static void DrawTexture(this SpriteBatch batch, Texture2D texture, Vector2 pos, in Vector2 size, in Color? color = null, float? layerDepth = null)
    {
        batch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), null, color ?? Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth ?? 0f);
    }

    public static void DrawBorder(this SpriteBatch batch, Vector2 pos, in Vector2 size, in Color? color = null, int bordersize = 2)
    {
        // Top
        batch.DrawLine(pos.X - bordersize, pos.Y - bordersize, new Vector2(size.X + bordersize, bordersize), color);

        // Bottom
        batch.DrawLine(pos.X - bordersize, pos.Y + size.Y - bordersize, new Vector2(size.X + bordersize, bordersize), color);

        // Left
        batch.DrawLine(pos.X - bordersize, pos.Y - bordersize, new Vector2(bordersize, size.Y + bordersize), color);

        // Right
        batch.DrawLine(pos.X + size.X, pos.Y - bordersize, new Vector2(bordersize, size.Y + bordersize), color);
    }

    public static void DrawBorderNoCorners(this SpriteBatch batch, Vector2 pos, in Vector2 size, in Color? color = null, int bordersize = 2)
    {
        // Top
        batch.DrawLine(pos.X - bordersize + 1, pos.Y - bordersize, new Vector2(size.X + bordersize - 2, bordersize), color);

        // Bottom
        batch.DrawLine(pos.X - bordersize + 1, pos.Y + size.Y - bordersize, new Vector2(size.X + bordersize - 2, bordersize), color);

        // Left
        batch.DrawLine(pos.X - bordersize, pos.Y - bordersize + 1, new Vector2(bordersize, size.Y + bordersize - 2), color);

        // Right
        batch.DrawLine(pos.X + size.X, pos.Y - bordersize + 1, new Vector2(bordersize, size.Y + bordersize - 2), color);
    }

    public static Vector2 GetPositionScreenCoords(Vector2 position)
    {
        return new Vector2
        (
            position.X - Game1.viewport.X,
            position.Y - Game1.viewport.Y
        );
    }

    /// <summary>
    /// <para>Get visible tiles, taken from Pathoschild's Tilehelper.GetVisibleTiles</para>
    /// 
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/TileHelper.cs#L95"/>
    /// </summary>
    /// <returns></returns>
    public static Rectangle GetVisibleArea()
    {
        return new Rectangle(
            x: (Game1.viewport.X),
            y: (Game1.viewport.Y),
            width: Game1.viewport.Width,
            height: Game1.viewport.Height
        );
    }
}
