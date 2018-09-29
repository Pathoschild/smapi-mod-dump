using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Kisekae.Menu {
    class PortraitComponent: IAutoComponent {
        public bool m_visible { get; set; } = true;
        public string m_name { get; set; }
        private int m_x, m_y;

        public PortraitComponent(int x, int y) {
            m_x = x;
            m_y = y;
        }

        public void draw(SpriteBatch b) {
            b.Draw(Game1.daybg, new Vector2(m_x, m_y), Color.White);
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2(
                m_x + Game1.tileSize / 2,
                m_y + Game1.tileSize / 2
                ), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);
        }

        public bool containsPoint(int x, int y) {
            return false;
        }
    }
}
