using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denifia.Stardew.SendItems.Domain
{
    public class GameDateTime : IComparable<GameDateTime>
    {

        public int TimeOfDay { get; set; }
        public int DayOfMonth { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        private int FirstTimeOfDay = 600;
        private int LastTimeOfDay = 2600;

        public GameDateTime()
        {

        }

        public GameDateTime(int timeOfDay, int dayOfMonth, string currentSeason, int year)
        {
            TimeOfDay = timeOfDay;
            DayOfMonth = dayOfMonth;
            Season = SeasonAsInt(currentSeason);
            Year = year;
        }

        private GameDateTime(int timeOfDay, int dayOfMonth, int currentSeason, int year)
        {
            TimeOfDay = timeOfDay;
            DayOfMonth = dayOfMonth;
            Season = currentSeason;
            Year = year;
        }

        public GameDateTime GetNightBefore()
        {
            return new GameDateTime(LastTimeOfDay, DayOfMonth - 1, Season, Year);
        }

        private int SeasonAsInt(string season)
        {
            var seasonInt = 0;
            switch (season)
            {
                case "spring":
                    seasonInt = 1;
                    break;
                case "summer":
                    seasonInt = 2;
                    break;
                case "fall":
                    seasonInt = 3;
                    break;
                case "winter":
                    seasonInt = 4;
                    break;
                default:
                    break;
            }
            return seasonInt;
        }

        public int CompareTo(GameDateTime other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            if (Year != other.Year) return Year.CompareTo(other.Year);
            if (Season != other.Season) return Season.CompareTo(other.Season);
            if (DayOfMonth != other.DayOfMonth) return DayOfMonth.CompareTo(other.DayOfMonth);
            if (TimeOfDay != other.TimeOfDay) return TimeOfDay.CompareTo(other.TimeOfDay);

            return 0;
        }

        public static int Compare(GameDateTime left, GameDateTime right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }
            if (ReferenceEquals(left, null))
            {
                return -1;
            }
            return left.CompareTo(right);
        }

        public override bool Equals(object obj)
        {
            GameDateTime other = obj as GameDateTime;
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            var stringRepresentation = $"{TimeOfDay}{DayOfMonth}{Season}{Year}";
            return stringRepresentation.GetHashCode();
        }

        public static bool operator ==(GameDateTime left, GameDateTime right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(GameDateTime left, GameDateTime right)
        {
            return !(left == right);
        }
        public static bool operator <(GameDateTime left, GameDateTime right)
        {
            return (Compare(left, right) < 0);
        }
        public static bool operator <=(GameDateTime left, GameDateTime right)
        {
            return (Compare(left, right) <= 0);
        }
        public static bool operator >(GameDateTime left, GameDateTime right)
        {
            return (Compare(left, right) > 0);
        }
        public static bool operator >=(GameDateTime left, GameDateTime right)
        {
            return (Compare(left, right) >= 0);
        }
    }
}
