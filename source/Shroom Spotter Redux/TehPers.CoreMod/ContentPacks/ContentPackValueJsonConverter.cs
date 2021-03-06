/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;
using TehPers.CoreMod.ContentPacks.Values;

namespace TehPers.CoreMod.ContentPacks {
    internal class ContentPackValueJsonConverter : JsonConverter {
        private static readonly HashSet<Type> _convertibleTypes = new HashSet<Type> { typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(short), typeof(int), typeof(long), typeof(sbyte), typeof(float), typeof(string), typeof(object), typeof(ushort), typeof(uint), typeof(ulong) };

        private readonly TokenParser _tokenParser;
        private readonly MethodInfo _readJsonMethod = typeof(ContentPackValueJsonConverter).GetMethod(nameof(ReadJson), BindingFlags.NonPublic | BindingFlags.Instance);

        public ContentPackValueJsonConverter(TokenParser tokenParser) {
            this._tokenParser = tokenParser;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            Type valueType = objectType.GenericTypeArguments[0];
            return this._readJsonMethod.MakeGenericMethod(valueType).Invoke(this, new object[] { reader, serializer });
        }

        private IContentPackValueProvider<T> ReadJson<T>(JsonReader reader, JsonSerializer serializer) {
            // Check if the current value is a string
            if (reader.Value is string tokenizedString) {
                // Make sure T is a type that can be converted to
                if (ContentPackValueJsonConverter._convertibleTypes.Contains(typeof(T))) {
                    return new TokenizedContentPackValueProvider<T>(tokenizedString, this._tokenParser);
                }

                // Couldn't convert it
                throw new InvalidOperationException($"Cannot convert a tokenized string to {typeof(T).FullName}");
            }

            // Check if the current value is an array
            if (reader.TokenType == JsonToken.StartArray) {
                return this.CreateConditionalValue<T>(reader);
            }

            // Check if the current value can be parsed as T
            if (serializer.Deserialize<T>(reader) is T result) {
                return new StaticContentPackValueProvider<T>(result);
            }

            // Failed to parse
            throw new InvalidOperationException("Could not parse the given value.");
        }

        private ConditionalContentPackValueProvider<T> CreateConditionalValue<T>(JsonReader reader) {
            // Read an array
            JArray array = JArray.Load(reader);

            // Make sure there are any cases
            if (!array.Any()) {
                throw new InvalidOperationException("A conditional value must contain at least one case");
            }

            // Construct each case
            List<ConditionalContentPackValueProvider<T>.ConditionalCase> conditionalCases = new List<ConditionalContentPackValueProvider<T>.ConditionalCase>();
            for (int i = 0; i < array.Count - 1; i++) {
                (JObject curCase, IContentPackValueProvider<T> valueProvider) = GetCaseAndValue(array[i]);

                //new ConditionalContentPackValueProvider<T>.UnconditionalCase(valueProvider)
                if (curCase.TryGetValue("When", StringComparison.OrdinalIgnoreCase, out JToken whenToken)) {
                    // Convert the whenToken into a conditional case
                    ConditionalContentPackValueProvider<T>.ConditionalCase @case = whenToken.Match<JToken, ConditionalContentPackValueProvider<T>.ConditionalCase>()
                        .When<JObject>(whenObject => new ConditionalContentPackValueProvider<T>.ConditionalCase(valueProvider, this._tokenParser, CreateConditionGroup(whenObject).Yield()))
                        .When<JArray>(whenArray => {
                            List<Dictionary<string, string>> conditionGroups = new List<Dictionary<string, string>>();

                            // Create a condition group out of each array item
                            foreach (JToken conditionGroupToken in whenArray) {
                                if (!(conditionGroupToken is JObject conditionGroupObject)) {
                                    throw new InvalidOperationException("Each group of conditions in a 'When' clause must be an object.");
                                }

                                conditionGroups.Add(CreateConditionGroup(conditionGroupObject));
                            }

                            return new ConditionalContentPackValueProvider<T>.ConditionalCase(valueProvider, this._tokenParser, conditionGroups);
                        })
                        .ElseThrow(_ => new InvalidOperationException("The 'When' part of a conditional value must be either an object or an array of objects."));

                    // Add the conditional case
                    conditionalCases.Add(@case);
                } else {
                    // An unconditional case randomly in the array
                    conditionalCases.Add(new ConditionalContentPackValueProvider<T>.ConditionalCase(valueProvider, this._tokenParser, new Dictionary<string, string>[0]));
                }
            }

            // Get the case object and value provider for the unconditional case
            (JObject unconditionalCase, IContentPackValueProvider<T> unconditionalValueProvider) = GetCaseAndValue(array.Last);

            // Make sure there's no condition
            if (unconditionalCase.TryGetValue("When", StringComparison.OrdinalIgnoreCase, out _)) {
                throw new InvalidOperationException("The last case in a conditional value must not have a condition.");
            }

            // Create the value provider
            return new ConditionalContentPackValueProvider<T>(conditionalCases, new ConditionalContentPackValueProvider<T>.UnconditionalCase(unconditionalValueProvider));

            (JObject caseObject, IContentPackValueProvider<T> valueProvider) GetCaseAndValue(JToken caseToken) {
                // Check that the case is an object
                if (!(caseToken is JObject caseObject)) {
                    throw new InvalidOperationException("Conditional value cases must be objects");
                }

                // Check that a value token exists
                if (!caseObject.TryGetValue("Value", StringComparison.OrdinalIgnoreCase, out JToken valueToken)) {
                    throw new InvalidOperationException("Conditional value cases must have a value");
                }

                // Make sure the value token can be parsed into a value provider
                if (!(valueToken.ToObject<IContentPackValueProvider<T>>() is IContentPackValueProvider<T> valueProvider)) {
                    throw new InvalidOperationException("Failed to parse the value of the conditional case");
                }

                return (caseObject, valueProvider);
            }

            Dictionary<string, string> CreateConditionGroup(JObject conditionToken) {
                Dictionary<string, string> conditions = new Dictionary<string, string>();

                // Loop through each key-value pair in the object
                foreach ((string tokenStr, JToken expectedValueToken) in conditionToken) {
                    // Make sure the value is a value type (which will be converted to a string)
                    if (!(expectedValueToken is JValue valueToken)) {
                        throw new InvalidOperationException("The expected value in a 'When' clause must be a value (string, number, bool, etc).");
                    }

                    // Add the condition
                    conditions.Add(tokenStr, valueToken.Value?.ToString() ?? "null");
                }

                return conditions;
            }
        }

        public override bool CanConvert(Type objectType) {
            return objectType.GetGenericTypeDefinition() == typeof(IContentPackValueProvider<>);
        }

        public override bool CanWrite => false;
    }
}
