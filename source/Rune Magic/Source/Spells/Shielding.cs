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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Spells
{
    public class Shielding : Spell
    {
        public Shielding() : base(School.Abjuration)
        {
            Description += "Rises the caster's defense."; Level = 1;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<Shielded>().Any())
            {
                Effect = new Shielded(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}