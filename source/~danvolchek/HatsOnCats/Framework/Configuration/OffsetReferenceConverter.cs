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
using System.IO;
using Newtonsoft.Json.Linq;

namespace HatsOnCats.Framework.Configuration
{
    internal class OffsetReferenceConverter : JsonConverter<OffsetReference>
    {
        public override void WriteJson(JsonWriter writer, OffsetReference value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override OffsetReference ReadJson(JsonReader reader, Type objectType, OffsetReference existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!(reader.Value is string value))
            {
                throw new JsonSerializationException($"Invalid value for config reference: {reader.Value}.");
            }

            value = value.Trim();

            if (value[0] != '!')
            {
                throw new JsonSerializationException("Config reference must start with a '!'.");
            }

            return new OffsetReference(new JValue(value.Substring(1)).ToObject<Frame>());
        }
    }
}
