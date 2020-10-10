/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.CustomBooks.Menu
{
    internal class InsertPage : SystemPage
    {
        /*********
        ** Public methods
        *********/
        public InsertPage(BookMenu menu) { }

        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            Utility.drawTextWithShadow(batch, "Add Page", Game1.dialogueFont, new Vector2(region.X, region.Y + 16), Game1.textColor, 1);
            this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 100, region.Width, "Add Title Page");
            this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 172, region.Width, "Add Text Page");
            this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 244, region.Width, "Add Image Page");
        }

        public override Bookshelf.Book.Page Serialize()
        {
            return null;
        }

        public override void Click(Rectangle region, int x, int y) { }
    }
}
