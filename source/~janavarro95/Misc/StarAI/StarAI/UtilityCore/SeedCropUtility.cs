using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.UtilityCore
{
    class SeedCropUtility
    {
       public static Dictionary<int, float> CropSeedUtilityDictionary = new Dictionary<int, float>();
       public static Dictionary<int, float> UserCropSeedUtilityDictionary = new Dictionary<int, float>();
        public static Dictionary<int, float> ScaleUtilityDictionary = new Dictionary<int, float>();

        public static void setUpUserCropUtilityDictionary()
        {
            UserCropSeedUtilityDictionary.Clear();
            Dictionary<KeyValuePair<int, Crop>, float> tempDictionary = new Dictionary<KeyValuePair<int, Crop>, float>();
            foreach (var item in Game1.objectInformation)
            {
                StardewValley.Object test = new StardewValley.Object(item.Key, 1);
                if (test.getCategoryName() == "Seed")
                {
                    KeyValuePair<int, Crop> pair = StarAI.TaskCore.CropLogic.SeedLogic.getSeedCropPair(test.parentSheetIndex);


                    if (pair.Value == null) continue;
                    //if (utilityValue <= 0) continue;
                    UserCropSeedUtilityDictionary.Add(test.parentSheetIndex, 0); //CHANGE THIS TO BE BASED ON THE MENU UTILITY
                    //numberOfInitializedUtilitySeedValues++;
                    ModCore.CoreMonitor.Log("Star AI: Utility Core: Calculating " + test.name + " for user utility picking with a value of: 0");
                }
            }
            ModCore.CoreMonitor.Log("UTIL COUNT:" + UserCropSeedUtilityDictionary.Count);
        }

        public static void setUpCropUtilityDictionaryDaily()
        {
            CropSeedUtilityDictionary.Clear();
            Dictionary<int, float> tempDictionary = new Dictionary<int, float>();
            int numberOfInitializedUtilitySeedValues = 0;
            float totalValue = 0;
            foreach (var item in Game1.objectInformation)
            {
                StardewValley.Object test = new StardewValley.Object(item.Key, 1);
                if (test.getCategoryName() == "Seed")
                {
                    KeyValuePair<int, Crop> pair = StarAI.TaskCore.CropLogic.SeedLogic.getSeedCropPair(test.parentSheetIndex);
                    if (pair.Value == null) continue;

                    float utilityValue = averageGoldPerDay(pair.Value, test);
                    if (!pair.Value.seasonsToGrowIn.Contains(Game1.currentSeason)) utilityValue = 0;
                    totalValue += utilityValue;
                    utilityValue =(float) Math.Round(utilityValue, 2);
                    //if (utilityValue <= 0) continue;
                    tempDictionary.Add(test.parentSheetIndex, utilityValue); //CHANGE THIS TO BE BASED ON THE MENU UTILITY
                    numberOfInitializedUtilitySeedValues++;
                    //ModCore.CoreMonitor.Log("Star AI: Utility Core: Calculating " + test.name + " for utility picking with a value of: "+utilityValue.ToString());
                }
            }

            foreach (var c in tempDictionary)
            {
                KeyValuePair<int, float> hello = c;
                float afterValue = hello.Value / totalValue;
                CropSeedUtilityDictionary.Add(hello.Key, afterValue);
                StardewValley.Object test = new StardewValley.Object(c.Key, 1);
                ModCore.CoreMonitor.Log("Star AI: Utility Core: Calculating " + test.name + " for utility picking value of (" + hello.Value + " : " + (afterValue * 100).ToString() + "%)");
            }
            ModCore.CoreMonitor.Log("Star AI: Utility Core: Calculating " + numberOfInitializedUtilitySeedValues + " seeds for utility picking.");
            if (numberOfInitializedUtilitySeedValues == 0)
            {
                ModCore.CoreMonitor.Log("No seed values initialized. There must be no possible seeds I can grow for the rest of this season.", StardewModdingAPI.LogLevel.Alert);
            }
            recalculateAllUtilityValues();
        }


        public static void recalculateAllUtilityValues()
        {
            ScaleUtilityDictionary.Clear();
            float totalValue = 0;
            foreach(var v in CropSeedUtilityDictionary)
            {
                totalValue +=Math.Abs(v.Value);
            }
            foreach (var v in UserCropSeedUtilityDictionary)
            {
                totalValue +=Math.Abs(v.Value);
            }
            Dictionary<int, float> ok = new Dictionary<int, float>();
            foreach(var v in CropSeedUtilityDictionary)
            {
                ok.Add(v.Key, v.Value);
            }

            Dictionary<int, float> ok2 = new Dictionary<int, float>();
            foreach (var v in UserCropSeedUtilityDictionary)
            {
                ok2.Add(v.Key, v.Value);
            }

            foreach (var v in ok)
            {
                CropSeedUtilityDictionary[v.Key] = CropSeedUtilityDictionary[v.Key] / totalValue;
            }
            foreach (var v in ok2)
            {
                UserCropSeedUtilityDictionary[v.Key] = UserCropSeedUtilityDictionary[v.Key] / totalValue;
            }
            ModCore.CoreMonitor.Log("TOTAL VALUE:"+totalValue.ToString());

            foreach (var v in CropSeedUtilityDictionary)
            {
                if (StardustCore.StaticExtentions.HasValue(CropSeedUtilityDictionary[v.Key]) == false)
                {
                    CropSeedUtilityDictionary[v.Key] = 0;
                }
                if (StardustCore.StaticExtentions.HasValue(UserCropSeedUtilityDictionary[v.Key]) == false)
                {
                    UserCropSeedUtilityDictionary[v.Key] = 0;
                }
                float scale = CropSeedUtilityDictionary[v.Key] + UserCropSeedUtilityDictionary[v.Key];
                if (scale <= 0)
                {
                    scale = 0;
                }
                ScaleUtilityDictionary.Add(v.Key,scale);
                StardewValley.Object obj = new StardewValley.Object(v.Key, 1);
                ModCore.CoreMonitor.Log("Updated: " + obj.name + " to now have a percent utility contribution of: "+ (CropSeedUtilityDictionary[v.Key] + UserCropSeedUtilityDictionary[v.Key]).ToString());
            }

        }

        public static float getUtilityScaleValue(int seedIndex)
        {
            return ScaleUtilityDictionary[seedIndex];
        }



        public static int numberOfDaysToGrow(Crop c)
        {
            int total = 0;
            foreach(var v in c.phaseDays)
            {
                if (v != -1) total += v;
            }
            return (total-100000+1);
        }

        public static void updateUserUtilities(int seedIndex,float amount)
        {

            ModCore.CoreMonitor.Log("This is the seed index:" + seedIndex);
            UserCropSeedUtilityDictionary[seedIndex] += amount;
            //if (UserCropSeedUtilityDictionary[seedIndex] <= 0) UserCropSeedUtilityDictionary[seedIndex] = 0;
            UserCropSeedUtilityDictionary[seedIndex] = (float) Math.Round(UserCropSeedUtilityDictionary[seedIndex], 2);
            StardewValley.Object obj = new StardewValley.Object(seedIndex, 1);
            ModCore.CoreMonitor.Log("Updated: " + obj.name + " to now have a user utility value of " + UserCropSeedUtilityDictionary[seedIndex] + " and a total utlity value of: " +(UserCropSeedUtilityDictionary[seedIndex] + CropSeedUtilityDictionary[seedIndex]));
            recalculateAllUtilityValues();
        }

        //AI cares about money, user might care about other things.
        //Doesn't recalculate crops that regrow. Favors crops with mutiple yields. Quantity over quality I suppose.
        public static float averageGoldPerDay(Crop c,Item seeds)
        {
     
            StardewValley.Object crop = new StardewValley.Object(c.indexOfHarvest, 1);

           
            int days = numberOfDaysToGrow(c);
            //ModCore.CoreMonitor.Log("DAYS: " + days);
            if (days <= 0) return 0;
            int maxHarvest = maxHarvestsRemaining(c);
            if (maxHarvest == 0) return 0;
            if (c.maxHarvest <= 0)
            {
                return ((Game1.player.farmingLevel * .02f + 1.01f) * (maxHarvest) * crop.price) - seeds.salePrice() / days;
            }
            else
            {
                return ((Game1.player.farmingLevel * .02f + 1.01f) * (maxHarvest *c.maxHarvest) * crop.price) - seeds.salePrice() / days;
            }
           
        }

        public static int maxHarvestsRemaining(Crop c)
        {
            return (28-Game1.dayOfMonth)/numberOfDaysToGrow(c);
        }


        public static List<Item> sortSeedListByUtility(List<Item> seedList)
        {
            List<KeyValuePair<int, float>> sortList = new List<KeyValuePair<int ,float>>();
            foreach(var seeds in seedList)
            {
                var ret = getKeyBySeedIndex(seeds.parentSheetIndex,ScaleUtilityDictionary);
                if (ret.Key == -999) continue;
                sortList.Add(ret);
            }

            //Sort the list by utility of the seeds.
            sortList.Sort(delegate (KeyValuePair < int,float> t1, KeyValuePair<int, float> t2)
            {
                return t1.Value.CompareTo(t2.Value);
            });

            sortList.Reverse(); //I want max to be first instead of min.

            float maxUtility = sortList.ElementAt(0).Value;
            List<int> finalList = new List<int>();
            foreach(var utilitySeed in sortList)
            {
                if (utilitySeed.Value >= maxUtility) finalList.Add(utilitySeed.Key);
            }
            List<Item> finalShopStock = new List<Item>();
            foreach(var seedIndex in finalList)
            {
                foreach(var seeds in seedList)
                {
                    if (seedIndex == seeds.parentSheetIndex) finalShopStock.Add(seeds);
                }
            }

            return finalShopStock;
        }



        public static KeyValuePair<int, float> getKeyBySeedIndex(int seedIndex,Dictionary<int,float> dic)
        {
            foreach (var key in dic)
            {
                if (key.Key == seedIndex) return key;
            }        
            return new KeyValuePair<int, float>(-999, -999);
        }

    }
}
