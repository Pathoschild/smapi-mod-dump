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
using StardewValley.BellsAndWhistles;

namespace Common.SpaceUI;

public class Label : Element
{
    /*********
     ** Accessors
     *********/
    public bool Bold { get; set; } = false;
    public float NonBoldScale { get; set; } = 1f; // Only applies when Bold = false
    public bool NonBoldShadow { get; set; } = true; // Only applies when Bold = false
    public Color IdleTextColor { get; set; } = Game1.textColor;
    public Color HoverTextColor { get; set; } = Game1.unselectedOptionColor;

    public SpriteFont Font { get; set; } = Game1.dialogueFont; // Only applies when Bold = false

    public float Scale => Bold ? 1f : NonBoldScale;

    public string String { get; set; }

    public Action<Element>? Callback { get; set; }

    /// <inheritdoc />
    public override int Width => (int)Measure().X;

    /// <inheritdoc />
    public override int Height => (int)Measure().Y;

    /// <inheritdoc />
    public override string? HoveredSound => Callback != null ? "shiny4" : null;


    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked)
            Callback?.Invoke(this);
    }

    /// <summary>Measure the label's rendered dialogue text size.</summary>
    public Vector2 Measure()
    {
        return MeasureString(String, Bold, scale: Bold ? 1f : NonBoldScale, font: Font);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        bool altColor = Hover && Callback != null;
        if (Bold)
            SpriteText.drawString(b, String, (int)Position.X, (int)Position.Y, layerDepth: 1, color: altColor ? SpriteText.color_Gray : null);
        else
        {
            Color col = altColor ? HoverTextColor : IdleTextColor;
            if (col.A <= 0)
                return;

            if (NonBoldShadow)
                Utility.drawTextWithShadow(b, String, Font, Position, col, NonBoldScale);
            else
                b.DrawString(Font, String, Position, col, 0f, Vector2.Zero, NonBoldScale, SpriteEffects.None, 1);
        }
    }

    /// <summary>Measure the rendered dialogue text size for the given text.</summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="scale">The scale to apply to the size.</param>
    /// <param name="font">The font to measure. Defaults to <see cref="Game1.dialogueFont"/> if <c>null</c>.</param>
    public static Vector2 MeasureString(string text, bool bold = false, float scale = 1f, SpriteFont? font = null)
    {
        if (bold)
            return new Vector2(SpriteText.getWidthOfString(text) * scale, SpriteText.getHeightOfString(text) * scale);
        else
            return (font ?? Game1.dialogueFont).MeasureString(text) * scale;
    }
}