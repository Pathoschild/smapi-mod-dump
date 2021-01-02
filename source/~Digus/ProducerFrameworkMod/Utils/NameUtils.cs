/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace ProducerFrameworkMod.Utils
{
    internal class NameUtils
    {
        private static readonly Dictionary<int, string> CustomNames = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> GenericParentNames = new Dictionary<int, string>();

        private static LocalizedContentManager.LanguageCode? _activeLanguageCode = null;

        internal static void AddCustomName(int index, string value)
        {
            AddName(index, value, CustomNames);
        }

        internal static void AddGenericParentName(int index, string value)
        {
            AddName(index, value, GenericParentNames);
        }

        private static void AddName(int index, string value, Dictionary<int, string> nameDictionary)
        {
            if (Game1.content.GetCurrentLanguage() != _activeLanguageCode)
            {
                _activeLanguageCode = Game1.content.GetCurrentLanguage();
                nameDictionary.Clear();
            }

            if (!nameDictionary.ContainsKey(index))
            {
                nameDictionary[index] = value;
            }
            else if (nameDictionary[index] != value)
            {
                if (nameDictionary == CustomNames)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log(
                        $"The custom name '{nameDictionary[index]}' is already in use for the object with the index '{index}'. The custom name '{value}' will be ignored."
                        , LogLevel.Warn);
                } else if (nameDictionary == GenericParentNames)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log(
                        $"The generic parent name '{nameDictionary[index]}' is already in use for the object with the index '{index}'. The generic parent name '{value}' will be ignored."
                        , LogLevel.Warn);
                }
            }
        }

        internal static string GetCustomNameFromIndex(int index)
        {
            return CustomNames[index];
        }

        internal static bool HasCustomNameForIndex(int index)
        {
            return CustomNames.ContainsKey(index);
        }

        internal static string GetGenericParentNameFromIndex(int index)
        {
            return GenericParentNames[index];
        }

        internal static bool HasGenericParentNameForIndex(int index)
        {
            return GenericParentNames.ContainsKey(index);
        }
    }
}
