/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace SpaceShared.UI
{
    internal class StaticContainer : Container
    {
        public Vector2 Size { get; set; }

        public Color? OutlineColor { get; set; } = null;

        public override int Width => (int)this.Size.X;

        public override int Height => (int)this.Size.Y;

        public override void Draw(SpriteBatch b)
        {
            if (this.OutlineColor.HasValue)
            {
                IClickableMenu.drawTextureBox(b, (int)this.Position.X - 12, (int)this.Position.Y - 12, this.Width + 24, this.Height + 24, this.OutlineColor.Value);
            }
            base.Draw(b);
        }
    }
}
