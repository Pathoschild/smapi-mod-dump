/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Effects
{
    /// <summary>
    /// Converts <see cref="FishingEffectEntry"/> subclasses.
    /// </summary>
    internal class EffectEntryJsonConverter : JsonConverter
    {
        private readonly Func<IEnumerable<FishingEffectRegistration>> registrationsFactory;

        public EffectEntryJsonConverter(
            Func<IEnumerable<FishingEffectRegistration>> registrationsFunc
        )
        {
            this.registrationsFactory = registrationsFunc
                ?? throw new ArgumentNullException(nameof(registrationsFunc));
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, value?.GetType());
        }

        /// <inheritdoc />
        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            var effectToken = JObject.Load(reader);
            if (!effectToken.TryGetValue(
                    "$Effect",
                    StringComparison.OrdinalIgnoreCase,
                    out var effectNameToken
                ))
            {
                throw new JsonException("Missing '$Effect'.");
            }

            if (effectNameToken.Type != JTokenType.String)
            {
                throw new JsonException("'$Effect' value must be a string");
            }

            var effectName = (string)effectNameToken!;
            var effectType = this.registrationsFactory()
                .FirstOrDefault(
                    registration => string.Equals(
                        registration.Name,
                        effectName,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                ?.EntryType;
            if (effectType is null)
            {
                throw new JsonException($"Unknown effect: '{effectName}'");
            }

            var readJson = effectToken.ToObject(effectType, serializer);
            return readJson;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FishingEffectEntry);
        }
    }
}
