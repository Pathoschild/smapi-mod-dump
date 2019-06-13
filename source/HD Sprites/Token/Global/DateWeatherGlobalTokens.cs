using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class DayGlobalToken : GlobalToken
    {
        public DayGlobalToken() : base("Day") { }

        public override void Update()
        {
            this.GlobalValue = SDate.Now().Day.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class DayEventGlobalToken : GlobalToken
    {
        public DayEventGlobalToken() : base("DayEvent") { }

        public override void Update()
        {
            this.GlobalValue = "";
            if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday)
            {
                this.GlobalValue = "wedding";
            }
            else
            {
                IDictionary<string, string> festivalDates = HDSpritesMod.ModHelper.Content.Load<Dictionary<string, string>>("Data\\Festivals\\FestivalDates", StardewModdingAPI.ContentSource.GameContent);
                if (festivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out string festivalName))
                {
                    this.GlobalValue = festivalName;
                }
            }

            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class DayOfWeekGlobalToken : GlobalToken
    {
        public DayOfWeekGlobalToken() : base("DayOfWeek") { }

        public override void Update()
        {
            this.GlobalValue = SDate.Now().DayOfWeek.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class DaysPlayedGlobalToken : GlobalToken
    {
        public DaysPlayedGlobalToken() : base("DaysPlayed") { }

        public override void Update()
        {
            this.GlobalValue = Game1.stats.DaysPlayed.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class SeasonGlobalToken : GlobalToken
    {
        public SeasonGlobalToken() : base("Season") { }

        public override void Update()
        {
            this.GlobalValue = SDate.Now().Season.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class YearGlobalToken : GlobalToken
    {
        public YearGlobalToken() : base("Year") { }

        public override void Update()
        {
            this.GlobalValue = SDate.Now().Year.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class WeatherGlobalToken : GlobalToken
    {
        public WeatherGlobalToken() : base("Weather") { }

        public override void Update()
        {
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || (SaveGame.loaded?.weddingToday ?? Game1.weddingToday))
            {
                this.GlobalValue = "Sun";
            }
            else if (Game1.isSnowing)
            {
                this.GlobalValue = "Snow";
            }
            else if (Game1.isRaining)
            {
                if (Game1.isLightning)
                {
                    this.GlobalValue = "Storm";
                }
                else
                {
                    this.GlobalValue = "Rain";
                }
            }
            else if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
            {
                this.GlobalValue = "Wind";
            }
            else
            {
                this.GlobalValue = "Sun";
            }

            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }
}
