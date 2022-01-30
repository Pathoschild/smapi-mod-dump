/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Incognito357/PrairieKingUIEnhancements
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrairieKingUIEnhancements
{
    class LevelTransitionColor : Renderable
    {
        public override void Render(SpriteBatch b, Config config, AbigailGame game)
        {
            if (!config.FixTransitionColor || !AbigailGame.scrollingMap)
            {
                return;
            }

            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)AbigailGame.topLeftScreenCoordinate.X,
                    -1, 
                    16 * AbigailGame.TileSize,
                    (int)AbigailGame.topLeftScreenCoordinate.Y),
                Game1.staminaRect.Bounds,
                Game1.bgColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f);
            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)AbigailGame.topLeftScreenCoordinate.X,
                    (int)AbigailGame.topLeftScreenCoordinate.Y + 16 * AbigailGame.TileSize,
                    16 * AbigailGame.TileSize,
                    (int)AbigailGame.topLeftScreenCoordinate.Y + 2),
                Game1.staminaRect.Bounds,
                Game1.bgColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f);
        }
    }
}
