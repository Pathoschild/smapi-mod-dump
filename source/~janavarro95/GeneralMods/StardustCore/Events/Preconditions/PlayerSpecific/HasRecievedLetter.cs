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
    public class HasRecievedLetter:EventPrecondition
    {
        public string id;


        public HasRecievedLetter()
        {

        }

        public HasRecievedLetter(string ID)
        {
            this.id = ID;
        }

        public override string ToString()
        {
            return this.precondition_playerHasRecievedLetter();
        }

        /// <summary>
        /// The player has seen the letter with the given id.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string precondition_playerHasRecievedLetter()
        {
            StringBuilder b = new StringBuilder();
            b.Append("n ");
            b.Append(this.id.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.hasOrWillReceiveMail(this.id);
        }

    }
}
