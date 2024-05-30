/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Util;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    public interface ISpellHelper
    {
        public void SetSpell(SpellBook spellBook, ISpell spell);


        Character GetPointedCharacter(Character entity, double range);

        HitResult Trace(ModEntry modEntry, Character entity, GameLocation level, double range, bool entities, bool mouseCursor);

        /// <summary>
        /// Get the stat value modified by the modifiers.
        /// </summary>
        /// <param name="baseValue">The base value for the stat.</param>
        /// <param name="stat">The stat that is modified.</param>
        /// <param name="spell">The spell that the part belongs to.</param>
        /// <param name="caster">The entity casting the spell.</param>
        /// <param name="target">The target of the spell cast.</param>
        /// <param name="componentIndex">The 1 based index of the currently invoked part.</param>
        /// <returns>The modified value of the stat.</returns>
        float GetModifiedStat(float baseValue, ISpellPartStat stat, List<ISpellModifier> modifiers, ISpell spell, IEntity caster, HitResult target, int componentIndex);

        /// <summary>
        /// Casts the spell.
        /// </summary>
        /// <param name="spell">The spell to cast.</param>
        /// <param name="caster">The entity casting the spell.</param>
        /// <param name="level">The level the spell is cast in.</param>
        /// <param name="target">The target of the spell cast.</param>
        /// <param name="castingTicks">How long the spell has already been cast.</param>
        /// <param name="index">The 1 based index of the currently invoked part.</param>
        /// <param name="awardXp"The magic xp awarded for casting this spell.></param>
        /// <returns>A SpellCastResult that represents the spell casting outcome.</returns>
        SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation level, HitResult target, int castingTicks, int index, bool awardXp);

        public void NextShapeGroup(ISpell spell);

        public void PrevShapeGroup(ISpell spell);
    }
}
