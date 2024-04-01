/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using CombatOverhaulMod.Elements;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatOverhaulMod.Buffs.Effects
{
    public class EffectOverTimeEffect : Effect
    {
        public InstantEffect effect;

        public EffectOverTimeEffect( InstantEffect effect )
        {
            this.effect = effect;
        }

        public override string Id => effect.Id.Replace( ".instant", "" );

        public override void Apply( Character character, float modifier )
        {
        }

        public override void Tick( Character character, float delta, float durationUsed, float duration, float modifier )
        {
            if ( durationUsed == 0 || ( ( durationUsed - delta ) % 1 ) > ( durationUsed % 1 ) )
            {
                effect.Apply( character, modifier );
            }
        }

        public override void Unapply( Character character, float modifier )
        {
        }
    }
}
