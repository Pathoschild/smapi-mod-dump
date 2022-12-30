/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AeroCore.Framework
{
    public class ColorConverter : JsonConverter
    {
        private Type SParseException = Reflection.TypeNamed("StardewModdingAPI.Toolkit.Serialization.SParseException");
        public override bool CanConvert(Type objectType)
            => (Nullable.GetUnderlyingType(objectType) ?? objectType) == typeof(Color);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null when Nullable.GetUnderlyingType(objectType) is not null:
                    return null;
                case JsonToken.StartObject:
                    var obj = JObject.Load(reader);
                    int r = obj.ValueIgnoreCase<int>(nameof(Color.R));
                    int g = obj.ValueIgnoreCase<int>(nameof(Color.G));
                    int b = obj.ValueIgnoreCase<int>(nameof(Color.B));
                    int a = obj.ValueIgnoreCase<int>(nameof(Color.A));
                    return new Color(r, g, b, a);
                case JsonToken.String:
                    var s = JToken.Load(reader).Value<string>();
                    if (s.TryParseColor(out var c))
                        return c;
                    throw (Exception)SParseException.New($"Failed to parse color string '{s}' @ '{reader.Path}'");
                case JsonToken.Integer:
                    var i = (uint)JToken.Load(reader).Value<long>();
                    return new Color(i);
                case JsonToken.StartArray:
                    var arr = JArray.Load(reader);
                    if (arr.Count < 3)
                        throw (Exception)SParseException.New($"Failed to parse color from array; a minimum of 3 channels is required @ '{reader.Path}'");
                    if (arr.Count < 4)
                        return new Color(
                            arr[0].ToObject<int>(),
                            arr[1].ToObject<int>(),
                            arr[2].ToObject<int>()
                        );
                    else
                        return new Color(
                            arr[0].ToObject<int>(),
                            arr[1].ToObject<int>(),
                            arr[2].ToObject<int>(),
                            arr[3].ToObject<int>()
                        );
                default:
                    throw (Exception)SParseException.New($"Failed to parse color from token type '{reader.TokenType}' @ '{reader.Path}'");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color)value;
            writer.WriteValue($"{c.R}, {c.G}, {c.B}, {c.A}");
        }
    }
}
