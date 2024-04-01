/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DecidedlyShared.Ui;

public class TextElement : UiElement
{
    private string text;
    private SpriteFont font;
    private int widthConstraint;

    public TextElement(
        string name, Rectangle bounds, Logger logger, int widthConstraint = 1000, string text = "",
        SpriteFont? font = null, DrawableType type = DrawableType.SlicedBox, Texture2D? texture = null, Rectangle? sourceRect = null, Color? color = null,
        int topEdgeSize = 16, int bottomEdgeSize = 16, int leftEdgeSize = 16, int rightEdgeSize = 16)
        : base(name, bounds, logger, type, texture, sourceRect, color, false,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        if (font == null)
        {
            font = Game1.dialogueFont;
        }

        this.widthConstraint = widthConstraint;
        this.text = text;
        string wrappedText = Game1.parseText(this.text, Game1.dialogueFont, this.widthConstraint);
        Vector2 stringSize = Game1.dialogueFont.MeasureString(wrappedText);

        // int height = SpriteText.getHeightOfString(this.text, widthConstraint);
        // int width = SpriteText.getWidthOfString(this.text, widthConstraint);
        this.bounds = new Rectangle(bounds.X, bounds.Y, (int)stringSize.X + leftEdgeSize + rightEdgeSize, (int)stringSize.Y + topEdgeSize + bottomEdgeSize);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Utility.drawTextWithShadow(
            spriteBatch,
            Game1.parseText(this.text, Game1.dialogueFont, this.widthConstraint),
            Game1.dialogueFont,
            new Vector2(this.bounds.X + this.leftEdgeSize, this.bounds.Y + this.topEdgeSize),
            Game1.textColor
            );

        // spriteBatch.DrawString(
        //     ,
        //     Color.Black);

        // SpriteText.drawString(
        //     spriteBatch,
        //     this.text,
        //     this.bounds.X + this.leftEdgeSize,
        //     this.bounds.Y + this.topEdgeSize,
        //     width: this.widthConstraint,
        //     drawBGScroll: -1,
        //     scroll_text_alignment: SpriteText.ScrollTextAlignment.Center
        // );
    }
}
