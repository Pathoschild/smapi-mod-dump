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
using StardewValley;
using System.Linq;

namespace RuneMagic.Source.Spells
{
    public class Invisibility : Spell
    {
        public Invisibility() : base(School.Alteration)
        {
            Description += "Makes the caster invisible"; Level = 4;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<Invisible>().Any())
            {
                Effect = new Invisible(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}