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

namespace Omegasis.StardustCore.Events.Preconditions.TimeSpecific
{
    /// <summary>
    /// Says which days events can't occur. If a value is true, then the event can not occur on that day.
    /// </summary>
    public class DayOfWeekPrecondition:EventPrecondition
    {
        public const string EventPreconditionId = "StardustCore.Events.Preconditions.TimeSpecific.DayOfWeekPrecondition";

        public bool canHappenOnSunday;
        public bool canHappenOnMonday;
        public bool canHappenOnTuesday;
        public bool canHappenOnWednesday;
        public bool canHappenOnThursday;
        public bool canHappenOnFriday;
        public bool canHappenOnSaturday;

        public DayOfWeekPrecondition()
        {

        }

        public DayOfWeekPrecondition(bool CanHappenOnSunday, bool CanHappenOnMonday, bool CanHappenOnTuesday, bool CanHappenOnWednesday, bool CanHappenOnThursday, bool CanHappenOnFriday, bool CanHappenOnSaturday)
        {
            this.canHappenOnSunday = CanHappenOnSunday;
            this.canHappenOnMonday = CanHappenOnMonday;
            this.canHappenOnTuesday = CanHappenOnTuesday;
            this.canHappenOnWednesday = CanHappenOnWednesday;
            this.canHappenOnThursday = CanHappenOnThursday;
            this.canHappenOnFriday = CanHappenOnFriday;
            this.canHappenOnSaturday = CanHappenOnSaturday;
        }

        public override bool meetsCondition()
        {
            int day = Game1.dayOfMonth;
            if (day % 7 == 0 && this.canHappenOnSunday)
            {
                return true;
            }
            if (day % 7 == 1 && this.canHappenOnMonday)
            {
                return true;
            }
            if (day % 7 == 2 && this.canHappenOnTuesday)
            {
                return true;
            }
            if (day % 7 == 3 && this.canHappenOnWednesday)
            {
                return true;
            }
            if (day % 7 == 4 && this.canHappenOnThursday)
            {
                return true;
            }
            if (day % 7 == 5 && this.canHappenOnFriday)
            {
                return true;
            }
            if (day % 7 == 6 && this.canHappenOnSaturday)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(EventPreconditionId);
            b.Append(" ");

            if (this.canHappenOnSunday)
            {
                b.Append("Sunday ");
            }
            if (this.canHappenOnMonday)
            {
                b.Append("Monday ");
            }
            if (this.canHappenOnTuesday)
            {
                b.Append("Tuesday ");
            }
            if (this.canHappenOnWednesday)
            {
                b.Append("Wednesday ");
            }
            if (this.canHappenOnThursday)
            {
                b.Append("Thursday ");
            }
            if (this.canHappenOnFriday)
            {
                b.Append("Friday ");
            }
            if (this.canHappenOnSaturday)
            {
                b.Append("Saturday ");
            }


            return b.ToString();
        }
    }
}
