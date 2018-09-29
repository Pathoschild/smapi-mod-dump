using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Igorious.StardewValley.DynamicApi2.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Igorious.StardewValley.DynamicApi2.Extensions
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
        }
    }
}