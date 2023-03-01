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
    public class Charming : Spell
    {
        public Charming() : base(School.Alteration)
        {
            Description += "Charms the target for a period time.";
            Level = 2;
        }

        public override bool Cast()
        {
            Target = Game1.currentLocation.characters.FirstOrDefault(c => c.getTileLocation() == Game1.currentCursorTile);
            if (!RuneMagic.PlayerStats.ActiveEffects.OfType<Charmed>().Any())
            {
                Effect = new Charmed(this, Target);
                return base.Cast();
            }
            else
                return false;
        }
    }
}