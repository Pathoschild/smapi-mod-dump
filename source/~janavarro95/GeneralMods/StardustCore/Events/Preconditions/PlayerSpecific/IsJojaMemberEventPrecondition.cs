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

namespace Omegasis.StardustCore.Events.Preconditions.PlayerSpecific
{
    public class IsJojaMemberEventPrecondition:EventPrecondition
    {

        public const string EventPreconditionId = "StardustCore.Events.Preconditions.PlayerSpecific.JojaMemeber";

        public bool isJojaMember;

        public IsJojaMemberEventPrecondition()
        {

        }

        public IsJojaMemberEventPrecondition(bool IsMember)
        {
            this.isJojaMember = IsMember;
        }

        public override bool meetsCondition()
        {
            if (this.isJojaMember)
            {
                return Game1.player.mailReceived.Contains("JojaMember") == true;
            }
            else
            {
                return Game1.player.mailReceived.Contains("JojaMember") == false;
            }
        }

        public override string ToString()
        {
            return EventPreconditionId +" " + this.isJojaMember.ToString(); 
        }
    }
}
