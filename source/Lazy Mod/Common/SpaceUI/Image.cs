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

namespace Common.SpaceUI;

public class Image : Element, ISingleTexture
{
    /// <summary>The image texture to display.</summary>
    public Texture2D Texture { get; set; }

    /// <summary>The pixel area within the texture to display, or <c>null</c> to show the entire image.</summary>
    public Rectangle? TexturePixelArea { get; set; }

    /// <summary>The zoom factor to apply to the image.</summary>
    public int Scale { get; set; }
    public Action<Element>? Callback { get; set; }
    public override int Width => (int)GetActualSize().X;
    public override int Height => (int)GetActualSize().Y;
    public override string? HoveredSound => Callback != null ? "shiny4" : null;
    public Color DrawColor { get; set; } = Color.White;
    
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked)
            Callback?.Invoke(this);
    }


    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        b.Draw(Texture, Position, TexturePixelArea, DrawColor, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
    }

    private Vector2 GetActualSize()
    {
        if (TexturePixelArea.HasValue)
            return new Vector2(TexturePixelArea.Value.Width, TexturePixelArea.Value.Height) * Scale;
        else
            return new Vector2(Texture.Width, Texture.Height) * Scale;
    }
}