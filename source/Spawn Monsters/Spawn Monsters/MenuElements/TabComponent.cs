using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Spawn_Monsters
{
    internal class TabComponent : ClickableComponent
    {
        public TabComponent(Rectangle b, string n)
            : base(b, n) {

        }

        public void Draw(SpriteBatch b, bool current) {

            Game1.spriteBatch.Draw(Game1.mouseCursors,
                new Vector2(bounds.X, current ? bounds.Y + 24 : bounds.Y),
                new Rectangle(16, 368, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);

            b.Draw(Game1.mouseCursors,
                new Vector2(bounds.X + 8, current ? bounds.Y + 38 : bounds.Y + 14),
                new Rectangle(32, 672, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                1f);
        }
    }
}
