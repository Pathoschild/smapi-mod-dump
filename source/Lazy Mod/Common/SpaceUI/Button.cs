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

namespace Common.SpaceUI;

public class Button : Element, ISingleTexture
{
    /*********
     ** Fields
     *********/
    private float Scale = 1f;


    /*********
     ** Accessors
     *********/
    public Texture2D Texture { get; set; }
    public Rectangle IdleTextureRect { get; set; }
    public Rectangle HoverTextureRect { get; set; }

    public Action<Element> Callback { get; set; }

    /// <inheritdoc />
    public override int Width => IdleTextureRect.Width;

    /// <inheritdoc />
    public override int Height => IdleTextureRect.Height;

    /// <inheritdoc />
    public override string HoveredSound => "Cowboy_Footstep";


    /*********
     ** Public methods
     *********/
    public Button()
    {
    }

    public Button(Texture2D tex)
    {
        Texture = tex;
        IdleTextureRect = new Rectangle(0, 0, tex.Width / 2, tex.Height);
        HoverTextureRect = new Rectangle(tex.Width / 2, 0, tex.Width / 2, tex.Height);
    }

    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        Scale = Hover ? Math.Min(Scale + 0.013f, 1.083f) : Math.Max(Scale - 0.013f, 1f);

        if (Clicked)
            Callback?.Invoke(this);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        var texRect = Hover ? HoverTextureRect : IdleTextureRect;
        Vector2 origin = new Vector2(texRect.Width / 2f, texRect.Height / 2f);
        b.Draw(Texture, Position + origin, texRect, Color.White, 0f, origin, Scale, SpriteEffects.None, 0f);
        Game1.activeClickableMenu?.drawMouse(b);
    }
}