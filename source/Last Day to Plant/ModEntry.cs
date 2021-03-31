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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LastDayToPlant
{
    public class ModEntry : Mod
    {
        private List<Crop> SpringCrops;
        private List<Crop> SummerCrops;
        private List<Crop> FallCrops;
        private List<Crop> WinterCrops;

        private const int DaysInAMonth = 28;
        private IModHelper MyHelper;
        private ModConfig MyConfig;

        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;
            MyConfig = MyHelper.ReadConfig<ModConfig>();

            if (MyConfig.IncludeBaseGameCrops)
            {
                SpringCrops = SetSpringCrops();
                SummerCrops = SetSummerCrops();
                FallCrops = SetFallCrops();
                WinterCrops = new List<Crop>();
            }
            else
            {
                SpringCrops = new List<Crop>();
                SummerCrops = new List<Crop>();
                FallCrops = new List<Crop>();
                WinterCrops = new List<Crop>();
            }

            List<ModCompat> modsToShow = new List<ModCompat>();

            // Load Mods
            if(MyConfig.PPJAFruitsAndVeggiesPath != "")
            {
                this.Monitor.Log("Loading [PPJA] Fruits and Veggies", LogLevel.Info);
                ModCompat ppjaFruitsAndVeggies = new ModCompat("[PPJA] Fruits and Veggies", MyConfig.PPJAFruitsAndVeggiesPath + @"\[JA] Fruits and Veggies\Crops\");
                modsToShow.Add(ppjaFruitsAndVeggies);
            }

            if (MyConfig.PPJAFantasyCropsPath != "")
            {
                this.Monitor.Log("Loading [PPJA] Fantasy Crops", LogLevel.Info);
                ModCompat ppjaFruitsAndVeggies = new ModCompat("[PPJA] Fantasy Crops", MyConfig.PPJAFantasyCropsPath + @"\[JA] Fantasy Crops\Crops\");
                modsToShow.Add(ppjaFruitsAndVeggies);
            }

            if (MyConfig.PPJAAncientCropsPath != "")
            {
                this.Monitor.Log("Loading [PPJA] Ancient Crops", LogLevel.Info);
                ModCompat ppjaFruitsAndVeggies = new ModCompat("[PPJA] Ancient Crops", MyConfig.PPJAAncientCropsPath + @"\[JA] Ancient Crops\Crops\");
                modsToShow.Add(ppjaFruitsAndVeggies);
            }

            if (MyConfig.PPJACannabisKitPath != "")
            {
                this.Monitor.Log("Loading [PPJA] Cannabis Kit", LogLevel.Info);
                ModCompat ppjaFruitsAndVeggies = new ModCompat("[PPJA] Cannabis Kit", MyConfig.PPJACannabisKitPath + @"\[JA] Cannabis Kit\Crops\");
                modsToShow.Add(ppjaFruitsAndVeggies);
            }

            if (MyConfig.BonstersFruitAndVeggiesPath != "")
            {
                this.Monitor.Log("Loading Bonster's Fruit & Veggies", LogLevel.Info);
                ModCompat ppjaFruitsAndVeggies = new ModCompat("Bonster's Fruit & Veggies", MyConfig.BonstersFruitAndVeggiesPath + @"\Crops\");
                modsToShow.Add(ppjaFruitsAndVeggies);
            }

            foreach (ModCompat mod in modsToShow)
            {
                ModCompatResult result = mod.LoadCrops(SpringCrops, SummerCrops, FallCrops, WinterCrops, MyHelper);

                if (result == ModCompatResult.Success) { continue; }

                this.Monitor.Log($"Unable to load {mod.Name}. Error Code: {result}", LogLevel.Error);
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

            // Load the Base Crops
            var currentDay = SDate.From(Game1.Date).Day;
            var currentSeason = SDate.From(Game1.Date).Season;

            switch (currentSeason)
            {
                case "spring":
                    ShowCrops(SpringCrops, currentDay);
                    break;
                case "summer":
                    ShowCrops(SummerCrops, currentDay);
                    break;
                case "fall":
                    ShowCrops(FallCrops, currentDay);
                    break;
                case "winter":
                    ShowCrops(WinterCrops, currentDay);
                    break;
                default:
                    return;
            }
        }

        private void ShowCrops(List<Crop> cropList, int day)
        {
            foreach (Crop crop in cropList)
            {                
                if (MyConfig.ShowBaseCrops)
                {
                    var DaysToMatureBoosted = ActualGrowRate(crop, Fertilizer.None);

                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.Message, HUDMessage.newQuest_type));
                    }
                }

                if (MyConfig.ShowSpeedGro)
                {
                    var DaysToMatureBoosted = ActualGrowRate(crop, Fertilizer.SpeedGro);

                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageSpeedGro, HUDMessage.newQuest_type));
                    }
                }

                if (MyConfig.ShowDeluxeSpeedGro)
                {
                    var DaysToMatureBoosted = ActualGrowRate(crop, Fertilizer.DeluxeSpeedGro);

                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageDelxueSpeedGro, HUDMessage.newQuest_type));
                    }
                }

                if (MyConfig.ShowHyperSpeedGro)
                {
                    var DaysToMatureBoosted = ActualGrowRate(crop, Fertilizer.HyperSpeedGro);

                    if (DaysToMatureBoosted + day == DaysInAMonth)
                    {
                        Game1.addHUDMessage(new HUDMessage(crop.MessageHyperSpeedGro, HUDMessage.newQuest_type));
                    }
                }
            }
        }

        private int ActualGrowRate(Crop crop, double fertilizerFactor)
        {
            double actualDaysToMature = crop.DaysToMature - crop.DaysToMature * fertilizerFactor;

            if (FarmingSkills.IsAgriculturist)
            {
                actualDaysToMature = crop.DaysToMature - crop.DaysToMature * (fertilizerFactor + FarmingSkills.AgriculturistGrowthRate);
                return (int)actualDaysToMature;
            }
            return (int)actualDaysToMature;
        }

        private List<Crop> SetSpringCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("spring","Blue Jazz",4, MyHelper),
                Crop.GetLocalizedCrop("spring","Cauliflower",12,MyHelper),
                Crop.GetLocalizedCrop("spring","Garlic",4,MyHelper),
                Crop.GetLocalizedCrop("spring","Green Bean",10,MyHelper),
                Crop.GetLocalizedCrop("spring","Kale",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Parsnip",4,MyHelper),
                Crop.GetLocalizedCrop("spring","Potato",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Rhubarb",13,MyHelper),
                Crop.GetLocalizedCrop("spring","Strawberry",8,MyHelper),
                Crop.GetLocalizedCrop("spring","Tulip",6,MyHelper),
                Crop.GetLocalizedCrop("spring","Rice",8,MyHelper)
            };

            return retval;
        }

        private List<Crop> SetSummerCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("summer","Blueberry",13, MyHelper),
                Crop.GetLocalizedCrop("summer","Hops",11,MyHelper),
                Crop.GetLocalizedCrop("summer","Hot Pepper",5,MyHelper),
                Crop.GetLocalizedCrop("summer","Coffee Bean",10,MyHelper),
                Crop.GetLocalizedCrop("summer","Melon",12,MyHelper),
                Crop.GetLocalizedCrop("summer","Poppy",7,MyHelper),
                Crop.GetLocalizedCrop("summer","Radish",6,MyHelper),
                Crop.GetLocalizedCrop("summer","Red Cabbage",9,MyHelper),
                Crop.GetLocalizedCrop("summer","Starfruit",13,MyHelper),
                Crop.GetLocalizedCrop("summer","Summer Spangle",8,MyHelper),
                Crop.GetLocalizedCrop("summer","Tomato",11,MyHelper)
            };

            return retval;
        }

        private List<Crop> SetFallCrops()
        {
            var retval = new List<Crop>()
            {
                Crop.GetLocalizedCrop("fall","Wheat",4, MyHelper),
                Crop.GetLocalizedCrop("fall","Corn",14,MyHelper),
                Crop.GetLocalizedCrop("fall","Amaranth",7,MyHelper),
                Crop.GetLocalizedCrop("fall","Artichoke",8,MyHelper),
                Crop.GetLocalizedCrop("fall","Beet",6,MyHelper),
                Crop.GetLocalizedCrop("fall","Bok Choy",4,MyHelper),
                Crop.GetLocalizedCrop("fall","Cranberries",7,MyHelper),
                Crop.GetLocalizedCrop("fall","Eggplant",5,MyHelper),
                Crop.GetLocalizedCrop("fall","Sunflower",8,MyHelper),
                Crop.GetLocalizedCrop("fall","Fairy Rose",12,MyHelper),
                Crop.GetLocalizedCrop("fall","Grape",10,MyHelper),
                Crop.GetLocalizedCrop("fall","Pumpkin",13,MyHelper),
                Crop.GetLocalizedCrop("fall","Yam",10,MyHelper),
                Crop.GetLocalizedCrop("fall","Ancient Fruit",28,MyHelper)
            };

            return retval;
        }
    }
}
