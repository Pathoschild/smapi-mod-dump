/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using RuneMagic.Source.Effects;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Spells
{
    public class Firebolt : Spell
    {
        public Firebolt() : base(School.Evocation)
        {
            Description += "Shoots bolt of fire to the target."; Level = 2;
        }

        public override bool Cast()
        {
            var texture = RuneMagic.Textures["spell_fireball"];
            var minDamage = 1;
            var maxDamage = 6;
            var bonusDamage = 2 * Player.MagicStats.ActiveSchool.Level;
            var area = 1;
            var speed = 5;

            Game1.currentLocation.projectiles.Add(new SpellProjectile(texture, minDamage, maxDamage, bonusDamage, area, speed, false));
            return base.Cast();
        }
    }
}