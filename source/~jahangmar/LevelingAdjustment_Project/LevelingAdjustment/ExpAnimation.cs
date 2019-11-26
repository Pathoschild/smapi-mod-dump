// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace LevelingAdjustment
{
    public class ExpAnimation
    {
        private int offX = 0;
        private int offY = 0;
        private int life = 90;
        private readonly string expString;
        private readonly Color skillColor;

        public ExpAnimation(int exp, int skill)
        {
            this.expString = exp.ToString();
            switch (skill)
            {
                case 0:
                    this.skillColor = Color.Wheat; break;
                case 3:
                    this.skillColor = Color.SteelBlue; break;
                case 1:
                    this.skillColor = Color.Aqua; break;
                case 2:
                    this.skillColor = Color.ForestGreen; break;
                case 4:
                    this.skillColor = Color.OrangeRed; break;
                default:
                    this.skillColor = Color.Black; break;
            }
        }

        public void Draw(SpriteBatch b)
        {
            SpriteFont f = Game1.tinyFont;
            f = Game1.tinyFontBorder;
            Vector2 pos = Game1.player.Position + new Vector2(Game1.player.GetBoundingBox().Width / 2 + offX - Game1.viewport.X, -Game1.player.GetBoundingBox().Height + offY - Game1.viewport.Y);
            //b.DrawString(f, expString, pos, skillColor);
            b.DrawString(Game1.tinyFont, expString, pos, skillColor, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
            b.DrawString(Game1.tinyFontBorder, expString, pos - new Vector2(4, 0), Color.Black, 0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
        }

        public void update(GameTime gameTime)
        {
            life -= 1;
            offY -= 2;
            offX += 1;
        }

        public bool Expired() => life <= 0;
    }
}
