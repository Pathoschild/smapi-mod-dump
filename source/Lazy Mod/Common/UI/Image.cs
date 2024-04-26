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
using StardewValley.Menus;

namespace Common.UI;

public class Image : Element
{
    private readonly Texture2D texture;

    public Rectangle LocalDestinationRectangle;

    public Rectangle DestinationRectangle
    {
        get
        {
            var location = LocalDestinationRectangle.Location.ToVector2() + (Parent?.Position ?? Vector2.Zero);
            return new Rectangle(location.ToPoint(), LocalDestinationRectangle.Size);
        }
    }
    private readonly Rectangle sourceRectangle;
    private readonly Color color;

    public override Vector2 LocalPosition
    {
        get => LocalDestinationRectangle.Location.ToVector2();
        set => LocalDestinationRectangle.Location = value.ToPoint();
    }

    public override int Width => LocalDestinationRectangle.Width;
    public override int Height => LocalDestinationRectangle.Height;

    private readonly bool isBackground;

    public Image(Texture2D texture, Rectangle localDestinationRectangle, Rectangle sourceRectangle, bool isBackground = false) :
        this(texture, localDestinationRectangle, sourceRectangle, Color.White, isBackground)
    {
    }

    public Image(Texture2D texture, Rectangle localDestinationRectangle, Rectangle sourceRectangle, Color color, bool isBackground = false)
    {
        this.texture = texture;
        LocalDestinationRectangle = localDestinationRectangle;
        this.sourceRectangle = sourceRectangle;
        this.color = color;
        this.isBackground = isBackground;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;

        if (isBackground)
            IClickableMenu.drawTextureBox(spriteBatch, texture, sourceRectangle, DestinationRectangle.X, DestinationRectangle.Y, 
                DestinationRectangle.Width, DestinationRectangle.Height, color, 1f, false);
        else
            spriteBatch.Draw(texture, DestinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
}