/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_SPACECORE
namespace SpaceCore.UI
{
    public
#else
namespace SpaceShared.UI
{
    internal
#endif
    class ItemSlot : ItemWithBorder
    {
        public Item Item { get; set; }

        public override void Draw( SpriteBatch b )
        {
            if (BoxColor.HasValue)
            {
                if (BoxIsThin)
                    b.Draw(Game1.menuTexture, Position, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), BoxColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                else
                    IClickableMenu.drawTextureBox(b, (int)Position.X, (int)Position.Y, Width, Height, BoxColor.Value);
            }
            if ( Item != null )
                Item.drawInMenu( b, Position + (BoxIsThin ? Vector2.Zero : new Vector2(16, 16)), 1, 1, 1 );
            else if ( ItemDisplay != null )
                ItemDisplay.drawInMenu( b, Position + (BoxIsThin ? Vector2.Zero : new Vector2(16, 16)), 1, TransparentItemDisplay ? 0.5f : 1, 1 );
        }
    }
}
