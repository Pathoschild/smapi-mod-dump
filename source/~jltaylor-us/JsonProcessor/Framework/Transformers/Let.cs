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
using Newtonsoft.Json.Linq;

namespace JsonProcessor.Framework.Transformers {
    /// <summary>
    /// The Let transformer provides scoped bindings for the expansion of its body
    /// </summary>
    public class Let : ITransformer {
        public Let() {
        }

        public string Name => "let";

        public bool ProcessChildrenFirst => false;

        public bool TransformNode(IJsonProcessor processor, JObject obj) {
            if (!obj.TryGetValue("bindings", out JToken? bindings)) {
                processor.LogError(obj.Path, $"missing required \"bindings\" property for let transformer");
                return false;
            }
            bool result = true;
            result = processor.Transform(bindings);
            if (obj.TryGetValue("bindings", out JToken? updatedBindings)) {
                bindings = updatedBindings;
            }

            if (! (bindings is JObject bindingsObj)) {
                processor.LogError(bindings.Path, "must be an object");
                return false;
            }
            if (!obj.TryGetValue("body", out JToken? body)) {
                processor.LogError(obj.Path, $"missing required \"body\" property for let transformer");
                return false;
            }
            processor.PushEnv(bindingsObj!);
            result = processor.Transform(body) && result;
            if (obj.TryGetValue("body", out JToken? updatedBody)) {
                body = updatedBody;
            }
            obj.Replace(body);
            processor.PopEnv();
            return true;
        }

        public void AddTo(IJsonProcessor processor) {
            processor.AddTransformer(this);
        }
    }
}

