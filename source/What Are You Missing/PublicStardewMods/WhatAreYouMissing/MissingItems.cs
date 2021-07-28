/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
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
        private Dictionary<int, SObject> AllFish;
        private Dictionary<int, SObject> AllCrops;
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
        private List<SObject> MissingFish;
        private List<SObject> MissingCrops;

        public MissingItems(IReflectionHelper reflection)
        {
            InitializeItemDictionaries();
            InitializeMissingLists();
            InitializePlayerItems(reflection);

            FindMissingItems();
        }

        private void InitializeItemDictionaries()
        {
            SpringItems = new SpringSpecificItems().GetItems();
            SummerItems = new SummerSpecificItems().GetItems();
            FallItems = new FallSpecificItems().GetItems();
            WinterItems = new WinterSpecificItems().GetItems();
            CCItems = new CommonCCItems().GetItems();
            AvailableMerchentStock = new AvailableMerchantStock(SpringItems, SummerItems, FallItems, WinterItems).GetItems();
            AvailableRecipes = new Recipes().GetItems();
            AllRecipeIngredients = new RecipeIngredients().GetRecipeAndIngredients();
            AllFish = new AllFish().GetItems();
            AllCrops = new AllCrops().GetItems();
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
            MissingFish = new List<SObject>();
            MissingCrops = new List<SObject>();
        }

        private void InitializePlayerItems(IReflectionHelper reflection)
        {
            OwnedItems = new OwnedItems(reflection);
            CondensedPlayerItems = OwnedItems.GetPlayerItems();
        }

        private void FindMissingItems()
        {
            FindMissingCCItems();
            FindMissingSeasonSpecifics();
            FindMissingMerchantSpecifics();
            FindMissingRecipes();
            FindMissingRecipeIngredients();
            FindMissingFish();
            FindMissingCrops();
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

        public List<SObject> GetMissingFish()
        {
            return MissingFish;
        }

        public List<SObject> GetMissingCrops()
        {
            return MissingCrops;
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
            Dictionary<string, string> rawBundleData = Game1.netWorldState.Value.BundleData;
            Dictionary<int, int[][]> processedBundleData = new Dictionary<int, int[][]>(); //the key will be the bundleId, the int[] is index from parent sheet, howMany, and quality

            //In the bundles.json file it appears that the key is RoomName/BundleID
            //value is: name/reward/itemsNeeded
            //where each item needed is: index howMany quality 
            //where quality is an int from 0 (normal), 1 (silver), 2 (gold), 4 (iridium)
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
                //-1 parent sheet index indicates its just money, there are some items in the bundle file that don't exist in the game so check for that too
                if ((itemInfo.ParentSheetIndex == -1 || Game1.objectInformation.ContainsKey(itemInfo.ParentSheetIndex)) && !IsItemCompleted(bundleID, i))
                {
                    SObject missingItem = new SObject(itemInfo.ParentSheetIndex, itemInfo.StackSize, quality: itemInfo.Quality);
                    AddMissingItemToProperList(missingItem);
                }
            }
        }

        private void AddMissingItemToProperList(SObject missingItem)
        {
            if (IsItemCurrentSeasonSpecific(missingItem.ParentSheetIndex))
            {
                MissingSpecificCCItems.Add(missingItem);
            }
            else if (IsItemMerchantSpecific(missingItem.ParentSheetIndex))
            {
                MissingMerchantCCItems.Add(missingItem);
            }
            else if (!IsItemSeasonSpecific(missingItem.ParentSheetIndex))
            {
                MissingCommonCCItems.Add(missingItem);
            }
            else
            {
                ModEntry.Logger.LogWarning($"Not adding {missingItem.Name} to any list (parent sheet index: {missingItem.ParentSheetIndex})");
            }
        }

        private bool IsItemCurrentSeasonSpecific(int parentSheetIndex)
        {
            return CurrentSeasonSpecifics.ContainsKey(parentSheetIndex) && !CCItems.ContainsKey(parentSheetIndex);
        }

        private bool IsItemMerchantSpecific(int parentSheetIndex)
        {
            return AvailableMerchentStock.ContainsKey(parentSheetIndex) && !CCItems.ContainsKey(parentSheetIndex);
        }

        private bool IsItemSeasonSpecific(int parentSheetIndex)
        {
            bool inSpring = SpringItems.ContainsKey(parentSheetIndex);
            bool inSummer = SummerItems.ContainsKey(parentSheetIndex);
            bool inFall = FallItems.ContainsKey(parentSheetIndex);
            bool inWinter = WinterItems.ContainsKey(parentSheetIndex);

            if (!inSpring && !inSummer && !inFall && !inWinter)
            {
                //if its not in any season specific items then its not season specific
                return false;
            }
            else
            {
                return !(inSpring && inSummer && inFall && inWinter);
            }
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
            foreach(KeyValuePair<int, SObject> pair in CurrentSeasonSpecifics)
            {
                if (IsQualityToBeIgnored())
                {
                    AddIfMissingIgnoringQuality(pair, MissingSpecifics);
                }
                else
                {
                    AddIfMissingConsiderQuality(pair, MissingSpecifics);
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
                if(commonQualityAmountMissing != 0 || ModEntry.modConfig.AlwaysShowAllRecipes)
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

        private void FindMissingFish()
        {
            foreach (KeyValuePair<int, SObject> pair in AllFish)
            {
                if (IsQualityToBeIgnored())
                {
                    AddIfMissingIgnoringQuality(pair, MissingFish);
                }
                else
                {
                    AddIfMissingConsiderQuality(pair, MissingFish, ModEntry.modConfig.AlwaysShowAllFish);
                }
            }
        }

        private void FindMissingCrops()
        {
            foreach (KeyValuePair<int, SObject> pair in AllCrops)
            {
                if(IsQualityToBeIgnored())
                {
                    AddIfMissingIgnoringQuality(pair, MissingCrops);
                }
                else
                {
                    AddIfMissingConsiderQuality(pair, MissingCrops);
                }
            }
        }

        private void AddIfMissingConsiderQuality(KeyValuePair<int, SObject> parentSheetIndexItemPair, List<SObject> list, bool alwaysShow = false)
        {
            HighestQuality highestQuality = new HighestQuality();

            int commonQualityAmountMissing = HowManyMissingCommonQuality(new SObject(parentSheetIndexItemPair.Key, 1, quality: Constants.COMMON_QUALITY));

            int maxQuality = highestQuality.GetHighestQualityForItem(parentSheetIndexItemPair.Key);
            int highestQualityAmountMissing = HowManyMissingHighestQuality(new SObject(parentSheetIndexItemPair.Key, 1, quality: maxQuality));

            if (commonQualityAmountMissing != 0 || alwaysShow)
            {
                list.Add(new SObject(parentSheetIndexItemPair.Key, commonQualityAmountMissing == 0 ? 1 : commonQualityAmountMissing));
            }
            if (highestQualityAmountMissing != 0)
            {
                list.Add(new SObject(parentSheetIndexItemPair.Key, highestQualityAmountMissing, quality: maxQuality));
            }
        }

        private void AddIfMissingIgnoringQuality(KeyValuePair<int, SObject> parentSheetIndexItemPair, List<SObject> list)
        {
            int amountMissing = HowManyMissingIgnoreQuality(new SObject(parentSheetIndexItemPair.Key, 1));
            if (amountMissing != 0)
            {
                list.Add(new SObject(parentSheetIndexItemPair.Key, amountMissing));
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
                int amountMissing = ModEntry.modConfig.CommonAmount - CondensedPlayerItems[item.ParentSheetIndex][Constants.COMMON_QUALITY].Stack;
                return amountMissing < 0 ? 0 : amountMissing;
            }
            return ModEntry.modConfig.CommonAmount;
        }

        private int HowManyMissingHighestQuality(SObject item)
        {
            if (CanPlayerOnlyHaveOne(item.ParentSheetIndex))
            {
                return IsItemInPlayerItems(item.ParentSheetIndex) ? 0 : 1;
            }
            if (IsQualityItemInPlayerItems(item))
            {
                int amountMissing = ModEntry.modConfig.HighestQualityAmount - CondensedPlayerItems[item.ParentSheetIndex][item.Quality].Stack;
                return amountMissing < 0 ? 0 : amountMissing;
            }
            return ModEntry.modConfig.HighestQualityAmount;
        }

        private int HowManyMissingIgnoreQuality(SObject item)
        {
            if (CanPlayerOnlyHaveOne(item.ParentSheetIndex))
            {
                return IsItemInPlayerItems(item.ParentSheetIndex) ? 0 : 1;
            }
            if (IsItemInPlayerItems(item.ParentSheetIndex))
            {
                int totalAmount = OwnedItems.GetTotalAmountOfItem(item.ParentSheetIndex);
                int amountMissing = ModEntry.modConfig.CommonAmount - totalAmount;
                return amountMissing < 0 ? 0 : amountMissing;
            }
            return ModEntry.modConfig.CommonAmount;
        }

        private bool IsQualityToBeIgnored()
        {
            return ModEntry.modConfig.IgnoreQuality == true;
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
