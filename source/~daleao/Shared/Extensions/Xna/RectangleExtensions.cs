/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Rectangle"/> struct.</summary>
public static class RectangleExtensions
{
    /// <summary>Draws the <paramref name="rectangle"/>'s border.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="batch"><see cref="SpriteBatch"/> to draw to.</param>
    public static void DrawBorder(
        this Rectangle rectangle, Texture2D pixel, int thickness, Color color, SpriteBatch batch)
    {
        batch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color); // top line
        batch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color); // left line
        batch.Draw(
            pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color); // right line
        batch.Draw(
            pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color); // bottom line
    }

    /// <summary>Draws the <paramref name="rectangle"/>'s border.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="batch"><see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="offset">An optional offset to the <paramref name="pixel"/>'s position.</param>
    public static void DrawBorder(
        this Rectangle rectangle, Texture2D pixel, int thickness, Color color, SpriteBatch batch, Vector2 offset)
    {
        batch.Draw(
            pixel, new Rectangle(rectangle.X + (int)offset.X, rectangle.Y + (int)offset.Y, rectangle.Width, thickness), color); // top line
        batch.Draw(
            pixel, new Rectangle(rectangle.X + (int)offset.X, rectangle.Y + (int)offset.Y, thickness, rectangle.Height), color); // left line
        batch.Draw(
            pixel, new Rectangle(rectangle.X + (int)offset.X + rectangle.Width - thickness, rectangle.Y + (int)offset.Y, thickness, rectangle.Height), color); // right line
        batch.Draw(
            pixel, new Rectangle(rectangle.X + (int)offset.X, rectangle.Y + (int)offset.Y + rectangle.Height - thickness, rectangle.Width, thickness), color); // bottom line
    }
}
