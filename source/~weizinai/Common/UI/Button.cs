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

namespace weizinai.StardewValleyMod.Common.UI;

public class Button : Element
{
    public int ContentPadding = 16;
    private readonly SpriteFont font;
    private readonly float scale;
    private readonly Rectangle sourceRectangle;
    private readonly string text;
    private readonly Color textColor;

    private readonly Texture2D texture;
    public Color TextureColor;

    public Button(string text, Vector2 localPosition, float scale = 1f) :
        this(Game1.menuTexture, new Rectangle(0, 256, 60, 60), Color.White,
            text, Game1.smallFont, Game1.textColor, localPosition, scale)
    {
    }


    public Button(Texture2D texture, Rectangle sourceRectangle, Color textureColor, string text, SpriteFont font, Color textColor, Vector2 localPosition,
        float scale = 1f)
    {
        this.texture = texture;
        this.sourceRectangle = sourceRectangle;
        this.TextureColor = textureColor;
        this.text = text;
        this.font = font;
        this.textColor = textColor;
        this.LocalPosition = localPosition;
        this.scale = scale;
    }

    public override int Width => (int)this.GetTextSize().X + this.ContentPadding * 2;
    public override int Height => (int)this.GetTextSize().Y + this.ContentPadding * 2;

    public override void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, this.texture, this.sourceRectangle, (int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, this.TextureColor, 1f, false);
        spriteBatch.DrawString(this.font, this.text, this.Position + new Vector2(this.ContentPadding, this.ContentPadding), this.textColor,
            0f, Vector2.Zero, this.scale, SpriteEffects.None, 0f);
    }

    private Vector2 GetTextSize()
    {
        return this.font.MeasureString(this.text) * this.scale;
    }
}