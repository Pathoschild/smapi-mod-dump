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
    /// Precondition that ensures that this event can't happen on a festival day.
    /// </summary>
    /// <returns></returns>
    public class NotAFestivalDay:EventPrecondition
    {
        public NotAFestivalDay()
        {

        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("F");
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return string.IsNullOrEmpty(Game1.whereIsTodaysFest) == true;
        }
    }
}
