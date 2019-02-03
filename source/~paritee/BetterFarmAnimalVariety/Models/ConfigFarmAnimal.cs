using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Paritee.StardewValleyAPI.Utilities;
using System;
using System.Runtime.Serialization;

namespace BetterFarmAnimalVariety.Models
{
    public class ConfigFarmAnimal
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum VanillaCategory
        {
            [EnumMember(Value = "Dairy Cow")]
            DairyCow,
            [EnumMember(Value = "Chicken")]
            Chicken,
            [EnumMember(Value = "Sheep")]
            Sheep,
            [EnumMember(Value = "Goat")]
            Goat,
            [EnumMember(Value = "Pig")]
            Pig,
            [EnumMember(Value = "Duck")]
            Duck,
            [EnumMember(Value = "Rabbit")]
            Rabbit,
            [EnumMember(Value = "Dinosaur")]
            Dinosaur
        }

        public string Category;
        
        [JsonProperty(Order = 1)]
        public string[] Types;

        [JsonProperty(Order = 2)]
        public string[] Buildings;

        [JsonProperty(Order = 3)]
        public ConfigFarmAnimalAnimalShop AnimalShop;

        [JsonConstructor]
        public ConfigFarmAnimal()
        {
            // Do nothing; this is for loading an existing config
        }

        public ConfigFarmAnimal(AppSetting appSetting)
        {
            string[] Values = appSetting.SplitValue();
            
            this.Category = this.ConvertAppSettingCategoryToVanillaCategory(appSetting.SplitKey()[AppSetting.FARMANIMALS_CATEGORY_INDEX]);
            this.Types = appSetting.Split(Values[AppSetting.FARMANIMALS_TYPES_INDEX], AppSetting.VALUE_ARRAY_DELIMITER);
            this.Buildings = appSetting.Split(Values[AppSetting.FARMANIMALS_BUILDINGS_INDEX], AppSetting.VALUE_ARRAY_DELIMITER);
            this.AnimalShop = new ConfigFarmAnimalAnimalShop(this.Category, appSetting);
        }

        private string ConvertAppSettingCategoryToVanillaCategory(string appSettingCategory)
        {
            Array values = Enum.GetValues(typeof(ConfigFarmAnimal.VanillaCategory));

            foreach (ConfigFarmAnimal.VanillaCategory value in values)
            {
                if (appSettingCategory.Equals(value.ToString()))
                {
                    return Enums.GetValue(value);
                }
            }

            throw new Exception();
        }

        public bool ShouldSerializeCategory()
        {
            return false;
        }

        public string[] GetTypes()
        {
            return this.Types;
        }

        public string[] GetBuildings()
        {
            return this.Buildings;
        }

        public bool CanBePurchased()
        {
            return this.AnimalShop.Name != null && this.AnimalShop.Description != null && this.AnimalShop.Price != null && this.AnimalShop.Icon != null;
        }
    }
}
