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
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    /// Interface that represents a spell.
    /// </summary>
    public interface ISpell
    {
        string GetName();

        /// <returns>Whether the spell is continuous or not.</returns>
        bool IsContinuous();

        /// <returns>Whether the spell is empty or not.</returns>
        bool IsEmpty();

        
        /// <returns>Whether all parts of the spell are non-null.</returns>
        bool IsNonNull();

        /// <summary>
        /// Validates the spell. This checks for non-emptiness of the spell stack, the shape groups being correct, and correct location of modifiers (if present).
        /// </summary>
        /// <returns>Whether the spell is valid or not.</returns>
        bool IsValid();

        /// <param name="currentShapeGroup">The shape group to get the shape for.</param>
        /// <returns>The first shape of the given shape group.</returns>
        ISpellShape FirstShape(int currentShapeGroup);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeGroup">The shape group index to get the shape group for.</param>
        /// <returns>The shape group by the given shape group index.</returns>
        ShapeGroup ShapeGroup(int shapeGroup);

        /// <returns>The currently selected shape group.</returns>
        ShapeGroup CurrentShapeGroup();

        /// <returns>The index of the currently selected shape group.</returns>
        int CurrentShapeGroupIndex();

        /// <summary>
        /// Sets the current shape group index.
        /// </summary>
        /// <param name="shapeGroup">The shape group index to set.</param>
        void CurrentShapeGroupIndex(int shapeGroup);

        /// <summary>
        /// Casts the spell.
        /// </summary>
        /// <param name="caster">The player that casts the spell.</param>
        /// <param name="gameLocation">The location that the caster is in.</param>
        /// <param name="castingTicks">The amount of ticks this spell has been cast already.</param>
        /// <param name="consume">Whether to consume the spell result or not.</param>
        /// <param name="awardXp">Whether to grant the player magic xp or not.</param>
        /// <returns>A SpellCastResult that represents the spell casting outcome.</returns>
        SpellCastResult Cast(IEntity caster, GameLocation gameLocation, int castingTicks, bool consume, bool awardXp);

        /// <returns>An list that represents a part list with their corresponding modifiers.</returns>
        List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> PartsWithModifiers();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caster">The player that casts this spell.</param>
        /// <returns>The amount of mana this spell costs.</returns>
        int Mana();

        /// <returns>The list of spell parts in this spell.</returns>
        List<ISpellPart> Parts();
        
        /// <returns>The list of shape groups in this spell.</returns>
        List<ShapeGroup> ShapeGroups();

        /// <returns>The spell stack for this spell.</returns>
        SpellStack SpellStack();
    }
}
