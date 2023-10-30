/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Monster
{
    static class MonsterData
    {
        public static StardewValley.Monsters.Monster CreateMonster(int spawnMob, Vector2 spawnVector, int combatModifier, bool partyHats = false)
        {

            StardewValley.Monsters.Monster theMonster;

            /*
              
             Medium
                Start + combat 2
                (2+1)^2 * 7 = 63
                Water + combat 5
                (2+2)^2 * 10 = 160
                Stars + combat 8
                (2+3)^2 * 13 = 325
             
             Hard
                Start + combat 2
                (3+1)^2 * 7 = 112
                Water + combat 5
                (3+2)^2 * 10 = 250
                Stars + combat 8
                (3+3)^2 * 13 = 468          

             */

            switch (spawnMob)
            {

                default: // Bat

                    theMonster = new StardewDruid.Monster.Bat(spawnVector, combatModifier);

                    break;

                case 0: // Green Slime

                    theMonster = new StardewDruid.Monster.Slime(spawnVector, combatModifier, partyHats);

                    break;

                case 1: // Shadow Brute

                    theMonster = new StardewDruid.Monster.Shadow(spawnVector, combatModifier);

                    break;

                case 2: // Skeleton

                    theMonster = new StardewDruid.Monster.Skeleton(spawnVector, combatModifier, partyHats);

                    break;

                case 3: // Golem

                    theMonster = new StardewDruid.Monster.Golem(spawnVector, combatModifier, partyHats);

                    break;

                case 4: // DustSpirit

                    theMonster = new StardewDruid.Monster.Spirit(spawnVector, combatModifier, partyHats);

                    break;

                // ------------------ Bosses

                case 11: // Bat

                    theMonster = new StardewDruid.Monster.BossBat(spawnVector, combatModifier);

                    break;

                case 12: // Shooter

                    theMonster = new StardewDruid.Monster.BossShooter(spawnVector, combatModifier);

                    break;

                case 13: // Slime

                    theMonster = new StardewDruid.Monster.BossSlime(spawnVector, combatModifier);

                    break;

                case 14: // Dino Monster

                    theMonster = new StardewDruid.Monster.BossDragon(spawnVector, combatModifier);

                    break;

                case 15: // firebird

                    theMonster = new StardewDruid.Monster.Firebird(spawnVector, combatModifier);

                    break;



            }

            return theMonster;

        }

    }

}
