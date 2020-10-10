/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using System.Collections.Generic;

namespace BetterFarmAnimalVariety.Models
{
    public class AppSetting
    {
        public const char KEY_DELIMITER = '_';
        public const char VALUE_DELIMITER = '/';
        public const char VALUE_ARRAY_DELIMITER = ',';

        //Keys
        public const string FARMANIMALS = "FarmAnimals";
        public const byte FARMANIMALS_CATEGORY_INDEX = 1;

        // Values
        public const byte FARMANIMALS_ANIMAL_SHOP_ORDER = 0;
        public const byte FARMANIMALS_ANIMAL_SHOP_NAME_ID_INDEX = 1;
        public const byte FARMANIMALS_ANIMAL_SHOP_DESCRIPTION_ID_INDEX = 2;
        public const byte FARMANIMALS_ANIMAL_SHOP_PRICE_INDEX = 3;
        public const byte FARMANIMALS_TYPES_INDEX = 4;
        public const byte FARMANIMALS_BUILDINGS_INDEX = 5;

        public string Key;
        public string Value;

        public AppSetting(KeyValuePair<string, string> setting)
        {
            this.Key = setting.Key;
            this.Value = setting.Value;
        }

        public bool IsFarmAnimal()
        {
            return this.StartsWith(this.Key, AppSetting.FARMANIMALS);
        }

        private bool StartsWith(string value, string @string)
        {
            return value.StartsWith(@string);
        }

        public string[] Split(string value, char delimiter)
        {
            return value.Split(delimiter);
        }

        public string[] SplitValue()
        {
            return this.Split(this.Value, AppSetting.VALUE_DELIMITER);
        }

        public string[] SplitKey()
        {
            return this.Split(this.Key, AppSetting.KEY_DELIMITER);
        }
    }
}
