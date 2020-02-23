using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace WhatAreYouMissing
{
    public struct FishInfo
    {
        private List<string> Locations;
        private List<string> Seasons;
        private string Weather;
        private string Times;

        public FishInfo(string location, string season, string weather, string times)
        {
            Locations = new List<string>() { location };
            Seasons = new List<string>() { season };
            Weather = weather;
            Times = times;
        }

        public void AddSeason(string season)
        {
            if (!Seasons.Contains(season))
            {
                Seasons.Add(season);
            }
        }

        public void AddLocation(string location)
        {
            if (!Locations.Contains(location))
            {
                Locations.Add(location);
            }
        }

        public List<string> GetLocations()
        {
            return Locations;
        }

        public List<string> GetSeasons()
        {
            return Seasons;
        }

        public string GetWeather()
        {
            return Weather;
        }

        public string GetTimes()
        {
            return Times;
        }

    }
    public class FishDisplayInfo
    {
        int ParentSheetIndex;
        Dictionary<int, string> FishData;
        Dictionary<string, string> LocationData;
        List<FishInfo> FishInfoList;
        private string DisplayInfo;
        public FishDisplayInfo(int parentSheetIndex)
        {
            ParentSheetIndex = parentSheetIndex;
            FishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            LocationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            FishInfoList = new List<FishInfo>();
            DisplayInfo = GetFishItemDisplayInfo();
        }

        public string GetFishDisplayInfo()
        {
            return DisplayInfo;
        }

        public List<FishInfo> GetFishInfoList()
        {
            return FishInfoList;
        }

        private string GetFishItemDisplayInfo()
        {
            Constants constants = new Constants();

            if (constants.LEGNEDARY_FISH_INFO.ContainsKey(ParentSheetIndex))
            {
                return constants.LEGNEDARY_FISH_INFO[ParentSheetIndex];
            }
            else if (constants.NIGHT_MARKET_FISH.Contains(ParentSheetIndex))
            {
                return Utilities.GetTranslation("NIGHT_MARKET");
            }
            else if (constants.SPECIAL_MINE_FISH_INFO.ContainsKey(ParentSheetIndex))
            {
                return constants.SPECIAL_MINE_FISH_INFO[ParentSheetIndex];
            }
            else if (IsFromACrabPot())
            {
                return GetCrabPotDisplayInfo();
            }
            else
            {
                return GetNormalFishDisplayInfo2();
            }
        }

        private string GetNormalFishDisplayInfo2()
        {
            string displayInfo = "";
            GetFishInfo();
            for (int i = 0; i < FishInfoList.Count; ++i)
            {
                FishInfo info = FishInfoList[i];

                displayInfo += Utilities.GetTranslation("LOCATIONS") + ": " + string.Join(", ", info.GetLocations()) + "\n";
                displayInfo += Utilities.GetTranslation("SEASONS") + ": " + string.Join(", ", info.GetSeasons()) + "\n";
                displayInfo += Utilities.GetTranslation("WEATHER") + ": " + info.GetWeather() + "\n";
                displayInfo += Utilities.GetTranslation("TIME") + ": " + info.GetTimes();

                displayInfo += i != FishInfoList.Count - 1 ? "\n\n" : "";
            }

            return displayInfo;
        }

        private void GetFishInfo()
        {
            foreach (KeyValuePair<string, string> data in LocationData)
            {
                Dictionary<string, FishInfo> fishInfo = new Dictionary<string, FishInfo>();
                for (int season = (int)SeasonIndex.Spring; season < (int)SeasonIndex.Winter + 1; ++season)
                {
                    AddToFishInfoIfInSeason(season, fishInfo, data);
                }
                AddToFishInfoList(fishInfo);
            }
        }

        /// <summary>
        /// Adds to the temporary dictionary fishInfo
        /// if the fish is in the season being looked at
        /// </summary>
        /// <param name="season"></param>
        /// <param name="fishInfo"></param>
        /// <param name="data"></param>
        private void AddToFishInfoIfInSeason(int season, Dictionary<string, FishInfo> fishInfo, KeyValuePair<string, string> data)
        {
            string[] seasonalFish = data.Value.Split('/')[season].Split(' ');
            if (seasonalFish.Contains(ParentSheetIndex.ToString()))
            {
                string seasonStr = GetTranslatedSeason((SeasonIndex)season);

                int areaCode = GetAreaCode(seasonalFish);
                string[] locationDisplayNames = GetLocationDisplayNames(data.Key, areaCode);

                AddToFishInfo(locationDisplayNames, fishInfo, seasonStr);
            }
        }

        /// <summary>
        /// Adds to the temperoray dictionary fishInfo as appropriate.
        /// This dictionary will be used to add the appropriate info
        /// to the class variable FishInfoList 
        /// 
        /// In the base game there is only one case where there are
        /// multiple locations in a single line of location data
        /// That is CindersapForest (the pond and the river)
        /// </summary>
        /// <param name="locationDisplayNames"></param>
        /// <param name="fishInfo"></param>
        /// <param name="season"></param>
        private void AddToFishInfo(string[] locationDisplayNames, Dictionary<string, FishInfo> fishInfo, string season)
        {
            string periodToCatch = GetAllPeriodsToCatchDisplayInfo();
            string weather = GetWeatherDisplayInfoForFish();
            foreach (string location in locationDisplayNames)
            {
                if (location != "" && !fishInfo.ContainsKey(location))
                {
                    FishInfo info = new FishInfo(location, season, weather, periodToCatch);
                    //The forest farm pond is not stored in locations data and 
                    //it is the same as the cindersap pond but with the addition
                    //of the woodskip
                    if (ParentSheetIndex == Constants.WOODSKIP)
                    {
                        info.AddLocation(Utilities.GetTranslation("FOREST_FARM_POND_DIAPLAY_NAME"));
                    }
                    fishInfo.Add(location, info);
                }
                else if (location != "")
                {
                    //same location, different season
                    fishInfo[location].AddSeason(season);
                }
            }
        }

        /// <summary>
        /// Adds to the class variable FishInfoList which contains bundles
        /// of distinct information. i.e. all the locations that have the same
        /// season, weather, and time data is one distinct bundle
        /// </summary>
        /// <param name="infoList"></param>
        private void AddToFishInfoList(Dictionary<string, FishInfo> infoList)
        {
            foreach (KeyValuePair<string, FishInfo> info in infoList)
            {
                bool shouldContinue = false;
                foreach (FishInfo resultInfo in FishInfoList)
                {
                    if (IsSameInfoAsOtherLocation(resultInfo, info.Value) && !FishInfoList.Contains(info.Value))
                    {
                        //This location has the same data as another location while not being
                        //the same location, just add the location to list of locations corresponding 
                        //to the other data (seasons, weather, time)
                        resultInfo.AddLocation(info.Value.GetLocations().First());
                        shouldContinue = true;
                    }
                }

                if (shouldContinue)
                {
                    continue;
                }

                //There is no location already in the list that has the same seasons, weather, and time
                //Add it as a new location with its data.
                FishInfoList.Add(info.Value);
            }
        }

        /// <summary>
        /// Checks to see if one bundle of location data contains
        /// the same info regarding seasons, weather, and time
        /// </summary>
        /// <param name="info"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        private bool IsSameInfoAsOtherLocation(FishInfo info, FishInfo newInfo)
        {
            bool sameSeason = true;
            if (info.GetSeasons().Count != newInfo.GetSeasons().Count)
            {
                return false;
            }
            else
            {
                foreach (string season in info.GetSeasons())
                {
                    if (!newInfo.GetSeasons().Contains(season))
                    {
                        return false;
                    }
                }
            }
            bool sameWeather = info.GetWeather() == newInfo.GetWeather();
            bool sameTimes = info.GetTimes() == newInfo.GetTimes();

            return sameSeason && sameWeather && sameTimes;
        }

        private string GetTranslatedSeason(SeasonIndex seasonIndex)
        {
            switch (seasonIndex)
            {
                case SeasonIndex.Spring:
                    return Utilities.GetTranslation("SPRING");
                case SeasonIndex.Summer:
                    return Utilities.GetTranslation("SUMMER");
                case SeasonIndex.Fall:
                    return Utilities.GetTranslation("FALL");
                case SeasonIndex.Winter:
                    return Utilities.GetTranslation("WINTER");
                default:
                    return "Oopsies";
            }
        }

        private string GetCrabPotDisplayInfo()
        {
            if (FishData[ParentSheetIndex].Split('/')[4] == "ocean")
            {
                return Utilities.GetTranslation("OBTAINED_FROM_CRAB_POT_IN_OCEAN");
            }
            else
            {
                return Utilities.GetTranslation("OBTAINED_FROM_CRAB_POT_IN_FRESHWATER");
            }
        }

        private bool IsFromACrabPot()
        {
            return FishData[ParentSheetIndex].Split('/')[1] == "trap";
        }

        private int GetAreaCode(string[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == ParentSheetIndex.ToString())
                {
                    return ParseAreaCode(data[i + 1]);
                }
            }
            //Should never get here
            return Constants.DEFAULT_AREA_CODE;
        }

        private int ParseAreaCode(string code)
        {
            bool successful = int.TryParse(code, out int areaCode);
            if (!successful)
            {
                ModEntry.Logger.LogAreaCodeParseFail(code);
                return Constants.DEFAULT_AREA_CODE;
            }
            return areaCode;
        }

        private string[] GetLocationDisplayNames(string gameName, int areaCode)
        {
            LocationDisplayNames locationDisplayNames = new LocationDisplayNames();
            return locationDisplayNames.GetLocationDisplayNames(gameName, areaCode);
        }

        private string GetWeatherDisplayInfoForFish()
        {
            string weather = FishData[ParentSheetIndex].Split('/')[7];
            switch (weather)
            {
                case "sunny":
                    return Utilities.GetTranslation("SUNNY_WEATHER");
                case "rainy":
                    return Utilities.GetTranslation("RAINY_WEATHER");
                case "both":
                    return Utilities.GetTranslation("ANY_WEATHER");
                default:
                    return "Oopsies";
            }
        }

        private string GetAllPeriodsToCatchDisplayInfo()
        {
            string periodsToCatch = GetPeriodToCatchDisplayInfo(0);
            string[] timesToCatch = FishData[ParentSheetIndex].Split('/')[5].Split(' ');

            for (int i = 2; i < timesToCatch.Length; ++i)
            {
                //its pairs of times for one period
                if (i % 2 == 0)
                {
                    periodsToCatch += ", " + GetPeriodToCatchDisplayInfo(i);
                }
            }
            return periodsToCatch;
        }

        private string GetPeriodToCatchDisplayInfo(int startTimeIndex)
        {
            string[] timesToCatch = FishData[ParentSheetIndex].Split('/')[5].Split(' ');

            string earliestTime = Convert24TimeTo12(timesToCatch[startTimeIndex]);
            string latestTime = Convert24TimeTo12(timesToCatch[startTimeIndex + 1]);

            if (earliestTime == "6:00 am" && latestTime == "2:00 am")
            {
                return Utilities.GetTranslation("ANYTIME");
            }
            else
            {
                return earliestTime + " - " + latestTime;
            }
        }

        private string Convert24TimeTo12(string twentyFourHourTime)
        {
            bool successful = int.TryParse(GetImportantDigits(twentyFourHourTime), out int importantDigits);

            if (!successful)
            {
                ModEntry.Logger.LogTimeParseFail(twentyFourHourTime);
                return "";
            }

            if (importantDigits < 12)
            {
                return importantDigits.ToString() + ":" + "00 am";
            }
            else if (importantDigits == 12)
            {
                return importantDigits.ToString() + ":" + "00 pm";
            }
            else if (importantDigits < 24)
            {
                return (importantDigits - 12).ToString() + ":" + "00 pm";
            }
            else if (importantDigits == 24)
            {
                return (importantDigits - 12).ToString() + ":" + "00 am";
            }
            else
            {
                return (importantDigits % 12).ToString() + ":" + "00 am";
            }
        }

        private string GetImportantDigits(string twentyFourHourTime)
        {
            return twentyFourHourTime.Substring(0, twentyFourHourTime.Length - 2);
        }
    }
}
