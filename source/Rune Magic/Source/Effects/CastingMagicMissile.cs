/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceCore.Skills;

namespace RuneMagic.Source.Effects
{
    public class CastingMagicMissile : SpellEffect
    {
        public int Interval { get; set; }

        public CastingMagicMissile(Spell spell) : base(spell, Duration.Instant)
        {
            Interval = 3;
            Timer = Player.MagicStats.ActiveSchool.Level;
            if (Timer > 12)
                Timer = 12;
            Start();
        }

        public override void Update()
        {
            var texture = RuneMagic.Textures["magic_missile"];
            var minDamage = 1;
            var maxDamage = 4;
            var bonusDamage = Player.MagicStats.ActiveSchool.Level;
            var area = 0;
            var speed = 5;

            if (Timer % (3 * Interval) == 0)
                Game1.currentLocation.projectiles.Add(new SpellProjectile(texture, minDamage, maxDamage, bonusDamage, area, speed, true));
            base.Update();
        }
    }
}