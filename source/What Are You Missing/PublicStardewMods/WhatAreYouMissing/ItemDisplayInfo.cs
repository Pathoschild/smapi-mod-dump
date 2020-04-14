using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class ItemDisplayInfo
    {
        private SObject Item;
        private int ParentSheetIndex;
        public ItemDisplayInfo(SObject item)
        {
            Item = item;
            ParentSheetIndex = item.ParentSheetIndex;
        }

        public string GetItemDisplayInfo()
        {
            if (Item.Category == SObject.FishCategory)
            {
                return new FishDisplayInfo(ParentSheetIndex).GetFishDisplayInfo();
            }
            else if(Item.Category == SObject.CookingCategory)
            {
                return GetCookedItemsDisplayInfo();
            }
            else
            {
                return "";
            }
        }

        private string GetCookedItemsDisplayInfo()
        {
            Dictionary<int, SObject> recipeIngredients = ModEntry.RecipesIngredients.GetRecipeIngredients(ParentSheetIndex);

            string displayInfo = GetCookedItemStatsInfo() + "\n";

            if (recipeIngredients != null)
            {
                displayInfo += Utilities.GetTranslation("INGREDIENTS") + ": ";

                int i = 0;
                foreach (KeyValuePair<int, SObject> ingredient in recipeIngredients)
                {
                    SObject ingredientObj = ingredient.Value;
                    string displayName = GetIngredientDisplayName(ingredient.Key, ingredientObj);
                    if (IsMissingIngredient(ingredient.Key))
                    {
                        displayInfo += "~" + ingredientObj.Stack.ToString() + " " + displayName;
                    }
                    else
                    {
                        displayInfo += ingredientObj.Stack.ToString() + " " + displayName;
                    }

                    i++;
                    if (i < recipeIngredients.Count)
                    {
                        displayInfo += ", ";
                    }
                }
            }
            return displayInfo;
        }

        private bool IsMissingIngredient(int ingredientParentSheetIndex)
        {
            Dictionary<int, Dictionary<int, SObject>> missingIngredients = ModEntry.MissingItems.GetMissingRecipeIngredients();

            if (missingIngredients.ContainsKey(ParentSheetIndex))
            {
                return missingIngredients[ParentSheetIndex].ContainsKey(ingredientParentSheetIndex);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The key is the "parent sheet index" from CookingRecipies.json
        /// It can also be a category which means the SObject wouldn't 
        /// have the correct parent sheet index since it would be meaningless
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        private string GetIngredientDisplayName(int key, SObject ingredient)
        {
            if (IsSepcialCookingItem(key))
            {
                return GetSpecialCookingItemDisplayName(key);
            }
            else
            {
                return ingredient.DisplayName;
            }
        }

        private string GetSpecialCookingItemDisplayName(int itemCategory)
        {
            switch (itemCategory)
            {
                case Constants.FISH_CATEGORY:
                    return Utilities.GetTranslation("ANY_FISH");
                case Constants.EGG_CATEGORY:
                    return Utilities.GetTranslation("ANY_EGG");
                case Constants.MILK_CATEGORY:
                    return Utilities.GetTranslation("ANY_MILK");
                default:
                    //should never get here
                    return "Oopsies";
            }
        }

        private bool IsSepcialCookingItem(int itemCategory)
        {
            Constants constants = new Constants();
            return constants.SPECIAL_COOKING_IDS.Contains(itemCategory);
        }

        private string GetCookedItemStatsInfo()
        {
            string stats = "";

            stats += Utilities.GetTranslation("ENERGY") + ": " + GetEnergyRestored() + "\n";
            stats += Utilities.GetTranslation("HEALTH") + ": " + GetHealthRestored();

            Dictionary<string, string> buffs = GetBuffs();

            if (buffs.Count > 0)
            {
                stats += "\n" + Utilities.GetTranslation("BUFFS") + ": ";
                foreach(KeyValuePair<string, string> buffIncreasePair in buffs)
                {
                    stats += "+" + buffIncreasePair.Value + " " + buffIncreasePair.Key + ", ";
                }
                stats = stats.Substring(0, stats.Length - 2) + "\n";
                int durationMin = GetBuffDuration()[0];
                int durationSec = GetBuffDuration()[1];
                stats += Utilities.GetTranslation("DURATION") + ": " + durationMin + " " + Utilities.GetTranslation("MINUTE") + " " + durationSec + " " + Utilities.GetTranslation("SECONDS");
            }

            return stats;
        }

        private double GetHealthRestored()
        {
            int edibility = Convert.ToInt32(Game1.objectInformation[ParentSheetIndex].Split('/')[SObject.objectInfoEdibilityIndex]);

            return Math.Round(edibility * 2.5 * 0.45); //this is what it actually is, not necessarily what the tooltip shows.
        }

        private double GetEnergyRestored()
        {
            int edibility = Convert.ToInt32(Game1.objectInformation[ParentSheetIndex].Split('/')[SObject.objectInfoEdibilityIndex]);

            return Math.Round(edibility * 2.5);
        }

        private Dictionary<string, string> GetBuffs()
        {
            Dictionary<string, string> nameOfBuffs = new Dictionary<string, string>();
            string[] buffs = null;
            try
            {
                buffs = Game1.objectInformation[ParentSheetIndex].Split('/')[SObject.objectInfoBuffTypesIndex].Split(' ');
            }
            catch
            {
                ModEntry.Logger.LogRecipeBuffError(Game1.objectInformation[ParentSheetIndex]);
                return nameOfBuffs;
            }

            for (int i = 0; i < buffs.Length; ++i)
            {
                if (buffs[i] != "0" && !IsUnImplementedBuff(i))
                {
                    nameOfBuffs.Add(GetBuffNameFromIndex(i), buffs[i]);
                }
            }
            return nameOfBuffs;
        }

        private bool IsUnImplementedBuff(int index)
        {
            return index == 3 || index == 6;
        }

        private string GetBuffNameFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Utilities.GetTranslation("FARMING");
                case 1:
                    return Utilities.GetTranslation("FISHING");
                case 2:
                    return Utilities.GetTranslation("MINING");
                case 4:
                    return Utilities.GetTranslation("LUCK");
                case 5:
                    return Utilities.GetTranslation("FORAGING");
                case 7:
                    return Utilities.GetTranslation("MAX_ENERGY");
                case 8:
                    return Utilities.GetTranslation("MAGNETISM");
                case 9:
                    return Utilities.GetTranslation("SPEED");
                case 10:
                    return Utilities.GetTranslation("DEFENSE");
                case 11:
                    return Utilities.GetTranslation("ATTACK");
                default:
                    return "Oopsies";
            }
        }

        /// <summary>
        /// Gets it in minutes seconds
        /// </summary>
        /// <returns></returns>
        private int[] GetBuffDuration()
        {
            int[] result = new int[2];
            double value = Convert.ToDouble(Game1.objectInformation[ParentSheetIndex].Split('/')[SObject.objectInfoBuffDurationIndex]) * 0.7;

            result[0] = (int)value / 60;
            result[1] = (int)value % 60;
            return  result;
        }
    }
}
