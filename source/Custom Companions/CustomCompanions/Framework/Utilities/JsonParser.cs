/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Utilities
{
    public static class JsonParser
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        public static string Serialize<Model>(Model model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented, settings);
        }

        public static Model Deserialize<Model>(object json)
        {
            return Deserialize<Model>(json.ToString());
        }

        public static Model Deserialize<Model>(string json)
        {
            return JsonConvert.DeserializeObject<Model>(json, settings);
        }

        public static bool CompareSerializedObjects(object first, object second)
        {
            return JsonConvert.SerializeObject(first) == JsonConvert.SerializeObject(second);
        }

        public static object GetUpdatedModel(object original, object updated)
        {
            JObject jOriginal = JObject.Parse(original.ToString());
            foreach (var updatedProperty in JObject.Parse(updated.ToString()).Properties())
            {
                var originalProperty = jOriginal.Properties().FirstOrDefault(p => p.Name == updatedProperty.Name);
                if (originalProperty != null)
                {
                    originalProperty.Value = updatedProperty.Value;
                }
                else
                {
                    jOriginal.Add(updatedProperty);
                }
            }

            return jOriginal.ToString();
        }
    }
}
