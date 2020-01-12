using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace ProducerFrameworkMod
{
    internal class NameUtils
    {
        private static readonly Dictionary<int, string> CustomNames = new Dictionary<int, string>();

        private static LocalizedContentManager.LanguageCode? _activeLanguageCode = null;

        internal static void AddCustomName(int index, string value)
        {
            if (Game1.content.GetCurrentLanguage() != _activeLanguageCode)
            {
                _activeLanguageCode = Game1.content.GetCurrentLanguage();
                CustomNames.Clear();
            }

            if (!CustomNames.ContainsKey(index))
            {
                CustomNames[index] = value;
            }
            else if (CustomNames[index] != value)
            {
                ProducerFrameworkModEntry.ModMonitor.Log($"The custom name '{CustomNames[index]}' is already in use for the object with the index '{index}'. The custom name '{value}' will be ignored.",LogLevel.Warn);
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
    }
}
