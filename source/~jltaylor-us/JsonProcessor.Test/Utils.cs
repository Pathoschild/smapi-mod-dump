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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonProcessor.Test {
    public static class Utils {
        public static JObject ReadTestData(string filename) {
            var dataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("JsonProcessor.Test.TestData." + filename);
            return (JObject)JToken.ReadFrom(new JsonTextReader(new System.IO.StreamReader(dataStream)));
        }

        public static void AssertTreesEqual(string who, JToken expected, JToken actual) {
            Assert.IsTrue(JToken.DeepEquals(expected, actual),
                $"{who}: expected {expected} but got {actual}");
        }

        // Assert that calling Transform on the token results in no changes;
        // returns the result of Transform
        public static bool AssertNoChanges(string who, IJsonProcessor processor, JToken tok) {
            bool result;
            JToken transformedTok = tok.DeepClone();
            result = processor.Transform(transformedTok);
            AssertTreesEqual(who, tok, transformedTok);
            return result;
        }

        public static void AssertCleanTransform(string who, IJsonProcessor processor, LogCollector log, JToken input, JToken expectedOutput) {
            Assert.IsTrue(log.IsEmpty, $"{who}: AssertCleanTransform called with a log that wasn't empty");
            JToken transformed = input.DeepClone();
            bool result = processor.Transform(transformed);
            Assert.IsTrue(log.IsEmpty, $"{who}: expected no error output, but got {log.ToString()}");
            Assert.IsTrue(result, $"{who}: Transform returned false");
            AssertTreesEqual(who, expectedOutput, transformed);
        }

    }
}

