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

namespace StardustCore.Events.Preconditions.MISC
{
    public class JojaWarehouseCompleted:EventPrecondition
    {
        public JojaWarehouseCompleted()
        {

        }

        public override string ToString()
        {
            return this.precondition_JojaWarehouseCompleted();
        }

        /// <summary>
        /// Adds in the precondition that the joja warehouse has been completed.
        /// </summary>
        /// <returns></returns>
        public string precondition_JojaWarehouseCompleted()
        {
            StringBuilder b = new StringBuilder();
            b.Append("J");
            return b.ToString();
        }

        /// <summary>
        /// TODO: Check if this is valid.
        /// </summary>
        /// <returns></returns>
        public override bool meetsCondition()
        {
            return (Game1.MasterPlayer.hasCompletedCommunityCenter() && Game1.MasterPlayer.mailReceived.Contains("JojaMember"));
        }
    }
}
