/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class DayGlobalToken : GlobalToken
    {
        public DayGlobalToken() : base("Day") { }

        protected override bool Update()
        {
            return this.SetValue(SDate.Now().Day.ToString());
        }
    }

    public class DayEventGlobalToken : GlobalToken
    {
        public DayEventGlobalToken() : base("DayEvent") { }

        protected override bool Update()
        {
            if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday)
                return this.SetValue("wedding");

            IDictionary<string, string> festivalDates = HDSpritesMod.ModHelper.Content.Load<Dictionary<string, string>>("Data\\Festivals\\FestivalDates", StardewModdingAPI.ContentSource.GameContent);
            if (festivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out string festivalName))
                return this.SetValue(festivalName);

            return false;
        }
    }

    public class DayOfWeekGlobalToken : GlobalToken
    {
        public DayOfWeekGlobalToken() : base("DayOfWeek") { }

        protected override bool Update()
        {
            return this.SetValue(SDate.Now().DayOfWeek.ToString());
        }
    }

    public class DaysPlayedGlobalToken : GlobalToken
    {
        public DaysPlayedGlobalToken() : base("DaysPlayed") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.stats.DaysPlayed.ToString());
        }
    }

    public class SeasonGlobalToken : GlobalToken
    {
        public SeasonGlobalToken() : base("Season") { }

        protected override bool Update()
        {
            return this.SetValue(SDate.Now().Season);
        }
    }

    public class YearGlobalToken : GlobalToken
    {
        public YearGlobalToken() : base("Year") { }

        protected override bool Update()
        {
            return this.SetValue(SDate.Now().Year.ToString());
        }
    }

    public class WeatherGlobalToken : GlobalToken
    {
        public WeatherGlobalToken() : base("Weather") { }

        protected override bool Update()
        {
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || (SaveGame.loaded?.weddingToday ?? Game1.weddingToday))
                return this.SetValue("Sun");
            if (Game1.isSnowing)
                return this.SetValue("Snow");
            if (Game1.isRaining)
                return this.SetValue(Game1.isLightning ? "Storm" : "Rain");
            if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
                return this.SetValue("Wind");
            return this.SetValue("Sun");
        }
    }
}
