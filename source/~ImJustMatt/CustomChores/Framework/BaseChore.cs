using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework
{
    internal abstract class BaseChore : IChore
    {
        public ChoreData ChoreData { get; }

        protected BaseChore(ChoreData choreData)
        {
            ChoreData = choreData;
        }

        public abstract bool CanDoIt(bool today = true);
        public abstract bool DoIt();

        public virtual IDictionary<string, Func<string>> GetTokens()
        {
            var spouse = Game1.player.getSpouse();
            
            return new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                // date and weather
                {
                    "Day", () => SDate.Now().Day.ToString(CultureInfo.InvariantCulture)
                },
                {
                    "DayEvent", GetDayEvent
                },
                {
                    "DaysPlayed", () => Game1.stats.DaysPlayed.ToString(CultureInfo.InvariantCulture)
                },
                {
                    "Season", () => SDate.Now().Season
                },
                {
                    "Year", () => SDate.Now().Year.ToString(CultureInfo.InvariantCulture)
                },
                {
                    "Weather", GetWeather
                },
                
                // player
                {
                    "PlayerGender", () => Game1.player.IsMale ? "Male" : "Female"
                },
                {
                    "PlayerName", () => Game1.player.Name
                },
                {
                    "PreferredPet", () => Game1.player.catPerson ? "Cat" : "Dog"
                },
                {
                    "PetName", Game1.player.getPetName
                },
                {
                    "NickName", () => spouse?.getTermOfSpousalEndearment()
                },
                
                // relationships
                {
                    "SpouseGender", () => spouse?.Gender == 0 ? "Male" : "Female"
                },
                {
                    "Spouse", () => spouse?.getName()
                },
                {
                    "HasChild", () => Game1.player.getChildrenCount() > 0 ? "Yes" : "No"
                },
                {
                    "ChildName", () => Game1.player.getChildrenCount() > 0
                        ? Game1.player.getChildren().Shuffle().First().getName()
                        : ""
                },

                // world
                {
                    "FarmName", () => Game1.player.farmName.Value
                },
            };
        }

        private static string GetDayEvent()
        {
            // marriage
            if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday)
                return "wedding";

            // festival
            IDictionary<string, string> festivalDates =
                CustomChoresMod.Instance.Helper.Content.Load<Dictionary<string, string>>("Day\\Festivals\\FestivalDates",
                    ContentSource.GameContent);

            return festivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out var festivalName) ? festivalName : null;
        }

        private static string GetWeather()
        {
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) ||
                (SaveGame.loaded?.weddingToday ?? Game1.weddingToday))
                return "Sun";
            if (Game1.isSnowing)
                return "Snow";
            if (Game1.isRaining)
                return Game1.isLightning ? "Storm" : "Rain";
            if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
                return "Wind";
            return "Sun";
        }
    }
}