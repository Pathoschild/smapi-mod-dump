/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatOverhaulMod.Buffs.Effects
{
    public abstract class InstantEffect : Effect
    {
        public override void Tick( Character character, float delta, float durationUsed, float duration, float modifier )
        {
        }

        public override void Unapply( Character character, float modifier )
        {
        }
    }
}
