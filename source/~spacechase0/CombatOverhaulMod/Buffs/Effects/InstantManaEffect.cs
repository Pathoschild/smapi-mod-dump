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
//using CombatOverhaulMod.Mana;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatOverhaulMod.Buffs.Effects
{
    /*
    public class InstantManaEffect : InstantEffect
    {
        public bool Percentage { get; }

        public InstantManaEffect( bool percentage )
        {
            Percentage = percentage;
        }

        public override string Id => "mana-" + ( Percentage ? "percentage" : "fixed" ) + ".instant";

        public override void Apply( Character character, float modifier )
        {
            if ( character is Farmer farmer )
            {
                farmer.AddMana( Percentage ? ( farmer.GetMaxMana() * modifier ) : modifier );
            }
        }
    }
    */
}
