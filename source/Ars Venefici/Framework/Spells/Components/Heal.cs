/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Heal : AbstractComponent
    {
        public Heal() : base(new SpellPartStats(SpellPartStatType.HEALING))
        {
            
        }

        public override string GetId()
        {
            return "heal";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation level, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

            var helper = SpellHelper.Instance();

            if (target.GetCharacter() is Farmer living) 
            {
                float healing = helper.GetModifiedStat(15, new SpellPartStats(SpellPartStatType.HEALING), modifiers, spell, caster, target, index);

                //if (living.isInvertedHealAndHarm())
                //{
                //    living.hurt(level.damageSources().indirectMagic(caster, caster), healing);
                //}
                //else
                //{
                //    living.heal(healing);
                //}

                //living.health += (int)healing;
                //living.health -= (int)healing;

                living.health = Math.Min(living.maxHealth, living.health + (int)healing);

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation level, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return 25;
        }
    }
}
