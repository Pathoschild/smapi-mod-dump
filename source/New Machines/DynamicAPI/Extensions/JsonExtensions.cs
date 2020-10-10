/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object o)
        {
            var buffer = new StringBuilder();
            using (var sw = new StringWriter(buffer))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.QuoteName = false;
                var ser = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented };
                ser.Converters.AddDefaults();
                ser.Serialize(writer, o);
            }
            var result = buffer.ToString();
            result = Regex.Replace(result, @"\{\r\n\s*(.+)\r\n\s*\}", @"{ $1 }");
            result = Regex.Replace(result, @"(\r\n\s*[\]\}])", @",$1");
            return result;
        }

        public static void AddDefaults(this IList<JsonConverter> converters)
        {
            converters.Add(new StringEnumConverter());
            converters.Add(new JsonPlainArrayConverter());
            converters.Add(new JsonDynamicIdConverter<ItemID>());
            converters.Add(new JsonDynamicIdConverter<CraftableID>());
            converters.Add(new JsonDynamicIdConverter<CategoryID>());
            converters.Add(new JsonDynamicIdConverter<ItemID, CategoryID>());
            converters.Add(new JsonDictionaryKeyConverter(new JsonDynamicIdConverter<ItemID>()));
            converters.Add(new JsonDictionaryKeyConverter(new JsonDynamicIdConverter<CraftableID>()));
            converters.Add(new JsonDictionaryKeyConverter(new JsonDynamicIdConverter<CategoryID>()));
            converters.Add(new JsonDictionaryKeyConverter(new JsonDynamicIdConverter<ItemID, CategoryID>()));
        }
    }
}
