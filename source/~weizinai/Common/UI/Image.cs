/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public class Image : Element
{
    private readonly Color color;
    public float Scale;
    private readonly Rectangle sourceRectangle;
    private readonly Texture2D texture;

    public Image(Texture2D texture, Vector2 localPosition, Rectangle sourceRectangle, Color? color = null, float scale = 1f)
    {
        this.texture = texture;
        LocalPosition = localPosition;
        this.sourceRectangle = sourceRectangle;
        this.color = color ?? Color.White;
        this.Scale = scale;
    }

    public override int Width => (int)GetImageSize().X;
    public override int Height => (int)GetImageSize().Y;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;

        spriteBatch.Draw(texture, Position, sourceRectangle, color, 0, Vector2.Zero, Scale, SpriteEffects.None, -1);
    }

    private Vector2 GetImageSize()
    {
        return new Vector2(sourceRectangle.Width, sourceRectangle.Height) * Scale;
    }
}

/// <summary>Simplifies access to the game's sprite sheets.</summary>
/// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
internal static class CommonImage
{
    public static Rectangle CloseButton => new(337, 494, 12, 12);
}