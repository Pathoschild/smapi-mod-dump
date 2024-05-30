/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements;

public class TextureBox
{
    public TextureBox(Rectangle bounds, Texture2D texture, Rectangle sourceRectangle)
    {
        this.Bounds = bounds;
        this.Texture = texture;
        this.SourceRectangle = sourceRectangle;
    }
    public Rectangle Bounds { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public float Scale { get; set; } = 1f;
    public Color Color { get; set; } = Color.White;
    public bool DrawShadow { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(
            spriteBatch,
            this.Texture,
            this.SourceRectangle,
            this.Bounds.X,
            this.Bounds.Y,
            this.Bounds.Width,
            this.Bounds.Height,
            this.Color,
            this.Scale,
            this.DrawShadow);
    }
}
