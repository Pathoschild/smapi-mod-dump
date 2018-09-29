using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using AnimalHusbandryMod.common;
using StardewValley;

namespace AnimalHusbandryMod.cooking
{
    public static class CookingExtension
    {
        public static string GetDescription(this Cooking value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetRecipe(this Cooking value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(RecipeAttribute)) as RecipeAttribute;

            return attribute.Recipe;
        }

        public static string GetObjectString(this Cooking value)
        {
            var cookingItem = DataLoader.CookingData.getCookingItem(value);
            var i18n = DataLoader.i18n;
            return String.Format("{0}/{1}/{2}/Cooking -7/{3}/{4}/food/{5} {6} {7} 0 {8} {9} 0 {10} {11} {12} {13} {14}/{15}",
                value.GetDescription(), cookingItem.Price, cookingItem.Edibility,
                i18n.Get($"Cooking.{value}.Name"), i18n.Get($"Cooking.{value}.Description"),
                cookingItem.Farming, cookingItem.Fishing, cookingItem.Mining, cookingItem.Luck,
                cookingItem.Foraging, cookingItem.MaxEnergy, cookingItem.Magnetism, cookingItem.Speed,
                cookingItem.Defense, cookingItem.Attack, cookingItem.Duration);
        }

        public static string GetRecipeChannelString(this Cooking value)
        {
            var i18n = DataLoader.i18n;
            return String.Format("{0}/{1}/{2}",
                value.GetDescription(),
                i18n.Get($"Cooking.{value}.TV"),
                i18n.Get($"Cooking.{value}.Name"));
        }

        public static string GetRecipeString(this Cooking value)
        {
            var cookingItem = DataLoader.CookingData.getCookingItem(value);
            var recipeString = $"{cookingItem.Recipe}/1 10/{(int) value} {cookingItem.Amount}/default";
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en){
                recipeString += "/" + DataLoader.i18n.Get($"Cooking.{value}.Name");
            }
            return recipeString;
        }
    }
}
