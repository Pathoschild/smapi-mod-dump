/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Json
{
    internal class NamespacedKeyJsonConverter : JsonConverter<NamespacedKey>
    {
        public override void WriteJson(
            JsonWriter writer,
            NamespacedKey value,
            JsonSerializer serializer
        )
        {
            writer.WriteValue(value.ToString());
        }

        public override NamespacedKey ReadJson(
            JsonReader reader,
            Type objectType,
            NamespacedKey existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.Value is not string raw || !NamespacedKey.TryParse(raw, out var key))
            {
                throw new JsonException(
                    "Expected colon-delimited string in the format 'namespace:key'."
                );
            }

            return key;
        }
    }
}