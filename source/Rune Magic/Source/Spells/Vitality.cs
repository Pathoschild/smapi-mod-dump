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
    public class Vitality : Spell
    {
        public Vitality() : base(School.Abjuration)
        {
            Description += "Increases the casters Health and Stamina for a long period of time."; Level = 4;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<Vitalized>().Any())
            {
                Effect = new Vitalized(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}