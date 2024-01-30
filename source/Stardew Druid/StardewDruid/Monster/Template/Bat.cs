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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{

    public class Bat : StardewValley.Monsters.Bat
    {

        public Bat() { }

        public Bat(Vector2 vector, int combatModifier)
            : base(vector * 64, combatModifier * 10)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 15;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(5, Math.Max(15, combatModifier));

            objectsToDrop.Clear();

            objectsToDrop.Add(767);

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

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

                List<string> ouchList= new()
            {
                "flap flap",
                "flippity",
                "cheeep"
            };
                int ouchIndex = Game1.random.Next(10);

                if (ouchIndex < ouchList.Count)
                {
                    showTextAboveHead(ouchList[ouchIndex], duration: 2000);
                }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }


    }

}
