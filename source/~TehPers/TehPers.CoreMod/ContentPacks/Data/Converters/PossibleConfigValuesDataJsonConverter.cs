using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehPers.CoreMod.ContentPacks.Data.Converters {
    internal class PossibleConfigValuesDataJsonConverter : JsonConverter<PossibleConfigValuesData> {
        public override void WriteJson(JsonWriter writer, PossibleConfigValuesData value, JsonSerializer serializer) {
            JArray.FromObject(value.PossibleValues).WriteTo(writer);
        }

        public override PossibleConfigValuesData ReadJson(JsonReader reader, Type objectType, PossibleConfigValuesData existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JToken token = JToken.ReadFrom(reader);

            switch (token) {
                case JValue valueToken when valueToken.Value is string stringValue:
                    return new PossibleConfigValuesData(stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray());
                case JArray arrayToken:
                    JToken[] invalidValues = arrayToken.Where(t => (t as JValue)?.Value is string).ToArray();

                    if (invalidValues.Any()) {
                        throw new InvalidOperationException($"Values must be strings. Invalid tokens: {string.Join(", ", invalidValues.Select(t => t.ToString()))}");
                    }

                    return new PossibleConfigValuesData(arrayToken.Cast<JValue>().Select(t => t.Value).Cast<string>().ToArray());
                default:
                    throw new InvalidOperationException($"Unexpected token: {token}");
            }
        }
    }
}