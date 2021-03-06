/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TupleElementNamesAttribute : Attribute {
        public IList<string> TransformNames { get; }

        public TupleElementNamesAttribute(string[] transformNames) {
            this.TransformNames = transformNames ?? throw new ArgumentNullException(nameof(transformNames));
        }

        public TupleElementNamesAttribute() {
            this.TransformNames = new List<string>();
        }
    }
}
