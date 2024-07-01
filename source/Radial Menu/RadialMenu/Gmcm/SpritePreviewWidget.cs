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
using StardewValley;

namespace RadialMenu.Gmcm;

internal class SpritePreviewWidget
{
    private const float BORDER_SCALE = 4.0f;
    private const int IMAGE_HEIGHT = 128;
    // Width is chosen to be larger than height to allow for wide aspects, even though essentially
    // all item sprites are either square or tall. Anyway, we have much more horizontal space than
    // vertical space to play with in the UI.
    private const int IMAGE_WIDTH = 160;
    private const int IMAGE_PADDING = 16;

    // Widget width/height are the entire outer width and height, and don't account for the border
    // itself. Assume that the padding is chosen to be large enough to make that a non-issue.
    public static int Height => IMAGE_HEIGHT + 2 * IMAGE_PADDING;
    public static int Width => IMAGE_WIDTH + 2 * IMAGE_PADDING;

    private readonly NinePatch border = new(Game1.mouseCursors, new(293, 360, 24, 24), new(5, 5));

    public Texture2D? Texture { get; set; }
    public Rectangle? SourceRect { get; set; }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Draw(spriteBatch, position.ToPoint());
    }

    public void Draw(SpriteBatch spriteBatch, Point position)
    {
        var borderRect = new Rectangle(position, new(Width, Height));
        border.Draw(spriteBatch, borderRect, BORDER_SCALE);

        if (Texture is null)
        {
            return;
        }
        var sourceRect = SourceRect ?? Texture.Bounds;
        var destinationWidth =
            (int)MathF.Round(sourceRect.Width / (float)sourceRect.Height * IMAGE_HEIGHT);
        var destinationRect = new Rectangle(
            borderRect.Center.X - destinationWidth / 2,
            position.Y + IMAGE_PADDING,
            destinationWidth,
            IMAGE_HEIGHT);
        spriteBatch.Draw(Texture, destinationRect, sourceRect, Color.White);
    }
}
