/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation
{
    /// <summary>
    /// A list of category names for objects.
    /// </summary>
    public static class CategoryNames
    {
        /// <summary>
        /// The category name for crafting station.
        /// </summary>
        public static string Crafting
        {
            get
            {
                return GetCategoryName("Crafting", "Crafting");
            }
        }

        public static string Farming
        {
            get
            {
                return GetCategoryName("Farming", "Farming");
            }
        }

        /// <summary>
        /// The category name for machines.
        /// </summary>
        public static string Machine
        {
            get
            {
                return GetCategoryName("Machine", "Machine");
            }
        }

        /// <summary>
        /// The category for miscelaneous objects.
        /// </summary>
        public static string Misc
        {
            get
            {
                return GetCategoryName("Misc", "Misc");
            }
        }

        /// <summary>
        /// The category name for Ore.
        /// </summary>
        public static string Ore
        {
            get
            {
                return GetCategoryName("Ore", "Ore");
            }
        }

        /// <summary>
        /// The category name for resources.
        /// </summary>
        public static string Resource
        {
            get
            {
                return GetCategoryName("Resource", "Resource");
            }
        }

        /// <summary>
        /// Gets a category name from a .json file.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultName"></param>
        /// <returns></returns>
        public static string GetCategoryName(string Key, string DefaultName)
        {
            string catrgoryName= JsonUtilities.LoadStringFromDictionaryFile(Key, LocalizationUtilities.GetLocalizationFilePath(Path.Combine(StringsPaths.Objects, "CategoryNames.json")));
            if (string.IsNullOrEmpty(catrgoryName))
            {
                return DefaultName;
            }
            return catrgoryName;
        }
    }
}
