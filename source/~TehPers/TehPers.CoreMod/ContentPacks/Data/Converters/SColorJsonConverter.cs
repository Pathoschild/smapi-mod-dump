using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.ContentPacks.Data.Converters {
    internal class SColorJsonConverter : JsonConverter<SColor> {
        public override void WriteJson(JsonWriter writer, SColor value, JsonSerializer serializer) {
            // TODO: Check if this is the proper way to write the token
            new JValue($"#{value.PackedValue:x8}").WriteTo(writer);
        }

        public override SColor ReadJson(JsonReader reader, Type objectType, SColor existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (!(JToken.ReadFrom(reader) is JValue token && token.Value is string value)) {
                throw new InvalidOperationException("Color must either be the name of a color, or a string in the format '#AARRGGBB'.");
            }

            // Remove excess whitespace
            value = value.Trim();

            // Try to parse it as a hex code
            if (value.StartsWith("#") && !uint.TryParse(value.Substring(1), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint packed)) {
                return new SColor(packed);
            }

            PropertyInfo colorProperty = typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(p => p.CanRead && p.PropertyType == typeof(Color) && string.Equals(p.Name, value, StringComparison.OrdinalIgnoreCase));

            if (colorProperty != null && colorProperty.GetValue(null) is Color color) {
                return color;
            }

            throw new InvalidOperationException($"Unknown color '{value}'.");
        }
    }
}