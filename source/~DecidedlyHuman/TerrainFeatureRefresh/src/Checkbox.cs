/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TerrainFeatureRefresh;

public class Checkbox : ClickableComponent
{
    private Texture2D checkboxImage;
    private Rectangle checkedSourceRect;
    private Rectangle uncheckedSourceRect;
    private bool isChecked;
    private string checkboxLabel;

    public Checkbox(Rectangle bounds, string name) : base(bounds, name)
    {
        this.checkboxImage = Game1.menuTexture;
        this.checkedSourceRect = new Rectangle(192, 768, 36, 36);
        this.uncheckedSourceRect = new Rectangle(128, 768, 36, 36);

        Vector2 labelBounds = Game1.smallFont.MeasureString(name);

        this.bounds.Width = (int)labelBounds.X + 40;
        this.bounds.Height = (int)labelBounds.Y;
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Draw(
            this.checkboxImage,
            new Rectangle(this.bounds.X, this.bounds.Y, 36, 36),
            this.isChecked ? this.checkedSourceRect : this.uncheckedSourceRect,
            Color.White);

        Utility.drawTextWithShadow(
            sb,
            this.name,
            Game1.smallFont,
            new Vector2(this.bounds.X + 36 + 4, this.bounds.Y + 4),
            Game1.textColor);
    }

    public void ReceiveLeftClick()
    {
        this.isChecked = !this.isChecked;
    }

    public override bool containsPoint(int x, int y)
    {
        return base.containsPoint(x, y);
    }
}
