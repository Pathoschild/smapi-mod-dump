/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;
using System.IO;

namespace BetterFarmAnimalVariety.Models
{
    public class ConfigFarmAnimalAnimalShop
    {
        public const string NOT_APPLICABLE = "null";
        public const string PRICE_PLACEHOLDER = "1000";

        public string DefaultNameStringID;
        public string DefaultDescriptionStringID;
        public string DefaultPrice;

        [JsonIgnore]
        public string Category;

        [JsonIgnore]
        public string ConfigName;

        [JsonIgnore]
        public string ConfigDescription;

        [JsonIgnore]
        public string ConfigIcon;

        [JsonIgnore]
        public string ConfigPrice;

        [JsonProperty(Order = 1)]
        public string Name
        {
            get
            {
                if (this.ConfigName != null)
                {
                    return this.ConfigName;
                }

                if (this.DefaultNameStringID == null || this.DefaultNameStringID == ConfigFarmAnimalAnimalShop.NOT_APPLICABLE)
                {
                    return null;
                }

                // Pull it from the data
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.DetermineNameStringKey());
            }
            set
            {
                this.ConfigName = value;
            }
        }

        [JsonProperty(Order = 2)]
        public string Description
        {
            get
            {
                if (this.ConfigDescription != null)
                {
                    return this.ConfigDescription;
                }

                if (this.DefaultDescriptionStringID == null || this.DefaultDescriptionStringID == ConfigFarmAnimalAnimalShop.NOT_APPLICABLE)
                {
                    return null;
                }

                // Pull it from the data
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.DetermineDescriptionStringKey());
            }
            set
            {
                this.ConfigDescription = value;
            }
        }

        [JsonProperty(Order = 3)]
        public string Price
        {
            get
            {
                if (this.ConfigPrice != null)
                {
                    return this.ConfigPrice;
                }

                if (this.DefaultPrice == null || this.DefaultPrice == ConfigFarmAnimalAnimalShop.NOT_APPLICABLE)
                {
                    return null;
                }

                return this.DefaultPrice;
            }
            set
            {
                this.ConfigPrice = value;
            }
        }

        [JsonProperty(Order = 4)]
        public string Icon
        {
            get
            {
                if (this.ConfigIcon != null)
                {
                    return this.ConfigIcon;
                }

                if (this.DefaultPrice == null || this.DefaultPrice == ConfigFarmAnimalAnimalShop.NOT_APPLICABLE)
                {
                    return null;
                }

                return this.GetDefaultIconPath();
            }
            set
            {
                this.ConfigIcon = value;
            }
        }

        [JsonConstructor]
        public ConfigFarmAnimalAnimalShop()
        {
            // Do nothing; this is for loading an existing config
        }

        public ConfigFarmAnimalAnimalShop(string category, AppSetting appSetting)
        {
            string[] Values = appSetting.SplitValue();

            this.Category = category;
            this.DefaultNameStringID = Values[AppSetting.FARMANIMALS_ANIMAL_SHOP_NAME_ID_INDEX];
            this.DefaultDescriptionStringID = Values[AppSetting.FARMANIMALS_ANIMAL_SHOP_DESCRIPTION_ID_INDEX];
            this.DefaultPrice = Values[AppSetting.FARMANIMALS_ANIMAL_SHOP_PRICE_INDEX];
        }

        public bool ShouldSerializeCategory()
        {
            return false;
        }

        public bool ShouldSerializeDefaultNameStringID()
        {
            return false;
        }

        public bool ShouldSerializeDefaultDescriptionStringID()
        {
            return false;
        }

        public bool ShouldSerializeDefaultPrice()
        {
            return false;
        }

        public string DetermineNameStringKey()
        {
            return "Utility.cs." + this.DefaultNameStringID;
        }

        public string DetermineDescriptionStringKey()
        {
            return "PurchaseAnimalsMenu.cs." + this.DefaultDescriptionStringID;
        }

        public string GetDescriptionPlaceholder()
        {
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:BluePrint.cs.1");
        }

        public string GetDefaultIconPath()
        {
            return Path.Combine(Properties.Settings.Default.AssetsDirectory, "animal_shop_" + this.Category.ToLower() + ".png");
        }
    }
}
