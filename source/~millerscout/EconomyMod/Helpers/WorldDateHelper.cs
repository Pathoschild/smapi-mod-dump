using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Model;
using StardewValley;

namespace EconomyMod.Helpers
{
    public static class Helper
    {
        public static CustomWorldDate ToWorldDate(this int day)
        {
            return new CustomWorldDate(day);
        }
        public static CustomWorldDate ToWorldDate(this uint day)
        {
            return new CustomWorldDate(Convert.ToInt32(day));
        }
        public static string GetLocalizedSeason(this Season season)
        {
            return Utility.getSeasonNameFromNumber((int)season - 1);
        }

        public static Dictionary<int, bool> GenerateCalendarTaxBool(this CustomWorldDate worldDate, IEnumerable<TaxSchedule> taxSchedules)
        {

            worldDate.SetToFirstDayOfSeason();
            var filtered = taxSchedules.Where(c => c.DayCount >= worldDate.DaysCount && c.DayCount <= worldDate.DaysCount + 28);

            return Enumerable.Range(0, 28).ToDictionary(c => worldDate.DaysCount + c, c => filtered.Any(f => f.DayCount == worldDate.DaysCount + c));

        }
    }
}
