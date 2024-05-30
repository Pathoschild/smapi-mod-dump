/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Util;
using StardewValley;
using StardewValley.Objects;
using System;
using static ArsVenefici.Framework.Interfaces.Spells.ISpellPart;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    public interface ISpellShape : ISpellPart
    {

        /// <summary>
        /// Casts the spell.
        /// </summary>
        /// <param name="spell">The spell that is cast.</param>
        /// <param name="caster">The player that casts the spell.</param>
        /// <param name="gameLocation">The location that the caster is in.</param>
        /// <param name="modifiers">A list of modifiers that affect this spell cast.</param>
        /// <param name="hit">The target of the spell.</param>
        /// <param name="ticksUsed">The amount of ticks this spell has been cast already.</param>
        /// <param name="index">The index of the next component.</param>
        /// <param name="awardXp">Whether to grant the player magic xp or not.</param>
        /// <returns>A SpellCastResult that represents the spell casting outcome.</returns>
        SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, HitResult hit, int ticksUsed, int index, bool awardXp);

        /// <returns>A set containing all spell part stats that affect this shape.</returns>
        HashSet<ISpellPartStat> GetStatsUsed();


        /// <returns>Whether the shape is continuous or not.</returns>
        bool IsContinuous();

        /// <returns>True if this shape can only be at the end, false otherwise.</returns>
        bool IsEndShape();


        ///<returns> True if this shape can not be at the beginning, false otherwise.</returns>
        bool NeedsPrecedingShape();

        ///<returns> True if this shape can only be at the beginning, false otherwise.</returns>
        bool NeedsToComeFirst();

        SpellPartType ISpellPart.GetType()
        {
            return SpellPartType.SHAPE;
        }
    }
}
