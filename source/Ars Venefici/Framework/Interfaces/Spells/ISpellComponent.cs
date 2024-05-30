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
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArsVenefici.Framework.Interfaces.Spells.ISpellPart;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    ///  Base interface for spell components
    /// </summary>
    public interface ISpellComponent : ISpellPart
    {

        /// <returns>The id of this spell part.</returns>
        string GetId();

        /// <summary>
        /// Invoke this spell component for an npc.
        /// </summary>
        /// <param name="spell">The spell being cast.</param>
        /// <param name="caster">The caster of the spell.</param>
        /// <param name="gameLocation">The location the spell is being cast in.</param>
        /// <param name="modifiers">The modifiers modifying this component.</param>
        /// <param name="target">The target.</param>
        /// <param name="index">The index of this spell component in the spell execution stack.</param>
        /// <param name="ticksUsed">The amount of ticks the spell is being cast for.</param>
        /// <returns>The spell cast result (success if anything was affected).</returns>
        SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed);

        /// <summary>
        ///  Invoke this spell component for a terrain feature.
        /// </summary>
        /// <param name="spell">The spell being cast.</param>
        /// <param name="caster">The caster of the spell.</param>
        /// <param name="gameLocation">The location the spell is being cast in.</param>
        /// <param name="modifiers">The modifiers modifying this component.</param>
        /// <param name="target">The target.</param>
        /// <param name="index">The index of this spell component in the spell execution stack.</param>
        /// <param name="ticksUsed">The amount of ticks the spell is being cast for.</param>
        /// <returns>The spell cast result (success if anything was affected).</returns>
        SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed);

        /// <returns>The stats used by this spell part.</returns>
        HashSet<ISpellPartStat> GetStatsUsed();

        SpellPartType ISpellPart.GetType()
        {
            return SpellPartType.COMPONENT;
        }
    }
}
