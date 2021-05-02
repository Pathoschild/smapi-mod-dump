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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PyTK.CustomElementHandler;

namespace Revitalize.Framework.Utilities.Serialization.Converters
{
    public class CustomObjectDataConverter: JsonConverter
    {
        JsonSerializerSettings settings;

        public CustomObjectDataConverter()
        {
            this.settings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new Framework.Utilities.Serialization.Converters.RectangleConverter(),
                    new Framework.Utilities.Serialization.Converters.Texture2DConverter(),
                    new Vector2Converter(),
                },
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<PropertyInfo> properties = value.GetType().GetProperties().ToList();
            List<FieldInfo> fields = value.GetType().GetFields().ToList();

            writer.WriteStartObject();
            
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyInfo p = properties[i];
                writer.WritePropertyName(p.Name);
                serializer.Serialize(writer, p.GetValue(value) != null ? ModCore.Serializer.ToJSONString(p.GetValue(value)) : null);
            }


            foreach (FieldInfo f in fields)
            {
                writer.WritePropertyName(f.Name);
                serializer.Serialize(writer, f.GetValue(value) != null ? ModCore.Serializer.ToJSONString(f.GetValue(value)) : null);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {

                throw new JsonSerializationException("Cant convert to item!");
                return null;
            }

            JObject jo = JObject.Load(reader);

            string id = jo["id"].Value<string>();
            string texture = jo["texture"].Value<string>();
            string type = jo["type"].Value<string>();
            string color = jo["color"].Value<string>();
            string bigcraftable = jo["bigCraftable"].Value<string>();

            Texture2D tex=ModCore.Serializer.DeserializeFromJSONString<Texture2D>(texture);
            Type t = Type.GetType(type);
            Color c = ModCore.Serializer.DeserializeFromJSONString<Color>(color);
            bool craftable = ModCore.Serializer.DeserializeFromJSONString<bool>(bigcraftable);

            return PyTKHelper.CreateOBJData(id, tex, t, c, craftable);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CustomObjectData);
        }
    }
}
