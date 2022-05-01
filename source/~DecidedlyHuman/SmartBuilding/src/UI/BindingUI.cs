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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SmartBuilding.UI
{
#if DEBUG
    public class BindingUi : IClickableMenu
    {
        private Texture2D texture;

        public BindingUi(int x, int y, int width, int height, Texture2D texture, bool showCloseButton = false) : base(x, y, width, height, showCloseButton)
        {
            this.texture = texture;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(
                texture: texture,
                position: new Vector2(this.xPositionOnScreen / 64, this.yPositionOnScreen / 64),
                sourceRectangle: texture.Bounds,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );

            base.draw(b);
        }
    }
#endif
}