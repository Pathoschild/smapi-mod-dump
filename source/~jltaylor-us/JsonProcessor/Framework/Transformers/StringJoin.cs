/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// // Copyright 2022 Jamie Taylor
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonProcessor.Framework.Transformers {
    /// <summary>
    /// The StringJoin transformer joins an array of strings into one string (which replaces the transformer's
    /// JSON node).
    /// </summary>
    public class StringJoin : ITransformer, IShorthandTransformer {
        public string Name => "string-join";
        public bool ProcessChildrenFirst => true;
        public bool ProcessArgumentFirst => true;
        public string? ArgumentNameWhenLongForm => null;

        public StringJoin() {
        }

        public void AddTo(IJsonProcessor processor) {
            processor.AddShorthandTransformer(this);
            processor.AddTransformer(this);
        }

        public bool TransformNode(IJsonProcessor processor, JObject obj) {
            string delimiter = "";
            if (obj.TryGetValue("delimiter", out JToken? delimiterJson)) {
                if (delimiterJson.Type == JTokenType.String) {
                    delimiter = delimiterJson.Value<string>()!;
                } else {
                    processor.LogError(delimiterJson.Path, $"string-join delimiter must be a string, but it is a {delimiterJson.Type}");
                    return false;
                }
            }
            if (!obj.TryGetValue("strings", out JToken? strings)) {
                processor.LogError(obj.Path, $"missing required \"strings\" property for string-join transformer");
                return false;
            }
            return DoTheJoin(processor, obj, strings, delimiter);
        }

        public bool TransformValue(IJsonProcessor processor, JObject nodeToReplace, JToken arg) {
            return DoTheJoin(processor, nodeToReplace, arg, "");
        }

        private bool errorEncountered;
        private bool DoTheJoin(IJsonProcessor processor, JObject nodeToReplace, JToken stringArray, string delimiter) {
            if (stringArray.Type != JTokenType.Array) {
                processor.LogError(stringArray.Path, $"string-join needs an array, but got a {stringArray.Type}");
                return false;
            }
            errorEncountered = false;
            string joinedString = String.Join(delimiter, AsStrings(processor, stringArray));
            nodeToReplace.Replace(new JValue(joinedString));
            return !errorEncountered;
        }
        private IEnumerable<string> AsStrings(IJsonProcessor processor, JToken arr) {
            foreach (var child in arr.Children()) {
                if (child.Type == JTokenType.String) {
                    yield return child.Value<string>()!;
                } else {
                    errorEncountered = true;
                    processor.LogError(child.Path, $"ignoring non-string in string-join");
                }
            }
        }

    }
}

