/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using System;
using StardewNewsFeed.Enums;

namespace StardewNewsFeed.Wrappers {
    public class GameDate {

        public Season Season;
        public int Day;

        public GameDate(string season, int day) {
            Season = (Season)Enum.Parse(typeof(Season), season);
            Day = day;
        }

        public override bool Equals(object obj) {
            var compareToDate = obj as GameDate;
            return compareToDate.Season == Season
                && compareToDate.Day == Day;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

    }
}
