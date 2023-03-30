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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Spells
{
    public class Cleansing : Spell
    {
        public Cleansing() : base(School.Abjuration)
        {
            Description += "Removes all effects from the player for a short duration of time."; Level = 2;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<Cleansed>().Any())
            {
                Effect = new Cleansed(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}