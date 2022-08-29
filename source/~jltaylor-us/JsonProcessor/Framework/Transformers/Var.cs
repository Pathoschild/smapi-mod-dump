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
    /// The Var transformer provides variable lookup in the environment and replaces the JSON node with the result.
    /// Results are recursively expanded (until the expansion runs in to a term it has already expanded from
    /// the same environment).  An unbound variable (i.e., one not found in the environment) results in no changes
    /// to the JSON node.
    /// </summary>
    public class Var : IShorthandTransformer {
        private readonly Stack<Tuple<string, object>> lookupStack = new();
        public Var() {
        }

        public string Name => "var";

        public string? ArgumentNameWhenLongForm => "var";

        public bool ProcessArgumentFirst => true;

        public bool TransformValue(IJsonProcessor processor, JObject nodeToReplace, JToken arg) {
            if (arg.Type != JTokenType.String) {
                processor.LogError(arg.Path, "must be a string");
                return false;
            }
            string name = arg.Value<string>()!;
            if (processor.TryApplyEnv(name, out JToken? value, out object? foundInEnv)) {
                var lookupStackObj = new Tuple<string, object>(name, foundInEnv);
                if (lookupStack.Contains(lookupStackObj)) {
                    // don't try to expand a reference that we're currently expanding (from the same env)
                    return true;
                }
                JArray temp = new JArray(new Object[] { value });
                lookupStack.Push(lookupStackObj);
                bool result = processor.Transform(temp);
                lookupStack.Pop();
                if (temp.HasValues) {
                    nodeToReplace.Replace(temp[0]);
                } else {
                    // shouldn't happen?
                    nodeToReplace.Remove();
                }
                //nodeToReplace.Replace(value);
                return result;
            }
            // not finding a value is not an error
            return true;
        }

        public void AddTo(IJsonProcessor processor) {
            processor.AddShorthandTransformer(this);
        }
    }
}

