/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    /// The result of a spell cast.
    /// </summary>
    public class SpellCastResult
    {
        SpellCastResultType spellCastResultType;

        public SpellCastResult()
        {
            spellCastResultType = SpellCastResultType.SUCCESS;
        }

        public SpellCastResult(SpellCastResultType spellCastResultType) 
        {
            this.spellCastResultType = spellCastResultType;
        }

        public void SetSpellCastResultType(SpellCastResultType spellCastResultType)
        {
            this.spellCastResultType = spellCastResultType;
        }

        public SpellCastResultType GetSpellCastResultType()
        {
            return this.spellCastResultType;
        }

        /// <returns>True if this spell cast result represents a failed cast, false otherwise.</returns>
        public bool IsFail()
        {
            return spellCastResultType != SpellCastResultType.SUCCESS;
        }

        /// <returns>True if this spell cast result represents a successful cast, false otherwise.</returns>
        public bool IsSuccess()
        {
            return spellCastResultType == SpellCastResultType.SUCCESS;
        }

        /// <returns>True if this spell cast result means that mana should be consumed, burnout should be given and reagents should be consumed, false otherwise.</returns>
        public bool IsConsume()
        {
            return spellCastResultType == SpellCastResultType.SUCCESS;
        }
    }

    public enum SpellCastResultType
    {
        SUCCESS,
        NOT_ENOUGH_MANA,
        EFFECT_FAILED
    }
}
