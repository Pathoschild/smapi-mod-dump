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

namespace PrairieKingUIEnhancements
{
    public class LevelIndicators : Renderable
    {
        private Texture2D indicators;

        public LevelIndicators(Texture2D indicators)
        {
            this.indicators = indicators;
        }

        public override void Render(SpriteBatch b, Config config, AbigailGame game)
        {
            int levelIndicatorHeight = (AbigailGame.whichWave + game.whichRound * 12) * 18;
            if (config.ColoredIndicators || config.FixLevelOverflow)
            {
                DrawRect(
                b,
                new Rectangle(
                    (int)AbigailGame.topLeftScreenCoordinate.X + AbigailGame.TileSize * 16 + 3,
                    (int)AbigailGame.topLeftScreenCoordinate.Y,
                    15,
                    levelIndicatorHeight),
                Game1.bgColor);
            }

            if (config.FixLevelOverflow)
            {
                for (int i = 0; i < AbigailGame.whichWave; i++)
                {
                    int world = config.ColoredIndicators ? i < 5 ? 0 : i < 9 ? 1 : 2 : 0;
                    b.Draw(indicators,
                        AbigailGame.topLeftScreenCoordinate + new Vector2(AbigailGame.TileSize * 16 + 3, i * 18),
                        new Rectangle(0, world * 5, 5, 5),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3f,
                        SpriteEffects.None,
                        0.5f);
                }

                if (game.whichRound > 0)
                {
                    b.Draw(Game1.mouseCursors,
                        AbigailGame.topLeftScreenCoordinate + new Vector2(AbigailGame.TileSize * 16 + AbigailGame.TileSize / 2, 0),
                        new Rectangle(528, 1776, 16, 16),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3f,
                        SpriteEffects.None,
                        0.5f);
                    b.DrawString(Game1.smallFont,
                        "x" + game.whichRound,
                        AbigailGame.topLeftScreenCoordinate + new Vector2(AbigailGame.TileSize * 17 + AbigailGame.TileSize / 2, AbigailGame.TileSize / 4),
                        Color.White);
                }
            }
            else if (config.ColoredIndicators)
            {
                for (int i = 0; i < AbigailGame.whichWave + game.whichRound * 13; i++)
                {
                    int lvl = i % 13;
                    int world = lvl < 5 ? 0 : lvl < 9 ? 1 : 2;
                    b.Draw(indicators,
                        AbigailGame.topLeftScreenCoordinate + new Vector2(AbigailGame.TileSize * 16 + 3, i * 18),
                        new Rectangle(0, world * 5, 5, 5),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3f,
                        SpriteEffects.None,
                        0.5f);
                }
            }
        }
    }
}
