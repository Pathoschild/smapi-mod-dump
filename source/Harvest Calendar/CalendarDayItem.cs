/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestCalendar
{
    /// <summary>
    /// a simple class that store the calculated harvest data for a day
    /// </summary>
    internal class CalendarDayItem
    {
        public int dayOfMonth;
        public string iconId;
        public List<(string, string, int)> locationCrops;

        public CalendarDayItem(int dayOfMonth, string id)
        {
            this.dayOfMonth = dayOfMonth;
            this.iconId = id;
            this.locationCrops = new();
        }

        public void AddCrops(string location, string cropId, int count)
        {
            this.locationCrops.Add((location, cropId, count));
        }
    }
}
