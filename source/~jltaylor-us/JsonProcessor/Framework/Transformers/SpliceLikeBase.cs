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
    /// A base class for transformers that splice things into the surrounding context.  (Note that "things" may
    /// also be empty, effectively removing the node from the tree.)
    /// </summary>
    public abstract class SpliceLikeBase : IShorthandTransformer, IPropertyTransformer {
        public SpliceLikeBase() {
        }
        public abstract string Name { get; }
        public abstract string? ArgumentNameWhenLongForm { get; }
        public abstract bool ProcessArgumentFirst { get; }

        public void AddTo(IJsonProcessor processor) {
            processor.AddShorthandTransformer(this);
            processor.AddPropertyTransformer(this);
        }

        public bool TransformValue(IJsonProcessor processor, JObject nodeToReplace, JToken arg) {
            if (nodeToReplace.Parent is null) {
                // The processor should have already disallowed this, but check anyway
                // because either we are paranoid or want to make the compiler's nullable check happy.
                processor.LogError(nodeToReplace.Path, $"can't {Name} at top level");
                return false;
            }
            if (nodeToReplace.Parent.Type == JTokenType.Property) {
                if (!ValidateArg(processor, arg, JTokenType.Object)) return false;
                return InObject(processor, (JProperty)nodeToReplace.Parent, arg);
            }
            // else splicing into Array
            if (!ValidateArg(processor, arg, nodeToReplace.Parent.Type)) return false;
            return InArray(processor, nodeToReplace, arg);
        }

        public bool TransformProperty(IJsonProcessor processor, JProperty theProperty) {
            if (!ValidateArg(processor, theProperty.Value, JTokenType.Object)) return false;
            return InObject(processor, theProperty, theProperty.Value);
        }

        protected abstract bool InObject(IJsonProcessor processor, JProperty parentProp, JToken arg);
        protected abstract bool InArray(IJsonProcessor processor, JObject nodeToReplace, JToken arg);
        protected abstract bool ValidateArg(IJsonProcessor processor, JToken arg, JTokenType parentType);
    }
}

