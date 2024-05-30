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
using StardewValley;

namespace DeluxeJournal.Framework.Serialization
{
    internal class WorldDateConverter : JsonConverter<WorldDate>
    {
        public override WorldDate ReadJson(JsonReader reader, Type objectType, WorldDate? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            string season = json["Season"]?.Value<string>() ?? "spring";
            int day = json["Day"]?.Value<int>() ?? 1;
            int year = json["Year"]?.Value<int>() ?? 1;

            return new WorldDate(year, season, day);
        }

        public override void WriteJson(JsonWriter writer, WorldDate? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Season");
            writer.WriteValue(value?.SeasonKey ?? "spring");
            writer.WritePropertyName("Day");
            writer.WriteValue(value?.DayOfMonth ?? 1);
            writer.WritePropertyName("Year");
            writer.WriteValue(value?.Year ?? 1);
            writer.WriteEndObject();
        }
    }
}
