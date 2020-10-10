/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmTypeManager.Monsters
{
    /// <summary>An interface for subclasses of Monster that replace hardcoded damage values with a customizable value.</summary>
    public interface ICustomDamage
    {
        /// <summary>
        /// A value that will preserve and/or replace this monster's hardcoded damage values.
        /// </summary>
        int CustomDamage { get; set; }
    }
}
