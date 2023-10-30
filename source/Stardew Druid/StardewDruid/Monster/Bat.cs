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
using StardewValley;
using StardewValley.Monsters;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

namespace StardewDruid.Monster
{

    public class Bat : StardewValley.Monsters.Bat
    {

        public List<string> ouchList;

        public int spawnBuff;

        public int spawnDamage;

        public Bat(Vector2 vector, int combatModifier)
            : base(vector * 64, combatModifier / 2)
        {

            focusedOnFarmers = true;

            Health = (int)(combatModifier * 0.375);

            MaxHealth = Health;

            DamageToFarmer = 0;

            spawnDamage = (int)Math.Max(2, combatModifier * 0.05);

            objectsToDrop.Clear();

            objectsToDrop.Add(767);

            spawnBuff = 60;

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add(767);
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add(767);
            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                List<int> batElixers = new()
                {
                    772,773,879,
                };

                objectsToDrop.Add(batElixers[Game1.random.Next(batElixers.Count)]);

            }

            ouchList = new()
            {
                "flap flap",
                "flippity",
                "cheeep"
            };

        }
        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (spawnBuff > 0)
            {
                return 0;
            }
            
            int ouchIndex = Game1.random.Next(10);

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 2000);
            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if(spawnBuff > 0)
            {

                spawnBuff--;

                if (spawnBuff < 1)
                {

                    DamageToFarmer = spawnDamage;

                }

            }

            base.behaviorAtGameTick(time);

        }

    }

}
