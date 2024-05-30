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
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Components
{
    public class LifeTap : AbstractComponent
    {
        public LifeTap() : base(new SpellPartStats(SpellPartStatType.DAMAGE), new SpellPartStats(SpellPartStatType.HEALING))
        {

        }

        public override string GetId()
        {
            return "life_tap";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            if (target.GetCharacter() is Monster living)
            {
                var helper = SpellHelper.Instance();
                float damage = helper.GetModifiedStat(2, new SpellPartStats(SpellPartStatType.DAMAGE), modifiers, spell, caster, target, index) * 2;

                Farmer farmer = ((Farmer)caster.entity);

                if (gameLocation.damageMonster(living.GetBoundingBox(), (int)damage, (int)(damage * (1f + farmer.buffs.AttackMultiplier)), false, farmer))
                {
                    farmer.AddMana((int)(damage * (farmer.GetMaxMana() / 25)));
                }

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            if (target.GetCharacter() is Farmer targetFarmer)
            {
                var helper = SpellHelper.Instance();
                float damage = helper.GetModifiedStat(2, new SpellPartStats(SpellPartStatType.DAMAGE), modifiers, spell, caster, target, index) * 2;

                targetFarmer.health -= (int)damage;

                Farmer farmer = ((Farmer)caster.entity);
                farmer.AddMana((int)(damage * (farmer.GetMaxMana() / 25)));

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return 5;
        }
    }
}
