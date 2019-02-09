using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class SpecificTrashData : ITrashData {
        public IEnumerable<int> PossibleIds { get; set; }
        public string Location { get; }
        public WaterType WaterType { get; }
        public Season Season { get; }
        public Weather Weather { get; }
        public int FishingLevel { get; }
        public int? MineLevel { get; }
        public bool InvertLocations { get; }
        public double Weight { get; }

        public SpecificTrashData(IEnumerable<int> ids, double weight, string location, WaterType waterType = WaterType.Both, Season season = Season.Spring | Season.Summer | Season.Fall | Season.Winter, Weather weather = Weather.Sunny | Weather.Rainy, int fishingLevel = 0, int? mineLevel = null, bool invertLocations = false) {
            this.PossibleIds = ids.ToArray();
            this.Weight = weight;
            this.Location = location;
            this.WaterType = waterType;
            this.Season = season;
            this.Weather = weather;
            this.FishingLevel = fishingLevel;
            this.MineLevel = mineLevel;
            this.InvertLocations = invertLocations;
        }

        public bool MeetsCriteria(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return (this.InvertLocations ^ (this.Location == null || locationName == this.Location))
                   && (this.WaterType & waterType) != 0
                   && (this.Season & date.GetSeason()) != 0
                   && (this.Weather & weather) != 0
                   && this.FishingLevel <= fishingLevel
                   && (this.MineLevel == null || this.MineLevel == mineLevel);
        }

        public double GetWeight() {
            return this.Weight;
        }
    }
}