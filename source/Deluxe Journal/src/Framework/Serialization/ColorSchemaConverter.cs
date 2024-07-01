/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Serialization
{
    internal class ColorSchemaConverter : JsonConverter<ColorSchema>
    {
        public override ColorSchema ReadJson(JsonReader reader, Type objectType, ColorSchema? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);

            return new(json[nameof(ColorSchema.Main)]?.Value<string>(),
                json[nameof(ColorSchema.Hover)]?.Value<string>(),
                json[nameof(ColorSchema.Header)]?.Value<string>(),
                json[nameof(ColorSchema.Accent)]?.Value<string>(),
                json[nameof(ColorSchema.Shadow)]?.Value<string>(),
                json[nameof(ColorSchema.Padding)]?.Value<string>(),
                json[nameof(ColorSchema.Corner)]?.Value<string>());
        }

        public override void WriteJson(JsonWriter writer, ColorSchema? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(ColorSchema.Main));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Main, value?.Main.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Hover));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Hover, value?.Hover.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Header));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Header, value?.Header.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Accent));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Accent, value?.Accent.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Shadow));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Shadow, value?.Shadow.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Padding));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Padding, value?.Padding.A < 255));
            writer.WritePropertyName(nameof(ColorSchema.Corner));
            writer.WriteValue(ColorSchema.ColorToHex(value?.Corner, value?.Corner.A < 255));
            writer.WriteEndObject();
        }
    }
}
