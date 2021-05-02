/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.AutoSpeed.Framework
{
    /// <summary>
    /// Interface used to interface AutoSpeed's API class. 
    /// </summary>
    public interface IAutoSpeedAPI
    {
        void addSpeedBoost(string ID, int Amount);
        /// <summary>
        /// Removes an added speed boost by passing in the unique key.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Amount"></param>
        void remvoveSpeedBoost(string ID, int Amount);
    }
}
