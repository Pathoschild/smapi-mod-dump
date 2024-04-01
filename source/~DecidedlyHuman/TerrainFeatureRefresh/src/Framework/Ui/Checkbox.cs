/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TerrainFeatureRefresh.Framework.Ui;

public class Checkbox : ClickableComponent
{
    internal Texture2D checkboxImage;
    internal Rectangle checkedSourceRect;
    internal Rectangle uncheckedSourceRect;
    internal Vector2 labelBounds;
    internal SpriteFont font;
    internal bool isChecked;

    public Checkbox(Rectangle bounds, string name, Texture2D texture, SpriteFont font) : base(bounds, name)
    {
        this.checkboxImage = texture;
        this.font = font;
        this.uncheckedSourceRect = new Rectangle(0, 0, 47, 47);
        this.checkedSourceRect = new Rectangle(52, 0, 47, 47);

        this.labelBounds = this.font.MeasureString(name);

        this.bounds.Width = (int)this.labelBounds.X + 40;
        this.bounds.Height = (int)this.labelBounds.Y;
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Draw(
            this.checkboxImage,
            new Rectangle(this.bounds.X, this.bounds.Y, 36, 36),
            this.isChecked ? this.checkedSourceRect : this.uncheckedSourceRect,
            Color.White);

        Drawing.DrawStringWithShadow(
            sb,
            this.font,
            this.name,
            new Vector2(this.bounds.X + 36 + 4, this.bounds.Y + 4),
            Color.Black,
            Color.Gray);

        // Utility.drawTextWithShadow(
        //     sb,
        //     this.name,
        //     Game1.smallFont,
        //     new Vector2(this.bounds.X + 36 + 4, this.bounds.Y + 4),
        //     Game1.textColor);
    }

    public virtual void ReceiveLeftClick()
    {
        this.isChecked = !this.isChecked;
    }

    public override bool containsPoint(int x, int y)
    {
        return base.containsPoint(x, y);
    }
}
