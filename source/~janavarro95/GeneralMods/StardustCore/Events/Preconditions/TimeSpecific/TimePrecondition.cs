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

namespace StardustCore.Events.Preconditions.TimeSpecific
{
    /// <summary>
    /// The event will only happen between the given times.
    /// </summary>
    /// <param name="StartTime"></param>
    /// <param name="EndTime"></param>
    /// <returns></returns>
    public class TimePrecondition:EventPrecondition
    {
        public int start;
        public int end;

        public TimePrecondition()
        {

        }

        public TimePrecondition(int Start, int End)
        {
            this.start = Start;
            this.end = End;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("t ");
            b.Append(this.start.ToString());
            b.Append(" ");
            b.Append(this.end.ToString());
            return b.ToString();

        }

        public override bool meetsCondition()
        {
            if (Game1.timeOfDay >= this.start && Game1.timeOfDay <= this.end) return true;
            else return false;
        }

    }
}
