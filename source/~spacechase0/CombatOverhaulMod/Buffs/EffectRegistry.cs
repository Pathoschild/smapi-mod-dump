/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using CombatOverhaulMod.Buffs.Effects;
using CombatOverhaulMod.Elements;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatOverhaulMod.Buffs
{
    public static class EffectRegistry
    {
        private static Dictionary<string, Effect> effects = new();

        public static void Add( Effect effect )
        {
            effects.Add( effect.Id, effect );
        }

        public static Effect Get( string id )
        {
            return effects[ id ];
        }

        public static string[] GetAll()
        {
            return effects.Keys.ToArray();
        }

        internal static void InitializeDefaultEffects()
        {
            effects.Clear();

            Add( new InstantDamageEffect( false, null ) );
            Add( new InstantDamageEffect( true, null ) );

            var elems = Game1.content.Load< Dictionary<string, ElementData> >( "spacechase0.CombatOverhaulMod\\Elements" );
            foreach ( var elem in elems )
            {
                Add( new InstantDamageEffect( false, elem.Key ) );
                Add( new InstantDamageEffect( true, elem.Key ) );
            }

            Add( new InstantStaminaEffect( false ) );
            Add( new InstantStaminaEffect( true ) );
            //Add( new InstantManaEffect( false ) );
            //Add( new InstantManaEffect( true ) );
            
            foreach ( string effectId in GetAll() )
            {
                var effect = Get( effectId );
                if ( effect is InstantEffect instant )
                    Add( new EffectOverTimeEffect( instant ) );
            }
        }
    }
}
