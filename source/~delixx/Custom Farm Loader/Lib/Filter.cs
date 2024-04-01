/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using ContentPatcher;

namespace Custom_Farm_Loader.Lib
{
    //Note: Filter is described as "Universal Conditions" in the documentation. Might rename it at some point™

    public class Filter
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public List<string> Seasons = new List<string>();
        public List<Weather> Weather = new List<Weather>();
        public int AfterDay = 0;
        public int BeforeDay = 0;
        public int StartTime = 600; //06:00
        public int EndTime = 2600; //02:00

        public int FishingLevel = 0;
        public int ForagingLevel = 0;
        public int MiningLevel = 0;
        public int CombatLevel = 0;
        public int FarmingLevel = 0;

        public Dictionary<string, string> CPConditions = new Dictionary<string, string>(); //Content Patcher conditions
        public IManifest Manifest;

        //Whether each filter was changed during parse
        //Makes it easier/cleaner and more flexible to seperate default value but assigned by cfl_map.json from default value but unassigned
        public bool ChangedSeasons = false;
        public bool ChangedWeather = false;
        public bool ChangedAfterDay = false;
        public bool ChangedBeforeDay = false;
        public bool ChangedStartTime = false;
        public bool ChangedEndTime = false;

        public bool ChangedFishingLevel = false;
        public bool ChangedForagingLevel = false;
        public bool ChangedMiningLevel = false;
        public bool ChangedCombatLevel = false;
        public bool ChangedFarmingLevel = false;

        public bool ChangedCPConditions = false;
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public bool parseAttribute(JProperty property)
        {
            string name = property.Name;
            string value = property.Value.ToString();

            switch (name.ToLower().Replace(" ","")) {
                case "time":
                    value = value.Replace("-", ";").Replace(":", "");
                    var times = value.Split(';');
                    if (times.Length >= 1) {
                        StartTime = int.Parse(times[0]);
                        ChangedStartTime = true;
                    }

                    if (times.Length >= 2) {
                        EndTime = int.Parse(times[1]);
                        ChangedEndTime = true;
                    }
                    break;

                case "beforeday":
                    BeforeDay = int.Parse(value);
                    ChangedBeforeDay = true; break;

                case "afterday":
                    AfterDay = int.Parse(value);
                    ChangedAfterDay = true; break;

                case "seasons":
                    Seasons = UtilityMisc.parseStringArray(property);
                    ChangedSeasons = true; break;

                case "weather":
                    if (!parseNativeWeather(value))
                        Weather = UtilityMisc.parseEnumArray<Weather>(property);
                    ChangedWeather = true; break;

                case "fishinglevel":
                    ChangedFishingLevel = true;
                    FishingLevel = int.Parse(value); break;
                case "foraginglevel":
                    ChangedForagingLevel = true;
                    ForagingLevel = int.Parse(value); break;
                case "mininglevel":
                    ChangedMiningLevel = true;
                    MiningLevel = int.Parse(value); break;
                case "combatlevel":
                    ChangedCombatLevel = true;
                    CombatLevel = int.Parse(value); break;
                case "farminglevel":
                    ChangedFarmingLevel = true;
                    FarmingLevel = int.Parse(value); break;

                case "when":
                    ChangedCPConditions = true;
                    CPConditions = parseCPConditions(property); break;

                default:
                    return false;
            }
            return true;
        }

        public bool isValid(bool excludeSeason = false, bool excludeWeather = false, bool excludeTime = false, Farmer who = null)
        {
            var debug = false;

            if(debug) {
                var validSeason = (isValidSeason() || excludeSeason);
                var validWeather = (isValidWeather() || excludeWeather);
                var validDay = isValidDay();
                var validSkill = isValidSkill(who);
                var validCPConditions = doCPConditionsMatch();
            }

            return (isValidSeason() || excludeSeason)
                && (isValidWeather() || excludeWeather)
                && (isValidTime() || excludeTime)
                && isValidDay()
                && isValidSkill(who)
                && doCPConditionsMatch();
        }

        private bool doCPConditionsMatch()
        {
            if (!CPConditions.Any())
                return true;

            var cpDependency = Array.Find(Manifest.Dependencies, el => el.UniqueID == "Pathoschild.ContentPatcher");

            if (cpDependency == null || cpDependency.IsRequired == false) {
                Monitor.LogOnce($"Content Patcher conditions were used by '{Manifest.Name}', but Content Patcher wasn't added as a required dependency by the mod", LogLevel.Error);
                return false;
            }

            if (cpDependency.MinimumVersion == null) {
                Monitor.LogOnce($"Content Patcher conditions were used by '{Manifest.Name}', but no MinimumVersion was mentioned in the manifest dependency", LogLevel.Error);
                return false;
            }

            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            var parsed = api.ParseConditions(Manifest, CPConditions, cpDependency.MinimumVersion);

            if(!parsed.IsValid) {
                Monitor.LogOnce($"The Content Patcher conditions used by '{Manifest.Name}' ran into a parsing error:\n{parsed.ValidationError}", LogLevel.Error);
                return false;
            }

            return parsed.IsMatch;
        }

        private bool isValidSeason()
        {
            return Seasons.Exists(el => el.ToLower() == Game1.currentSeason) || Seasons.Count == 0;
        }

        private bool isValidWeather()
        {
            if (Weather.Count == 0)
                return true;

            Farm farm = Game1.getFarm();

            if (Game1.IsLightningHere(farm) && Weather.Contains(Enums.Weather.Lightning))
                return true;
            if (Game1.IsRainingHere(farm) && Weather.Contains(Enums.Weather.Rain))
                return true;
            if ((Game1.IsSnowingHere(farm) || (Game1.IsDebrisWeatherHere(farm) && Game1.currentSeason == "winter")) && Weather.Contains(Enums.Weather.Snow))
                return true;
            if (Game1.IsDebrisWeatherHere(farm) && Weather.Contains(Enums.Weather.Wind) && Game1.currentSeason != "winter")
                return true;
            if (Game1.weddingToday && Weather.Contains(Enums.Weather.Wedding))
                return true;
            if (Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season) && Weather.Contains(Enums.Weather.Festival))
                return true;
            if (!Game1.IsRainingHere(farm)
                && !Game1.IsSnowingHere(farm) && !(Game1.IsDebrisWeatherHere(farm) && Game1.currentSeason == "winter")
                && Weather.Contains(Enums.Weather.Sun))
                return true;

            return false;
        }

        private bool isValidDay()
        {
            return (!ChangedAfterDay || AfterDay < Game1.Date.TotalDays + 1)
                && (!ChangedBeforeDay || BeforeDay > Game1.Date.TotalDays + 1);
        }

        private bool isValidTime()
        {
            return Game1.timeOfDay >= StartTime && Game1.timeOfDay <= EndTime;
        }

        private bool isValidSkill(Farmer who)
        {
            if (who == null)
                return true;

            return FishingLevel <= who.FishingLevel
                && ForagingLevel <= who.ForagingLevel
                && MiningLevel <= who.MiningLevel
                && CombatLevel <= who.CombatLevel
                && FarmingLevel <= who.FarmingLevel;
        }

        public bool parseNativeWeather(string weather)
        {
            switch (weather.ToLower()) {
                case "sunny":
                    Weather = new Weather[] { Enums.Weather.Sun, Enums.Weather.Wedding, Enums.Weather.Festival, Enums.Weather.Wind }.ToList();
                    return true;
                case "rainy":
                    Weather = new Weather[] { Enums.Weather.Rain, Enums.Weather.Lightning, Enums.Weather.Snow }.ToList();
                    return true;
                case "both":
                    Weather = new List<Weather>();
                    return true;
                default:
                    return false;
            }
        }

        public Dictionary<string, string> parseCPConditions(JProperty jProperty)
        {
            var ret = new Dictionary<string, string>();
            if (jProperty.Value.Type != JTokenType.Object)
                throw new ArgumentException($"Provided CP conditions of unexpected type '{jProperty.Value.Type}'. Required Type of Dictionary<string, string> (JSON Object), but was provided: '{jProperty.Value}'", jProperty.Name);

            foreach (JProperty prop in (jProperty.First() as JObject).Properties()) {
                if (prop.Value.Type != JTokenType.String && prop.Value.Type != JTokenType.Integer)
                    throw new ArgumentException($"Provided CP condition value of unexpected type '{prop.Value.Type}'. Typeof 'String' required: '{prop.Value}'", prop.Name);

                ret.Add(prop.Name, prop.Value.ToString());
            }    

            return ret;
        }
    }
}
