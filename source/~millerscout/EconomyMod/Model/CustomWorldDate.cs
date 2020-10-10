using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyMod.Model
{
    public class CustomWorldDate
    {
        public void AddDays(int days)
        {
            DaysCount += days;
        }
        public CustomWorldDate(int Day)
        {
            DaysCount = Day;
        }

        private Season CalculateMonth()
        {
            var wholeYearMonths = Math.Floor(DaysCount / 28f / 4);
            var ignoredDays = 112 * wholeYearMonths / DaysCount == 1 ? 112 * (wholeYearMonths - 1) : 112 * wholeYearMonths;
            return (Season)Convert.ToInt32(Math.Ceiling((DaysCount - ignoredDays) / 28f));

        }

        public CustomWorldDate Next(DayOfWeek nextWeekDay)
        {
            var dayOfWeek = (int)CalculateDay(DaysCount);

            if ((int)nextWeekDay > dayOfWeek)
            {
                DaysCount += (int)nextWeekDay - dayOfWeek;
            }
            else
            {
                //DaysCount++;
                var ad = (7 - (int)dayOfWeek);
                DaysCount += ad;
                Next(nextWeekDay);
            }

            return this;
        }

        internal void SetToFirstDayOfSeason()
        {
            this.AddDays(this.DaysLeftToEndOfMonth);
            this.AddDays(-27);
        }

        public DayOfWeek Day => CalculateDay(DaysCount);

        private DayOfWeek CalculateDay(int days)
        {
            return (DayOfWeek)(days - 7 * Math.Floor((days / 7f)));
        }

        public Season Season => CalculateMonth();
        public int Year => ((DaysCount - 1) / 28 / 4) + 1;
        public int DayOfMonth => CalculateDayOfMonth();

        public int DaysLeftToEndOfMonth => 28 - DayOfMonth;

        private int CalculateDayOfMonth()
        {
            return calculate(DaysCount);

            int calculate(int day)
            {
                var wholeYearMonths = day / 28f / 4;
                var ignoredDays = wholeYearMonths > 1 ? Math.Floor(wholeYearMonths) * 28 * 4 : 0;
                var result = Convert.ToInt32(day - ignoredDays);
                if (result > 28)
                {
                    return calculate(result - 28);
                }
                return result;
            }
            //var wholeYearMonths = Math.Floor(DaysCount / 28f / 4);
            //var wholeMonth = 28f;
            //var ignoredDays = 112 * wholeYearMonths / DaysCount == 1 ? 112 * (wholeYearMonths - 1) : 112 * wholeYearMonths;
            //ignoredDays += ((DaysCount - ignoredDays) / 28 > 1 ? wholeMonth : 0);
            //return Convert.ToInt32(DaysCount - ignoredDays);
        }

        public int DaysCount { get; private set; }

    }
}
