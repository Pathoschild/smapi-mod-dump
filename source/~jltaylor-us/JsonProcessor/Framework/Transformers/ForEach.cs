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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
namespace JsonProcessor.Framework.Transformers {
    /// <summary>
    /// The ForEach transformer loops over a source array or object, repeatedly expanding its body with new
    /// bindings of var.  The ForEach JSON node is replaced with an array containing the output.
    /// The names of the required properties match the pronunciaion of this operation as
    /// "for each VAR v IN source, YIELD body"
    /// </summary>
    public class ForEach : ITransformer {

        public string Name => "for-each";

        public bool ProcessChildrenFirst => false;

        public bool TransformNode(IJsonProcessor processor, JObject obj) {
            if (!obj.TryGetValue("var", out JToken? varClause)) {
                processor.LogError(obj.Path, $"missing required \"var\" property for for-each transformer");
                return false;
            }
            if (!obj.TryGetValue("in", out JToken? source)) {
                processor.LogError(obj.Path, $"missing required \"in\" property for for-each transformer");
                return false;
            }
            if (!obj.TryGetValue("yield", out JToken? body)) {
                processor.LogError(obj.Path, $"missing required \"yield\" property for for-each transformer");
                return false;
            }
            bool result = true;

            result = processor.Transform(source) && result;
            if (obj.TryGetValue("in", out JToken? maybeUpdatedSource)) {
                source = maybeUpdatedSource;
            }

            if (source.Type == JTokenType.Array) {
                string varName;
                if (varClause.Type == JTokenType.String) {
                    varName = varClause.Value<string>()!;
                } else if (varClause is JArray arr && arr.Count == 1 && arr[0].Type == JTokenType.String) {
                    varName = arr[0].Value<string>()!;
                } else {
                    processor.LogError(varClause.Path, "must be a string or array containing one string");
                    return false;
                }
                JArray resultNode = new JArray();
                foreach (JToken elt in source.Children()) {
                    resultNode.Add(body.DeepClone());
                    processor.PushEnv(new Dictionary<string, JToken>() { { varName, elt } });
                    result = processor.Transform(resultNode.Last!) && result;
                    processor.PopEnv();
                }
                obj.Replace(resultNode);
            } else if (source.Type == JTokenType.Object) {
                if (varClause is JArray arr && arr.Count == 2
                     && arr[0].Type == JTokenType.String && arr[1].Type == JTokenType.String) {
                    string propVar = arr[0].Value<string>()!;
                    string valVar = arr[1].Value<string>()!;
                    JArray resultNode = new JArray();
                    foreach (JProperty elt in ((JObject)source).Properties()) {
                        resultNode.Add(body.DeepClone());
                        processor.PushEnv(new Dictionary<string, JToken>() { { propVar, elt.Name }, { valVar, elt.Value } });
                        result = processor.Transform(resultNode.Last!) && result;
                        processor.PopEnv();
                    }
                    obj.Replace(resultNode);
                } else {
                    processor.LogError(varClause.Path, "must be an array containing two strings");
                    return false;
                }
            } else {
                processor.LogError(source.Path, "must be an array or object");
                return false;
            }

            return result;
        }

        public void AddTo(IJsonProcessor processor) {
            processor.AddTransformer(this);
        }

    }
}

