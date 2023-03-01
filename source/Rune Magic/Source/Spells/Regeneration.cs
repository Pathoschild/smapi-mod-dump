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
    public class Regeneration : Spell
    {
        public Regeneration() : base(School.Abjuration)
        {
            Description += "Slowly regenerates the caster's Stamina.";
            Level = 4;
        }

        public override bool Cast()
        {
            if (!RuneMagic.PlayerStats.ActiveEffects.OfType<Regenerating>().Any())
            {
                Effect = new Regenerating(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}