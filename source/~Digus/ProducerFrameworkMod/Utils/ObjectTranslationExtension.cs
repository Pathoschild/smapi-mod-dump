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
using ProducerFrameworkMod.ContentPack;
using SObject = StardewValley.Object;

namespace ProducerFrameworkMod.Utils
{
    internal static class ObjectTranslationExtension
    {
        private const string OutputTranslationKey = "Digus.ProducerFrameworkMod/OutputTranslationKey";
        private const string OutputGenericParentNameTranslationKey = "Digus.ProducerFrameworkMod/OutputGenericParentNameTranslationKey";
        private const string ContentPackUniqueID = "Digus.ProducerFrameworkMod/ContentPackUniqueID";

        public static readonly Dictionary<string, ITranslationHelper> TranslationHelpers = new Dictionary<string, ITranslationHelper>();

        internal static void AddCustomName(this SObject obj, OutputConfig outputConfig)
        {
            obj.modData[OutputTranslationKey] = outputConfig.OutputTranslationKey ?? outputConfig.OutputName;
        }

        internal static void AddGenericParentName(this SObject obj, OutputConfig outputConfig)
        {
            obj.modData[OutputGenericParentNameTranslationKey] = outputConfig.OutputGenericParentNameTranslationKey ?? outputConfig.OutputGenericParentName;
        }

        internal static void AddContentPackUniqueID(this SObject obj, OutputConfig outputConfig)
        {
            obj.modData[ContentPackUniqueID] = outputConfig.ModUniqueID;
        }

        internal static string GetCustomName(this SObject obj)
        {
            if (obj.GetKey(OutputTranslationKey) is string key && GetKey(obj, ContentPackUniqueID) is string id)
            {
                string translation = TranslationHelpers[id].Get(key);
                return !translation.Contains("(no translation:") ? translation : key;
            }
            return null;
        }

        internal static string GetGenericParentName(this SObject obj)
        {
            if (obj.GetKey(OutputGenericParentNameTranslationKey) is string key && GetKey(obj, ContentPackUniqueID) is string id)
            {
                string translation = TranslationHelpers[id].Get(key);
                return !translation.Contains("(no translation:") ? translation : key;
            }
            return null;
        }

        private static string GetKey(this SObject obj, string key)
        {
            return obj.modData.TryGetValue(key, out string value) ? value : null;
        }
    }
}
