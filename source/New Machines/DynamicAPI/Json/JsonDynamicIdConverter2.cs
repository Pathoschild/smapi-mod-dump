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
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Json
{
    public sealed class JsonDynamicIdConverter<TEnum1, TEnum2> : JsonConverter
    {
        private static Dictionary<string, int> NameToInt { get; } = new Dictionary<string, int>();
        private static Dictionary<int, string> IntToName { get; } = new Dictionary<int, string>();

        static JsonDynamicIdConverter()
        {
            var names1 = Enum.GetNames(typeof(TEnum1));
            var values1 = (TEnum1[])Enum.GetValues(typeof(TEnum1));
            for (var i = 0; i < values1.Length; ++i)
            {
                IntToName.Add(Convert.ToInt32(values1[i]), names1[i]);
                NameToInt.Add(names1[i].ToLower(), Convert.ToInt32(values1[i]));
            }

            var names2 = Enum.GetNames(typeof(TEnum2));
            var values2 = (TEnum2[])Enum.GetValues(typeof(TEnum2));
            for (var i = 0; i < values2.Length; ++i)
            {
                if (string.Equals(names2[i], "Undefined")) continue;
                IntToName.Add(Convert.ToInt32(values2[i]), names2[i]);
                NameToInt.Add(names2[i].ToLower(), Convert.ToInt32(values2[i]));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var intValue = Convert.ToInt32(value);
            string name;
            if (IntToName.TryGetValue(intValue, out name))
            {
                if (writer.WriteState == WriteState.Object)
                {
                    writer.WritePropertyName(name);
                }
                else
                {
                    writer.WriteValue(name);
                }
            }
            else
            {
                if (writer.WriteState == WriteState.Object)
                {
                    writer.WritePropertyName(intValue.ToString());
                }
                else
                {
                    writer.WriteValue(intValue);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var converter = objectType.GetMethod("op_Implicit", new[] { typeof(int) });

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                {
                    var intValue = Convert.ToInt32(reader.Value);
                    return converter.Invoke(null, new object[] { intValue });
                }
                case JsonToken.PropertyName:
                case JsonToken.String:
                {
                    int intValue;
                    if (!int.TryParse(reader.Value.ToString(), out intValue))
                    {
                        intValue = NameToInt[reader.Value.ToString().ToLower()];
                    }
                    return converter.Invoke(null, new object[] { intValue });
                }
                default: return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType 
                && objectType.GetGenericTypeDefinition() == typeof(DynamicID<,>)
                && objectType.GenericTypeArguments.First() == typeof(TEnum1)
                && objectType.GenericTypeArguments.Last() == typeof(TEnum2);
        }
    }
}
