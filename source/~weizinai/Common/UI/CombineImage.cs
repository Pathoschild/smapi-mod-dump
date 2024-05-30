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
using StardewValley;
using StardewValley.Menus;

namespace Common.UI;

public class CombineImage : Element
{
    private const int ContentPadding = 16;
    private readonly Texture2D background;
    private readonly Color backgroundColor;
    private readonly Rectangle backgroundSourceRectangle;
    private readonly Texture2D content;
    private readonly Color contentColor;
    private readonly Rectangle contentSourceRectangle;
    private readonly float scale;

    public CombineImage(Texture2D content, Rectangle contentSourceRectangle, Vector2 localPosition, float scale = 1f) :
        this(Game1.menuTexture, new Rectangle(0, 256, 60, 60), Color.White,
            content, contentSourceRectangle, Color.White, localPosition, scale)
    {
    }

    private CombineImage(Texture2D background, Rectangle backgroundSourceRectangle, Color backgroundColor,
        Texture2D content, Rectangle contentSourceRectangle, Color contentColor, Vector2 localPosition, float scale = 1f)
    {
        this.background = background;
        this.backgroundSourceRectangle = backgroundSourceRectangle;
        this.backgroundColor = backgroundColor;
        this.content = content;
        this.contentSourceRectangle = contentSourceRectangle;
        this.contentColor = contentColor;
        LocalPosition = localPosition;
        this.scale = scale;
    }

    public override int Width => (int)GetImageSize().X;
    public override int Height => (int)GetImageSize().Y;


    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        IClickableMenu.drawTextureBox(spriteBatch, background, backgroundSourceRectangle,
            (int)Position.X, (int)Position.Y, Width, Height, backgroundColor, 1f, false);
        spriteBatch.Draw(content, GetContentRectangle(), contentSourceRectangle, contentColor);
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle((int)Position.X + ContentPadding, (int)Position.Y + ContentPadding, Width - ContentPadding * 2, Height - ContentPadding * 2);
    }

    private Vector2 GetImageSize()
    {
        return new Vector2(backgroundSourceRectangle.Width, backgroundSourceRectangle.Height) * scale;
    }
}