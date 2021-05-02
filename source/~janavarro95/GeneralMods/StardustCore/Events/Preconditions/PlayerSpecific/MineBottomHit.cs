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
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class MineBottomHit:EventPrecondition
    {
        public int amount;

        public MineBottomHit()
        {

        }

        public MineBottomHit(int Amount)
        {
            this.amount = Amount;
        }

        public override string ToString()
        {
            return this.precondition_playerHasReachedMineBottomXTimes();
        }


        /// <summary>
        /// Creates the precondition that the player has reached the bottom of the mines this many times.
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public string precondition_playerHasReachedMineBottomXTimes()
        {
            StringBuilder b = new StringBuilder();
            b.Append("b ");
            b.Append(this.amount.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.timesReachedMineBottom >= this.amount;
        }
    }
}
