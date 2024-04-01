/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using AnimalHusbandryMod.common;
using StardewValley;
using AnimalHusbandryMod.meats;
using StardewValley.GameData.Objects;
using DataLoader = AnimalHusbandryMod.common.DataLoader;
using StardewValley.GameData.Buffs;

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

        public static ObjectData GetObjectData(this Cooking value)
        {
            var cookingItem = DataLoader.CookingData.getCookingItem(value);
            var i18n = DataLoader.i18n;
            return new ObjectData()
            {
                SpriteIndex = (int)value,
                Name = value.GetDescription(),
                Price = cookingItem.Price,
                Edibility = cookingItem.Edibility,
                DisplayName = i18n.Get($"Cooking.{value}.Name"),
                Description = i18n.Get($"Cooking.{value}.Description"),
                Category = -7,
                Type = "Cooking",
                Buffs = new List<ObjectBuffData>
                {
                    new ObjectBuffData()
                    {
                        Duration = cookingItem.Duration,
                        CustomAttributes =  new BuffAttributesData()
                        {
                            FarmingLevel = cookingItem.Farming,
                            FishingLevel = cookingItem.Fishing,
                            MiningLevel = cookingItem.Mining,
                            LuckLevel = cookingItem.Luck,
                            ForagingLevel = cookingItem.Foraging,
                            MaxStamina = cookingItem.MaxEnergy,
                            MagneticRadius = cookingItem.Magnetism,
                            Speed = cookingItem.Speed,
                            Defense = cookingItem.Defense,
                            Attack = cookingItem.Attack
                        }
                    }
                }
            };
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
            var recipeString = $"{cookingItem.Recipe}/1 10/{(int) value} {cookingItem.Amount}/null";
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en){
                recipeString += "/" + DataLoader.i18n.Get($"Cooking.{value}.Name");
            }
            return recipeString;
        }
    }
}
