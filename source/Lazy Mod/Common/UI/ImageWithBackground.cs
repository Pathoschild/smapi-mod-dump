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

namespace Common.UI;

public class ImageWithBackground : Container
{
    private const int ContentPadding = 16;

    private Rectangle localDestinationRectangle;

    private Rectangle DestinationRectangle
    {
        get
        {
            var location = localDestinationRectangle.Location.ToVector2() + (Parent?.Position ?? Vector2.Zero);
            return new Rectangle(location.ToPoint(), localDestinationRectangle.Size);
        }
    }

    public override Vector2 LocalPosition
    {
        get => localDestinationRectangle.Location.ToVector2();
        set => localDestinationRectangle.Location = value.ToPoint();
    }

    public override int Width => DestinationRectangle.Width;
    public override int Height => DestinationRectangle.Height;

    public ImageWithBackground(Texture2D content, Rectangle contentRectangle, Rectangle localDestinationRectangle) :
        this(Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles"), new Rectangle(0, 256, 64, 64), Color.White,
            content, contentRectangle, Color.White, localDestinationRectangle)
    {
    }

    private ImageWithBackground(Texture2D background, Rectangle backgroundRectangle, Color backgroundColor,
        Texture2D content, Rectangle contentRectangle, Color contentColor, Rectangle localDestinationRectangle)
    {
        this.localDestinationRectangle = localDestinationRectangle;
        AddChild(new Image(background, new Rectangle(0,0,64,64), backgroundRectangle, backgroundColor, true),
            new Image(content, GetContentRectangle(), contentRectangle, contentColor));
    }

    private Rectangle GetContentRectangle()
    {
        return new Rectangle(ContentPadding, ContentPadding, 
            localDestinationRectangle.Width - ContentPadding * 2, localDestinationRectangle.Height - ContentPadding * 2);
    }
}