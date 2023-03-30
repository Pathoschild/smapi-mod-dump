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
    public class Strength : Spell
    {
        public Strength() : base(School.Abjuration)
        {
            Description += "Increases the caster's attack damage."; Level = 3;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<Strengthened>().Any())
            {
                Effect = new Strengthened(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}