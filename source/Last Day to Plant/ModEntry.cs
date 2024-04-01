/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dmcrider/LastDayToPlant
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LastDayToPlant
{
    public class ModEntry : Mod
    {
        private const int DaysInAMonth = 28;
        private IModHelper MyHelper;
        private ModConfig MyConfig;
        private readonly List<Crop> AllCrops = new();
        internal static IMonitor InternalMonitor;

        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;
            I18n.Init(helper.Translation);
            MyConfig = MyHelper.ReadConfig<ModConfig>();
            InternalMonitor = this.Monitor;

            LoadBaseGameCrops();
            LoadModCrops();

            // Localize the messages
            foreach(var crop in AllCrops)
            {
                var baseName = crop.Name;
                crop.Localize(MyHelper, baseName);
            }

            MyHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            // Check if the farmer has the agriculturist profession
            var farmers = Game1.getAllFarmers();

            foreach(var farmer in farmers)
            {
                if(farmer.IsMainPlayer && farmer.getProfessionForSkill(Farmer.farmingSkill,10) == Farmer.agriculturist)
                {
                    FarmingSkills.IsAgriculturist = true;
                    break;
                }
            }

            // Show any crops that can't be planted after today and still be harvested
            var currentDay = SDate.From(Game1.Date).Day;
            var currentYear = SDate.From(Game1.Date).Year;
            var currentSeason = SDate.From(Game1.Date).Season;
            switch (currentSeason)
            {
                case StardewValley.Season.Spring:
                    ShowCrops(Season.spring, currentDay, currentYear);
                    break;
                case StardewValley.Season.Summer:
                    ShowCrops(Season.summer, currentDay, currentYear);
                    break;
                case StardewValley.Season.Fall:
                    ShowCrops(Season.fall, currentDay, currentYear);
                    break;
                case StardewValley.Season.Winter:
                    ShowCrops(Season.winter, currentDay, currentYear);
                    break;
                default:
                    return;
            }
        }

        private void LoadBaseGameCrops()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LastDayToPlant.BaseGameCrops.Crops.json"))
            using (var reader = new StreamReader(stream))
            {
                var json = JArray.Parse(reader.ReadToEnd());
                var list = json.ToObject<List<Crop>>();
                AllCrops.AddRange(list);
            }
        }

        private void LoadModCrops()
        {
            var modsPath = Path.Combine(Constants.GamePath, "Mods");
            var modsPathExists = Directory.Exists(modsPath);

            if (modsPathExists)
            {
                FindAndLoadCrops(modsPath);
            }
        }

        private void FindAndLoadCrops(string path)
        {
            var files = Directory.GetFiles(path, "crop.json", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                var info = new FileInfo(file);
                if(info.Name == "crop.json")
                {
                    AllCrops.Add(Crop.FromModFile(file));
                }
            }
        }

        private void ShowCrops(Season season, int day, int year)
        {
            // Some crops are only available in Year 2. We don't want to show them in Year 1
            var filtered = AllCrops.Where(x => x.IsLastGrowSeason(season) && year >= x.AvailableYear);
            if(MyConfig.ShowGingerIslandCrops == false)
            {
                filtered = filtered.Where(x => x.GingerIsland == false);
            }

            foreach (var crop in filtered)
            {
                // No Fertilizer
                var daysToMatureBoosted = CalculateActualGrowRate(crop, Fertilizer.None);
                if (daysToMatureBoosted + day == DaysInAMonth)
                {
                    Game1.addHUDMessage(new HUDMessage(crop.Message, HUDMessage.newQuest_type));
                }

                // SpeedGro
                if (MyConfig.ShowSpeedGro)
                {
                    var DaysToMatureBoosted = CalculateActualGrowRate(crop, Fertilizer.SpeedGro);
                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageSpeedGro, HUDMessage.newQuest_type));
                    }
                }

                // Deluxe SpeedGro
                if (MyConfig.ShowDeluxeSpeedGro)
                {
                    var DaysToMatureBoosted = CalculateActualGrowRate(crop, Fertilizer.DeluxeSpeedGro);
                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageDelxueSpeedGro, HUDMessage.newQuest_type));
                    }
                }

                // Hyper SppedGro
                if (MyConfig.ShowHyperSpeedGro)
                {
                    var DaysToMatureBoosted = CalculateActualGrowRate(crop, Fertilizer.HyperSpeedGro);
                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageHyperSpeedGro, HUDMessage.newQuest_type));
                    }
                }
            }
        }

        private int CalculateActualGrowRate(Crop crop, double factor)
        {
            int daysToGrow = crop.DaysToGrowIrrigated > 0 ? crop.DaysToGrowIrrigated : crop.DaysToGrow;
            if (FarmingSkills.IsAgriculturist)
            {
                return (int)(daysToGrow - (daysToGrow * (factor + FarmingSkills.AgriculturistGrowthRate)));
            }

            return (int)(daysToGrow - (daysToGrow * factor));
        }
    }

    public enum Season
    {
        spring,
        summer,
        fall,
        winter
    }
}