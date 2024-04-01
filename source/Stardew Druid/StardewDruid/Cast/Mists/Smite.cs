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
using StardewDruid.Event;
using StardewValley;
using System;
using System.Collections.Generic;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Cast.Mists
{
    internal class Smite : CastHandle
    {

        private StardewValley.Monsters.Monster targetMonster;

        public float damage;

        public Smite(Vector2 target,  StardewValley.Monsters.Monster TargetMonster, float Damage)
            : base(target)
        {

            int castCombat = Game1.player.CombatLevel / 2;

            castCost = Math.Max(6, 12 - castCombat);

            targetMonster = TargetMonster;

            damage = Damage;

        }

        public override void CastEffect()
        {

            if (targetMonster == null || targetMonster.Health <= 0 || !targetLocation.characters.Contains(targetMonster))
            {

                return;

            }

            float critChance = 0.1f;

            if (!Mod.instance.rite.castTask.ContainsKey("masterSmite"))
            {

                Mod.instance.UpdateTask("lessonSmite", 1);

            }
            else
            {

                critChance += 0.3f;

            }

            SpellHandle bolt = new(targetLocation, targetVector * 64, Game1.player.Position, 1, 1, -1, damage);

            bolt.type = SpellHandle.barrages.bolt;

            bolt.critical = critChance;

            bolt.monster = targetMonster;

            Mod.instance.spellRegister.Add(bolt);

            castFire = true;

        }

    }

}
