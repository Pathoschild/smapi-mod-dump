/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using RuneMagic.Source.Effects;
using StardewValley;
using System.Linq;

namespace RuneMagic.Source.Spells
{
    public class Warding : Spell
    {
        public Warding() : base(School.Abjuration)
        {
            Description += "Protects the caster from damage for a short period of time.";
            Level = 5;
        }

        public override bool Cast()
        {
            if (!RuneMagic.PlayerStats.ActiveEffects.OfType<Warded>().Any())
            {
                Effect = new Warded(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}