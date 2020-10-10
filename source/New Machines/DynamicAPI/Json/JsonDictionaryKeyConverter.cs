/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Json
{
    public class JsonDictionaryKeyConverter : JsonConverter
    {
        private JsonConverter KeyJsonConverter { get; }

        public JsonDictionaryKeyConverter(JsonConverter keyJsonConverter)
        {
            KeyJsonConverter = keyJsonConverter;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var dictionary = (IDictionary)value;
            var valueType = value.GetType().GenericTypeArguments.Last();

            writer.WriteStartObject();
            foreach (DictionaryEntry kv in dictionary)
            {
                KeyJsonConverter.WriteJson(writer, kv.Key, serializer);
                serializer.Serialize(writer, kv.Value, valueType);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var dictionary = (IDictionary)Activator.CreateInstance(objectType);
            var keyType = objectType.GenericTypeArguments.First();
            var valueType = objectType.GenericTypeArguments.Last();
            while (true)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndObject) break;
                var key = KeyJsonConverter.ReadJson(reader, keyType, null, serializer);
                reader.Read();
                var value = serializer.Deserialize(reader, valueType);
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsImplementGenericInterface(typeof(IDictionary<,>))
                && KeyJsonConverter.CanConvert(objectType.GenericTypeArguments.First());
        }


    }
}