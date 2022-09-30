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
using Omegasis.StardustCore.Events.Preconditions;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events.EventPreconditions
{
    public class IsMarriedToPrecondition:EventPrecondition
    {
        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.EventPreconditions.IsMarriedToPrecondition";

        public string spouseName;

        public IsMarriedToPrecondition()
        {

        }

        public IsMarriedToPrecondition(string SpouseName)
        {
            this.spouseName = SpouseName;
        }

        public override string ToString()
        {
            return EventPreconditionId+" "+this.spouseName;
        }

        public override bool meetsCondition()
        {
            if (Game1.player.getSpouse() == null) return false;
            else
            {
                return Game1.player.getSpouse().Name.Equals(this.spouseName);
            }
        }
    }
}
