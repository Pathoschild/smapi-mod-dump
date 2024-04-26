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
using StardewDruid.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class Shadow : ShadowBrute
    {

        public Shadow() { }

        public Shadow(Vector2 position, int combatModifier, bool champion = false)
            : base(position * 64)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 25;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(10, Math.Max(20, combatModifier));

            objectsToDrop.Clear();

            objectsToDrop.Add("769");

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add("768");
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                List<string> shadowGems = new()
                {
                    "62","66","68","70",
                };

                objectsToDrop.Add(shadowGems[Game1.random.Next(shadowGems.Count)]);

            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                List<string> shadowGems = new()
                {
                    "60","64","72",
                };

                objectsToDrop.Add(shadowGems[Game1.random.Next(shadowGems.Count)]);
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
