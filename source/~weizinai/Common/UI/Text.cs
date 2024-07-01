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

namespace weizinai.StardewValleyMod.Common.UI;

public sealed class Text : Element
{
    private readonly Color color;

    private readonly SpriteFont font;
    private readonly float scale;
    private readonly string text;

    public Text(string text, Vector2 localPosition, SpriteFont? font = null, Color? color = null, float scale = 1f)
    {
        this.font = font ?? Game1.dialogueFont;
        this.text = text;
        this.LocalPosition = localPosition;
        this.color = color ?? Game1.textColor;
        this.scale = scale;
    }

    public override int Width => (int)this.GetTextSize().X;
    public override int Height => (int)this.GetTextSize().Y;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (this.IsHidden()) return;


        spriteBatch.DrawString(this.font, this.text, this.Position, this.color, 0f, Vector2.Zero, this.scale, SpriteEffects.None, 0f);
    }

    private Vector2 GetTextSize()
    {
        return this.font.MeasureString(this.text) * this.scale;
    }
}