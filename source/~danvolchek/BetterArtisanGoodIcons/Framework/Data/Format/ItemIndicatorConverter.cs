/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;
using Newtonsoft.Json;

namespace BetterArtisanGoodIcons.Framework.Data.Format
{
    internal class ItemIndicatorConverter : JsonConverter<ItemIndicator>
    {
        public override void WriteJson(JsonWriter writer, ItemIndicator value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override ItemIndicator ReadJson(JsonReader reader, Type objectType, ItemIndicator existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return new ItemIndicator((string)reader.Value);
                case JsonToken.Integer:
                    return new ItemIndicator(int.Parse(reader.Value.ToString()));
                default:
                    throw new JsonSerializationException($"Invalid value for item indicator: {reader.Value}.");
            }
        }
    }
}
