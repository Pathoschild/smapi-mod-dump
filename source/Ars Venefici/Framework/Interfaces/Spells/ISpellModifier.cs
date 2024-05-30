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
using static ArsVenefici.Framework.Interfaces.Spells.ISpellPart;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    /// Base interface for a spell modifier.
    /// </summary>
    public interface ISpellModifier : ISpellPart
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat">The stat to modify.</param>
        /// <returns>A modifier for the given stat.</returns>
        ISpellPartStatModifier GetStatModifier(ISpellPartStat stat);

        /// <returns>A set containing all stats this modifier modifies.</returns>
        List<ISpellPartStat> GetStatsModified();

        SpellPartType ISpellPart.GetType()
        {
            return SpellPartType.MODIFIER;
        }
    }
}
