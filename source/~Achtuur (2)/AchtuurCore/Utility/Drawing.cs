/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
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
    public static Rectangle PixelSourceRect = new Rectangle(0, 0, 1, 1);
    /****
    ** Drawing 
    ****/
    /// <summary>Draw a sprite to the screen. (<see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/CommonHelper.cs#L370"/>)</summary>
    /// <param name="batch">The sprite batch.</param>
    /// <param name="x">The X-position at which to start the line.</param>
    /// <param name="y">The X-position at which to start the line.</param>
    /// <param name="size">The line dimensions.</param>
    /// <param name="color">The color to tint the sprite.</param>
    public static void DrawRect(this SpriteBatch batch, float x, float y, in Vector2 size, in Color? color = null)
    {
        batch.Draw(Drawing.Pixel, new Rectangle((int)x, (int)y, (int)size.X, (int)size.Y), color ?? Color.White);
    }
    public static void DrawRect(this SpriteBatch batch, in Vector2 pos, in Vector2 size, in Color? color = null)
    {
        batch.Draw(Drawing.Pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), color ?? Color.White);
    }

    public static void DrawLine(this SpriteBatch batch, in Vector2 start, in Vector2 end, in Color? color = null, int? thickness = null)
    {
        Vector2 dim = end - start;
        Rectangle r = new Rectangle((int)start.X, (int)start.Y, (int)dim.Length(), thickness ?? 1);
        float rot = (float)Math.Atan2(dim.Y, dim.X);
        batch.Draw(Drawing.Pixel, r, PixelSourceRect, color ?? Color.White, rot, Vector2.Zero, SpriteEffects.None, 0);
    }

    /// <summary>
    /// Draws a texture to screen using position and scale. The top left of the drawn texture is pos, the bottom right is pos + texture size * scale.
    /// </summary>
    /// <param name="batch"></param>
    /// <param name="texture"></param>
    /// <param name="pos"></param>
    /// <param name="scale"></param>
    /// <param name="color"></param>
    /// <param name="layerDepth"></param>
    public static void DrawTexture(this SpriteBatch batch, in Texture2D texture, in Vector2 pos, in Vector2? scale = null, in Color? color = null, float? layerDepth = null)
    {
        batch.Draw(texture, pos, null, color ?? Color.White, 0f, Vector2.Zero, scale ?? Vector2.One, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Draws texture to screen using a position and size. Top left of the drawn texture is pos, bottom right is pos + size.
    /// </summary>
    /// <param name="batch"></param>
    /// <param name="texture"></param>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="layerDepth"></param>
    public static void DrawSizedTexture(this SpriteBatch batch, in Texture2D texture, in Vector2 pos, in Vector2 size, in Color? color = null, float? layerDepth = null)
    {
        batch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), null, color ?? Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth ?? 0f);
    }

    public static void DrawBorder(this SpriteBatch batch, in Rectangle rect, in Color? color = null, int bordersize = 2)
    {
        DrawBorder(batch, new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), color, bordersize);
    }

    public static void DrawBorder(this SpriteBatch batch, in Vector2 pos, in Vector2 size, in Color? color = null, int bordersize = 2)
    {
        // Top
        batch.DrawRect(pos.X - bordersize, pos.Y - bordersize, new Vector2(size.X + bordersize, bordersize), color);

        // Bottom
        batch.DrawRect(pos.X - bordersize, pos.Y + size.Y - bordersize, new Vector2(size.X + bordersize, bordersize), color);

        // Left
        batch.DrawRect(pos.X - bordersize, pos.Y - bordersize, new Vector2(bordersize, size.Y + bordersize), color);

        // Right
        batch.DrawRect(pos.X + size.X, pos.Y - bordersize, new Vector2(bordersize, size.Y + bordersize), color);
    }

    public static void DrawBorderNoCorners(this SpriteBatch batch, in Vector2 pos, in Vector2 size, in Color? color = null, int bordersize = 2)
    {
        // Top
        batch.DrawRect(pos.X - bordersize + 1, pos.Y - bordersize, new Vector2(size.X + bordersize - 2, bordersize), color);

        // Bottom
        batch.DrawRect(pos.X - bordersize + 1, pos.Y + size.Y - bordersize, new Vector2(size.X + bordersize - 2, bordersize), color);

        // Left
        batch.DrawRect(pos.X - bordersize, pos.Y - bordersize + 1, new Vector2(bordersize, size.Y + bordersize - 2), color);

        // Right
        batch.DrawRect(pos.X + size.X, pos.Y - bordersize + 1, new Vector2(bordersize, size.Y + bordersize - 2), color);
    }

    public static void DrawLightSource(this SpriteBatch batch, LightSource lightSource)
    {
        Texture2D texture = lightSource.lightTexture;
        int lightQuality = Game1.options.lightingQuality;

        //spriteBatch.DrawTexture(texture, position, null);
        batch.Draw(texture,
            lightSource.position.Value,
            new Rectangle?(texture.Bounds),
            lightSource.color.Value,
            0f,
            new Vector2((float)(texture.Bounds.Width / 2), (float)(texture.Bounds.Height / 2)),
            lightSource.radius.Value / (float)(lightQuality / 2),
            SpriteEffects.None,
            0.9f);
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
