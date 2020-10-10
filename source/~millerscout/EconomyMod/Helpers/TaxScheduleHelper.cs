/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Model;

namespace EconomyMod.Helpers
{
    public static class TaxScheduleHelper
    {
        public static IEnumerable<TaxSchedule> GetAllFromThisSeason(this IEnumerable<TaxSchedule> collection, int dayCount)
        {
            var wd = dayCount.ToWorldDate();
            wd.SetToFirstDayOfSeason();

            return collection.Where(c => c.DayCount >= wd.DaysCount && c.DayCount <= wd.DaysCount + 27);
        }
        public static IEnumerable<TaxSchedule> GetAllFromThisSeason(this IEnumerable<TaxSchedule> collection, uint dayCount) => GetAllFromThisSeason(collection, Convert.ToInt32(dayCount));
    }
}
