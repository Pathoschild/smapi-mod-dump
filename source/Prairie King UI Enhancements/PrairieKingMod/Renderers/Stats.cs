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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.AbigailGame;

namespace PrairieKingUIEnhancements
{
    public class Stats : Renderable
    {
        private int deaths = 0;
        private int eventDeaths = 0;
        private float deathTimer = 0;
        private float eventDeathTimer = 0;

        public override void SaveLoaded(Save save)
        {
            deaths = save.deaths;
        }

        public override void Save(ref Save save)
        {
            save.deaths = deaths;
        }

        private void checkDeaths(AbigailGame game, ref int deaths, ref float timer)
        {
            if (deaths > 0 && !game.died && game.whichRound == 0)
            {
                deaths = 0;
            }
            if (game.died && AbigailGame.deathTimer > timer)
            {
                deaths++;
            }
            timer = AbigailGame.deathTimer;
        }

        public override void Tick(Config config, AbigailGame game)
        {
            if (AbigailGame.playingWithAbigail)
            {
                checkDeaths(game, ref eventDeaths, ref eventDeathTimer);
            }
            else
            {
                checkDeaths(game, ref deaths, ref deathTimer);
            }
        }

        public override void Render(SpriteBatch b, Config config, AbigailGame game)
        {
            int offset = config.FixNumberOverflow ? 24 : 0;

            if (config.FixNumberOverflow)
            {
                DrawRect(
                    b,
                    new Rectangle(
                        (int)AbigailGame.topLeftScreenCoordinate.X - AbigailGame.TileSize * 2,
                        (int)AbigailGame.topLeftScreenCoordinate.Y + AbigailGame.TileSize + 18,
                        AbigailGame.TileSize * 2,
                        AbigailGame.TileSize * 2 + AbigailGame.TileSize / 4 + 18),
                    Game1.bgColor);

                b.Draw(
                    Game1.mouseCursors,
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize * 2 + offset, -AbigailGame.TileSize - 18),
                    new Rectangle(400, 1776, 16, 16),
                    Color.White, 
                    0f, 
                    Vector2.Zero, 
                    3f, 
                    SpriteEffects.None, 
                    0.5f);
                b.DrawString(
                    Game1.smallFont,
                    "x" + ToShortString(Math.Max(game.lives, 0)),
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize + offset, -AbigailGame.TileSize - AbigailGame.TileSize / 4 - 18), 
                    Color.White);
                b.Draw(
                    Game1.mouseCursors, 
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize * 2 + offset, -AbigailGame.TileSize * 2 - 18), 
                    new Rectangle(272, 1808, 16, 16), 
                    Color.White, 
                    0f, 
                    Vector2.Zero, 
                    3f, 
                    SpriteEffects.None, 
                    0.5f);
                b.DrawString(
                    Game1.smallFont, 
                    "x" + ToShortString(game.coins),
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize + offset, -AbigailGame.TileSize * 2 - AbigailGame.TileSize / 4 - 18),
                    Color.White);
            }

            if (config.ShowDeathCounter)
            {
                b.Draw(
                    Game1.mouseCursors,
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize * 2 + offset, -AbigailGame.TileSize * 3 - 18),
                    new Rectangle(496, 1776, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    0.5f);
                b.DrawString(
                    Game1.smallFont,
                    "x" + ToShortString(AbigailGame.playingWithAbigail ? eventDeaths : deaths),
                    AbigailGame.topLeftScreenCoordinate - new Vector2(AbigailGame.TileSize + offset, -AbigailGame.TileSize * 3 - AbigailGame.TileSize / 4 - 18),
                    Color.White);
            }
        }
    }
}
