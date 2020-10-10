/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace HatsOnCats.Framework.Configuration
{
    internal class OffsetConverter : JsonConverter<Offset>
    {
        public override void WriteJson(JsonWriter writer, Offset value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.IsReference ? value.Reference : (object)value.Value);
        }

        public override Offset ReadJson(JsonReader reader, Type objectType, Offset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                OffsetReference value = serializer.Deserialize<OffsetReference>(reader);
                return new Offset(value);
            }
            catch
            {
                try
                {
                    Vector2 value = serializer.Deserialize<Vector2>(reader);
                    return new Offset(value);
                }
                catch (JsonSerializationException exception)
                {
                    throw new JsonReaderException($"Error converting value \"{reader.Value}\" to one of type '{typeof(Vector2)}' or '{typeof(OffsetReference)}'. Path '{exception.Path}', line {exception.LineNumber}, position {exception.LinePosition}.");
                }
            }
        }
    }
}
