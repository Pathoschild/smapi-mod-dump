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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{

    public class Bat : StardewValley.Monsters.Bat
    {

        public Bat() { }

        public Bat(Vector2 vector, int combatModifier, bool champion = false)
            : base(vector * 64, combatModifier * 10)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 15;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(10, Math.Max(20, combatModifier));
            
            objectsToDrop.Clear();

            objectsToDrop.Add("767");

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add("767");
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add("767");
            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                List<string> batElixers = new()
                {
                    "772","773","879",
                };

                objectsToDrop.Add(batElixers[Game1.random.Next(batElixers.Count)]);

            }

            if (champion)
            {
                isHardModeMonster.Set(true);

            }
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            DialogueData.DisplayText(this, 3);

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void onDealContactDamage(Farmer who)
        {

            if ((who.health + who.buffs.Defense) - DamageToFarmer < 10)
            {

                who.health = (DamageToFarmer - who.buffs.Defense) + 10;

                Mod.instance.CriticalCondition();

            }

        }

    }

}
