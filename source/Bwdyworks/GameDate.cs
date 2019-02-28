namespace bwdyworks
{
    public class GameDate
    {
        public GameSeason Season;
        public int Day;

        public GameDate(GameSeason season, int day)
        {
            Season = season;
            Day = day;
        }
        //TODO - YEAR WRAPPING
        public string GetSeasonString()
        {
            switch (Season)
            {
                case GameSeason.SUMMER: return "Summer";
                case GameSeason.FALL: return "Fall";
                case GameSeason.WINTER: return "Winter";
                default: return "Spring";
            }
        }

        public bool IsToday()
        {
            return StardewValley.Game1.currentSeason == GetSeasonString().ToLower() && StardewValley.Game1.dayOfMonth == Day;
        }

        public GameDate MinusDays(int DaysToSubtract)
        {
            return PlusDays(-DaysToSubtract);
        }

        public GameDate PlusDays(int DaysToAdd)
        {
            int newDay = Day + DaysToAdd;
            GameSeason gameSeason = Season;
            while(newDay > 28)
            {
                gameSeason = GetNextSeason(gameSeason);
                newDay -= 28;
            }
            while(newDay < 1)
            {
                newDay += 28;
                gameSeason = GetLastSeason(gameSeason);
            }
            return new GameDate(gameSeason, newDay);
        }

        public GameDate MinusSeasons(int SeasonsToSubtract)
        {
            return PlusSeasons(-SeasonsToSubtract);
        }

        public GameDate PlusSeasons(int SeasonsToAdd)
        {
            GameSeason s = Season;
            while(SeasonsToAdd > 0)
            {
                s = GetNextSeason(s);
                SeasonsToAdd--;
            }
            while (SeasonsToAdd < 0)
            {
                s = GetLastSeason(s);
                SeasonsToAdd++;
            }
            return new GameDate(s, Day);
        }

        public static GameDate Current()
        {
            return new GameDate(GetGameSeason(StardewValley.Game1.currentSeason), StardewValley.Game1.dayOfMonth);
        }

        public static GameDate CreateWeddingDate()
        {
            GameDate gd = Current();
            gd.Day += 3;
            if(gd.Day > 28)
            {
                gd.Season = GetNextSeason(gd.Season);
                gd.Day -= 28;
            }
            return gd;
        }

        public static GameSeason GetNextSeason(GameSeason season)
        {
            if (season == GameSeason.SUMMER) return GameSeason.FALL;
            if (season == GameSeason.FALL) return GameSeason.WINTER;
            if (season == GameSeason.WINTER) return GameSeason.SPRING;
            return GameSeason.SUMMER;
        }

        public static GameSeason GetLastSeason(GameSeason season)
        {
            if (season == GameSeason.SUMMER) return GameSeason.SPRING;
            if (season == GameSeason.FALL) return GameSeason.SUMMER;
            if (season == GameSeason.WINTER) return GameSeason.FALL;
            return GameSeason.WINTER;
        }

        public static GameSeason GetGameSeason(string season)
        {
            if (season.ToLower().Contains("summer")) return GameSeason.SUMMER;
            if (season.ToLower().Contains("fall")) return GameSeason.FALL;
            if (season.ToLower().Contains("winter")) return GameSeason.WINTER;
            return GameSeason.SPRING;
        }

        public enum GameSeason
        {
            SPRING = 0,
            SUMMER = 1,
            FALL = 2,
            WINTER = 3
        }
    }
}
