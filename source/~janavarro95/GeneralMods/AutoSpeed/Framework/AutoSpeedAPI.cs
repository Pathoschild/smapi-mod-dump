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
    /// API for auto speed to hook into the Game1.player.addedSpeed function.
    /// </summary>
    public class AutoSpeedAPI
    {

        /// <summary>
        /// Allows adding a speed for Auto Speed to take acount for when calculating Game1.player.addedSpeed; Will fail if a unique key has already been added.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Amount"></param>
        public void addSpeedBoost(string ID, int Amount)
        {
            AutoSpeed.Instance.combinedAddedSpeed.Add(ID, Amount);
        }
        /// <summary>
        /// Removes an added speed boost by passing in the unique key.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Amount"></param>
        public void remvoveSpeedBoost(string ID, int Amount)
        {
            AutoSpeed.Instance.combinedAddedSpeed.Remove(ID);
        }
    }
}
