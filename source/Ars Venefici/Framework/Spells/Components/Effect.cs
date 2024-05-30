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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Effect : AbstractComponent
    {
        private Buff buff;
        private string id;
        private int manaCost;

        public Effect(string id, int manaCost, Buff buff) : base(new SpellPartStats(SpellPartStatType.DURATION), new SpellPartStats(SpellPartStatType.POWER))
        {
            this.id = id;
            this.manaCost = manaCost;
            this.buff = buff;
        }

        public override string GetId()
        {
            return id;
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            var helper = SpellHelper.Instance();
            //int amplifier = (int)helper.GetModifiedStat(30, new SpellPartStats(SpellPartStatType.POWER), modifiers, spell, caster, target, index);
            int duration = (int)helper.GetModifiedStat(30_000, new SpellPartStats(SpellPartStatType.DURATION), modifiers, spell, caster, target, index);

            buff.millisecondsDuration = duration;

            Game1.player.applyBuff(buff);

            return new SpellCastResult(SpellCastResultType.SUCCESS);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return manaCost;
        }
    }
}
