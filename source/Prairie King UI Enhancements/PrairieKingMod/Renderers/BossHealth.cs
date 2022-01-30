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
using static StardewValley.Minigames.AbigailGame;

namespace PrairieKingUIEnhancements
{
    class BossHealth : Renderable
    {
        public override void Tick(Config config, AbigailGame game)
        {
            if (game.whichRound > 0 && AbigailGame.whichWave == 12)
            {
                if (AbigailGame.monsters.Count != 1) {
                    return;
                }
                Dracula dracula = AbigailGame.monsters.Last() as Dracula;
                if (dracula is null)
                {
                    return;
                }

                if (config.FixBossHPOverflow && dracula.health > dracula.fullHealth)
                {
                    int baseHealth = dracula.fullHealth;
                    dracula.fullHealth = dracula.health;
                    if (config.HarderFinalBoss)
                    {
                        dracula.fullHealth = baseHealth * (game.whichRound + 1);
                        dracula.health = dracula.fullHealth;
                    }
                }
            }
        }
    }
}
