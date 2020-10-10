/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    public class CustomTranslator
    {
        private static IDictionary<String, IDictionary<String, String>> translations;
        static CustomTranslator()
        {
            translations = new Dictionary<String, IDictionary<String, String>>();
            String translationsPath = Path.Combine(ModEntry.modHelper.DirectoryPath, "i18n");
            string[] fileEntries = Directory.GetFiles(translationsPath);
            foreach (String filePath in fileEntries)
            {
                String fileName = Path.GetFileName(filePath);
                if (fileName.Contains(".json") && !fileName.Contains("backup"))
                {
                    String localeName = fileName.Replace(".json", "");
                    IDictionary<String, String> localeValues = ModEntry.modHelper.Data.ReadJsonFile<IDictionary<String, String>>("i18n/" + fileName);
                    translations.Add(localeName, localeValues);
                }
            }
        }

        public static String getTranslation(String locale, String parameter)
        {
            return translations[locale][parameter];
        }

        public static IDictionary<String, String> getAllTranslationsByLocales(String parameter)
        {
            IDictionary<String, String> result = new Dictionary<String, String>();
            foreach (String locale in translations.Keys)
            {
                if (locale != "default")
                {
                    result.Add(locale, getTranslation(locale, parameter));
                }
            }

            return result;
        }
    }
}
