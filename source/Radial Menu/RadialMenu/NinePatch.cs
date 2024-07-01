/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RadialMenu;

// Also sometimes called "nine-slice". Scales certain textures without visible stretching.
//
// This is a simplified version that assumes a more-or-less symmetrical shape; there is only one
// horizontal border thickness (applies to left and right edges) and one vertical (top/bottom).
internal class NinePatch
{
    private readonly Texture2D texture;
    private readonly Point borderWidths;
    private readonly Rectangle[,] sourceGrid;

    public NinePatch(Texture2D texture, Rectangle? sourceRect, Point borderWidths)
    {
        this.texture = texture;
        this.borderWidths = borderWidths;

        var bounds = sourceRect ?? texture.Bounds;
        sourceGrid = GetGrid(bounds, borderWidths);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRect, float borderScale = 1.0f)
    {
        var destinationGrid = GetGrid(
            destinationRect,
            borderScale != 1.0f
                ? (borderWidths.ToVector2() * borderScale).ToPoint()
                : borderWidths);
        for (int y = 0; y < destinationGrid.GetLength(0); y++)
        {
            for (int x = 0; x < destinationGrid.GetLength(1); x++)
            {
                spriteBatch.Draw(texture, destinationGrid[y, x], sourceGrid[y, x], Color.White);
            }
        }
    }

    private static Rectangle[,] GetGrid(Rectangle bounds, Point borderWidths)
    {
        var left = bounds.X;
        var top = bounds.Y;
        var innerWidth = bounds.Width - 2 * borderWidths.X;
        var innerHeight = bounds.Height - 2 * borderWidths.Y;
        var startRight = bounds.Right - borderWidths.X;
        var startBottom = bounds.Bottom - borderWidths.Y;
        return new Rectangle[3, 3]
        {
            {
                new(left, top, borderWidths.X, borderWidths.Y),
                new(left + borderWidths.X, top, innerWidth, borderWidths.Y),
                new(startRight, top, borderWidths.X, borderWidths.Y),
            },
            {
                new(left, top + borderWidths.Y, borderWidths.X, innerHeight),
                new(left + borderWidths.X, top + borderWidths.Y, innerWidth, innerHeight),
                new(startRight, top + borderWidths.Y, borderWidths.X, innerHeight),
            },
            {
                new(left, startBottom, borderWidths.X, borderWidths.Y),
                new(left + borderWidths.X, startBottom, innerWidth, borderWidths.Y),
                new(startRight, startBottom, borderWidths.X, borderWidths.Y),
            }
        };
    }
}
