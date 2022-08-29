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
using JsonProcessor.Framework.Transformers;
using StardewModdingAPI;

namespace JsonProcessor.Framework {
    public class JsonProcessorAPI : IJsonProcessorAPI {
        private IMonitor Monitor { get; }
        public JsonProcessorAPI(IMonitor monitor) {
            Monitor = monitor;
        }

        /// <inheritdoc/>
        public IJsonProcessor NewProcessor(string errorLogPrefix, bool includeDefaultTransformers = true) {
            JsonProcessorImpl result = new((path, msg) => {
                Monitor.Log($"{errorLogPrefix} - ${path}: ${msg}", LogLevel.Error);
            });
            AddDefaultProcessors(result);
            return result;
        }

        public static void AddDefaultProcessors(IJsonProcessor processor) {
            new Define().AddTo(processor);
            new ForEach().AddTo(processor);
            new Let().AddTo(processor);
            new Splice().AddTo(processor);
            new StringJoin().AddTo(processor);
            new Var().AddTo(processor);
        }
    }
}

