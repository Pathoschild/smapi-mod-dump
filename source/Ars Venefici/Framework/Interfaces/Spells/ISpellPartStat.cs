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
    /// Interface representing a spell part stat.
    /// </summary>
    public interface ISpellPartStat
    {
        
        /// <returns>The id of this spell part stat.</returns>
        string GetId();

        bool equals(ISpellPartStat other)
        {
            return GetId().Equals(other.GetId());
        }
    }
}
