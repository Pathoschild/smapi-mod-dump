using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public struct BundleItemInfo
    {
        public int ParentSheetIndex;
        public int StackSize;
        public int Quality;

        public BundleItemInfo(int[] itemInfo)
        {
            ParentSheetIndex = itemInfo[0];
            StackSize = itemInfo[1];
            Quality = itemInfo[2];
        }
    };
    public class MissingItems
    {
        private Dictionary<int, SObject> SpringItems;
        private Dictionary<int, SObject> SummerItems;
        private Dictionary<int, SObject> FallItems;
        private Dictionary<int, SObject> WinterItems;
        private Dictionary<int, SObject> CCItems;
        private Dictionary<int, SObject> AvailableMerchentStock;
        private Dictionary<int, SObject> AvailableRecipes;
        private Dictionary<int , Dictionary<int, SObject>> AllRecipeIngredients;
        private Dictionary<int, Dictionary<int, SObject>> CondensedPlayerItems;
        private Dictionary<int, bool[]> CompletedItems;
        private Dictionary<int, SObject> CurrentSeasonSpecifics;
        private OwnedItems OwnedItems;

        private List<SObject> MissingCommonCCItems;
        private List<SObject> MissingSpecificCCItems;
        private List<SObject> MissingMerchantCCItems;
        private List<SObject> MissingMerchantItems;
        private List<SObject> MissingSpecifics;
        private List<SObject> MissingRecipes;
        private Dictionary<int, Dictionary<int, SObject>> MissingRecipeIngredients;

        public MissingItems()
        {
            InitializeItemDictionaries();
            InitializeMissingLists();
            InitializePlayerItems();

            FindMissingItems();
        }

        private void InitializeItemDictionaries()
        {
            SpringItems = new SpringSpecificItems().GetItems();
            SummerItems = new SummerSpecificItems().GetItems();
            FallItems = new FallSpecificItems().GetItems();
            WinterItems = new WinterSpecificItems().GetItems();
            CCItems = new CommonCCItems().GetItems();
            AvailableMerchentStock = new AvailableMerchantStock(SpringItems, SummerItems, FallItems, WinterItems, CCItems).GetItems();
            AvailableRecipes = new Recipes().GetItems();
            AllRecipeIngredients = new RecipeIngredients().GetRecipeAndIngredients();
            CurrentSeasonSpecifics = GetCurrentSeasonSpecifics();

            CompletedItems = new CommunityCenter().bundlesDict();
        }

        private void InitializeMissingLists()
        {
            MissingCommonCCItems = new List<SObject>();
            MissingSpecificCCItems = new List<SObject>();
            MissingSpecifics = new List<SObject>();
            MissingMerchantItems = new List<SObject>();
            MissingMerchantCCItems = new List<SObject>();
            MissingRecipes = new List<SObject>();
            MissingRecipeIngredients = new Dictionary<int, Dictionary<int, SObject>>();
        }

        private void InitializePlayerItems()
        {
            OwnedItems = new OwnedItems();
            CondensedPlayerItems = OwnedItems.GetPlayerItems();
        }

        private void FindMissingItems()
        {
            FindMissingCCItems();
            FindMissingSeasonSpecifics();
            FindMissingMerchantSpecifics();
            FindMissingRecipes();
            FindMissingRecipeIngredients();
        }

        private Dictionary<int, SObject> GetCurrentSeasonSpecifics()
        {
            switch (Game1.currentSeason)
            {
                case "spring":
                    return SpringItems;
                case "summer":
                    return SummerItems;
                case "fall":
                    return FallItems;
                case "winter":
                    return WinterItems;
                default:
                    //should never reach here
                    return null;
            }
        }

        public List<SObject> GetMissingCommonCCItems()
        {
            return MissingCommonCCItems;
        }

        public List<SObject> GetMissingSpecificCCItems()
        {
            return MissingSpecificCCItems;
        }

        public List<SObject> GetMissingSpecifics()
        {
            return MissingSpecifics;
        }

        public List<SObject> GetMissingMerchantItems()
        {
            return MissingMerchantItems;
        }

        public List<SObject> GetMissingMerchantCCItems()
        {
            return MissingMerchantCCItems;
        }

        public List<SObject> GetMissingRecipes()
        {
            return MissingRecipes;
        }
        /// <summary>
        /// <recipeParentSheetIndex, <IngredientParentSheetIndex, IngredientObj>>
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, SObject>> GetMissingRecipeIngredients()
        {
            return MissingRecipeIngredients;
        }

        private void FindMissingCCItems()
        {
            Dictionary<int, int[][]> processedBundlesData = ProcessCCBundles();

            foreach (KeyValuePair<int, int[][]> pair in processedBundlesData)
            {
                int bundleID = pair.Key;
                if (!IsBundleCompleted(bundleID))
                {
                    CheckForAndAddMissingBundleItems(bundleID, pair.Value);
                }
            }
        }

        private Dictionary<int, int[][]> ProcessCCBundles()
        {
            Dictionary<string, string> rawBundleData = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");
            Dictionary<int, int[][]> processedBundleData = new Dictionary<int, int[][]>(); //the key will be the bundleId, the int[] is index from parent sheet, howMany, and quality

            //In the bundles.json file it appears that the key is RoomName/BundleID
            //value is: name/reward/itemsNeeded
            //where each item needed is: index howMany quality 
            //where quality is an int from 0-3, 0 being normal, 2 being gold
            foreach (KeyValuePair<string, string> pair in rawBundleData)
            {
                int key = Convert.ToInt32(pair.Key.Split('/')[1]);
                int length = pair.Value.Split('/')[2].Split(' ').Length / 3;
                int[][] itemsNeeded = new int[length][];

                for (int i = 0; i < length; ++i)
                {
                    itemsNeeded[i] = new int[3];
                    for (int j = 0; j < 3; ++j)
                    {
                        itemsNeeded[i][j] = Convert.ToInt32(pair.Value.Split('/')[2].Split(' ')[i * 3 + j]);
                    }
                }
                processedBundleData.Add(key, itemsNeeded);
            }

            return processedBundleData;
        }

        private void CheckForAndAddMissingBundleItems(int bundleID, int[][] bundleInfo)
        {
            for (int i = 0; i < bundleInfo.Length; ++i)
            {
                BundleItemInfo itemInfo = new BundleItemInfo(bundleInfo[i]);

                if (!IsItemCompleted(bundleID, i))
                {
                    SObject missingItem = new SObject(itemInfo.ParentSheetIndex, itemInfo.StackSize, quality: itemInfo.Quality);
                    AddMissingItemToProperList(missingItem);
                }
            }
        }

        private void AddMissingItemToProperList(SObject missingItem)
        {
            if (IsItemSeasonSpecific(missingItem.ParentSheetIndex))
            {
                MissingSpecificCCItems.Add(missingItem);
            }
            else if (IsItemMerchantSpecific(missingItem.ParentSheetIndex))
            {
                MissingMerchantCCItems.Add(missingItem);
            }
            else if (IsItemCommonSpecific(missingItem.ParentSheetIndex))
            {
                MissingCommonCCItems.Add(missingItem);
            }
        }

        private bool IsItemCommonSpecific(int parentSheetIndex)
        {
            return CCItems.ContainsKey(parentSheetIndex);
        }

        private bool IsItemSeasonSpecific(int parentSheetIndex)
        {
            return CurrentSeasonSpecifics.ContainsKey(parentSheetIndex) && !CCItems.ContainsKey(parentSheetIndex);
        }

        private bool IsItemMerchantSpecific(int parentSheetIndex)
        {
            return AvailableMerchentStock.ContainsKey(parentSheetIndex) && !CCItems.ContainsKey(parentSheetIndex);
        }

        private bool IsBundleCompleted(int bundleID)
        {
            foreach(bool completed in CompletedItems[bundleID])
            {
                if (!completed)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsItemCompleted(int bundleID, int arrayIndex)
        {
            return CompletedItems[bundleID][arrayIndex];
        }

        private void FindMissingSeasonSpecifics()
        {
            HighestQuality highestQuality = new HighestQuality();
            foreach(KeyValuePair<int, SObject> pair in CurrentSeasonSpecifics)
            {
                int commonQualityAmountMissing = HowManyMissingCommonQuality(new SObject(pair.Key, 1, quality: Constants.COMMON_QUALITY));

                int maxQuality = highestQuality.GetHighestQualityForItem(pair.Key);
                int highestQualityAmountMissing = HowManyMissingHighestQuality(new SObject(pair.Key, 1, quality: maxQuality));

                if(commonQualityAmountMissing != 0)
                {
                    MissingSpecifics.Add(new SObject(pair.Key, commonQualityAmountMissing));
                }
                if(highestQualityAmountMissing != 0)
                {
                    MissingSpecifics.Add(new SObject(pair.Key, highestQualityAmountMissing, quality: maxQuality));
                }
            }
        }

        private void FindMissingMerchantSpecifics()
        {
            foreach (KeyValuePair<int, SObject> pair in AvailableMerchentStock)
            {
                int commonQualityAmountMissing = HowManyMissingCommonQuality(new SObject(pair.Key, 1, quality: Constants.COMMON_QUALITY));
                if (commonQualityAmountMissing != 0)
                {
                    MissingMerchantItems.Add(new SObject(pair.Key, commonQualityAmountMissing));
                }
            }
        }

        private void FindMissingRecipes()
        {
            foreach(KeyValuePair<int, SObject> pair in AvailableRecipes)
            {
                int commonQualityAmountMissing = HowManyMissingCommonQuality(new SObject(pair.Key, 1, quality: Constants.COMMON_QUALITY));
                if(commonQualityAmountMissing != 0 || ModEntry.Config.AlwaysShowAllRecipes)
                {
                    MissingRecipes.Add(new SObject(pair.Key, commonQualityAmountMissing == 0 ? 1 : commonQualityAmountMissing));
                }
            }
        }

        private void FindMissingRecipeIngredients()
        {
            foreach(KeyValuePair<int, Dictionary<int, SObject>> recipeAndIngredients in AllRecipeIngredients)
            {
                foreach(KeyValuePair<int, SObject> ingredient in recipeAndIngredients.Value)
                {
                    int amountMissing = HowManyMissingIngredient(ingredient.Value, ingredient.Key);
                    if (amountMissing != 0)
                    {
                        SObject obj = new SObject(ingredient.Key, amountMissing);
                        Dictionary<int, SObject> missingIngredient = new Dictionary<int, SObject> { [ingredient.Key] = obj };
                        if (MissingRecipeIngredients.ContainsKey(recipeAndIngredients.Key))
                        {
                            MissingRecipeIngredients[recipeAndIngredients.Key].Add(ingredient.Key, obj);
                        }
                        else
                        {
                            MissingRecipeIngredients.Add(recipeAndIngredients.Key, missingIngredient);
                        }
                    }
                }
            }
        }

        private int HowManyMissingIngredient(SObject ingredient, int parentSheetIndex)
        {
            Constants constants = new Constants();
            int totalAmount;
            int amountMissing;

            if (constants.SPECIAL_COOKING_IDS.Contains(parentSheetIndex))
            {
                totalAmount = OwnedItems.GetTotalAmountOfFishEggsOrMilk(parentSheetIndex);
            }
            else
            {
                totalAmount = OwnedItems.GetTotalAmountOfItem(parentSheetIndex);
            }
            amountMissing = ingredient.Stack - totalAmount;
            return amountMissing < 0 ? 0 : amountMissing;
        }

        private int HowManyMissingCommonQuality(SObject item)
        {
            if (CanPlayerOnlyHaveOne(item.ParentSheetIndex))
            {
                //Its a legendary fish, only show them that they
                //are missing the highest qulaity of it
                return 0;
            }
            if (IsQualityItemInPlayerItems(item))
            {
                int amountMissing = ModEntry.Config.CommonQualityAmount - CondensedPlayerItems[item.ParentSheetIndex][Constants.COMMON_QUALITY].Stack;
                return amountMissing < 0 ? 0 : amountMissing;
            }
            return ModEntry.Config.CommonQualityAmount;
        }

        private int HowManyMissingHighestQuality(SObject item)
        {
            if (CanPlayerOnlyHaveOne(item.ParentSheetIndex))
            {
                return IsItemInPlayerItems(item.ParentSheetIndex) ? 0 : 1;
            }
            if (IsQualityItemInPlayerItems(item))
            {
                int amountMissing = ModEntry.Config.HighestQualityAmount - CondensedPlayerItems[item.ParentSheetIndex][item.Quality].Stack;
                return amountMissing < 0 ? 0 : amountMissing;
            }
            return ModEntry.Config.HighestQualityAmount;
        }

        private bool IsQualityItemInPlayerItems(SObject item)
        {
            if (IsItemInPlayerItems(item.ParentSheetIndex))
            {
                return CondensedPlayerItems[item.ParentSheetIndex].ContainsKey(item.Quality);
            }
            return false;
        }

        private bool IsItemInPlayerItems(int parentSheetIndex)
        {
            return CondensedPlayerItems.ContainsKey(parentSheetIndex);
        }

        private bool CanPlayerOnlyHaveOne(int parentSheetIndex)
        {
            Constants constants = new Constants();
            return constants.ITEMS_PLAYER_CAN_ONLY_HAVE_ONE_OF.Contains(parentSheetIndex) ? true : false;
        }
    }
}
