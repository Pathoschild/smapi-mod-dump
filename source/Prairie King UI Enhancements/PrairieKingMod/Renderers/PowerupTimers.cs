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
    public class PowerupTimers : Renderable
    {
        Dictionary<int, int> powerups = new Dictionary<int, int>();
        Dictionary<int, int> powerupsMax = new Dictionary<int, int>();

        private void updateValue(int key, int value)
        {
            if (powerups.ContainsKey(key))
            {
                if (value > powerups[key])
                {
                    powerupsMax[key] = value;
                }
            }
            else
            {
                powerupsMax[key] = value;
            }
            powerups[key] = value;
        }

        public override void Tick(Config config, AbigailGame game)
        {
            if (!config.ShowPowerupTimers)
            {
                return;
            }

            HashSet<int> toRemove = powerups.Keys.ToHashSet();
            foreach (var kvp in game.activePowerups)
            {
                updateValue(kvp.Key, kvp.Value);
                toRemove.Remove(kvp.Key);
            }

            if (AbigailGame.zombieModeTimer > 0)
            {
                updateValue(5, AbigailGame.zombieModeTimer);
                toRemove.Remove(5);
            }

            if (AbigailGame.monsterConfusionTimer > 0)
            {
                updateValue(9, AbigailGame.monsterConfusionTimer);
                toRemove.Remove(9);
            }

            foreach (int key in toRemove)
            {
                powerups.Remove(key);
            }
        }

        public override void Render(SpriteBatch b, Config config, AbigailGame game)
        {
            if (!config.ShowPowerupTimers || AbigailGame.scrollingMap)
            {
                return;
            }
            int i = AbigailGame.shootoutLevel ? 1 : 0;
            foreach (var kvp in powerups.OrderByDescending(x => x.Value))
            {
                float percentage = (float)kvp.Value / powerupsMax[kvp.Key];
                b.Draw(
                        Game1.mouseCursors,
                        AbigailGame.topLeftScreenCoordinate + new Vector2(0, AbigailGame.TileSize * 16 + (AbigailGame.TileSize / 2 + 10) * i),
                        new Rectangle(272 + kvp.Key * 16, 1808, 16, 16),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        2f,
                        SpriteEffects.None,
                        0.5f);
                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(
                        (int)AbigailGame.topLeftScreenCoordinate.X + AbigailGame.TileSize / 2 + 12,
                        (int)AbigailGame.topLeftScreenCoordinate.Y + AbigailGame.TileSize * 17 + (AbigailGame.TileSize / 2 + 10) * i - AbigailGame.TileSize + 10,
                        (int)((float)(16 * AbigailGame.TileSize - AbigailGame.TileSize / 2 + 3) * percentage),
                        AbigailGame.TileSize / 4),
                    (percentage < 0.2f) ? new Color(188, 51, 74) : new Color(147, 177, 38));
                i++;
            }
        }
    }
}
